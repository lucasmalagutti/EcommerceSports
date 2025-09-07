using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Repository.Interfaces
{
    public interface IEnderecoRepository
    {
        Task CadastrarEndereco(int id, Endereco endereco);
        Task EditarEndereco(int id, Endereco endereco);
        Task<Endereco?> BuscarEnderecoPorId(int id);
        Task<List<Endereco>> ListarEnderecosPorCliente(int clienteId);
    }
}
