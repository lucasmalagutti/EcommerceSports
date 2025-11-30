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
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IGoogleGeminiService _geminiService;
        private readonly string _systemPrompt;
        private readonly string _fewShotPrompt;

        public ChatbotService(
            IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository,
            IGoogleGeminiService geminiService)
        {
            _produtoRepository = produtoRepository;
            _pedidoRepository = pedidoRepository;
            _geminiService = geminiService;

            (_systemPrompt, _fewShotPrompt) = CarregarPrompts();
        }

        private (string systemPrompt, string fewShotPrompt) CarregarPrompts()
        {
            var systemPromptPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", "system_prompt.txt");
            var fewShotPath = Path.Combine(Directory.GetCurrentDirectory(), "Prompts", "few_shot_examples.txt");

            var systemPrompt = LerArquivoOuVazio(systemPromptPath);
            var fewShotPrompt = LerArquivoOuVazio(fewShotPath);

            return (systemPrompt, fewShotPrompt);
        }

        private static string LerArquivoOuVazio(string caminhoArquivo)
        {
            try
            {
                return File.Exists(caminhoArquivo) ? File.ReadAllText(caminhoArquivo) : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<ChatbotRespostaDTO> ProcessarMensagem(string mensagemUsuario, int? usuarioId = null)
        {
            if (string.IsNullOrWhiteSpace(mensagemUsuario))
            {
                return new ChatbotRespostaDTO
                {
                    Tipo = "texto",
                    Mensagem = "Digite algo para que eu possa ajudar."
                };
            }

            var usuarioContexto = usuarioId ?? 33; // TODO: remover ID fixo 33 quando houver contexto de usuário autenticado.
            var contexto = await MontarContexto(usuarioId);
            contexto["consultaAtual"] = mensagemUsuario;

            var fullPrompt = await MontarPromptCompleto(mensagemUsuario, contexto);
            var respostaBruta = await _geminiService.GerarConteudo(fullPrompt, contexto);

            var resposta = InterpretarResposta(respostaBruta ?? string.Empty);

            if (resposta.Produtos != null && resposta.Produtos.Any())
            {
                resposta.Produtos = await NormalizarProdutos(resposta.Produtos, usuarioContexto);
                if (string.IsNullOrWhiteSpace(resposta.Layout) || resposta.Layout == "texto")
                {
                    resposta.Layout = "cards";
                }
            }

            return resposta;
        }

        private async Task<Dictionary<string, object>> MontarContexto(int? usuarioId)
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
                ["catalogo_sample"] = new List<Dictionary<string, object>>()
            };

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
                catch
                {
                    // Histórico indisponível não impede a geração de resposta.
                }
            }

            // Buscar catálogo completo de produtos
            try
            {
                var todosProdutos = await _produtoRepository.ListarTodos();
                var catalogoCompleto = todosProdutos
                    .Select(p => new Dictionary<string, object>
                    {
                        ["id"] = p.Id.ToString(),
                        ["nome"] = p.Nome,
                        ["categoria"] = p.Categoria,
                        ["preco"] = (decimal)p.Preco,
                        ["estoque"] = p.QtdEstoque,
                        ["imagem"] = p.Imagem ?? string.Empty,
                        ["descricao"] = p.Descricao?.Trim() is { Length: > 400 } descricaoLonga
                            ? descricaoLonga.Substring(0, 397) + "..."
                            : p.Descricao?.Trim() ?? string.Empty
                    })
                    .Cast<Dictionary<string, object>>()
                    .ToList();

                contexto["catalogo_completo"] = catalogoCompleto;
                contexto["catalogo_sample"] = catalogoCompleto.Take(50).ToList();
            }
            catch
            {
                // Falha ao carregar catálogo; o modelo ainda pode responder com base no contexto disponível.
            }

            return contexto;
        }

        private Task<string> MontarPromptCompleto(string mensagemUsuario, Dictionary<string, object> contexto)
        {
            var instrucaoContexto = "Use o JSON de contexto fornecido separadamente (catalogo_completo, historico e dados do usuário) para construir a resposta no formato solicitado.";

            var partes = new List<string>();
            if (!string.IsNullOrWhiteSpace(_systemPrompt))
            {
                partes.Add(_systemPrompt);
            }
            if (!string.IsNullOrWhiteSpace(_fewShotPrompt))
            {
                partes.Add(_fewShotPrompt);
            }
            partes.Add(instrucaoContexto);
            partes.Add($"Usuario: {mensagemUsuario}");

            var prompt = string.Join("\n\n", partes);
            return Task.FromResult(prompt);
        }

        private ChatbotRespostaDTO InterpretarResposta(string respostaBruta)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(respostaBruta) || respostaBruta == "{}")
                {
                    return CriarRespostaPadrao();
                }

                var json = ExtrairPrimeiroJson(respostaBruta);
                if (string.IsNullOrWhiteSpace(json) || json == "{}")
                {
                    return CriarRespostaPadrao();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var dto = JsonSerializer.Deserialize<ChatbotRespostaDTO>(json, options);

                if (dto == null)
                {
                    return CriarRespostaPadrao();
                }

                // Validar tipo
                var tiposValidos = new[] { "lista", "texto", "pergunta", "produto", "erro" };
                if (string.IsNullOrWhiteSpace(dto.Tipo) || !tiposValidos.Contains(dto.Tipo.ToLower()))
                {
                    dto.Tipo = "texto";
                }

                // Garantir que mensagem não seja nula
                if (string.IsNullOrWhiteSpace(dto.Mensagem))
                {
                    dto.Mensagem = "Não consegui processar sua solicitação. Pode reformular?";
                }

                return dto;
            }
            catch
            {
                return CriarRespostaPadrao();
            }
        }

        private static ChatbotRespostaDTO CriarRespostaPadrao()
        {
            return new ChatbotRespostaDTO
            {
                Tipo = "texto",
                Mensagem = "Desculpe, não entendi. Pode ser mais específico?"
            };
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

        private async Task<List<ProdutoDTO>> NormalizarProdutos(List<ProdutoDTO> produtos, int usuarioId)
        {
            var produtosNormalizados = new List<ProdutoDTO>();
            List<Produto>? catalogo = null;

            foreach (var produto in produtos)
            {
                if (!int.TryParse(produto.Id, out var produtoId))
                {
                    produtosNormalizados.Add(produto);
                    continue;
                }

                try
                {
                    catalogo ??= await _produtoRepository.ListarTodos();
                    var produtoDb = catalogo.FirstOrDefault(p => p.Id == produtoId);

                    if (produtoDb != null)
                    {
                        produtosNormalizados.Add(CriarProdutoDto(produtoDb, usuarioId));
                    }
                    else
                    {
                        if (produto.Preco.HasValue && produto.Acao == null)
                        {
                            produto.Acao = CriarAcaoAdicionarCarrinho(usuarioId, produtoId, produto.Preco.Value, produto.Nome);
                        }
                        produtosNormalizados.Add(produto);
                    }
                }
                catch
                {
                    if (produto.Preco.HasValue && produto.Acao == null)
                    {
                        produto.Acao = CriarAcaoAdicionarCarrinho(usuarioId, produtoId, produto.Preco.Value, produto.Nome);
                    }
                    produtosNormalizados.Add(produto);
                }
            }

            return produtosNormalizados;
        }

        private ChatbotAcaoDTO CriarAcaoAdicionarCarrinho(int usuarioId, int produtoId, decimal precoUnitario, string produtoNome)
        {
            return new ChatbotAcaoDTO
            {
                Tipo = "comprar",
                Label = "Comprar",
                Metodo = "POST",
                Endpoint = $"/api/Carrinho/{usuarioId}/adicionar",
                Payload = new
                {
                    produtoId,
                    quantidade = 1,
                    precoUnitario,
                    nomeProduto = produtoNome
                },
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json"
                }
            };
        }

        private static string? ConstruirDescricaoCurta(string? descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
            {
                return null;
            }

            var texto = descricao.Trim();
            return texto.Length <= 160 ? texto : texto.Substring(0, 157) + "...";
        }

        private ProdutoDTO CriarProdutoDto(Produto produto, int usuarioId)
        {
            var preco = (decimal)produto.Preco;
            return new ProdutoDTO
            {
                Id = produto.Id.ToString(),
                Nome = produto.Nome,
                Preco = preco,
                Categoria = produto.Categoria,
                ImagemUrl = produto.Imagem ?? string.Empty,
                LinkProduto = $"/produto/{produto.Id}",
                DescricaoCurta = ConstruirDescricaoCurta(produto.Descricao),
                Acao = CriarAcaoAdicionarCarrinho(usuarioId, produto.Id, preco, produto.Nome)
            };
        }

    }
}
