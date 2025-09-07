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

        public async Task CadastrarEndereco(int id, Endereco endereco)
        {
            endereco.ClienteId = id;
            _context.Enderecos.Add(endereco);
            await _context.SaveChangesAsync();
        }

        public async Task<Endereco?> BuscarEnderecoPorId(int id)
        {
            return await _context.Enderecos.FindAsync(id);
        }

        public async Task EditarEndereco(int id, Endereco endereco)
        {
            var enderecoExistente = await _context.Enderecos.FindAsync(id);
            if (enderecoExistente != null)
            {
                if (!string.IsNullOrEmpty(endereco.Nome))
                    enderecoExistente.Nome = endereco.Nome;
                
                if (!string.IsNullOrEmpty(endereco.Logradouro))
                    enderecoExistente.Logradouro = endereco.Logradouro;
                
                if (!string.IsNullOrEmpty(endereco.Numero))
                    enderecoExistente.Numero = endereco.Numero;
                
                if (!string.IsNullOrEmpty(endereco.Cep))
                    enderecoExistente.Cep = endereco.Cep;
                
                if (!string.IsNullOrEmpty(endereco.Bairro))
                    enderecoExistente.Bairro = endereco.Bairro;
                
                if (!string.IsNullOrEmpty(endereco.Cidade))
                    enderecoExistente.Cidade = endereco.Cidade;
                
                if (!string.IsNullOrEmpty(endereco.Estado))
                    enderecoExistente.Estado = endereco.Estado;
                
                if (!string.IsNullOrEmpty(endereco.Pais))
                    enderecoExistente.Pais = endereco.Pais;
                
                if (endereco.Observacoes != null)
                    enderecoExistente.Observacoes = endereco.Observacoes;

                enderecoExistente.TipoResidencia = endereco.TipoResidencia;
                enderecoExistente.TipoLogradouro = endereco.TipoLogradouro;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Endereco>> ListarEnderecosPorCliente(int clienteId)
        {
            return await _context.Enderecos
                .Where(e => e.ClienteId == clienteId)
                .ToListAsync();
        }
    }
}
