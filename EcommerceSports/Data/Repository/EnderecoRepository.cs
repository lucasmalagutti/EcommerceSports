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


    }
}
