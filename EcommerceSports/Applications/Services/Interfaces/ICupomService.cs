using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface ICupomService
    {
        Task<ResponseCupomDTO> ValidarCupomAsync(string nome);
    }
}
