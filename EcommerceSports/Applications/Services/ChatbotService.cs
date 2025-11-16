using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace EcommerceSports.Applications.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IGoogleGeminiService _geminiService;
        private readonly ILogger<ChatbotService>? _logger;
        private readonly IConfiguration _configuration;

        public ChatbotService(
            IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository,
            IGoogleGeminiService geminiService,
            IConfiguration configuration,
            ILogger<ChatbotService>? logger = null)
        {
            _produtoRepository = produtoRepository;
            _pedidoRepository = pedidoRepository;
            _geminiService = geminiService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ChatbotRespostaDTO> ProcessarMensagem(string mensagemUsuario, int? usuarioId = null)
        {
            // Caso 7: Entrada vazia
            if (string.IsNullOrWhiteSpace(mensagemUsuario))
            {
                return new ChatbotRespostaDTO
                {
                    Tipo = "texto",
                    Mensagem = "Digite algo para que eu possa ajudar."
                };
            }

            // Detectar feedback negativo
            var feedbackNegativo = DetectarFeedbackNegativo(mensagemUsuario);
            var produtosExcluir = new List<string>();

            // Montar contexto
            var contexto = await MontarContexto(usuarioId, feedbackNegativo, produtosExcluir);

            // Caso 4: Busca direta por produto
            var buscaDireta = await TentarBuscaDireta(mensagemUsuario);
            if (buscaDireta != null)
            {
                return buscaDireta;
            }

            // Caso 6: Detectar perguntas fora de escopo
            if (DetectarForaDeEscopo(mensagemUsuario))
            {
                return new ChatbotRespostaDTO
                {
                    Tipo = "texto",
                    Mensagem = "Desculpe, não tenho acesso a essa informação. Posso ajudar em produtos esportivos ou tirar dúvidas sobre equipamentos. Como posso ajudar?"
                };
            }

            // Montar prompt completo
            var fullPrompt = await MontarPromptCompleto(mensagemUsuario, contexto);
            _logger?.LogDebug("Prompt completo montado. Tamanho: {Tamanho} caracteres", fullPrompt.Length);

            // Chamar Gemini
            _logger?.LogInformation("Chamando Gemini para mensagem: {Mensagem}", mensagemUsuario);
            var respostaBruta = await _geminiService.GerarConteudo(fullPrompt, contexto);
            _logger?.LogInformation("Resposta bruta do Gemini recebida. Tamanho: {Tamanho}, Conteúdo: {Conteudo}", 
                respostaBruta?.Length ?? 0, respostaBruta?.Substring(0, Math.Min(500, respostaBruta?.Length ?? 0)) ?? "vazio");

            // Interpretar resposta
            var resposta = InterpretarResposta(respostaBruta);
            
            // Se a resposta ainda estiver vazia ou inválida, tentar fallback baseado na mensagem
            if (string.IsNullOrWhiteSpace(resposta.Mensagem) || resposta.Mensagem == "Desculpe, não entendi. Pode ser mais específico?")
            {
                resposta = await GerarRespostaFallback(mensagemUsuario, contexto);
            }
            
            _logger?.LogInformation("Resposta interpretada. Tipo: {Tipo}, Mensagem: {Mensagem}, Produtos: {Count}", 
                resposta.Tipo, resposta.Mensagem, resposta.Produtos?.Count ?? 0);

            // Normalizar produtos (buscar dados canônicos do banco)
            if (resposta.Produtos != null && resposta.Produtos.Any())
            {
                resposta.Produtos = await NormalizarProdutos(resposta.Produtos);
            }

            // Log de métricas
            LogMetrica(resposta.Tipo);

            return resposta;
        }

        private async Task<Dictionary<string, object>> MontarContexto(int? usuarioId, bool feedbackNegativo, List<string> produtosExcluir)
        {
            var usuarioDict = new Dictionary<string, object>
            {
                ["logado"] = usuarioId.HasValue
            };
            if (usuarioId.HasValue)
            {
                usuarioDict["id"] = usuarioId.Value.ToString();
            }
            else
            {
                usuarioDict["id"] = null!;
            }

            var contexto = new Dictionary<string, object>
            {
                ["usuario"] = usuarioDict,
                ["historico"] = new List<Dictionary<string, object>>(),
                ["catalogo_sample"] = new List<Dictionary<string, object>>(),
                ["excluirProdutos"] = produtosExcluir
            };

            if (feedbackNegativo && produtosExcluir.Any())
            {
                contexto["feedbackNegativoPara"] = produtosExcluir;
            }

            // Se usuário logado, buscar histórico
            if (usuarioId.HasValue)
            {
                try
                {
                    var historicoCompras = await _pedidoRepository.ObterHistoricoComprasPorCliente(usuarioId.Value, 20);
                    var historicoList = historicoCompras
                        .Where(ip => ip.Produto != null)
                        .Select(ip => new Dictionary<string, object>
                        {
                            ["id"] = ip.Produto!.Id.ToString(),
                            ["nome"] = ip.Produto.Nome,
                            ["categoria"] = ip.Produto.Categoria
                        })
                        .Distinct()
                        .Take(20)
                        .Cast<Dictionary<string, object>>()
                        .ToList();

                    contexto["historico"] = historicoList;

                    // Preferências (categoria mais comprada)
                    var categoriaFavorita = historicoCompras
                        .Where(ip => ip.Produto != null)
                        .GroupBy(ip => ip.Produto!.Categoria)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(categoriaFavorita))
                    {
                        contexto["usuario"] = new Dictionary<string, object>
                        {
                            ["id"] = usuarioId.Value.ToString(),
                            ["logado"] = true,
                            ["preferencias"] = new[] { categoriaFavorita }
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Erro ao buscar histórico do usuário {UsuarioId}", usuarioId);
                }
            }

            // Buscar top 50 produtos do catálogo
            try
            {
                var todosProdutos = await _produtoRepository.ListarTodos();
                var catalogoList = todosProdutos
                    .Take(50)
                    .Select(p => new Dictionary<string, object>
                    {
                        ["id"] = p.Id.ToString(),
                        ["nome"] = p.Nome,
                        ["categoria"] = p.Categoria,
                        ["preco"] = (decimal)p.Preco,
                        ["descricao"] = p.Descricao?.Substring(0, Math.Min(200, p.Descricao.Length)) ?? ""
                    })
                    .Cast<Dictionary<string, object>>()
                    .ToList();

                contexto["catalogo_sample"] = catalogoList;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Erro ao buscar catálogo de produtos");
            }

            return contexto;
        }

        private async Task<string> MontarPromptCompleto(string mensagemUsuario, Dictionary<string, object> contexto)
        {
            var systemPromptPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", "system_prompt.txt");
            var fewShotPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", "few_shot_examples.txt");

            var systemPrompt = "";
            var fewShot = "";

            try
            {
                if (File.Exists(systemPromptPath))
                {
                    systemPrompt = await File.ReadAllTextAsync(systemPromptPath);
                }
                else
                {
                    _logger?.LogWarning("Arquivo system_prompt.txt não encontrado em {Path}", systemPromptPath);
                }

                if (File.Exists(fewShotPath))
                {
                    fewShot = await File.ReadAllTextAsync(fewShotPath);
                }
                else
                {
                    _logger?.LogWarning("Arquivo few_shot_examples.txt não encontrado em {Path}", fewShotPath);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao ler arquivos de prompt");
            }

            var contextoJson = JsonSerializer.Serialize(contexto, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            return $"{systemPrompt}\n\n{fewShot}\n\nContexto: {contextoJson}\n\nUsuario: {mensagemUsuario}";
        }

        private ChatbotRespostaDTO InterpretarResposta(string respostaBruta)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(respostaBruta) || respostaBruta == "{}")
                {
                    _logger?.LogWarning("Resposta bruta vazia ou inválida: {Resposta}", respostaBruta);
                    throw new Exception("Resposta bruta vazia");
                }

                var json = ExtrairPrimeiroJson(respostaBruta);
                _logger?.LogDebug("JSON extraído: {Json}", json);
                
                if (string.IsNullOrWhiteSpace(json) || json == "{}")
                {
                    _logger?.LogWarning("JSON extraído está vazio. Resposta bruta: {Resposta}", respostaBruta);
                    throw new Exception("JSON vazio após extração");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var dto = JsonSerializer.Deserialize<ChatbotRespostaDTO>(json, options);

                if (dto == null)
                {
                    _logger?.LogWarning("DTO nulo após deserialização. JSON: {Json}", json);
                    throw new Exception("DTO nulo após deserialização");
                }

                // Validar tipo
                var tiposValidos = new[] { "lista", "texto", "pergunta", "produto", "erro" };
                if (string.IsNullOrWhiteSpace(dto.Tipo) || !tiposValidos.Contains(dto.Tipo.ToLower()))
                {
                    _logger?.LogWarning("Tipo inválido: {Tipo}. Alterando para 'texto'", dto.Tipo);
                    dto.Tipo = "texto";
                }

                // Garantir que mensagem não seja nula
                if (string.IsNullOrWhiteSpace(dto.Mensagem))
                {
                    _logger?.LogWarning("Mensagem vazia no DTO. Definindo mensagem padrão.");
                    dto.Mensagem = "Não consegui processar sua solicitação. Pode reformular?";
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao interpretar resposta do Gemini. Resposta bruta: {Resposta}", respostaBruta);
                
                // Fallback seguro
                return new ChatbotRespostaDTO
                {
                    Tipo = "texto",
                    Mensagem = "Desculpe, não entendi. Pode ser mais específico?"
                };
            }
        }

        private string ExtrairPrimeiroJson(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return "{}";
            }

            texto = texto.Trim();

            // Remover markdown code blocks se existirem
            if (texto.StartsWith("```", StringComparison.Ordinal))
            {
                var sb = new StringBuilder();
                var linhas = texto.Split('\n');
                var dentroDoBloco = false;

                foreach (var linha in linhas)
                {
                    var linhaTrim = linha.Trim();
                    if (linhaTrim.StartsWith("```", StringComparison.Ordinal))
                    {
                        dentroDoBloco = !dentroDoBloco;
                        continue;
                    }
                    if (dentroDoBloco)
                    {
                        sb.AppendLine(linha);
                    }
                }

                texto = sb.ToString().Trim();
            }

            // Encontrar primeiro bloco JSON balanceado
            var primeiroAbre = texto.IndexOf('{');
            if (primeiroAbre < 0)
            {
                return "{}";
            }

            var nivel = 0;
            var inicio = primeiroAbre;
            var fim = -1;

            for (int i = primeiroAbre; i < texto.Length; i++)
            {
                if (texto[i] == '{')
                {
                    nivel++;
                }
                else if (texto[i] == '}')
                {
                    nivel--;
                    if (nivel == 0)
                    {
                        fim = i;
                        break;
                    }
                }
            }

            if (fim > inicio)
            {
                return texto.Substring(inicio, fim - inicio + 1);
            }

            return "{}";
        }

        private async Task<List<ProdutoDTO>> NormalizarProdutos(List<ProdutoDTO> produtos)
        {
            var produtosNormalizados = new List<ProdutoDTO>();

            foreach (var produto in produtos)
            {
                if (int.TryParse(produto.Id, out var produtoId))
                {
                    try
                    {
                        var todosProdutos = await _produtoRepository.ListarTodos();
                        var produtoDb = todosProdutos.FirstOrDefault(p => p.Id == produtoId);

                        if (produtoDb != null)
                        {
                            produtosNormalizados.Add(new ProdutoDTO
                            {
                                Id = produtoDb.Id.ToString(),
                                Nome = produtoDb.Nome,
                                Preco = (decimal)produtoDb.Preco,
                                Categoria = produtoDb.Categoria,
                                ImagemUrl = produtoDb.Imagem ?? "",
                                LinkProduto = $"/produto/{produtoDb.Id}" // Ajustar conforme sua rota
                            });
                        }
                        else
                        {
                            // Manter produto original se não encontrar no DB
                            produtosNormalizados.Add(produto);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Erro ao normalizar produto {ProdutoId}", produtoId);
                        produtosNormalizados.Add(produto);
                    }
                }
                else
                {
                    produtosNormalizados.Add(produto);
                }
            }

            return produtosNormalizados;
        }

        private bool DetectarFeedbackNegativo(string mensagem)
        {
            var mensagemLower = mensagem.ToLowerInvariant();
            var termosNegativos = new[] { "não gostei", "não curti", "tem outra", "outra opção", "não quero", "não serve" };
            return termosNegativos.Any(termo => mensagemLower.Contains(termo));
        }

        private async Task<ChatbotRespostaDTO?> TentarBuscaDireta(string mensagem)
        {
            try
            {
                // Buscar produtos que correspondem exatamente ao nome
                var todosProdutos = await _produtoRepository.ListarTodos();
                var produtoEncontrado = todosProdutos
                    .FirstOrDefault(p => p.Nome.Equals(mensagem, StringComparison.OrdinalIgnoreCase));

                if (produtoEncontrado != null)
                {
                    return new ChatbotRespostaDTO
                    {
                        Tipo = "produto",
                        Mensagem = "Encontrei o produto que você procura:",
                        Produtos = new List<ProdutoDTO>
                        {
                            new ProdutoDTO
                            {
                                Id = produtoEncontrado.Id.ToString(),
                                Nome = produtoEncontrado.Nome,
                                Preco = (decimal)produtoEncontrado.Preco,
                                Categoria = produtoEncontrado.Categoria,
                                ImagemUrl = produtoEncontrado.Imagem ?? "",
                                LinkProduto = $"/produto/{produtoEncontrado.Id}"
                            }
                        }
                    };
                }

                // Busca parcial (contém o nome)
                var produtosParciais = todosProdutos
                    .Where(p => p.Nome.Contains(mensagem, StringComparison.OrdinalIgnoreCase))
                    .Take(1)
                    .ToList();

                if (produtosParciais.Any())
                {
                    var produto = produtosParciais.First();
                    return new ChatbotRespostaDTO
                    {
                        Tipo = "produto",
                        Mensagem = "Encontrei um produto similar:",
                        Produtos = new List<ProdutoDTO>
                        {
                            new ProdutoDTO
                            {
                                Id = produto.Id.ToString(),
                                Nome = produto.Nome,
                                Preco = (decimal)produto.Preco,
                                Categoria = produto.Categoria,
                                ImagemUrl = produto.Imagem ?? "",
                                LinkProduto = $"/produto/{produto.Id}"
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Erro ao tentar busca direta para: {Mensagem}", mensagem);
            }

            return null;
        }

        private bool DetectarForaDeEscopo(string mensagem)
        {
            var mensagemLower = mensagem.ToLowerInvariant();
            var termosForaEscopo = new[] { "previsão do tempo", "clima", "tempo amanhã", "cotação", "dólar", "bitcoin" };
            return termosForaEscopo.Any(termo => mensagemLower.Contains(termo));
        }

        private async Task<ChatbotRespostaDTO> GerarRespostaFallback(string mensagemUsuario, Dictionary<string, object> contexto)
        {
            _logger?.LogInformation("Gerando resposta fallback para: {Mensagem}", mensagemUsuario);
            
            var mensagemLower = mensagemUsuario.ToLowerInvariant();
            
            // Detectar intenção de busca por categoria
            if (mensagemLower.Contains("futebol"))
            {
                try
                {
                    var produtos = await _produtoRepository.ListarTodos();
                    var produtosFutebol = produtos
                        .Where(p => p.Categoria.Contains("Futebol", StringComparison.OrdinalIgnoreCase) || 
                                   p.Nome.Contains("futebol", StringComparison.OrdinalIgnoreCase))
                        .Take(5)
                        .Select(p => new ProdutoDTO
                        {
                            Id = p.Id.ToString(),
                            Nome = p.Nome,
                            Preco = (decimal)p.Preco,
                            Categoria = p.Categoria,
                            ImagemUrl = p.Imagem ?? "",
                            LinkProduto = $"/produto/{p.Id}"
                        })
                        .ToList();

                    if (produtosFutebol.Any())
                    {
                        return new ChatbotRespostaDTO
                        {
                            Tipo = "lista",
                            Mensagem = "Encontrei alguns produtos de futebol para você:",
                            Produtos = produtosFutebol
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Erro ao buscar produtos de futebol no fallback");
                }
            }

            if (mensagemLower.Contains("basquete"))
            {
                try
                {
                    var produtos = await _produtoRepository.ListarTodos();
                    var produtosBasquete = produtos
                        .Where(p => p.Categoria.Contains("Basquete", StringComparison.OrdinalIgnoreCase) || 
                                   p.Nome.Contains("basquete", StringComparison.OrdinalIgnoreCase))
                        .Take(5)
                        .Select(p => new ProdutoDTO
                        {
                            Id = p.Id.ToString(),
                            Nome = p.Nome,
                            Preco = (decimal)p.Preco,
                            Categoria = p.Categoria,
                            ImagemUrl = p.Imagem ?? "",
                            LinkProduto = $"/produto/{p.Id}"
                        })
                        .ToList();

                    if (produtosBasquete.Any())
                    {
                        return new ChatbotRespostaDTO
                        {
                            Tipo = "lista",
                            Mensagem = "Encontrei alguns produtos de basquete para você:",
                            Produtos = produtosBasquete
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Erro ao buscar produtos de basquete no fallback");
                }
            }

            // Se contém "recomendação" ou "recomenda", fazer pergunta de qualificação
            if (mensagemLower.Contains("recomend") || mensagemLower.Contains("sugest"))
            {
                return new ChatbotRespostaDTO
                {
                    Tipo = "pergunta",
                    Mensagem = "Para te ajudar melhor, você pratica futebol ou basquete? Ou tem alguma preferência específica?"
                };
            }

            // Fallback genérico
            return new ChatbotRespostaDTO
            {
                Tipo = "texto",
                Mensagem = "Posso ajudar você a encontrar produtos de futebol ou basquete. Pode ser mais específico sobre o que procura?"
            };
        }

        private void LogMetrica(string tipo)
        {
            _logger?.LogInformation("Chatbot resposta tipo: {Tipo}", tipo);
            // Aqui você pode adicionar métricas para Prometheus, AppInsights, etc.
        }
    }
}
