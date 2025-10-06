using Microsoft.EntityFrameworkCore;
using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly AppDbContext _context;

        public EstoqueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Produto?> ObterProdutoAsync(int produtoId)
        {
            return await _context.Produtos.FindAsync(produtoId);
        }

        public async Task<bool> AtualizarEstoqueAsync(int produtoId, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null)
            {
                return false;
            }

            // Verificar se há estoque suficiente
            if (produto.QtdEstoque < quantidade)
            {
                return false;
            }

            // Reduzir o estoque
            produto.QtdEstoque -= quantidade;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerificarEstoqueDisponivelAsync(int produtoId, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null)
            {
                return false;
            }

            return produto.QtdEstoque >= quantidade;
        }

        public async Task<List<Produto>> ObterProdutosPorIdsAsync(List<int> produtoIds)
        {
            return await _context.Produtos
                .Where(p => produtoIds.Contains(p.Id))
                .ToListAsync();
        }
    }
}
