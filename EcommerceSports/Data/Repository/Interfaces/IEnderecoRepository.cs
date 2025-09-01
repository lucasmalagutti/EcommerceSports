using EcommerceSports.Models;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface IEnderecoRepository
    {
        Task CadastrarEndereco(int id, Endereco endereco);
        Task EditarEndereco(int id, Endereco endereco);
    }
}
