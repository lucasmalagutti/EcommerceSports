using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Applications.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;

        public EstoqueService(IEstoqueRepository estoqueRepository, ICarrinhoRepository carrinhoRepository)
        {
            _estoqueRepository = estoqueRepository;
            _carrinhoRepository = carrinhoRepository;
        }

        public async Task<bool> ReduzirEstoquePedidoAsync(int pedidoId)
        {
            try
            {
                // Obter todos os itens do pedido
                var itensPedido = await _carrinhoRepository.ObterItensPedidoAsync(pedidoId);
                
                if (itensPedido == null || !itensPedido.Any())
                {
                    return false;
                }

                // Verificar se há estoque disponível para todos os produtos
                foreach (var item in itensPedido)
                {
                    if (!await _estoqueRepository.VerificarEstoqueDisponivelAsync(item.ProdutoId, item.Quantidade))
                    {
                        throw new InvalidOperationException($"Estoque insuficiente para o produto ID {item.ProdutoId}. Quantidade solicitada: {item.Quantidade}");
                    }
                }

                // Reduzir o estoque de cada produto
                foreach (var item in itensPedido)
                {
                    var sucesso = await _estoqueRepository.AtualizarEstoqueAsync(item.ProdutoId, item.Quantidade);
                    if (!sucesso)
                    {
                        throw new InvalidOperationException($"Erro ao atualizar estoque do produto ID {item.ProdutoId}");
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> VerificarEstoqueDisponivelAsync(int produtoId, int quantidade)
        {
            return await _estoqueRepository.VerificarEstoqueDisponivelAsync(produtoId, quantidade);
        }

        public async Task<bool> VerificarEstoquePedidoAsync(int pedidoId)
        {
            try
            {
                // Obter todos os itens do pedido
                var itensPedido = await _carrinhoRepository.ObterItensPedidoAsync(pedidoId);
                
                if (itensPedido == null || !itensPedido.Any())
                {
                    return false;
                }

                // Verificar se há estoque disponível para todos os produtos
                foreach (var item in itensPedido)
                {
                    if (!await _estoqueRepository.VerificarEstoqueDisponivelAsync(item.ProdutoId, item.Quantidade))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
