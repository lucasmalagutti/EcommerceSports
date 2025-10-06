using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface IPagamentoRepository
    {
        Task<Pagamento> CriarPagamentoAsync(Pagamento pagamento);
        Task<List<Pagamento>> CriarPagamentosAsync(List<Pagamento> pagamentos);
        Task<List<Pagamento>> ObterPagamentosPorTransacaoAsync(int transacaoId);
        Task<Pagamento?> ObterPagamentoPorIdAsync(int id);
    }
}
