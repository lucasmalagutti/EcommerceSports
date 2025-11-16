using System.Text;
using System.Text.Json;
using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EcommerceSports.Applications.Services
{
    public class GoogleGeminiService : IGoogleGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleGeminiService>? _logger;
        private const int TimeoutSeconds = 15;
        private const int MaxRetries = 1;

        public GoogleGeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GoogleGeminiService>? logger = null)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
        }

        public async Task<string> GerarConteudo(string prompt, object? contexto = null)
        {
            var apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger?.LogError("Chave de API do Gemini não configurada");
                throw new InvalidOperationException("Chave de API do Gemini não configurada em appsettings.json");
            }

            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            int retryCount = 0;
            while (retryCount <= MaxRetries)
            {
                try
                {
                    _logger?.LogInformation("Enviando requisição para Gemini API (tentativa {Tentativa})", retryCount + 1);
                    var response = await _httpClient.PostAsync(endpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        _logger?.LogDebug("Resposta do Gemini recebida: {Resposta}", responseString);

                        try
                        {
                            using (JsonDocument doc = JsonDocument.Parse(responseString))
                            {
                                var candidates = doc.RootElement.GetProperty("candidates");
                                if (candidates.GetArrayLength() == 0)
                                {
                                    _logger?.LogWarning("Resposta do Gemini sem candidatos");
                                    return "{}";
                                }
                                
                                var text = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                                _logger?.LogInformation("Texto extraído do Gemini: {Texto}", text);
                                return text ?? "{}";
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Erro ao parsear resposta do Gemini. Resposta: {Resposta}", responseString);
                            return "{}";
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger?.LogError("Erro na API Gemini. Status: {Status}, Resposta: {Resposta}", response.StatusCode, errorContent);
                        
                        if (response.StatusCode >= System.Net.HttpStatusCode.InternalServerError && retryCount < MaxRetries)
                        {
                            retryCount++;
                            await Task.Delay(1000);
                            continue;
                        }
                        return "{}";
                    }
                }
                catch (TaskCanceledException ex) when (retryCount < MaxRetries)
                {
                    _logger?.LogWarning(ex, "Timeout na requisição ao Gemini (tentativa {Tentativa})", retryCount + 1);
                    retryCount++;
                    await Task.Delay(1000);
                    continue;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Erro inesperado ao chamar Gemini API");
                    if (retryCount < MaxRetries)
                    {
                        retryCount++;
                        await Task.Delay(1000);
                        continue;
                    }
                    return "{}";
                }
            }

            _logger?.LogError("Falha ao obter resposta do Gemini após {Tentativas} tentativas", MaxRetries + 1);
            return "{}";
        }
    }
}

