using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;

namespace EcommerceSports.Applications.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IGoogleGeminiService _geminiService;

        public ChatbotService(IProdutoRepository produtoRepository, IGoogleGeminiService geminiService)
        {
            _produtoRepository = produtoRepository;
            _geminiService = geminiService;
        }

        public async Task<List<ListarProdutosDTO>> ProcessarMensagem(string mensagemUsuario)
        {
            string prompt = CriarPromptParaGemini(mensagemUsuario);
            string respostaBruta = await _geminiService.GerarConteudo(prompt);
            string respostaJson = ExtrairJsonValido(respostaBruta);

            ChatbotFiltrosDTO filtros;

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                filtros = JsonSerializer.Deserialize<ChatbotFiltrosDTO>(respostaJson, options) ?? new ChatbotFiltrosDTO();
            }
            catch
            {
                filtros = new ChatbotFiltrosDTO();
            }

            NormalizarFiltros(filtros);

            if (!PossuiFiltrosValidos(filtros))
            {
                return new List<ListarProdutosDTO>();
            }

            var produtosEncontrados = await _produtoRepository.BuscarProdutosPorFiltro(
                filtros.Categoria,
                filtros.TermosDeBusca
            );

            return produtosEncontrados.Select(p => new ListarProdutosDTO
            {
                Id = p.Id,
                Nome = p.Nome ?? string.Empty,
                Descricao = p.Descricao ?? string.Empty,
                Preco = p.Preco,
                Categoria = p.Categoria ?? string.Empty,
                Imagem = p.Imagem ?? string.Empty
            }).ToList();
        }

        private string CriarPromptParaGemini(string mensagemUsuario)
        {
            return @$"
                Analise a seguinte mensagem de um usuário de e-commerce de esportes: '{mensagemUsuario}'.
                Sua tarefa é extrair a categoria do produto e termos de busca relevantes.
                Responda APENAS com um objeto JSON válido, contendo 'categoria' (string ou null) e 'termosDeBusca' (um array de strings ou null).
                Use o seguinte formato:
                {{
                  ""categoria"": ""nome_da_categoria"",
                  ""termosDeBusca"": [""termo1"", ""termo2""]
                }}
                - Se o usuário pedir por ""chuteira de society"", a categoria é ""Chuteira"" e termosDeBusca é [""society""].
                - Se o usuário pedir por ""tênis de corrida confortável"", a categoria é ""Tênis"" e termosDeBusca é [""corrida"", ""confortável""].
                - Se o usuário pedir por ""camisa do Flamengo"", a categoria é ""Camisa de Time"" e termosDeBusca é [""Flamengo""].
                - Se não conseguir identificar, retorne: {{ ""categoria"": null, ""termosDeBusca"": null }}
                Não adicione NENHUM texto, explicação ou formatação (como markdown ```json) antes ou depois do JSON.
            ";
        }

        private static string ExtrairJsonValido(string respostaGemini)
        {
            if (string.IsNullOrWhiteSpace(respostaGemini))
            {
                return "{}";
            }

            var texto = respostaGemini.Trim();

            if (texto.StartsWith("```", StringComparison.Ordinal))
            {
                var sb = new StringBuilder();
                foreach (var linha in texto.Split('\n'))
                {
                    var linhaTrim = linha.Trim();
                    if (linhaTrim.StartsWith("```", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    sb.AppendLine(linha);
                }

                texto = sb.ToString().Trim();
            }

            var primeiroAbre = texto.IndexOf('{');
            var ultimoFecha = texto.LastIndexOf('}');

            if (primeiroAbre >= 0 && ultimoFecha >= primeiroAbre)
            {
                texto = texto.Substring(primeiroAbre, ultimoFecha - primeiroAbre + 1);
            }

            return string.IsNullOrWhiteSpace(texto) ? "{}" : texto;
        }

        private static void NormalizarFiltros(ChatbotFiltrosDTO filtros)
        {
            if (filtros == null)
            {
                return;
            }

            filtros.Categoria = string.IsNullOrWhiteSpace(filtros.Categoria)
                ? null
                : filtros.Categoria.Trim();

            if (filtros.TermosDeBusca != null)
            {
                filtros.TermosDeBusca = filtros.TermosDeBusca
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (filtros.TermosDeBusca.Count == 0)
                {
                    filtros.TermosDeBusca = null;
                }
            }
        }

        private static bool PossuiFiltrosValidos(ChatbotFiltrosDTO filtros)
        {
            if (filtros == null)
            {
                return false;
            }

            var temCategoria = !string.IsNullOrWhiteSpace(filtros.Categoria);
            var temTermos = filtros.TermosDeBusca != null && filtros.TermosDeBusca.Any();

            return temCategoria || temTermos;
        }
    }
}

