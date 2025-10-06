using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IEstoqueService _estoqueService;
        private readonly ICarrinhoService _carrinhoService;

        public PagamentoService(
            IPagamentoRepository pagamentoRepository,
            ITransacaoRepository transacaoRepository,
            IEstoqueService estoqueService,
            ICarrinhoService carrinhoService)
        {
            _pagamentoRepository = pagamentoRepository;
            _transacaoRepository = transacaoRepository;
            _estoqueService = estoqueService;
            _carrinhoService = carrinhoService;
        }

        public async Task<ResponseTransacaoComPagamentosDTO> CriarTransacaoComPagamentosAsync(CriarTransacaoComPagamentosDTO criarDto)
        {
            // Verificar se já existe uma transação para este pedido
            if (await _transacaoRepository.ExisteTransacaoParaPedidoAsync(criarDto.PedidoId))
            {
                throw new InvalidOperationException("Já existe uma transação para este pedido");
            }

            // Verificar se há estoque disponível para todos os produtos do pedido
            if (!await _estoqueService.VerificarEstoquePedidoAsync(criarDto.PedidoId))
            {
                throw new InvalidOperationException("Estoque insuficiente para um ou mais produtos do pedido");
            }

            // Validar se a soma dos pagamentos é igual ao valor total
            var somaPagamentos = criarDto.Pagamentos.Sum(p => p.Valor);
            if (somaPagamentos != criarDto.ValorTotal)
            {
                throw new ArgumentException($"A soma dos pagamentos ({somaPagamentos:C}) deve ser igual ao valor total ({criarDto.ValorTotal:C})");
            }

            // Criar a transação
            var transacao = new Transacao
            {
                PedidoId = criarDto.PedidoId,
                ValorTotal = criarDto.ValorTotal,
                ValorFrete = criarDto.ValorFrete,
                EnderecoId = criarDto.EnderecoId,
                StatusTransacao = criarDto.StatusTransacao,
                DataTransacao = DateTime.UtcNow
            };

            var transacaoCriada = await _transacaoRepository.CriarTransacaoAsync(transacao);

            // Criar os pagamentos
            var pagamentos = criarDto.Pagamentos.Select(p => new Pagamento
            {
                TransacaoId = transacaoCriada.Id,
                CartaoId = p.CartaoId,
                Valor = p.Valor,
                StatusPagamento = StatusPagamento.Pendente,
                DataPagamento = DateTime.UtcNow
            }).ToList();

            var pagamentosCriados = await _pagamentoRepository.CriarPagamentosAsync(pagamentos);

            // Reduzir o estoque dos produtos após criar a transação
            await _estoqueService.ReduzirEstoquePedidoAsync(criarDto.PedidoId);

            // Alterar status do pedido para "Em Transporte" (2)
            await _carrinhoService.AtualizarStatusPedidoAsync(criarDto.PedidoId, 2);

            // Limpar o carrinho (remover todos os itens)
            await _carrinhoService.LimparCarrinhoPorPedidoAsync(criarDto.PedidoId);

            // Retornar a resposta com os pagamentos
            return new ResponseTransacaoComPagamentosDTO
            {
                Id = transacaoCriada.Id,
                PedidoId = transacaoCriada.PedidoId,
                ValorTotal = transacaoCriada.ValorTotal,
                ValorFrete = transacaoCriada.ValorFrete,
                EnderecoId = transacaoCriada.EnderecoId,
                StatusTransacao = transacaoCriada.StatusTransacao,
                DataTransacao = transacaoCriada.DataTransacao,
                Pagamentos = pagamentosCriados.Select(p => new PagamentoDTO
                {
                    Id = p.Id,
                    TransacaoId = p.TransacaoId,
                    CartaoId = p.CartaoId,
                    Valor = p.Valor,
                    StatusPagamento = p.StatusPagamento,
                    DataPagamento = p.DataPagamento
                }).ToList(),
                Mensagem = "Transação criada com sucesso, estoque atualizado e carrinho limpo"
            };
        }

        public async Task<ResponseTransacaoComPagamentosDTO> ObterTransacaoComPagamentosAsync(int transacaoId)
        {
            var transacao = await _transacaoRepository.ObterTransacaoPorIdAsync(transacaoId);
            
            if (transacao == null)
            {
                throw new ArgumentException("Transação não encontrada");
            }

            var pagamentos = await _pagamentoRepository.ObterPagamentosPorTransacaoAsync(transacaoId);

            return new ResponseTransacaoComPagamentosDTO
            {
                Id = transacao.Id,
                PedidoId = transacao.PedidoId,
                ValorTotal = transacao.ValorTotal,
                ValorFrete = transacao.ValorFrete,
                EnderecoId = transacao.EnderecoId,
                StatusTransacao = transacao.StatusTransacao,
                DataTransacao = transacao.DataTransacao,
                Pagamentos = pagamentos.Select(p => new PagamentoDTO
                {
                    Id = p.Id,
                    TransacaoId = p.TransacaoId,
                    CartaoId = p.CartaoId,
                    Valor = p.Valor,
                    StatusPagamento = p.StatusPagamento,
                    DataPagamento = p.DataPagamento
                }).ToList(),
                Mensagem = "Transação encontrada"
            };
        }

        public async Task<List<PagamentoDTO>> ObterPagamentosPorTransacaoAsync(int transacaoId)
        {
            var pagamentos = await _pagamentoRepository.ObterPagamentosPorTransacaoAsync(transacaoId);

            return pagamentos.Select(p => new PagamentoDTO
            {
                Id = p.Id,
                TransacaoId = p.TransacaoId,
                CartaoId = p.CartaoId,
                Valor = p.Valor,
                StatusPagamento = p.StatusPagamento,
                DataPagamento = p.DataPagamento
            }).ToList();
        }
    }
}
