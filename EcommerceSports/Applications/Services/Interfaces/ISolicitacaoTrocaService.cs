using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface ISolicitacaoTrocaService
    {
        Task<ResponseSolicitacaoTrocaDTO> CriarSolicitacaoAsync(CriarSolicitacaoTrocaDTO criarDto, int clienteId);
        Task<ResponseSolicitacaoTrocaDTO> ObterSolicitacaoPorIdAsync(int id);
        Task<IEnumerable<ResponseSolicitacaoTrocaDTO>> ObterSolicitacoesPorClienteAsync(int clienteId);
        Task<IEnumerable<ResponseSolicitacaoTrocaDTO>> ObterTodasSolicitacoesAsync();
        Task<ResponseSolicitacaoTrocaDTO> AprovarSolicitacaoAsync(int solicitacaoId, AprovarSolicitacaoTrocaDTO aprovarDto);
        Task<ResponseSolicitacaoTrocaDTO> AtualizarStatusAsync(int solicitacaoId, Models.Enums.StatusSolicitacaoTroca novoStatus, string? observacoes = null);
        Task<ResponseSolicitacaoTrocaDTO> DefinirEmTransporteAsync(int solicitacaoId);
        Task<ResponseSolicitacaoTrocaDTO> ConfirmarRecebimentoAsync(int solicitacaoId);
        Task<ResponseSolicitacaoTrocaDTO> GerarCupomTrocaAsync(int solicitacaoId);
        Task<ResponseSolicitacaoTrocaDTO> FinalizarTrocaAsync(int solicitacaoId);
    }
}

