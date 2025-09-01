using EcommerceSports.Data.Context;
using EcommerceSports.Data.Infra.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task AlterarSenha(int id, string novaSenha)
        {
            throw new NotImplementedException();
        }

        public Task<Cliente> BuscarClientePorId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task CadastrarCliente(Cliente cliente, List<Endereco> enderecos, Telefone telefone, CartaoCredito cartao)
        {
            await _context.Clientes.AddAsync(cliente);

            foreach (var endereco in enderecos)
            {
                endereco.ClienteId = cliente.Id;
                await _context.Enderecos.AddAsync(endereco);
            }

            telefone.ClienteId = cliente.Id;
            await _context.Telefones.AddAsync(telefone);

            cartao.ClienteId = cliente.Id;
            await _context.Cartoes.AddAsync(cartao);

            await _context.SaveChangesAsync();
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
