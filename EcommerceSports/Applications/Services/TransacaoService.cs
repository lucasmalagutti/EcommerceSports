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
            if (await _transacaoRepository.ExisteTransacaoParaPedidoAsync(criarDto.PedidoId))
            {
                throw new InvalidOperationException("JÃ¡ existe uma transaÃ§Ã£o para este pedido");
            }

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

            await _estoqueService.ReduzirEstoquePedidoAsync(criarDto.PedidoId);

            await _carrinhoService.AtualizarStatusPedidoAsync(criarDto.PedidoId, 6); // Status inicial: AguardandoConfirmacao = 6


            return new ResponseTransacaoDTO
            {
                Id = transacaoCriada.Id,
                PedidoId = transacaoCriada.PedidoId,
                ValorTotal = transacaoCriada.ValorTotal,
                ValorFrete = transacaoCriada.ValorFrete,
                EnderecoId = transacaoCriada.EnderecoId,
                StatusTransacao = transacaoCriada.StatusTransacao,
                StatusPedido = transacaoCriada.Pedido?.StatusPedido,
                DataTransacao = transacaoCriada.DataTransacao,
                ClienteId = transacaoCriada.Pedido?.ClienteId ?? 0,
                Mensagem = "TransaÃ§Ã£o criada com sucesso, estoque atualizado e pedido finalizado"
            };
        }

        public async Task<ResponseTransacaoDTO> ObterTransacaoPorIdAsync(int id)
        {
            var transacao = await _transacaoRepository.ObterTransacaoPorIdAsync(id);
            
            if (transacao == null)
            {
                throw new ArgumentException("TransaÃ§Ã£o nÃ£o encontrada");
            }

            return new ResponseTransacaoDTO
            {
                Id = transacao.Id,
                PedidoId = transacao.PedidoId,
                ValorTotal = transacao.ValorTotal,
                ValorFrete = transacao.ValorFrete,
                EnderecoId = transacao.EnderecoId,
                StatusTransacao = transacao.StatusTransacao,
                StatusPedido = transacao.Pedido?.StatusPedido,
                DataTransacao = transacao.DataTransacao,
                ClienteId = transacao.Pedido?.ClienteId ?? 0,
                Mensagem = "TransaÃ§Ã£o encontrada"
            };
        }

        public async Task<ResponseTransacaoDTO> ObterTransacaoPorPedidoIdAsync(int pedidoId)
        {
            var transacao = await _transacaoRepository.ObterTransacaoPorPedidoIdAsync(pedidoId);
            
            if (transacao == null)
            {
                throw new ArgumentException("TransaÃ§Ã£o nÃ£o encontrada para este pedido");
            }

            return new ResponseTransacaoDTO
            {
                Id = transacao.Id,
                PedidoId = transacao.PedidoId,
                ValorTotal = transacao.ValorTotal,
                ValorFrete = transacao.ValorFrete,
                EnderecoId = transacao.EnderecoId,
                StatusTransacao = transacao.StatusTransacao,
                StatusPedido = transacao.Pedido?.StatusPedido,
                DataTransacao = transacao.DataTransacao,
                ClienteId = transacao.Pedido?.ClienteId ?? 0,
                Mensagem = "TransaÃ§Ã£o encontrada",
                Itens = transacao.Pedido?.Itens.Select(i => new ItemPedidoDTO
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto?.Nome ?? "Produto não informado",
                    PrecoUnitario = (decimal)(i.Produto?.Preco ?? 0),
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemPedidoDTO>()
            };
        }

        public async Task<IEnumerable<ResponseTransacaoDTO>> ObterTransacoesPorCliente(int clienteId)
        {
            var transacoes = await _transacaoRepository.ObterPorCliente(clienteId);

            return transacoes.Select(t => new ResponseTransacaoDTO
            {
                Id = t.Id,
                PedidoId = t.PedidoId,
                ValorTotal = t.ValorTotal,
                ValorFrete = t.ValorFrete,
                StatusTransacao = t.StatusTransacao,
                StatusPedido = t.Pedido?.StatusPedido,
                DataTransacao = t.DataTransacao,
                ClienteId = t.Pedido?.ClienteId ?? 0,
                EnderecoId = t.EnderecoId,
                Itens = t.Pedido?.Itens.Select(i => new ItemPedidoDTO
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto?.Nome ?? "Produto nÃ£o informado",
                    PrecoUnitario = (decimal)(i.Produto?.Preco ?? 0),
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemPedidoDTO>()
            }).ToList();
        }

        public async Task<ResponseTransacaoDTO> AtualizarStatusPedidoAsync(int pedidoId, Models.Enums.StatusPedido novoStatus)
        {
            var transacao = await _transacaoRepository.AtualizarStatusPedidoAsync(pedidoId, novoStatus);

            if (transacao == null)
            {
                throw new ArgumentException("TransaÃ§Ã£o nÃ£o encontrada para este pedido");
            }

            // Buscar a transaÃ§Ã£o atualizada com todos os dados
            var transacaoCompleta = await _transacaoRepository.ObterTransacaoPorPedidoIdAsync(pedidoId);

            if (transacaoCompleta == null)
            {
                throw new ArgumentException("Erro ao buscar transaÃ§Ã£o atualizada");
            }

            return new ResponseTransacaoDTO
            {
                Id = transacaoCompleta.Id,
                PedidoId = transacaoCompleta.PedidoId,
                ValorTotal = transacaoCompleta.ValorTotal,
                ValorFrete = transacaoCompleta.ValorFrete,
                EnderecoId = transacaoCompleta.EnderecoId,
                StatusTransacao = transacaoCompleta.StatusTransacao,
                StatusPedido = transacaoCompleta.Pedido?.StatusPedido ?? novoStatus,
                DataTransacao = transacaoCompleta.DataTransacao,
                ClienteId = transacaoCompleta.Pedido?.ClienteId ?? 0,
                Mensagem = $"Status do pedido atualizado para {novoStatus} com sucesso",
                Itens = transacaoCompleta.Pedido?.Itens.Select(i => new ItemPedidoDTO
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto?.Nome ?? "Produto nÃ£o informado",
                    PrecoUnitario = (decimal)(i.Produto?.Preco ?? 0),
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemPedidoDTO>()
            };
        }

        public async Task<ResponseTransacaoDTO> AtualizarStatusTransacaoAsync(int transacaoId, Models.Enums.StatusTransacao novoStatus)
        {
            var transacao = await _transacaoRepository.AtualizarStatusTransacaoAsync(transacaoId, novoStatus);

            if (transacao == null)
            {
                throw new ArgumentException("Transação não encontrada");
            }

            // Buscar a transação atualizada com todos os dados
            var transacaoCompleta = await _transacaoRepository.ObterTransacaoPorIdAsync(transacaoId);

            if (transacaoCompleta == null)
            {
                throw new ArgumentException("Erro ao buscar transação atualizada");
            }

            return new ResponseTransacaoDTO
            {
                Id = transacaoCompleta.Id,
                PedidoId = transacaoCompleta.PedidoId,
                ValorTotal = transacaoCompleta.ValorTotal,
                ValorFrete = transacaoCompleta.ValorFrete,
                EnderecoId = transacaoCompleta.EnderecoId,
                StatusTransacao = transacaoCompleta.StatusTransacao,
                StatusPedido = transacaoCompleta.Pedido?.StatusPedido,
                DataTransacao = transacaoCompleta.DataTransacao,
                ClienteId = transacaoCompleta.Pedido?.ClienteId ?? 0,
                Mensagem = $"Status da transação atualizado para {novoStatus} com sucesso",
                Itens = transacaoCompleta.Pedido?.Itens.Select(i => new ItemPedidoDTO
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto?.Nome ?? "Produto não informado",
                    PrecoUnitario = (decimal)(i.Produto?.Preco ?? 0),
                    Quantidade = i.Quantidade
                }).ToList() ?? new List<ItemPedidoDTO>()
            };
        }
    }
}
