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

            try
            {
                _clienteRepository.CadastrarCliente(cliente, enderecos, telefone, cartao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


    }
}