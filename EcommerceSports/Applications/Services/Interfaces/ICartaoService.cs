using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface ICartaoService
    {
        Task<ResponseCartaoDTO> CadastrarCartao(CadastrarCartaoDTO cartao);
        Task<List<ResponseCartaoDTO>> ListarCartoesPorCliente(int clienteId);
        Task<ResponseCartaoDTO?> ObterCartaoPorId(int cartaoId);
        Task<bool> ValidarBandeiraCartao(int bandeira);
    }
}
