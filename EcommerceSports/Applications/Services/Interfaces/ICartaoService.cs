using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface ICartaoService
    {
        Task CadastrarCartao(int id, CartaoDTO cartao);
    }
}
