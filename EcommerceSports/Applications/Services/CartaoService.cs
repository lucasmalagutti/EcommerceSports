using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.Services
{
    public class CartaoService : ICartaoService
    {
        private readonly ICartaoRepository _cartaoRepository;

        public CartaoService(ICartaoRepository cartaoRepository)
        {
            _cartaoRepository = cartaoRepository;
        }

        public async Task<ResponseCartaoDTO> CadastrarCartao(CadastrarCartaoDTO cartao)
        {
            // Validações de negócio
            await ValidarCartao(cartao);

            // Verificar se já existe cartão com o mesmo número para o cliente
            var cartaoExistente = await _cartaoRepository.ExisteCartaoComNumero(cartao.NumCartao, cartao.ClienteId);
            if (cartaoExistente)
            {
                throw new ArgumentException("Já existe um cartão com este número cadastrado para este cliente.");
            }

            // Criar entidade
            var cartaoCredito = new CartaoCredito
            {
                NumCartao = cartao.NumCartao,
                NomeImpresso = cartao.NomeImpresso,
                Bandeira = cartao.Bandeira,
                Cvc = cartao.Cvc,
                ClienteId = cartao.ClienteId,
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

        public async Task<ResponseCartaoDTO?> ObterCartaoPorId(int cartaoId)
        {
            var cartao = await _cartaoRepository.ObterCartaoPorId(cartaoId);
            
            if (cartao == null)
                return null;

            return new ResponseCartaoDTO
            {
                Id = cartao.Id,
                NumCartao = cartao.NumCartao,
                NomeImpresso = cartao.NomeImpresso,
                Bandeira = cartao.Bandeira,
                Preferencial = cartao.Preferencial,
                ClienteId = cartao.ClienteId
            };
        }

        public Task<bool> ValidarBandeiraCartao(int bandeira)
        {
            return Task.FromResult(Enum.IsDefined(typeof(BandeiraCartao), bandeira));
        }

        private async Task ValidarCartao(CadastrarCartaoDTO cartao)
        {
            // Validar bandeira (RN0025)
            if (!await ValidarBandeiraCartao((int)cartao.Bandeira))
            {
                throw new ArgumentException("Bandeira de cartão inválida. Bandeiras permitidas: Visa, Mastercard, American Express, Elo, HiperCard, Aura.");
            }

            // Validar formato do número do cartão
            if (!ValidarFormatoNumeroCartao(cartao.NumCartao))
            {
                throw new ArgumentException("Formato do número do cartão inválido.");
            }

            // Validar CVC baseado na bandeira
            if (!ValidarCvc(cartao.Cvc, cartao.Bandeira))
            {
                throw new ArgumentException("CVC inválido para a bandeira do cartão.");
            }
        }

        private bool ValidarFormatoNumeroCartao(string numCartao)
        {
            // Remove espaços e hífens
            var numeroLimpo = numCartao.Replace(" ", "").Replace("-", "");
            
            // Verifica se contém apenas dígitos
            if (!numeroLimpo.All(char.IsDigit))
                return false;

            // Verifica comprimento baseado na bandeira (implementação básica)
            return numeroLimpo.Length >= 13 && numeroLimpo.Length <= 19;
        }

        private bool ValidarCvc(int cvc, BandeiraCartao bandeira)
        {
            // American Express usa 4 dígitos, outras bandeiras usam 3
            if (bandeira == BandeiraCartao.AmericanExpress)
            {
                return cvc >= 1000 && cvc <= 9999;
            }
            else
            {
                return cvc >= 100 && cvc <= 999;
            }
        }
    }
}
