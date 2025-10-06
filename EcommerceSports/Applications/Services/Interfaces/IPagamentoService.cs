using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IPagamentoService
    {
        Task<ResponseTransacaoComPagamentosDTO> CriarTransacaoComPagamentosAsync(CriarTransacaoComPagamentosDTO criarDto);
        Task<ResponseTransacaoComPagamentosDTO> ObterTransacaoComPagamentosAsync(int transacaoId);
        Task<List<PagamentoDTO>> ObterPagamentosPorTransacaoAsync(int transacaoId);
    }
}
