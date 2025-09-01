using EcommerceSports.Models;

namespace EcommerceSports.Data.Repository.Interfaces
{ 
    public interface ICartaoRepository
    {
        Task CadastrarCartao(int id, CartaoCredito cartao);
    }
}
