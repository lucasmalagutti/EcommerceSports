using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Services.Interfaces
{
    public interface IClienteService
    {
        Task<ResponseClienteDTO> BuscarClientePorId(int id);
        Task<IEnumerable<ResponseClienteDTO>> ListarClientes();
        Task CadastrarCliente(ClienteDTO clientedto);
        Task EditarCliente(int id, EditarClienteDTO dto);
        Task<bool> InativarCliente(int id);
        Task AlterarSenha(int id, string novaSenha);
        Task<int> ContarClientesAtivos();
        Task<IEnumerable<EnderecoDTO>> ListarEnderecosCliente(int clienteId);
        Task<IEnumerable<CartaoDTO>> ListarCartoesCliente(int clienteId);
        Task ValidarCpf(string cpf);
        Task ValidarExistenciaCliente(string cpf, int? id = null);
        Task ValidarSenhaForte(string senha);
        Task<int> AtribuirNumeroRanking();
        Task<string> CriptografarSenha(string senha);
        Task<bool> VerificarSenha(string senha, string hash);
    }
}
