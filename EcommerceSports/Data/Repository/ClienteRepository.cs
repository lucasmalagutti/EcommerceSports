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

        public void CadastrarCliente(Cliente cliente)
        {
            try
            {
                _context.Clientes.Add(cliente);
                _context.SaveChanges();
            }
            catch (Exception ex) 
            {
                throw new Exception("Erro ao cadastrar cliente e dados relacionados", ex);
            }

        }
    }
}

