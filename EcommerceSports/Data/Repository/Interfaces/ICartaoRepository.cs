using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface ICartaoRepository
    {
        Task<CartaoCredito> CadastrarCartao(CartaoCredito cartao);
        Task<List<CartaoCredito>> ListarCartoesPorCliente(int clienteId);
    }
}
