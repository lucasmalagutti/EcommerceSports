using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IEstoqueService
    {
        Task<bool> ReduzirEstoquePedidoAsync(int pedidoId);
        Task<bool> VerificarEstoqueDisponivelAsync(int produtoId, int quantidade);
        Task<bool> VerificarEstoquePedidoAsync(int pedidoId);
    }
}
