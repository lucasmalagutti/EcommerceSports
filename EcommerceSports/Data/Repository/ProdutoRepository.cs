using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceSports.Data.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Produto>> ListarTodos()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<IEnumerable<Produto>> BuscarProdutosPorFiltro(string? categoria, List<string>? termosDeBusca)
        {
            var query = _context.Produtos.AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(p => EF.Functions.ILike(p.Categoria, $"%{categoria}%"));
            }

            if (termosDeBusca != null && termosDeBusca.Any())
            {
                foreach (var termo in termosDeBusca)
                {
                    var pattern = $"%{termo}%";
                    query = query.Where(p =>
                        EF.Functions.ILike(p.Nome, pattern) ||
                        EF.Functions.ILike(p.Descricao ?? string.Empty, pattern));
                }
            }

            return await query.ToListAsync();
        }
    }
}
