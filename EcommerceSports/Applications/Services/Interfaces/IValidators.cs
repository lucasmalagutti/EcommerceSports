using EcommerceSports.Models.Entity;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IValidators
    {
        public void ValidarSenha(string senha);
        public void ValidarEnderecos(List<Endereco> enderecos);
        public Task ValidarCpfExistente(string cpf, int? clienteIdExcluir = null);

    }
}
