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
                    .ThenInclude(p => p!.Itens)
                        .ThenInclude(i => i.Produto)
                .Include(t => t.Endereco)
                .FirstOrDefaultAsync(t => t.PedidoId == pedidoId);
        }

        public async Task<bool> ExisteTransacaoParaPedidoAsync(int pedidoId)
        {
            return await _context.Transacoes
                .AnyAsync(t => t.PedidoId == pedidoId);
        }

        public async Task<IEnumerable<Transacao>> ObterPorCliente(int clienteId)
        {
            return await _context.Transacoes
                .Include(t => t.Pedido)
                    .ThenInclude(p => p!.Itens)
                        .ThenInclude(i => i.Produto)
                .Include(t => t.Endereco)
                .Where(t => t.Pedido!.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<Transacao?> AtualizarStatusPedidoAsync(int pedidoId, Models.Enums.StatusPedido novoStatus)
        {
            var transacao = await _context.Transacoes
                .Include(t => t.Pedido)
                .FirstOrDefaultAsync(t => t.PedidoId == pedidoId);

            if (transacao == null || transacao.Pedido == null)
            {
                return null;
            }

            transacao.Pedido.StatusPedido = novoStatus;
            await _context.SaveChangesAsync();

            return transacao;
        }

        public async Task<Transacao?> AtualizarStatusTransacaoAsync(int transacaoId, Models.Enums.StatusTransacao novoStatus)
        {
            var transacao = await _context.Transacoes
                .FirstOrDefaultAsync(t => t.Id == transacaoId);

            if (transacao == null)
            {
                return null;
            }

            transacao.StatusTransacao = novoStatus;
            await _context.SaveChangesAsync();

            return transacao;
        }
        public async Task<IEnumerable<Transacao>> ListarTodasTransacoes()
        {
            return await _context.Transacoes
             .Include(t => t.Pedido)
                 .ThenInclude(p => p.Itens)
                     .ThenInclude(i => i.Produto)
             .Include(t => t.Endereco)
             .ToListAsync();
        }
        public async Task<List<Transacao>> ObterTransacoesPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            dataInicio = DateTime.SpecifyKind(dataInicio, DateTimeKind.Utc);
            dataFim = DateTime.SpecifyKind(dataFim, DateTimeKind.Utc);

            return await _context.Transacoes
                .Include(t => t.Pedido)
                    .ThenInclude(p => p.Itens)
                        .ThenInclude(i => i.Produto)
                .Where(t => t.DataTransacao >= dataInicio && t.DataTransacao <= dataFim)
                .ToListAsync(); 
        }
    }
}
