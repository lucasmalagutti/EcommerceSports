using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;

namespace EcommerceSports.Applications.Services
{
    public class CupomService : ICupomService
    {
        private readonly ICupomRepository _cupomRepository;

        public CupomService(ICupomRepository cupomRepository)
        {
            _cupomRepository = cupomRepository;
        }

        public async Task<ResponseCupomDTO> ValidarCupomAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return new ResponseCupomDTO
                {
                    Valido = false,
                    Mensagem = "Nome do cupom é obrigatório"
                };
            }

            var cupom = await _cupomRepository.ObterCupomPorNomeAsync(nome);

            if (cupom == null)
            {
                return new ResponseCupomDTO
                {
                    Valido = false,
                    Mensagem = "Cupom não encontrado"
                };
            }

            if (cupom.Utilizado)
            {
                return new ResponseCupomDTO
                {
                    Valido = false,
                    Nome = cupom.Nome,
                    Mensagem = "Este cupom já foi utilizado e não pode ser utilizado novamente"
                };
            }

            return new ResponseCupomDTO
            {
                Valido = true,
                Nome = cupom.Nome,
                Desconto = cupom.Desconto,
                Mensagem = "Cupom válido"
            };
        }
    }
}
