using EcommerceSports.Applications.DTO;
using EcommerceSports.Models;
using EcommerceSports.Services.Interfaces;

namespace EcommerceSports.Services
{
    public class EnderecoService : IEnderecoService
    {
        public Task CadastrarEndereco(int id, EnderecoDTO endereco)
        {
            throw new NotImplementedException();
        }

        public Task EditarEndereco(int id, EditarEnderecoDTO enderecoDTO)
        {
            throw new NotImplementedException();
        }

        public Task ValidarEndereco(IEnumerable<Endereco> enderecos)
        {
            throw new NotImplementedException();
        }
    }
}
