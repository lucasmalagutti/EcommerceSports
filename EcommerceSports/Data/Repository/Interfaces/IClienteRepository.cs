using EcommerceSports.Models;

namespace EcommerceSports.Data.Infra.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente> BuscarClientePorId(int id);
        Task<IEnumerable<Cliente>> ListarClientes();
        Task CadastrarCliente(Cliente cliente, List<Endereco> enderecos, Telefone telefone, CartaoCredito cartao);
        Task EditarCliente(int id, Cliente cliente);
        Task InativarCliente(int id);
        Task AlterarSenha(int id, string novaSenha);
        Task ContarClientesAtivos();
        Task<IEnumerable<Endereco>> ListarEnderecosCliente(int clienteId);
        Task<IEnumerable<CartaoCredito>> ListarCartoesCliente(int clienteId);



    }
}