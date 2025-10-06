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
    }
}
