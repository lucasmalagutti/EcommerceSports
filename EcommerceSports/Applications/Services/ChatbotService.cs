using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
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

        private static readonly Regex PalavraChaveRegex = new(@"[0-9A-Za-zÀ-ÿ]+(?:[.,][0-9]+)?", RegexOptions.Compiled);
        private static readonly HashSet<string> StopWords = new(
            new[]
            {
                "de", "da", "do", "das", "dos", "para", "pra", "por", "com", "sem", "uma", "um", "uns", "umas",
                "o", "a", "os", "as", "no", "na", "nos", "nas", "em", "ao", "à", "às", "e", "ou", "que", "qual",
                "quais", "ser", "estar", "ter", "tem", "tenho", "quero", "gostaria", "preciso", "procurando",
                "me", "você", "vocês", "ajudar", "ajuda", "pode", "poderia", "favor"
            },
            StringComparer.OrdinalIgnoreCase);
        private static readonly CultureInfo CulturaPtBr = CultureInfo.GetCultureInfo("pt-BR");
        private static readonly string[] GatihosRecomendacao = new[]
        {
            "recomenda", "recomendar", "recomendaria", "recomende", "sugira", "me sugere", "sugere", "sugerir", "indica", "indicar",
            "o que você me recomenda", "alguma sugestão", "qual recomendação", "me indica"
        };
        private static readonly ConcurrentDictionary<int, RecomendacaoMemoria> ContextoRecomendacoes = new();

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
            if (string.IsNullOrWhiteSpace(mensagemUsuario))
            {
                return new ChatbotRespostaDTO
                {
                    Tipo = "texto",
                    Mensagem = "Digite algo para que eu possa ajudar."
                };
            }

            var usuarioContexto = usuarioId ?? 33;
            var contexto = await MontarContexto(usuarioContexto, feedbackNegativo: false, produtosExcluir: new List<string>());
            contexto["consultaAtual"] = mensagemUsuario;

            var fullPrompt = await MontarPromptCompleto(mensagemUsuario, contexto);
            _logger?.LogDebug("Prompt completo montado. Tamanho: {Tamanho} caracteres", fullPrompt.Length);

            _logger?.LogInformation("Chamando Gemini para mensagem: {Mensagem}", mensagemUsuario);
            var respostaBruta = await _geminiService.GerarConteudo(fullPrompt, contexto);
            _logger?.LogInformation("Resposta bruta do Gemini recebida. Tamanho: {Tamanho}, Conteúdo: {Conteudo}",
                respostaBruta?.Length ?? 0, respostaBruta?.Substring(0, Math.Min(500, respostaBruta?.Length ?? 0)) ?? "vazio");

            var resposta = InterpretarResposta(respostaBruta ?? string.Empty);

            if (resposta.Produtos != null && resposta.Produtos.Any())
            {
                resposta.Produtos = await NormalizarProdutos(resposta.Produtos, usuarioContexto);
                if (string.IsNullOrWhiteSpace(resposta.Layout) || resposta.Layout == "texto")
                {
                    resposta.Layout = "cards";
                }
            }

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

            var instrucaoContexto = "Use o JSON de contexto fornecido separadamente (catalogo_completo, historico e dados do usuário) para construir a resposta no formato solicitado.";
            return $"{systemPrompt}\n\n{fewShot}\n\n{instrucaoContexto}\n\nUsuario: {mensagemUsuario}";
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

        private async Task<ChatbotRespostaDTO?> TentarRespostaComPalavrasChave(string mensagemUsuario, int usuarioId)
        {
            var termos = ExtrairPalavrasChave(mensagemUsuario);
            if (!termos.Any())
            {
                return null;
            }

            try
            {
                var produtos = await _produtoRepository.BuscarProdutosPorFiltro(null, termos);
                var produtosLista = produtos
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .Take(8)
                    .Select(p => CriarProdutoDto(p, usuarioId))
                    .ToList();

                if (produtosLista.Any())
                {
                    return new ChatbotRespostaDTO
                    {
                        Tipo = "lista",
                        Layout = "cards",
                        Mensagem = "Encontrei algumas opções que combinam com o que você descreveu:",
                        Produtos = produtosLista
                    };
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao buscar produtos por palavras-chave para a mensagem: {Mensagem}", mensagemUsuario);
            }

            return null;
        }

        private static List<string> ExtrairPalavrasChave(string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
            {
                return new List<string>();
            }

            var termos = new List<string>();

            foreach (Match match in PalavraChaveRegex.Matches(mensagem))
            {
                var token = match.Value.Trim();
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }

                if (StopWords.Contains(token))
                {
                    continue;
                }

                var ehNumero = decimal.TryParse(token, NumberStyles.Number, CultureInfo.InvariantCulture, out _) ||
                               decimal.TryParse(token, NumberStyles.Number, CulturaPtBr, out _);

                if (!ehNumero && token.Length <= 2)
                {
                    continue;
                }

                termos.Add(token);
            }

            return termos
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(10)
                .ToList();
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
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Erro ao normalizar produto {ProdutoId}", produtoId);
                    if (produto.Preco.HasValue && produto.Acao == null)
                    {
                        produto.Acao = CriarAcaoAdicionarCarrinho(usuarioId, produtoId, produto.Preco.Value, produto.Nome);
                    }
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

        private async Task<ChatbotRespostaDTO?> TentarBuscaDireta(string mensagem, int usuarioId)
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
                        Layout = "cards",
                        Mensagem = "Encontrei o produto que você procura:",
                        Produtos = new List<ProdutoDTO>
                        {
                            CriarProdutoDto(produtoEncontrado, usuarioId)
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
                        Layout = "cards",
                        Mensagem = "Encontrei um produto similar:",
                        Produtos = new List<ProdutoDTO>
                        {
                            CriarProdutoDto(produto, usuarioId)
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

        private async Task<ChatbotRespostaDTO?> TentarRecomendacaoPersonalizada(string mensagemUsuario, int usuarioId)
        {
            if (!ContemGatilhoRecomendacao(mensagemUsuario))
            {
                return null;
            }

            try
            {
                var historico = await _pedidoRepository.ObterHistoricoComprasPorCliente(usuarioId, 30);
                var itensHistorico = historico.Where(ip => ip.Produto != null).ToList();

                if (!itensHistorico.Any())
                {
                    return null;
                }

                var categoriasOrdenadas = itensHistorico
                    .Where(ip => !string.IsNullOrWhiteSpace(ip.Produto!.Categoria))
                    .GroupBy(ip => ip.Produto!.Categoria)
                    .OrderByDescending(g => g.Sum(ip => ip.Quantidade))
                    .Select(g => g.Key!)
                    .ToList();

                if (!categoriasOrdenadas.Any())
                {
                    return null;
                }

                var produtosCatalogo = await _produtoRepository.ListarTodos();
                var idsJaComprados = itensHistorico.Select(ip => ip.Produto!.Id).ToHashSet();

                var recomendados = new List<Produto>();
                foreach (var categoria in categoriasOrdenadas)
                {
                    var produtosCategoria = produtosCatalogo
                        .Where(p => categoria.Equals(p.Categoria, StringComparison.OrdinalIgnoreCase) && !idsJaComprados.Contains(p.Id))
                        .Take(3)
                        .ToList();

                    recomendados.AddRange(produtosCategoria);

                    if (recomendados.Count >= 5)
                    {
                        break;
                    }
                }

                if (!recomendados.Any())
                {
                    var categoriaPrincipal = categoriasOrdenadas.First();
                    recomendados = produtosCatalogo
                        .Where(p => categoriaPrincipal.Equals(p.Categoria, StringComparison.OrdinalIgnoreCase))
                        .Take(5)
                        .ToList();
                }

                if (!recomendados.Any())
                {
                    return null;
                }

                recomendados = recomendados
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .Take(5)
                    .ToList();

                var produtoDestaque = itensHistorico
                    .OrderByDescending(ip => ip.Quantidade)
                    .Select(ip => ip.Produto!.Nome)
                    .FirstOrDefault();

                var categoriaDestaque = categoriasOrdenadas.First();
                var mensagem = produtoDestaque != null
                    ? $"Notei que você gostou de {produtoDestaque}. Que tal conferir também esta opção da linha de {categoriaDestaque}?"
                    : $"Notei que você curte {categoriaDestaque}. Veja se essa sugestão faz sentido para você:";

                return CriarRespostaRecomendacao(usuarioId, recomendados, categoriaDestaque, produtoDestaque, mensagem);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao gerar recomendação personalizada para o usuário {UsuarioId}", usuarioId);
                return null;
            }
        }

        private ChatbotRespostaDTO CriarRespostaRecomendacao(int usuarioId, List<Produto> candidatos, string categoriaReferencia, string? nomeReferencia, string mensagemBase)
        {
            var candidatosDistintos = candidatos
                .Where(p => p != null)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();

            if (!candidatosDistintos.Any())
            {
                ContextoRecomendacoes.TryRemove(usuarioId, out _);
                return new ChatbotRespostaDTO
                {
                    Tipo = "texto",
                    Mensagem = "Ainda não encontrei uma boa sugestão agora, que tal tentar outro tipo de produto?"
                };
            }

            var principal = candidatosDistintos.First();
            var sugestaoPrincipal = ConverterProdutoParaSugestao(principal);

            var memoria = new RecomendacaoMemoria
            {
                Categoria = categoriaReferencia,
                NomeReferencia = nomeReferencia ?? principal.Nome,
                UltimaSugestao = sugestaoPrincipal
            };

            memoria.Rejeitados.Add(sugestaoPrincipal.Id);
            foreach (var restante in candidatosDistintos.Skip(1))
            {
                var mesmoNome = !string.IsNullOrWhiteSpace(nomeReferencia) &&
                                restante.Nome.Contains(nomeReferencia, StringComparison.OrdinalIgnoreCase);
                var mesmaCategoria = !string.IsNullOrWhiteSpace(categoriaReferencia) &&
                                     categoriaReferencia.Equals(restante.Categoria, StringComparison.OrdinalIgnoreCase);

                if (!mesmoNome && !mesmaCategoria)
                {
                    continue;
                }

                memoria.Alternativas.Enqueue(ConverterProdutoParaSugestao(restante));
            }

            ContextoRecomendacoes.AddOrUpdate(usuarioId, memoria, (_, __) => memoria);

            var mensagem = string.IsNullOrWhiteSpace(mensagemBase)
                ? $"Minha sugestão é {sugestaoPrincipal.Nome}. Que acha?"
                : $"{mensagemBase} Minha sugestão é {sugestaoPrincipal.Nome}.";

            return new ChatbotRespostaDTO
            {
                Tipo = "lista",
                Layout = "cards",
                Mensagem = mensagem,
                Produtos = new List<ProdutoDTO> { ConverterSugestaoParaDto(sugestaoPrincipal, usuarioId) }
            };
        }

        private static ProdutoSugestao ConverterProdutoParaSugestao(Produto produto)
        {
            return new ProdutoSugestao
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Categoria = produto.Categoria,
                Preco = (decimal)produto.Preco,
                Imagem = produto.Imagem,
                Descricao = produto.Descricao
            };
        }

        private ProdutoDTO ConverterSugestaoParaDto(ProdutoSugestao sugestao, int usuarioId)
        {
            return CriarProdutoDto(sugestao, usuarioId);
        }

        private static string FormatarListaProdutos(IReadOnlyList<string> nomes)
        {
            if (nomes.Count == 0)
            {
                return string.Empty;
            }

            if (nomes.Count == 1)
            {
                return nomes[0];
            }

            var prefixo = string.Join(", ", nomes.Take(nomes.Count - 1));
            return $"{prefixo} ou {nomes.Last()}";
        }

        private async Task<ChatbotRespostaDTO?> TentarResponderFeedbackNegativo(int usuarioId, string mensagemUsuario)
        {
            if (!ContextoRecomendacoes.TryGetValue(usuarioId, out var memoria))
            {
                return null;
            }

            _logger?.LogInformation("Feedback negativo do usuário {UsuarioId}: {Mensagem}", usuarioId, mensagemUsuario);

            var alternativasSelecionadas = new List<ProdutoSugestao>();

            ProdutoSugestao? ObterProximaAlternativa()
            {
                while (memoria.Alternativas.Count > 0)
                {
                    var candidato = memoria.Alternativas.Dequeue();
                    if (memoria.Rejeitados.Contains(candidato.Id))
                    {
                        continue;
                    }

                    memoria.Rejeitados.Add(candidato.Id);
                    return candidato;
                }

                return null;
            }

            var proxima = ObterProximaAlternativa();

            if (proxima == null)
            {
                var produtos = await _produtoRepository.ListarTodos();
                var novosCandidatos = produtos
                    .Where(p =>
                        (!string.IsNullOrWhiteSpace(memoria.NomeReferencia) && p.Nome.Contains(memoria.NomeReferencia, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(memoria.Categoria) && memoria.Categoria.Equals(p.Categoria, StringComparison.OrdinalIgnoreCase)))
                    .Where(p => !memoria.Rejeitados.Contains(p.Id))
                    .Take(5)
                    .Select(ConverterProdutoParaSugestao)
                    .ToList();

                foreach (var candidato in novosCandidatos)
                {
                    memoria.Alternativas.Enqueue(candidato);
                }

                proxima = ObterProximaAlternativa();
            }

            if (proxima != null)
            {
                alternativasSelecionadas.Add(proxima);
                memoria.UltimaSugestao = proxima;
            }

            var segunda = ObterProximaAlternativa();
            if (segunda != null)
            {
                alternativasSelecionadas.Add(segunda);
            }

            if (!alternativasSelecionadas.Any())
            {
                ContextoRecomendacoes.TryRemove(usuarioId, out _);
                return null;
            }

            var nomes = alternativasSelecionadas.Select(a => a.Nome).ToList();
            var mensagem = $"Entendido. Que tal experimentar {FormatarListaProdutos(nomes)}?";

            var produtosDto = alternativasSelecionadas
                .Select(a => ConverterSugestaoParaDto(a, usuarioId))
                .ToList();

            memoria.UltimaSugestao = alternativasSelecionadas.Last();

            return new ChatbotRespostaDTO
            {
                Tipo = "lista",
                Layout = "cards",
                Mensagem = mensagem,
                Produtos = produtosDto
            };
        }

        private static bool ContemGatilhoRecomendacao(string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
            {
                return false;
            }

            var mensagemLower = mensagem.ToLowerInvariant();
            return GatihosRecomendacao.Any(gatilho => mensagemLower.Contains(gatilho));
        }

        private class RecomendacaoMemoria
        {
            public string? Categoria { get; set; }
            public string? NomeReferencia { get; set; }
            public Queue<ProdutoSugestao> Alternativas { get; } = new();
            public HashSet<int> Rejeitados { get; } = new();
            public ProdutoSugestao? UltimaSugestao { get; set; }
        }

        private class ProdutoSugestao
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public string Categoria { get; set; } = string.Empty;
            public decimal Preco { get; set; }
            public string? Imagem { get; set; }
            public string? Descricao { get; set; }
        }

        private bool DetectarForaDeEscopo(string mensagem)
        {
            var mensagemLower = mensagem.ToLowerInvariant();
            var termosForaEscopo = new[] { "previsão do tempo", "clima", "tempo amanhã", "cotação", "dólar", "bitcoin" };
            return termosForaEscopo.Any(termo => mensagemLower.Contains(termo));
        }

        private async Task<ChatbotRespostaDTO> GerarRespostaFallback(string mensagemUsuario, Dictionary<string, object> contexto, int usuarioId)
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
                        .Select(p => CriarProdutoDto(p, usuarioId))
                        .ToList();

                    if (produtosFutebol.Any())
                    {
                        return new ChatbotRespostaDTO
                        {
                            Tipo = "lista",
                            Layout = "cards",
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
                        .Select(p => CriarProdutoDto(p, usuarioId))
                        .ToList();

                    if (produtosBasquete.Any())
                    {
                        return new ChatbotRespostaDTO
                        {
                            Tipo = "lista",
                            Layout = "cards",
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

        private ProdutoDTO CriarProdutoDto(ProdutoSugestao sugestao, int usuarioId)
        {
            return new ProdutoDTO
            {
                Id = sugestao.Id.ToString(),
                Nome = sugestao.Nome,
                Preco = sugestao.Preco,
                Categoria = sugestao.Categoria,
                ImagemUrl = sugestao.Imagem ?? string.Empty,
                LinkProduto = $"/produto/{sugestao.Id}",
                DescricaoCurta = ConstruirDescricaoCurta(sugestao.Descricao),
                Acao = CriarAcaoAdicionarCarrinho(usuarioId, sugestao.Id, sugestao.Preco, sugestao.Nome)
            };
        }
    }
}
