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

        public async Task EditarEndereco(int id, Endereco endereco)
        {
            var enderecoExistente = await _context.Enderecos.FindAsync(id);
            if (enderecoExistente != null)
            {
                enderecoExistente.TipoEndereco = endereco.TipoEndereco;
                enderecoExistente.TipoResidencia = endereco.TipoResidencia;
                enderecoExistente.TipoLogradouro = endereco.TipoLogradouro;
                enderecoExistente.Nome = endereco.Nome;
                enderecoExistente.Logradouro = endereco.Logradouro;
                enderecoExistente.Numero = endereco.Numero;
                enderecoExistente.Cep = endereco.Cep;
                enderecoExistente.Bairro = endereco.Bairro;
                enderecoExistente.Cidade = endereco.Cidade;
                enderecoExistente.Estado = endereco.Estado;
                enderecoExistente.Pais = endereco.Pais;
                enderecoExistente.Observacoes = endereco.Observacoes;

                await _context.SaveChangesAsync();
            }
        }
    }
}
