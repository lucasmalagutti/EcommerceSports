using EcommerceSports.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface ICarrinhoRepository
    {
        Task<Pedido?> ObterCarrinhoAtivoAsync(int clienteId);
        Task<Pedido> CriarCarrinhoAsync(int clienteId);
        Task<ItemPedido?> ObterItemCarrinhoAsync(int pedidoId, int produtoId);
        Task<ItemPedido> AdicionarItemCarrinhoAsync(int pedidoId, int produtoId, int quantidade, decimal precoUnitario);
        Task AtualizarQuantidadeItemAsync(int itemId, int quantidade);
        Task RemoverItemCarrinhoAsync(int itemId);
        Task LimparCarrinhoAsync(int pedidoId);
        Task<decimal> CalcularValorTotalAsync(int pedidoId);
        Task AtualizarValorTotalPedidoAsync(int pedidoId);
        Task<bool> ExisteProdutoAsync(int produtoId);
        Task<Produto?> ObterProdutoAsync(int produtoId);
    }
}
