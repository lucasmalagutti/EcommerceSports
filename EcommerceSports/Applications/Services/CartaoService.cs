using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services
{
    public class CartaoService : ICartaoService
    {
        private readonly ICartaoRepository _cartaoRepository;

        public CartaoService(ICartaoRepository cartaoRepository)
        {
            _cartaoRepository = cartaoRepository;
        }

        public async Task<ResponseCartaoDTO> CadastrarCartao(int clienteId, CadastrarCartaoDTO cartao)
        {
            // Criar entidade
            var cartaoCredito = new CartaoCredito
            {
                NumCartao = cartao.NumCartao,
                NomeImpresso = cartao.NomeImpresso,
                Bandeira = cartao.Bandeira,
                Cvc = cartao.Cvc,
                ClienteId = clienteId,
                Preferencial = cartao.Preferencial
            };

            // Salvar no banco
            var cartaoSalvo = await _cartaoRepository.CadastrarCartao(cartaoCredito);

            // Retornar DTO de resposta
            return new ResponseCartaoDTO
            {
                Id = cartaoSalvo.Id,
                NumCartao = cartaoSalvo.NumCartao,
                NomeImpresso = cartaoSalvo.NomeImpresso,
                Bandeira = cartaoSalvo.Bandeira,
                Preferencial = cartaoSalvo.Preferencial,
                ClienteId = cartaoSalvo.ClienteId
            };
        }

        public async Task<List<ResponseCartaoDTO>> ListarCartoesPorCliente(int clienteId)
        {
            var cartoes = await _cartaoRepository.ListarCartoesPorCliente(clienteId);
            
            return cartoes.Select(c => new ResponseCartaoDTO
            {
                Id = c.Id,
                NumCartao = c.NumCartao,
                NomeImpresso = c.NomeImpresso,
                Bandeira = c.Bandeira,
                Preferencial = c.Preferencial,
                ClienteId = c.ClienteId
            }).ToList();
        }

    }
}
