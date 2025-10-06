using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public TransacaoService(ITransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }

        public async Task<ResponseTransacaoDTO> CriarTransacaoAsync(CriarTransacaoDTO criarDto)
        {
            // Verificar se já existe uma transação para este pedido
            if (await _transacaoRepository.ExisteTransacaoParaPedidoAsync(criarDto.PedidoId))
            {
                throw new InvalidOperationException("Já existe uma transação para este pedido");
            }

            var transacao = new Transacao
            {
                PedidoId = criarDto.PedidoId,
                ValorTotal = criarDto.ValorTotal,
                ValorFrete = criarDto.ValorFrete,
                EnderecoId = criarDto.EnderecoId,
                CartaoId = criarDto.CartaoId,
                StatusTransacao = criarDto.StatusTransacao,
                DataTransacao = DateTime.UtcNow
            };

            var transacaoCriada = await _transacaoRepository.CriarTransacaoAsync(transacao);

            return new ResponseTransacaoDTO
            {
                Id = transacaoCriada.Id,
                PedidoId = transacaoCriada.PedidoId,
                ValorTotal = transacaoCriada.ValorTotal,
                ValorFrete = transacaoCriada.ValorFrete,
                EnderecoId = transacaoCriada.EnderecoId,
                CartaoId = transacaoCriada.CartaoId,
                StatusTransacao = transacaoCriada.StatusTransacao,
                DataTransacao = transacaoCriada.DataTransacao,
                Mensagem = "Transação criada com sucesso"
            };
        }

        public async Task<ResponseTransacaoDTO> ObterTransacaoPorIdAsync(int id)
        {
            var transacao = await _transacaoRepository.ObterTransacaoPorIdAsync(id);
            
            if (transacao == null)
            {
                throw new ArgumentException("Transação não encontrada");
            }

            return new ResponseTransacaoDTO
            {
                Id = transacao.Id,
                PedidoId = transacao.PedidoId,
                ValorTotal = transacao.ValorTotal,
                ValorFrete = transacao.ValorFrete,
                EnderecoId = transacao.EnderecoId,
                CartaoId = transacao.CartaoId,
                StatusTransacao = transacao.StatusTransacao,
                DataTransacao = transacao.DataTransacao,
                Mensagem = "Transação encontrada"
            };
        }

        public async Task<ResponseTransacaoDTO> ObterTransacaoPorPedidoIdAsync(int pedidoId)
        {
            var transacao = await _transacaoRepository.ObterTransacaoPorPedidoIdAsync(pedidoId);
            
            if (transacao == null)
            {
                throw new ArgumentException("Transação não encontrada para este pedido");
            }

            return new ResponseTransacaoDTO
            {
                Id = transacao.Id,
                PedidoId = transacao.PedidoId,
                ValorTotal = transacao.ValorTotal,
                ValorFrete = transacao.ValorFrete,
                EnderecoId = transacao.EnderecoId,
                CartaoId = transacao.CartaoId,
                StatusTransacao = transacao.StatusTransacao,
                DataTransacao = transacao.DataTransacao,
                Mensagem = "Transação encontrada"
            };
        }
    }
}
