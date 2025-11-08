using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<Transacao> CriarTransacaoAsync(Transacao transacao);
        Task<Transacao?> ObterTransacaoPorIdAsync(int id);
        Task<Transacao?> ObterTransacaoPorPedidoIdAsync(int pedidoId);
        Task<bool> ExisteTransacaoParaPedidoAsync(int pedidoId);
        Task<IEnumerable<Transacao>> ObterPorCliente(int clienteId);
        Task<Transacao?> AtualizarStatusPedidoAsync(int pedidoId, Models.Enums.StatusPedido novoStatus);
        Task<Transacao?> AtualizarStatusTransacaoAsync(int transacaoId, Models.Enums.StatusTransacao novoStatus);
        Task<IEnumerable<Transacao>> ListarTodasTransacoes();
        Task<List<Transacao>> ObterTransacoesPorPeriodo(DateTime dataInicio, DateTime dataFim);
    }
}
