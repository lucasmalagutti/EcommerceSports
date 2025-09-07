using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.Services
{
    public class EnderecoService : IEnderecoService
    {
        private readonly IEnderecoRepository _enderecoRepository;

        public EnderecoService(IEnderecoRepository enderecoRepository)
        {
            _enderecoRepository = enderecoRepository;
        }

        public async Task CadastrarEndereco(int id, EnderecoDTO enderecoDTO)
        {
            var endereco = new Endereco
            {
                ClienteId = id,
                TipoEndereco = enderecoDTO.TipoEndereco,
                TipoResidencia = enderecoDTO.TipoResidencia,
                TipoLogradouro = enderecoDTO.TipoLogradouro,
                Nome = enderecoDTO.Nome,
                Logradouro = enderecoDTO.Logradouro,
                Numero = enderecoDTO.Numero,
                Cep = enderecoDTO.Cep,
                Bairro = enderecoDTO.Bairro,
                Cidade = enderecoDTO.Cidade,
                Estado = enderecoDTO.Estado,
                Pais = enderecoDTO.Pais,
                Observacoes = enderecoDTO.Observacoes
            };

            await _enderecoRepository.CadastrarEndereco(id, endereco);
        }
        public async Task ValidarEndereco(IEnumerable<Endereco> enderecos)
        {
            // Implementar validações de negócio se necessário
            await Task.CompletedTask;
        }
    }
}
