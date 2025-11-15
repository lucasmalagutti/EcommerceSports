using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;
using Microsoft.EntityFrameworkCore;
using static EcommerceSports.Applications.DTO.GraficoDTO;

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

        public async Task<IEnumerable<ResponseTransacaoDTO>> ListarTodasTransacoes()
        {
            var transacoes = await _transacaoRepository.ListarTodasTransacoes();

            if (!transacoes.Any())
                throw new ArgumentException("Nenhuma transação encontrada.");

            return transacoes.Select(t => new ResponseTransacaoDTO
            {
                Id = t.Id,
                PedidoId = t.PedidoId,
                ClienteId = t.Pedido?.ClienteId ?? 0,
                ValorTotal = t.ValorTotal,
                ValorFrete = t.ValorFrete,
                EnderecoId = t.EnderecoId,
                StatusTransacao = t.StatusTransacao,
                DataTransacao = t.DataTransacao,
                Mensagem = "Transação listada com sucesso",
                Itens = t.Pedido?.Itens.Select(i => new ItemPedidoDTO
                {
                    ProdutoId = i.ProdutoId,
                    NomeProduto = i.Produto.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList() ?? new List<ItemPedidoDTO>()
            }).ToList();

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
                StatusPedido = transacao.Pedido?.StatusPedido,
                DataTransacao = transacao.DataTransacao,
                ClienteId = transacao.Pedido?.ClienteId ?? 0,
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
                StatusPedido = transacao.Pedido?.StatusPedido,
                DataTransacao = transacao.DataTransacao,
                ClienteId = transacao.Pedido?.ClienteId ?? 0,
                Mensagem = "Transação encontrada",
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
        
        public async Task<List<GraficoVendasDTO>> ObterVolumeVendasPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var transacoes = await _transacaoRepository.ObterTransacoesPorPeriodo(dataInicio, dataFim);

            var resultado = transacoes
                .GroupBy(t => t.DataTransacao.Date)
                .Select(g => new GraficoVendasDTO
                {
                    Data = g.Key,
                    ValorTotal = g.Sum(t => t.ValorTotal),
                    QuantidadeVendas = g.Count()
                })
                .OrderBy(r => r.Data)
                .ToList();

            return resultado;
        }

        public async Task<List<GraficoVendasCategoriaDTO>> ObterVolumeVendasPorCategoria(DateTime dataInicio, DateTime dataFim)
        {
            var transacoes = await _transacaoRepository.ObterTransacoesPorPeriodo(dataInicio, dataFim);

            var resultado = transacoes
                .SelectMany(t => t.Pedido!.Itens.Select(i => new
                {
                    Categoria = i.Produto!.Categoria,
                    t.DataTransacao,
                    Valor = i.Quantidade * i.PrecoUnitario
                }))
                .GroupBy(x => new { x.Categoria, Data = x.DataTransacao.Date })
                .Select(g => new GraficoVendasCategoriaDTO
                {
                    CategoriaNome = g.Key.Categoria,
                    Data = g.Key.Data,
                    ValorTotal = g.Sum(x => x.Valor)
                })
                .OrderBy(r => r.Data)
                .ToList();

            return resultado;
        }

        public async Task<List<GraficoVendasProdutoDTO>> ObterVolumeVendasPorProduto(DateTime dataInicio, DateTime dataFim)
        {
            var transacoes = await _transacaoRepository.ObterTransacoesPorPeriodo(dataInicio, dataFim);

            var resultado = transacoes
                .SelectMany(t => t.Pedido!.Itens.Select(i => new
                {
                    Produto = i.Produto?.Nome ?? "Produto não informado",
                    t.DataTransacao,
                    Quantidade = i.Quantidade,
                    Valor = i.Quantidade * i.PrecoUnitario
                }))
                .GroupBy(x => new { x.Produto, Data = x.DataTransacao.Date })
                .Select(g => new GraficoVendasProdutoDTO
                {
                    ProdutoNome = g.Key.Produto,
                    Data = g.Key.Data,
                    Quantidade = g.Sum(x => x.Quantidade),
                    ValorTotal = g.Sum(x => x.Valor)
                })
                .OrderBy(r => r.Data)
                .ThenBy(r => r.ProdutoNome)
                .ToList();

            return resultado;
        }

   
    }
}
