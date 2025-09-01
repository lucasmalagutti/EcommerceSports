using EcommerceSports.Applications.DTO;
using EcommerceSports.Models;

namespace EcommerceSports.Services.Interfaces
{
    public interface IEnderecoService
    {
        Task CadastrarEndereco(int id, EnderecoDTO endereco);
        Task EditarEndereco(int id, EditarEnderecoDTO enderecoDTO);
        Task ValidarEndereco(IEnumerable<Endereco> enderecos);
    }
}
