using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<Produto?> ObterProdutoAsync(int produtoId);
        Task<bool> AtualizarEstoqueAsync(int produtoId, int quantidade);
        Task<bool> VerificarEstoqueDisponivelAsync(int produtoId, int quantidade);
        Task<List<Produto>> ObterProdutosPorIdsAsync(List<int> produtoIds);
    }
}
