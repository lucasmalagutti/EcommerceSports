using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IChatbotService
    {
        Task<List<ListarProdutosDTO>> ProcessarMensagem(string mensagemUsuario);
    }
}

