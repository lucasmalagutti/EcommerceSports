using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Data.Repository
{
    public class CartaoRepository : ICartaoRepository
    {
        private readonly AppDbContext _context;

        public CartaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartaoCredito> CadastrarCartao(CartaoCredito cartao)
        {
            _context.Cartoes.Add(cartao);
            await _context.SaveChangesAsync();
            return cartao;
        }

        public async Task<List<CartaoCredito>> ListarCartoesPorCliente(int clienteId)
        {
            return await _context.Cartoes
                .Where(c => c.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<CartaoCredito?> ObterCartaoPorId(int cartaoId)
        {
            return await _context.Cartoes
                .FirstOrDefaultAsync(c => c.Id == cartaoId);
        }

        public async Task<bool> ExisteCartaoComNumero(string numCartao, int clienteId)
        {
            return await _context.Cartoes
                .AnyAsync(c => c.NumCartao == numCartao && c.ClienteId == clienteId);
        }
    }
}
