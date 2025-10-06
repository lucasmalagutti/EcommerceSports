using Microsoft.EntityFrameworkCore;
using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly AppDbContext _context;

        public TransacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transacao> CriarTransacaoAsync(Transacao transacao)
        {
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
            return transacao;
        }

        public async Task<Transacao?> ObterTransacaoPorIdAsync(int id)
        {
            return await _context.Transacoes
                .Include(t => t.Pedido)
                .Include(t => t.Endereco)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transacao?> ObterTransacaoPorPedidoIdAsync(int pedidoId)
        {
            return await _context.Transacoes
                .Include(t => t.Pedido)
                .Include(t => t.Endereco)
                .FirstOrDefaultAsync(t => t.PedidoId == pedidoId);
        }

        public async Task<bool> ExisteTransacaoParaPedidoAsync(int pedidoId)
        {
            return await _context.Transacoes
                .AnyAsync(t => t.PedidoId == pedidoId);
        }
    }
}
