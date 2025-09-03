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

        public void CadastrarCliente(Cliente cliente)
        {
            try
            {

                // Adicionar o cliente primeiro
                _context.Clientes.Add(cliente);
                
                // Salvar para gerar o ID do cliente
                _context.SaveChanges();
                
                // Agora configurar os IDs das entidades relacionadas
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
                
                // Salvar novamente para persistir as entidades relacionadas
                _context.SaveChanges();
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
    
        
    }
}

