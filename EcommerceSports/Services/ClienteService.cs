using EcommerceSports.Applications.DTO;
using EcommerceSports.Data.Infra.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceSports.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public Task AlterarSenha(int id, string novaSenha)
        {
            throw new NotImplementedException();
        }

        public Task<int> AtribuirNumeroRanking()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseClienteDTO> BuscarClientePorId(int id)
        {
            throw new NotImplementedException();
        }

        public async Task CadastrarCliente(ClienteDTO clientedto)
        {
            var cliente = new Cliente
            {
                Nome = clientedto.Nome,
                Cpf = clientedto.Cpf,
                DtNasc = clientedto.DtNasc,
                Email = clientedto.Email,
                Senha = clientedto.Senha,
                Genero = (Models.Enums.Genero)clientedto.Genero,
                DtCadastro = System.DateTime.Now,
                CadastroAtivo = true
            };

            var enderecos = new List<Endereco>();
            foreach (var endDto in clientedto.Enderecos)
            {
                enderecos.Add(new Endereco
                {
                    Bairro = endDto.Bairro,
                    Cep = endDto.Cep,
                    Cidade = endDto.Cidade,
                    Estado = endDto.Estado,
                    Logradouro = endDto.Logradouro,
                    Nome = endDto.Nome,
                    Numero = endDto.Numero,
                    Observacoes = endDto.Observacoes,
                    Pais = endDto.Pais,
                    TipoEndereco = endDto.TipoEndereco,
                    TipoLogradouro = endDto.TipoLogradouro,
                    TipoResidencia = endDto.TipoResidencia
                });
            }

            var telefone = new Telefone
            {
                TipoTelefone = (Models.Enums.TipoTelefone)clientedto.TipoTelefone,
                Ddd = clientedto.Ddd,
                Numero = clientedto.NumeroTelefone
            };

            var cartao = new CartaoCredito
            {
                NumCartao = clientedto.NumCartao,
                NomeImpresso = clientedto.NomeImpresso,
                Cvc = clientedto.Cvc,
                Bandeira = (Models.Enums.BandeiraCartao)clientedto.Bandeira,
                Preferencial = clientedto.Preferencial
            };

            await _clienteRepository.CadastrarCliente(cliente, enderecos, telefone, cartao);
        }

        public Task<int> ContarClientesAtivos()
        {
            throw new NotImplementedException();
        }

        public Task<string> CriptografarSenha(string senha)
        {
            throw new NotImplementedException();
        }

        public Task EditarCliente(int id, EditarClienteDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InativarCliente(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CartaoDTO>> ListarCartoesCliente(int clienteId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ResponseClienteDTO>> ListarClientes()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EnderecoDTO>> ListarEnderecosCliente(int clienteId)
        {
            throw new NotImplementedException();
        }

        public Task ValidarCpf(string cpf)
        {
            throw new NotImplementedException();
        }

        public Task ValidarExistenciaCliente(string cpf, int? id = null)
        {
            throw new NotImplementedException();
        }

        public Task ValidarSenhaForte(string senha)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerificarSenha(string senha, string hash)
        {
            throw new NotImplementedException();
        }

        // ... outros métodos
    }
}