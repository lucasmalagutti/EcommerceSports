using EcommerceSports.Applications.DTO;
using EcommerceSports.Data.Context;
using EcommerceSports.Data.Infra.Interfaces;
using EcommerceSports.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Data.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> BuscarPorId(int id)
        {
            return await _context.Clientes
            .Include(c => c.Endereco)
            .Include(c => c.Telefones)
            .Include(c => c.Cartoes)
            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente?> BuscarPorCpf(string cpf)
        {
            return await _context.Clientes
            .Include(c => c.Endereco)
            .Include(c => c.Telefones)
            .Include(c => c.Cartoes)
            .FirstOrDefaultAsync(c => c.Cpf == cpf);
        }

        public async Task AtualizarCliente(Cliente cliente)
        {
            try
            {
                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao atualizar cliente", ex);
            }
        }

        public async Task CadastrarCliente(Cliente cliente)
        {
            try
            {

                _context.Clientes.Add(cliente);
                
                _context.SaveChanges();
                
                foreach (var endereco in cliente.Endereco)
                {
                    endereco.ClienteId = cliente.Id;
                }
                
                foreach (var telefone in cliente.Telefones)
                {
                    telefone.ClienteId = cliente.Id;
                }
                
                foreach (var cartao in cliente.Cartoes)
                {
                    cartao.ClienteId = cliente.Id;
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException;
                var errorMessage = "Erro ao cadastrar cliente no banco de dados: ";
                
                if (innerException != null)
                {
                    errorMessage += innerException.Message;
                }
                else
                {
                    errorMessage += dbEx.Message;
                }
                
                throw new Exception(errorMessage, dbEx);
            }
            catch (Exception ex) 
            {
                throw new Exception($"Erro ao cadastrar cliente e dados relacionados: {ex.Message}", ex);
            }
        }

        public async Task<List<Cliente>> ListarTodos()
        {
            return await _context.Clientes
        .Include(c => c.Endereco)      
        .Include(c => c.Telefones)     
        .Include(c => c.Cartoes)       
        .ToListAsync();
        }

        public async Task<List<Cliente>> BuscarPorFiltro(ClienteFiltroDTO filtros)
        {
            var query = _context.Clientes
        .Include(c => c.Endereco)
        .Include(c => c.Telefones)
        .Include(c => c.Cartoes)
        .AsQueryable();

            if (!string.IsNullOrEmpty(filtros.Nome))
                query = query.Where(c => c.Nome.Contains(filtros.Nome));

            if (!string.IsNullOrEmpty(filtros.Cpf))
                query = query.Where(c => c.Cpf == filtros.Cpf);

            if (!string.IsNullOrEmpty(filtros.Email))
                query = query.Where(c => c.Email.Contains(filtros.Email));

            if (!string.IsNullOrEmpty(filtros.Telefone))
                query = query.Where(c => c.Telefones.Any(t => t.Numero.Contains(filtros.Telefone)));

            if (!string.IsNullOrEmpty(filtros.TipoTelefone))
                query = query.Where(c => c.Telefones.Any(t =>
                    t.TipoTelefone.ToString().ToLower() == filtros.TipoTelefone.ToLower()
                ));

            if (!string.IsNullOrEmpty(filtros.Status))
            {
                bool ativo = filtros.Status.ToLower() == "ativo";
                query = query.Where(c => c.CadastroAtivo == ativo);
            }

            return await query.ToListAsync();
        }
    }
}

