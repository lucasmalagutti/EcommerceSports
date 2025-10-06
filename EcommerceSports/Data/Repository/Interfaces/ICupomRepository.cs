using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface ICupomRepository
    {
        Task<Cupom?> ObterCupomPorNomeAsync(string nome);
        Task<bool> ExisteCupomAsync(string nome);
    }
}
