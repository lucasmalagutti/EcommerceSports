using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceSports.Data.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AppDbContext _context;

        public PedidoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ItemPedido>> ObterHistoricoComprasPorCliente(int clienteId, int limite = 20)
        {
            return await _context.ItensPedido
                .Include(ip => ip.Produto)
                .Include(ip => ip.Pedido)
                .Where(ip => ip.Pedido != null && ip.Pedido.ClienteId == clienteId)
                .OrderByDescending(ip => ip.Pedido!.DataPedido)
                .Take(limite)
                .ToListAsync();
        }

        public async Task<List<Produto>> ObterProdutosMaisVisualizados(int clienteId, int limite = 5)
        {
            // Como não há tabela de visualizações, retornamos produtos mais comprados pelo cliente
            var produtosIds = await _context.ItensPedido
                .Include(ip => ip.Pedido)
                .Where(ip => ip.Pedido != null && ip.Pedido.ClienteId == clienteId)
                .GroupBy(ip => ip.ProdutoId)
                .OrderByDescending(g => g.Sum(ip => ip.Quantidade))
                .Take(limite)
                .Select(g => g.Key)
                .ToListAsync();

            return await _context.Produtos
                .Where(p => produtosIds.Contains(p.Id))
                .ToListAsync();
        }
    }
}

