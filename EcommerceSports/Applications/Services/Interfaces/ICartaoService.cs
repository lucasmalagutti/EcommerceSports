using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface ICartaoService
    {
        Task<ResponseCartaoDTO> CadastrarCartao(int clienteId, CadastrarCartaoDTO cartao);
        Task<List<ResponseCartaoDTO>> ListarCartoesPorCliente(int clienteId);
        Task<ResponseCartaoDTO?> ObterCartaoPorId(int cartaoId);
    }
}
