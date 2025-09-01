using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Services.Interfaces
{
    public interface ICartaoService
    {
        Task CadastrarCartao(int id, CartaoDTO cartao);
    }
}
