using EcommerceSports.Data.Infra.Interfaces;
using EcommerceSports.Models;

namespace EcommerceSports.Data.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        public Task AlterarSenha(int id, string novaSenha)
        {
            throw new NotImplementedException();
        }

        public Task<Cliente> BuscarClientePorId(int id)
        {
            throw new NotImplementedException();
        }

        public Task CadastrarCliente(Cliente cliente, List<Endereco> enderecos, Telefone telefone, CartaoCredito cartao)
        {
            throw new NotImplementedException();
        }

        public Task<int> ContarClientesAtivos()
        {
            throw new NotImplementedException();
        }

        public Task EditarCliente(int id, Cliente cliente)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InativarCliente(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CartaoCredito>> ListarCartoesCliente(int clienteId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Cliente>> ListarClientes()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Endereco>> ListarEnderecosCliente(int clienteId)
        {
            throw new NotImplementedException();
        }
    }
}
