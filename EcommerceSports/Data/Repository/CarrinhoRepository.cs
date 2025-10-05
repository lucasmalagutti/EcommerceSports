using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Data.Repository
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly AppDbContext _context;

        public CarrinhoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pedido?> ObterCarrinhoAtivoAsync(int clienteId)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.ClienteId == clienteId && p.StatusPedido == StatusPedido.EmProcessamento);
        }

        public async Task<Pedido> CriarCarrinhoAsync(int clienteId)
        {
            var carrinho = new Pedido
            {
                ClienteId = clienteId,
                DataPedido = DateTime.UtcNow,
                StatusPedido = StatusPedido.EmProcessamento,
                ValorTotal = 0
            };

            _context.Pedidos.Add(carrinho);
            await _context.SaveChangesAsync();
            return carrinho;
        }

        public async Task<ItemPedido?> ObterItemCarrinhoAsync(int pedidoId, int produtoId)
        {
            return await _context.ItensPedido
                .FirstOrDefaultAsync(i => i.PedidoId == pedidoId && i.ProdutoId == produtoId);
        }

        public async Task<ItemPedido> AdicionarItemCarrinhoAsync(int pedidoId, int produtoId, int quantidade, decimal precoUnitario)
        {
            var item = new ItemPedido
            {
                PedidoId = pedidoId,
                ProdutoId = produtoId,
                Quantidade = quantidade,
                PrecoUnitario = precoUnitario
            };

            _context.ItensPedido.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task AtualizarQuantidadeItemAsync(int itemId, int quantidade)
        {
            var item = await _context.ItensPedido.FindAsync(itemId);
            if (item != null)
            {
                item.Quantidade = quantidade;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoverItemCarrinhoAsync(int itemId)
        {
            var item = await _context.ItensPedido.FindAsync(itemId);
            if (item != null)
            {
                _context.ItensPedido.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task LimparCarrinhoAsync(int pedidoId)
        {
            var itens = await _context.ItensPedido
                .Where(i => i.PedidoId == pedidoId)
                .ToListAsync();

            _context.ItensPedido.RemoveRange(itens);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> CalcularValorTotalAsync(int pedidoId)
        {
            return await _context.ItensPedido
                .Where(i => i.PedidoId == pedidoId)
                .SumAsync(i => i.Quantidade * i.PrecoUnitario);
        }

        public async Task<bool> ExisteProdutoAsync(int produtoId)
        {
            return await _context.Produtos.AnyAsync(p => p.Id == produtoId);
        }

        public async Task<Produto?> ObterProdutoAsync(int produtoId)
        {
            return await _context.Produtos.FindAsync(produtoId);
        }
    }
}
