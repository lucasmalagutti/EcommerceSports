using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Infra.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task AtualizarCliente(int id, EditarClienteDTO clientedto)
        {
            var clienteExistente = await _clienteRepository.BuscarPorId(id);

            if (clienteExistente == null)
                throw new Exception("Cliente não encontrado.");

            clienteExistente.Nome = clientedto.Nome;
            clienteExistente.DtNasc = clientedto.DtNascimento ?? clienteExistente.DtNasc;
            clienteExistente.Cpf = clientedto.Cpf;
            clienteExistente.Email = clientedto.Email;
            clienteExistente.Genero = clientedto.Genero ?? clienteExistente.Genero;

            var telefone = clienteExistente.Telefones.First();
            telefone.TipoTelefone = clientedto.TipoTelefone;
            telefone.Ddd = clientedto.Ddd;
            telefone.Numero = clientedto.NumeroTelefone;

            await _clienteRepository.AtualizarCliente(clienteExistente);
        }

        public async Task AtualizarSenha(int id, EditarSenhaDTO senha)
        {
            var cliente = await _clienteRepository.BuscarPorId(id);

            if (cliente == null)
                throw new Exception("Cliente não existe.");

            if (cliente.Senha != senha.SenhaAtual)
                throw new Exception("Senha atual incorreta.");

            if (senha.NovaSenha != senha.ConfirmarNovaSenha)
                throw new Exception("A nova senha e a confirmação não coincidem.");

            cliente.Senha = senha.NovaSenha;
            await _clienteRepository.AtualizarCliente(cliente);
        }

        public async Task AtualizarStatusCliente(int id, EditarStatusClienteDTO status)
        {
            var cliente = await _clienteRepository.BuscarPorId(id);

            if (cliente == null)
                throw new Exception("Cliente não existe.");

            cliente.CadastroAtivo = status.CadastroAtivo;

            await _clienteRepository.AtualizarCliente(cliente);
        }

        public async Task CadastrarCliente(ClienteDTO clientedto)
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

           await _clienteRepository.CadastrarCliente(cliente);
        }

        public async Task<List<ListarClienteDTO>> ListarDadosCliente()
        {
            var clientes = await _clienteRepository.ListarTodos();
            return clientes.Select(c => new ListarClienteDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Cpf = c.Cpf,
                Email = c.Email,
                DtNasc = c.DtNasc,
                Genero = c.Genero,
                Ddd = c.Telefones.FirstOrDefault()?.Ddd,
                NumeroTelefone = c.Telefones.FirstOrDefault()?.Numero,
                TipoTelefone = c.Telefones.FirstOrDefault()?.TipoTelefone ?? 0,
                CadastroAtivo = c.CadastroAtivo
            }).ToList();
        }
    }
}