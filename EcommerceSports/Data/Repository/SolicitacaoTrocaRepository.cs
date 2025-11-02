using Microsoft.EntityFrameworkCore;
using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Data.Repository
{
    public class SolicitacaoTrocaRepository : ISolicitacaoTrocaRepository
    {
        private readonly AppDbContext _context;

        public SolicitacaoTrocaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SolicitacaoTroca> CriarSolicitacaoTrocaAsync(SolicitacaoTroca solicitacao)
        {
            _context.SolicitacoesTroca.Add(solicitacao);
            await _context.SaveChangesAsync();
            return solicitacao;
        }

        public async Task<SolicitacaoTroca?> ObterSolicitacaoPorIdAsync(int id)
        {
            return await _context.SolicitacoesTroca
                .Include(s => s.Pedido)
                    .ThenInclude(p => p!.Cliente)
                .Include(s => s.ItemPedido)
                    .ThenInclude(i => i!.Produto)
                .Include(s => s.Cliente)
                .Include(s => s.Cupom)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SolicitacaoTroca>> ObterSolicitacoesPorClienteAsync(int clienteId)
        {
            return await _context.SolicitacoesTroca
                .Include(s => s.Pedido)
                    .ThenInclude(p => p!.Cliente)
                .Include(s => s.ItemPedido)
                    .ThenInclude(i => i!.Produto)
                .Include(s => s.Cliente)
                .Include(s => s.Cupom)
                .Where(s => s.ClienteId == clienteId)
                .OrderByDescending(s => s.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<SolicitacaoTroca>> ObterTodasSolicitacoesAsync()
        {
            return await _context.SolicitacoesTroca
                .Include(s => s.Pedido)
                    .ThenInclude(p => p!.Cliente)
                .Include(s => s.ItemPedido)
                    .ThenInclude(i => i!.Produto)
                .Include(s => s.Cliente)
                .Include(s => s.Cupom)
                .OrderByDescending(s => s.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<SolicitacaoTroca?> AtualizarStatusAsync(int solicitacaoId, StatusSolicitacaoTroca novoStatus)
        {
            var solicitacao = await _context.SolicitacoesTroca
                .FirstOrDefaultAsync(s => s.Id == solicitacaoId);

            if (solicitacao == null)
            {
                return null;
            }

            solicitacao.Status = novoStatus;

            if (novoStatus == StatusSolicitacaoTroca.Aprovada || novoStatus == StatusSolicitacaoTroca.Negada)
            {
                solicitacao.DataAprovacao = DateTime.UtcNow;
            }

            if (novoStatus == StatusSolicitacaoTroca.ProdutoRecebido)
            {
                solicitacao.DataRecebimento = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return solicitacao;
        }

        public async Task<SolicitacaoTroca?> AtualizarSolicitacaoAsync(SolicitacaoTroca solicitacao)
        {
            var solicitacaoExistente = await _context.SolicitacoesTroca
                .FirstOrDefaultAsync(s => s.Id == solicitacao.Id);

            if (solicitacaoExistente == null)
            {
                return null;
            }

            solicitacaoExistente.Status = solicitacao.Status;
            solicitacaoExistente.ObservacoesAdm = solicitacao.ObservacoesAdm;
            solicitacaoExistente.DataAprovacao = solicitacao.DataAprovacao;
            solicitacaoExistente.DataRecebimento = solicitacao.DataRecebimento;
            solicitacaoExistente.CupomId = solicitacao.CupomId;
            solicitacaoExistente.ValorCupom = solicitacao.ValorCupom;

            await _context.SaveChangesAsync();
            return solicitacaoExistente;
        }
    }
}

