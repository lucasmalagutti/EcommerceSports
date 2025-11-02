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
        private readonly ICupomRepository _cupomRepository;

        public PagamentoService(
            IPagamentoRepository pagamentoRepository,
            ITransacaoRepository transacaoRepository,
            IEstoqueService estoqueService,
            ICarrinhoService carrinhoService,
            ICupomRepository cupomRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _transacaoRepository = transacaoRepository;
            _estoqueService = estoqueService;
            _carrinhoService = carrinhoService;
            _cupomRepository = cupomRepository;
        }

        public async Task<ResponseTransacaoComPagamentosDTO> CriarTransacaoComPagamentosAsync(CriarTransacaoComPagamentosDTO criarDto)
        {
            if (await _transacaoRepository.ExisteTransacaoParaPedidoAsync(criarDto.PedidoId))
            {
                throw new InvalidOperationException("Já existe uma transação para este pedido");
            }

            if (!await _estoqueService.VerificarEstoquePedidoAsync(criarDto.PedidoId))
            {
                throw new InvalidOperationException("Estoque insuficiente para um ou mais produtos do pedido");
            }

            var somaPagamentos = criarDto.Pagamentos.Sum(p => p.Valor);
            if (somaPagamentos != criarDto.ValorTotal)
            {
                throw new ArgumentException($"A soma dos pagamentos ({somaPagamentos:C}) deve ser igual ao valor total ({criarDto.ValorTotal:C})");
            }

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

            var pagamentos = criarDto.Pagamentos.Select(p => new Pagamento
            {
                TransacaoId = transacaoCriada.Id,
                CartaoId = p.CartaoId,
                Valor = p.Valor,
                StatusPagamento = StatusPagamento.Pendente,
                DataPagamento = DateTime.UtcNow
            }).ToList();

            var pagamentosCriados = await _pagamentoRepository.CriarPagamentosAsync(pagamentos);

            await _estoqueService.ReduzirEstoquePedidoAsync(criarDto.PedidoId);

            await _carrinhoService.AtualizarStatusPedidoAsync(criarDto.PedidoId, 6); // Status inicial: AguardandoConfirmacao = 6

            if (criarDto.Cupons != null && criarDto.Cupons.Count > 0)
            {
                foreach (var codigoCupom in criarDto.Cupons.Where(c => !string.IsNullOrWhiteSpace(c)))
                {
                    var codigoNormalizado = codigoCupom.Trim();
                    if (codigoNormalizado.ToUpper().Contains("TROCA"))
                    {
                        await _cupomRepository.MarcarComoUtilizadoAsync(codigoNormalizado);
                    }
                }
            }



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
                Mensagem = "Transação criada com sucesso, estoque atualizado e pedido finalizado"
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
