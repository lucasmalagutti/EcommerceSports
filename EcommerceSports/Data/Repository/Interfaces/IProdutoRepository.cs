using EcommerceSports.Models.Entity;
using System.Collections.Generic;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> ListarTodos();
        Task<IEnumerable<Produto>> BuscarProdutosPorFiltro(string? categoria, List<string>? termosDeBusca);
    }
}
