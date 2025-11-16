using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

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

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                var categoriaTermo = categoria.Trim();
                query = query.Where(p => EF.Functions.ILike(p.Categoria, $"%{categoriaTermo}%"));
            }

            if (termosDeBusca != null && termosDeBusca.Any())
            {
                var termosValidos = termosDeBusca
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (!termosValidos.Any())
                {
                    return new List<Produto>();
                }

                foreach (var termo in termosValidos)
                {
                    var pattern = $"%{termo}%";
                    decimal valorNumerico = 0m;
                    bool termoEhNumero =
                        decimal.TryParse(termo, NumberStyles.Number, CultureInfo.InvariantCulture, out valorNumerico) ||
                        decimal.TryParse(termo, NumberStyles.Number, new CultureInfo("pt-BR"), out valorNumerico);

                    var margemPreco = 0.01m;
                    query = query.Where(p =>
                        EF.Functions.ILike(p.Nome, pattern) ||
                        EF.Functions.ILike(p.Descricao ?? string.Empty, pattern) ||
                        EF.Functions.ILike(p.Categoria, pattern) ||
                        (termoEhNumero && (decimal)p.Preco >= valorNumerico - margemPreco && (decimal)p.Preco <= valorNumerico + margemPreco));
                }
            }
            else
            {
                return new List<Produto>();
            }

            return await query.ToListAsync();
        }
    }
}
