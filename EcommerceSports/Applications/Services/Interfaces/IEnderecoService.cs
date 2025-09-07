using EcommerceSports.Applications.DTO;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IEnderecoService
    {
        Task CadastrarEndereco(int id, EnderecoDTO endereco);
        Task ValidarEndereco(IEnumerable<Endereco> enderecos);
        Task EditarEndereco(int id, EditarEnderecoDTO endereco);
        Task<List<ResponseEnderecoDTO>> ListarEnderecosPorCliente(int clienteId);
    }
}
