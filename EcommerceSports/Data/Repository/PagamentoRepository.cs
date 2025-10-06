using Microsoft.EntityFrameworkCore;
using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly AppDbContext _context;

        public PagamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pagamento> CriarPagamentoAsync(Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();
            return pagamento;
        }

        public async Task<List<Pagamento>> CriarPagamentosAsync(List<Pagamento> pagamentos)
        {
            _context.Pagamentos.AddRange(pagamentos);
            await _context.SaveChangesAsync();
            return pagamentos;
        }

        public async Task<List<Pagamento>> ObterPagamentosPorTransacaoAsync(int transacaoId)
        {
            return await _context.Pagamentos
                .Include(p => p.Cartao)
                .Where(p => p.TransacaoId == transacaoId)
                .ToListAsync();
        }

        public async Task<Pagamento?> ObterPagamentoPorIdAsync(int id)
        {
            return await _context.Pagamentos
                .Include(p => p.Cartao)
                .Include(p => p.Transacao)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
