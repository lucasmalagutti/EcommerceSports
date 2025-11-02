using Microsoft.EntityFrameworkCore;
using EcommerceSports.Data.Context;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository
{
    public class CupomRepository : ICupomRepository
    {
        private readonly AppDbContext _context;

        public CupomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cupom?> ObterCupomPorNomeAsync(string nome)
        {
            return await _context.Cupons
                .FirstOrDefaultAsync(c => c.Nome.ToUpper() == nome.ToUpper());
        }

        public async Task<bool> ExisteCupomAsync(string nome)
        {
            return await _context.Cupons
                .AnyAsync(c => c.Nome.ToUpper() == nome.ToUpper());
        }

        public async Task<Cupom> CriarCupomAsync(Cupom cupom)
        {
            _context.Cupons.Add(cupom);
            await _context.SaveChangesAsync();
            return cupom;
        }

        public async Task<bool> MarcarComoUtilizadoAsync(string nome)
        {
            var cupom = await _context.Cupons.FirstOrDefaultAsync(c => c.Nome.ToUpper() == nome.ToUpper());

            if (cupom == null)
            {
                return false;
            }

            if (cupom.Utilizado)
            {
                return true; // Já estava marcado como utilizado
            }

            cupom.Utilizado = true;
            cupom.DataUtilizacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
