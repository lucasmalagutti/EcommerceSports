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

        public void CadastrarCliente(Cliente cliente)
        {
            try
            {
                // Verificar se o cliente já existe
                var clienteExistente = _context.Clientes.FirstOrDefault(c => c.Cpf == cliente.Cpf);
                if (clienteExistente != null)
                {
                    throw new Exception($"Cliente com CPF {cliente.Cpf} já está cadastrado.");
                }

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

