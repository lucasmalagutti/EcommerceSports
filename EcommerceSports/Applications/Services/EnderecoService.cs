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
            var enderecoExistente = await _enderecoRepository.BuscarEnderecoPorId(id);
            var endereco = new Endereco
            {
                Id = id,
                TipoEndereco = enderecoExistente.TipoEndereco, // Mantém o valor atual
                TipoResidencia = enderecoDTO.TipoResidencia.HasValue ? (TipoResidencia)enderecoDTO.TipoResidencia.Value : enderecoExistente.TipoResidencia,
                TipoLogradouro = enderecoDTO.TipoLogradouro.HasValue ? (TipoLogradouro)enderecoDTO.TipoLogradouro.Value : enderecoExistente.TipoLogradouro,
                Nome = enderecoDTO.Nome ?? enderecoExistente.Nome,
                Logradouro = enderecoDTO.Logradouro ?? enderecoExistente.Logradouro,
                Numero = enderecoDTO.Numero ?? enderecoExistente.Numero,
                Cep = enderecoDTO.Cep ?? enderecoExistente.Cep,
                Bairro = enderecoDTO.Bairro ?? enderecoExistente.Bairro,
                Cidade = enderecoDTO.Cidade ?? enderecoExistente.Cidade,
                Estado = enderecoDTO.Estado ?? enderecoExistente.Estado,
                Pais = enderecoDTO.Pais ?? enderecoExistente.Pais,
                Observacoes = enderecoDTO.Observacoes ?? enderecoExistente.Observacoes
            };

            await _enderecoRepository.EditarEndereco(id, endereco);
        }

        public async Task ValidarEndereco(IEnumerable<Endereco> enderecos)
        {
            await Task.CompletedTask;
        }

        public async Task<List<ResponseEnderecoDTO>> ListarEnderecosPorCliente(int clienteId)
        {
            var enderecos = await _enderecoRepository.ListarEnderecosPorCliente(clienteId);
            
            return enderecos.Select(e => new ResponseEnderecoDTO
            {
                Id = e.Id,
                Nome = e.Nome,
                TipoEndereco = e.TipoEndereco,
                TipoResidencia = e.TipoResidencia,
                TipoLogradouro = e.TipoLogradouro,
                Logradouro = e.Logradouro,
                Numero = e.Numero,
                Cep = e.Cep,
                Bairro = e.Bairro,
                Cidade = e.Cidade,
                Estado = e.Estado,
                Pais = e.Pais,
                Observacoes = e.Observacoes,
                ClienteId = e.ClienteId
            }).ToList();
        }
    }
}
