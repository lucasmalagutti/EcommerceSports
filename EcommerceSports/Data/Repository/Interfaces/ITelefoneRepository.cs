using EcommerceSports.Models;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface ITelefoneRepository
    {
        Task<Telefone> CadastrarTelefone(Telefone telefone);
    }
}
