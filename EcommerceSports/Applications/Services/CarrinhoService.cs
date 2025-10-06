using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.Services
{
    public class CarrinhoService : ICarrinhoService
    {
        private readonly ICarrinhoRepository _carrinhoRepository;

        public CarrinhoService(ICarrinhoRepository carrinhoRepository)
        {
            _carrinhoRepository = carrinhoRepository;
        }

        public async Task<ResponseCarrinhoDTO> ObterCarrinhoAsync(int clienteId)
        {
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            
            if (carrinho == null)
            {
                return new ResponseCarrinhoDTO
                {
                    ClienteId = clienteId,
                    PedidoId = 0,
                    Itens = new List<ResponseItemCarrinhoDTO>(),
                    ValorTotal = 0,
                    TotalItens = 0
                };
            }

            var itens = carrinho.Itens.Select(item => new ResponseItemCarrinhoDTO
            {
                ProdutoId = item.ProdutoId,
                NomeProduto = item.Produto?.Nome ?? "Produto não encontrado",
                ImagemProduto = item.Produto?.Imagem ?? "",
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Subtotal = item.Quantidade * item.PrecoUnitario
            }).ToList();

            var valorTotal = itens.Sum(i => i.Subtotal);
            var totalItens = itens.Sum(i => i.Quantidade);

            return new ResponseCarrinhoDTO
            {
                ClienteId = clienteId,
                PedidoId = carrinho.Id,
                Itens = itens,
                ValorTotal = valorTotal,
                TotalItens = totalItens
            };
        }

        public async Task<ResponseCarrinhoDTO> AdicionarItemAsync(int clienteId, int produtoId, int quantidade)
        {
            // Verificar se o produto existe
            var produto = await _carrinhoRepository.ObterProdutoAsync(produtoId);
            if (produto == null)
            {
                throw new ArgumentException("Produto não encontrado");
            }

            // Obter ou criar carrinho
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            if (carrinho == null)
            {
                carrinho = await _carrinhoRepository.CriarCarrinhoAsync(clienteId);
            }

            // Verificar se o item já existe no carrinho
            var itemExistente = await _carrinhoRepository.ObterItemCarrinhoAsync(carrinho.Id, produtoId);
            
            if (itemExistente != null)
            {
                // Atualizar quantidade do item existente
                itemExistente.Quantidade += quantidade;
                await _carrinhoRepository.AtualizarQuantidadeItemAsync(itemExistente.Id, itemExistente.Quantidade);
            }
            else
            {
                // Adicionar novo item
                await _carrinhoRepository.AdicionarItemCarrinhoAsync(carrinho.Id, produtoId, quantidade, (decimal)produto.Preco);
            }

            // Atualizar valor total do pedido no banco
            await _carrinhoRepository.AtualizarValorTotalPedidoAsync(carrinho.Id);

            return await ObterCarrinhoAsync(clienteId);
        }

        public async Task<ResponseCarrinhoDTO> AtualizarItemAsync(int clienteId, int produtoId, int quantidade)
        {
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            if (carrinho == null)
            {
                throw new InvalidOperationException("Carrinho não encontrado");
            }

            var item = await _carrinhoRepository.ObterItemCarrinhoAsync(carrinho.Id, produtoId);
            if (item == null)
            {
                throw new ArgumentException("Item não encontrado no carrinho");
            }

            if (quantidade <= 0)
            {
                await _carrinhoRepository.RemoverItemCarrinhoAsync(item.Id);
            }
            else
            {
                await _carrinhoRepository.AtualizarQuantidadeItemAsync(item.Id, quantidade);
            }

            // Atualizar valor total do pedido no banco
            await _carrinhoRepository.AtualizarValorTotalPedidoAsync(carrinho.Id);

            return await ObterCarrinhoAsync(clienteId);
        }

        public async Task<ResponseCarrinhoDTO> RemoverItemAsync(int clienteId, int produtoId)
        {
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            if (carrinho == null)
            {
                throw new InvalidOperationException("Carrinho não encontrado");
            }

            var item = await _carrinhoRepository.ObterItemCarrinhoAsync(carrinho.Id, produtoId);
            if (item != null)
            {
                await _carrinhoRepository.RemoverItemCarrinhoAsync(item.Id);
            }

            // Atualizar valor total do pedido no banco
            await _carrinhoRepository.AtualizarValorTotalPedidoAsync(carrinho.Id);

            return await ObterCarrinhoAsync(clienteId);
        }

        public async Task<ResponseCarrinhoDTO> LimparCarrinhoAsync(int clienteId)
        {
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            if (carrinho != null)
            {
                await _carrinhoRepository.LimparCarrinhoAsync(carrinho.Id);
                await _carrinhoRepository.AtualizarValorTotalPedidoAsync(carrinho.Id);
            }

            return await ObterCarrinhoAsync(clienteId);
        }

        public async Task<bool> FinalizarCarrinhoAsync(int clienteId)
        {
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            if (carrinho == null || !carrinho.Itens.Any())
            {
                return false;
            }

            // Aqui você pode implementar a lógica para finalizar o pedido
            // Por exemplo, alterar o status para "EmTransporte" ou criar uma transação
            // Por enquanto, vamos apenas marcar como processado
            carrinho.StatusPedido = StatusPedido.EmTransporte;
            
            return true;
        }

        public async Task<ResponseCarrinhoDTO> AdicionarQuantidadeAsync(int clienteId, int produtoId)
        {
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            if (carrinho == null)
            {
                throw new InvalidOperationException("Carrinho não encontrado");
            }

            var item = await _carrinhoRepository.ObterItemCarrinhoAsync(carrinho.Id, produtoId);
            if (item == null)
            {
                throw new ArgumentException("Item não encontrado no carrinho");
            }

            // Aumentar quantidade em 1
            item.Quantidade += 1;
            await _carrinhoRepository.AtualizarQuantidadeItemAsync(item.Id, item.Quantidade);

            // Atualizar valor total do pedido no banco
            await _carrinhoRepository.AtualizarValorTotalPedidoAsync(carrinho.Id);

            return await ObterCarrinhoAsync(clienteId);
        }

        public async Task<ResponseCarrinhoDTO> DiminuirQuantidadeAsync(int clienteId, int produtoId)
        {
            var carrinho = await _carrinhoRepository.ObterCarrinhoAtivoAsync(clienteId);
            if (carrinho == null)
            {
                throw new InvalidOperationException("Carrinho não encontrado");
            }

            var item = await _carrinhoRepository.ObterItemCarrinhoAsync(carrinho.Id, produtoId);
            if (item == null)
            {
                throw new ArgumentException("Item não encontrado no carrinho");
            }

            if (item.Quantidade > 1)
            {
                // Diminuir quantidade em 1
                item.Quantidade -= 1;
                await _carrinhoRepository.AtualizarQuantidadeItemAsync(item.Id, item.Quantidade);
            }
            else
            {
                // Se quantidade for 1, remover o item
                await _carrinhoRepository.RemoverItemCarrinhoAsync(item.Id);
            }

            // Atualizar valor total do pedido no banco
            await _carrinhoRepository.AtualizarValorTotalPedidoAsync(carrinho.Id);

            return await ObterCarrinhoAsync(clienteId);
        }

        public async Task<bool> AtualizarStatusPedidoAsync(int pedidoId, int status)
        {
            return await _carrinhoRepository.AtualizarStatusPedidoAsync(pedidoId, status);
        }

        public async Task LimparCarrinhoPorPedidoAsync(int pedidoId)
        {
            await _carrinhoRepository.LimparCarrinhoAsync(pedidoId);
        }
    }
}
