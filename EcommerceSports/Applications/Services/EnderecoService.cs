using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services
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
