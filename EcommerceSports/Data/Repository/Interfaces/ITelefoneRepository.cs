using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface ITelefoneRepository
    {
        Task<Telefone> CadastrarTelefone(Telefone telefone);
    }
}
