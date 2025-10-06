using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Applications.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IEstoqueService _estoqueService;
        private readonly ICarrinhoService _carrinhoService;

        public TransacaoService(ITransacaoRepository transacaoRepository, IEstoqueService estoqueService, ICarrinhoService carrinhoService)
        {
            _transacaoRepository = transacaoRepository;
            _estoqueService = estoqueService;
            _carrinhoService = carrinhoService;
        }

        public async Task<ResponseTransacaoDTO> CriarTransacaoAsync(CriarTransacaoDTO criarDto)
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

            // Reduzir o estoque dos produtos após criar a transação
            await _estoqueService.ReduzirEstoquePedidoAsync(criarDto.PedidoId);

            // Alterar status do pedido para "Em Transporte" (2)
            await _carrinhoService.AtualizarStatusPedidoAsync(criarDto.PedidoId, 2);

            // Limpar o carrinho (remover todos os itens)
            await _carrinhoService.LimparCarrinhoPorPedidoAsync(criarDto.PedidoId);

            return new ResponseTransacaoDTO
            {
                Id = transacaoCriada.Id,
                PedidoId = transacaoCriada.PedidoId,
                ValorTotal = transacaoCriada.ValorTotal,
                ValorFrete = transacaoCriada.ValorFrete,
                EnderecoId = transacaoCriada.EnderecoId,
                StatusTransacao = transacaoCriada.StatusTransacao,
                DataTransacao = transacaoCriada.DataTransacao,
                Mensagem = "Transação criada com sucesso, estoque atualizado e carrinho limpo"
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
                StatusTransacao = transacao.StatusTransacao,
                DataTransacao = transacao.DataTransacao,
                Mensagem = "Transação encontrada"
            };
        }

        public async Task<IEnumerable<ResponseTransacaoDTO>> ObterTransacoesPorCliente(int clienteId)
        {
            var transacoes = await _transacaoRepository.Transacoes
        .Include(t => t.Pedido)
            .ThenInclude(p => p.Itens)
                .ThenInclude(i => i.Produto)
        .Include(t => t.Endereco)
        .Where(t => t.Pedido.ClienteId == clienteId)
        .ToListAsync();

            return transacoes.Select(t => new ResponseTransacaoDTO
            {
                Id = t.Id,
                PedidoId = t.PedidoId,
                ValorTotal = t.ValorTotal,
                ValorFrete = t.ValorFrete,
                StatusTransacao = t.StatusTransacao,
                DataTransacao = t.DataTransacao,
                ClienteId = t.Pedido?.ClienteId ?? 0,
                EnderecoId = t.EnderecoId,

                // 🔹 Mapeia os itens do pedido
                Itens = t.Pedido?.Itens.Select(i => new ItemPedidoDTO
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto?.Nome ?? "Produto não informado",
                    PrecoUnitario = i.Produto?.Preco ?? 0,
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemPedidoDTO>()
            }).ToList();
        }
    }
}
