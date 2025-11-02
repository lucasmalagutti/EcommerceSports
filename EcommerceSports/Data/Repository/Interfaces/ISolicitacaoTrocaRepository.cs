using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface ISolicitacaoTrocaRepository
    {
        Task<SolicitacaoTroca> CriarSolicitacaoTrocaAsync(SolicitacaoTroca solicitacao);
        Task<SolicitacaoTroca?> ObterSolicitacaoPorIdAsync(int id);
        Task<IEnumerable<SolicitacaoTroca>> ObterSolicitacoesPorClienteAsync(int clienteId);
        Task<IEnumerable<SolicitacaoTroca>> ObterTodasSolicitacoesAsync();
        Task<SolicitacaoTroca?> AtualizarStatusAsync(int solicitacaoId, StatusSolicitacaoTroca novoStatus);
        Task<SolicitacaoTroca?> AtualizarSolicitacaoAsync(SolicitacaoTroca solicitacao);
    }
}

