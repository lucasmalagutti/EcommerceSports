using EcommerceSports.Applications.DTO;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IEnderecoService
    {
        Task CadastrarEndereco(int id, EnderecoDTO endereco);
        Task EditarEndereco(int id, EditarEnderecoDTO enderecoDTO);
        Task ValidarEndereco(IEnumerable<Endereco> enderecos);
    }
}
