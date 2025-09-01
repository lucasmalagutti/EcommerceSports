using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Infra.Interfaces;
using EcommerceSports.Models.Entity;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace EcommerceSports.Applications.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public void CadastrarCliente(ClienteDTO clientedto)
        {
            var enderecos = new List<Endereco>();
            foreach (var endDto in clientedto.Enderecos)
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

            var telefone = new Telefone
            {
                TipoTelefone = clientedto.TipoTelefone,
                Ddd = clientedto.Ddd,
                Numero = clientedto.NumeroTelefone
            };

            var cartao = new CartaoCredito
            {
                NumCartao = clientedto.NumCartao,
                NomeImpresso = clientedto.NomeImpresso,
                Cvc = clientedto.Cvc,
                Bandeira = clientedto.Bandeira,
                Preferencial = clientedto.Preferencial,
            };


            var cliente = new Cliente
            {
                Nome = clientedto.Nome,
                Cpf = clientedto.Cpf,
                DtNasc = DateTime.SpecifyKind(clientedto.DtNasc, DateTimeKind.Utc),
                Email = clientedto.Email,
                Senha = clientedto.Senha,
                Genero = (Models.Enums.Genero)clientedto.Genero,
                DtCadastro = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                CadastroAtivo = true,

                Endereco = enderecos,
                Telefones = new List<Telefone> { telefone },
                Cartoes = new List<CartaoCredito> { cartao }
                
            };

            // Configurar os relacionamentos bidirecionais
            foreach (var endereco in enderecos)
            {
                endereco.Cliente = cliente;
            }
            
            telefone.Cliente = cliente;
            cartao.Cliente = cliente;

            _clienteRepository.CadastrarCliente(cliente);
        }
    }
}