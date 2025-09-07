using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Data.Repository
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly AppDbContext _context;

        public EnderecoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarEndereco(int clienteId, Endereco endereco)
        {
            try
            {

                endereco.ClienteId = clienteId;

                // Adicionar o endereço
                _context.Enderecos.Add(endereco);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException;
                var errorMessage = "Erro ao adicionar endereço no banco de dados: ";
                

                throw new Exception(errorMessage, dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar endereço: {ex.Message}", ex);
            }
        }

        public Task CadastrarEndereco(int id, Endereco endereco)
        {
            throw new NotImplementedException();
        }

        public Task EditarEndereco(int id, Endereco endereco)
        {
            throw new NotImplementedException();
        }
    }
}
