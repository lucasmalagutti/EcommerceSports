using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface ITransacaoService
    {
        Task<ResponseTransacaoDTO> CriarTransacaoAsync(CriarTransacaoDTO criarDto);
        Task<ResponseTransacaoDTO> ObterTransacaoPorIdAsync(int id);
        Task<ResponseTransacaoDTO> ObterTransacaoPorPedidoIdAsync(int pedidoId);
    }
}
