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

        public async Task EditarEndereco(int id, EditarEnderecoDTO enderecoDTO)
        {
            var endereco = new Endereco
            {
                Id = id,
                TipoEndereco = TipoEndereco.Cobranca, // Valor padrão, pois não está no DTO
                TipoResidencia = enderecoDTO.TipoResidencia.HasValue ? (TipoResidencia)enderecoDTO.TipoResidencia.Value : TipoResidencia.Casa,
                TipoLogradouro = enderecoDTO.TipoLogradouro.HasValue ? (TipoLogradouro)enderecoDTO.TipoLogradouro.Value : TipoLogradouro.Rua,
                Nome = enderecoDTO.Nome ?? string.Empty,
                Logradouro = enderecoDTO.Logradouro ?? string.Empty,
                Numero = enderecoDTO.Numero ?? string.Empty,
                Cep = enderecoDTO.Cep ?? string.Empty,
                Bairro = enderecoDTO.Bairro ?? string.Empty,
                Cidade = enderecoDTO.Cidade ?? string.Empty,
                Estado = enderecoDTO.Estado ?? string.Empty,
                Pais = enderecoDTO.Pais ?? string.Empty,
                Observacoes = enderecoDTO.Observacoes
            };

            await _enderecoRepository.EditarEndereco(id, endereco);
        }

        public async Task ValidarEndereco(IEnumerable<Endereco> enderecos)
        {
            // Implementar validações de negócio se necessário
            await Task.CompletedTask;
        }
    }
}
