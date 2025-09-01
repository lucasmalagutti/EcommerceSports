using EcommerceSports.Applications.DTO;
using EcommerceSports.Services.Interfaces;

namespace EcommerceSports.Services
{
    public class ClienteService : IClienteService
    {
        public Task AlterarSenha(int id, string novaSenha)
        {
            throw new NotImplementedException();
        }

        public Task<int> AtribuirNumeroRanking()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseClienteDTO> BuscarClientePorId(int id)
        {
            throw new NotImplementedException();
        }

        public Task CadastrarCliente(ClienteDTO clientedto)
        {
            throw new NotImplementedException();
        }

        public Task<int> ContarClientesAtivos()
        {
            throw new NotImplementedException();
        }

        public Task<string> CriptografarSenha(string senha)
        {
            throw new NotImplementedException();
        }

        public Task EditarCliente(int id, EditarClienteDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InativarCliente(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CartaoDTO>> ListarCartoesCliente(int clienteId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ResponseClienteDTO>> ListarClientes()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EnderecoDTO>> ListarEnderecosCliente(int clienteId)
        {
            throw new NotImplementedException();
        }

        public Task ValidarCpf(string cpf)
        {
            throw new NotImplementedException();
        }

        public Task ValidarExistenciaCliente(string cpf, int? id = null)
        {
            throw new NotImplementedException();
        }

        public Task ValidarSenhaForte(string senha)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerificarSenha(string senha, string hash)
        {
            throw new NotImplementedException();
        }
    }
}
