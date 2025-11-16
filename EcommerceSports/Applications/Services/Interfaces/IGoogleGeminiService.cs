using System.Threading.Tasks;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IGoogleGeminiService
    {
        Task<string> GerarConteudo(string prompt, object? contexto = null);
    }
}

