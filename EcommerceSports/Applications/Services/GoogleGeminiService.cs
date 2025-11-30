using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EcommerceSports.Applications.Services
{
    public class GoogleGeminiService : IGoogleGeminiService, IDisposable, IAsyncDisposable
    {
        private readonly Client _client;
        private readonly ILogger<GoogleGeminiService>? _logger;
        private const string ModelName = "gemini-2.5-flash";

        public GoogleGeminiService(IConfiguration configuration, ILogger<GoogleGeminiService>? logger = null)
        {
            _logger = logger;

            var apiKey = configuration["GoogleGenAI:ApiKey"] ?? configuration["Gemini:ApiKey"];
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                if (string.IsNullOrWhiteSpace(System.Environment.GetEnvironmentVariable("GEMINI_API_KEY")))
                {
                    System.Environment.SetEnvironmentVariable("GEMINI_API_KEY", apiKey);
                }

                _client = new Client(apiKey: apiKey);
            }
            else
            {
                var envKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                if (string.IsNullOrWhiteSpace(envKey))
                {
                    _logger?.LogWarning("Nenhuma chave de API Gemini configurada via user-secrets ou variável de ambiente.");
                }

                _client = new Client();
            }
        }

        public async Task<string> GerarConteudo(string prompt, object? contexto = null)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                _logger?.LogWarning("Prompt vazio recebido ao chamar Gemini.");
                return "{}";
            }

            try
            {
                _logger?.LogInformation("Enviando prompt ao Gemini usando modelo {Modelo}.", ModelName);

                var userContent = new Content
                {
                    Role = "user",
                    Parts = new List<Part>
                    {
                        new Part { Text = prompt }
                    }
                };

                if (contexto != null)
                {
                    var contextoJson = JsonSerializer.Serialize(contexto, new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });

                    userContent.Parts.Add(new Part
                    {
                        Text = $"Contexto JSON:\n{contextoJson}"
                    });
                }

                var response = await _client.Models.GenerateContentAsync(ModelName, userContent);
                var texto = response?.Candidates?
                    .FirstOrDefault(candidate => candidate?.Content?.Parts?.Any() == true)?
                    .Content?.Parts?
                    .FirstOrDefault(part => !string.IsNullOrWhiteSpace(part.Text))?
                    .Text;

                if (string.IsNullOrWhiteSpace(texto))
                {
                    _logger?.LogWarning("Resposta do Gemini sem conteúdo textual retornado.");
                    return "{}";
                }

                return texto;
            }
            catch (ClientError ex)
            {
                _logger?.LogError(ex, "Erro de cliente ao chamar a API Gemini.");
                return "{}";
            }
            catch (ServerError ex)
            {
                _logger?.LogError(ex, "Erro de servidor ao chamar a API Gemini.");
                return "{}";
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado ao chamar a API Gemini.");
                return "{}";
            }
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
        {
            return _client.DisposeAsync();
        }
    }
}

