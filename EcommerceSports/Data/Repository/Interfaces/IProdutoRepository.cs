using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> ListarTodos();
    }
}
