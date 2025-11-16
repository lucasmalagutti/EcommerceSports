using EcommerceSports.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface IPedidoRepository
    {
        Task<List<ItemPedido>> ObterHistoricoComprasPorCliente(int clienteId, int limite = 20);
        Task<List<Produto>> ObterProdutosMaisVisualizados(int clienteId, int limite = 5);
    }
}

