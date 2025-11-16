using System.Threading.Tasks;
using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IChatbotService
    {
        Task<ChatbotRespostaDTO> ProcessarMensagem(string mensagemUsuario, int? usuarioId = null);
    }
}

