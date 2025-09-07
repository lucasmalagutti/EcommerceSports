using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services
{
    public class EnderecoService : IEnderecoService
    {
        private readonly IEnderecoRepository _enderecoRepository;

        public EnderecoService(IEnderecoRepository enderecoRepository)
        {
            _enderecoRepository = enderecoRepository;
        }

        public async Task AdicionarEndereco(int clienteId, EnderecoDTO enderecoDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(enderecoDTO.Nome))
                    throw new Exception("Nome do endereço é obrigatório.");

                if (string.IsNullOrWhiteSpace(enderecoDTO.Logradouro))
                    throw new Exception("Logradouro é obrigatório.");

                if (string.IsNullOrWhiteSpace(enderecoDTO.Numero))
                    throw new Exception("Número é obrigatório.");

                if (string.IsNullOrWhiteSpace(enderecoDTO.Cep))
                    throw new Exception("CEP é obrigatório.");

                if (string.IsNullOrWhiteSpace(enderecoDTO.Bairro))
                    throw new Exception("Bairro é obrigatório.");

                if (string.IsNullOrWhiteSpace(enderecoDTO.Cidade))
                    throw new Exception("Cidade é obrigatória.");

                if (string.IsNullOrWhiteSpace(enderecoDTO.Estado))
                    throw new Exception("Estado é obrigatório.");

                if (string.IsNullOrWhiteSpace(enderecoDTO.Pais))
                    throw new Exception("País é obrigatório.");

                // Converter DTO para Entity
                var endereco = new Endereco
                {
                    Nome = enderecoDTO.Nome,
                    Logradouro = enderecoDTO.Logradouro,
                    Numero = enderecoDTO.Numero,
                    Cep = enderecoDTO.Cep,
                    Bairro = enderecoDTO.Bairro,
                    Cidade = enderecoDTO.Cidade,
                    Estado = enderecoDTO.Estado,
                    Pais = enderecoDTO.Pais,
                    Observacoes = enderecoDTO.Observacoes,
                    TipoEndereco = enderecoDTO.TipoEndereco,
                    TipoLogradouro = enderecoDTO.TipoLogradouro,
                    TipoResidencia = enderecoDTO.TipoResidencia
                };

                await _enderecoRepository.AdicionarEndereco(clienteId, endereco);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar endereço: {ex.Message}", ex);
            }
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
