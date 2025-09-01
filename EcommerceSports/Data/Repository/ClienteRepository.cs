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

        public void CadastrarCliente(Cliente cliente, List<Endereco> enderecos, Telefone telefone, CartaoCredito cartao)
        {
            _context.Clientes.Add(cliente);
            _context.Enderecos.AddRange(enderecos);
            _context.Telefones.Add(telefone);
            _context.Cartoes.Add(cartao);

        }
    }
}

