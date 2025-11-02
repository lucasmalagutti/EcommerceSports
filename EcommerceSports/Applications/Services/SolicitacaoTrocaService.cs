using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSports.Applications.Services
{
    public class SolicitacaoTrocaService : ISolicitacaoTrocaService
    {
        private readonly ISolicitacaoTrocaRepository _solicitacaoTrocaRepository;
        private readonly ICupomRepository _cupomRepository;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly Data.Context.AppDbContext _context;

        public SolicitacaoTrocaService(
            ISolicitacaoTrocaRepository solicitacaoTrocaRepository,
            ICupomRepository cupomRepository,
            ITransacaoRepository transacaoRepository,
            Data.Context.AppDbContext context)
        {
            _solicitacaoTrocaRepository = solicitacaoTrocaRepository;
            _cupomRepository = cupomRepository;
            _transacaoRepository = transacaoRepository;
            _context = context;
        }

        public async Task<ResponseSolicitacaoTrocaDTO> CriarSolicitacaoAsync(CriarSolicitacaoTrocaDTO criarDto, int clienteId)
        {
            // Validar se o pedido existe e pertence ao cliente
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == criarDto.PedidoId && p.ClienteId == clienteId);

            if (pedido == null)
            {
                throw new ArgumentException("Pedido não encontrado ou não pertence ao cliente");
            }

            // Validar se o pedido já foi entregue
            if (pedido.StatusPedido != StatusPedido.Entregue)
            {
                throw new InvalidOperationException("Apenas pedidos entregues podem ter solicitação de troca/devolução");
            }

            // Se for troca de item específico, validar se o item existe no pedido
            if (criarDto.TipoSolicitacao == TipoSolicitacao.Troca && criarDto.ItemPedidoId.HasValue)
            {
                var itemExiste = pedido.Itens.Any(i => i.Id == criarDto.ItemPedidoId.Value);
                if (!itemExiste)
                {
                    throw new ArgumentException("Item não encontrado no pedido");
                }
            }

            var solicitacao = new SolicitacaoTroca
            {
                PedidoId = criarDto.PedidoId,
                ItemPedidoId = criarDto.ItemPedidoId,
                ClienteId = clienteId,
                TipoSolicitacao = criarDto.TipoSolicitacao,
                Status = StatusSolicitacaoTroca.Pendente,
                Motivo = criarDto.Motivo,
                DataSolicitacao = DateTime.UtcNow
            };

            var solicitacaoCriada = await _solicitacaoTrocaRepository.CriarSolicitacaoTrocaAsync(solicitacao);

            // Atualizar status do pedido para EmTroca
            pedido.StatusPedido = StatusPedido.EmTroca;
            await _context.SaveChangesAsync();

            return MapToResponseDTO(solicitacaoCriada, "Solicitação de troca/devolução criada com sucesso");
        }

        public async Task<ResponseSolicitacaoTrocaDTO> ObterSolicitacaoPorIdAsync(int id)
        {
            var solicitacao = await _solicitacaoTrocaRepository.ObterSolicitacaoPorIdAsync(id);
            
            if (solicitacao == null)
            {
                throw new ArgumentException("Solicitação não encontrada");
            }

            return MapToResponseDTO(solicitacao, "Solicitação encontrada");
        }

        public async Task<IEnumerable<ResponseSolicitacaoTrocaDTO>> ObterSolicitacoesPorClienteAsync(int clienteId)
        {
            var solicitacoes = await _solicitacaoTrocaRepository.ObterSolicitacoesPorClienteAsync(clienteId);
            return solicitacoes.Select(s => MapToResponseDTO(s));
        }

        public async Task<IEnumerable<ResponseSolicitacaoTrocaDTO>> ObterTodasSolicitacoesAsync()
        {
            var solicitacoes = await _solicitacaoTrocaRepository.ObterTodasSolicitacoesAsync();
            return solicitacoes.Select(s => MapToResponseDTO(s));
        }

        public async Task<ResponseSolicitacaoTrocaDTO> AprovarSolicitacaoAsync(int solicitacaoId, AprovarSolicitacaoTrocaDTO aprovarDto)
        {
            var solicitacao = await _solicitacaoTrocaRepository.ObterSolicitacaoPorIdAsync(solicitacaoId);
            
            if (solicitacao == null)
            {
                throw new ArgumentException("Solicitação não encontrada");
            }

            if (solicitacao.Status != StatusSolicitacaoTroca.Pendente)
            {
                throw new InvalidOperationException("Apenas solicitações pendentes podem ser aprovadas/negadas");
            }

            if (aprovarDto.Aprovar)
            {
                solicitacao.Status = StatusSolicitacaoTroca.Aprovada;
                solicitacao.DataAprovacao = DateTime.UtcNow;
            }
            else
            {
                solicitacao.Status = StatusSolicitacaoTroca.Negada;
                solicitacao.DataAprovacao = DateTime.UtcNow;
                
                // Reverter status do pedido se negada
                var pedido = await _context.Pedidos.FindAsync(solicitacao.PedidoId);
                if (pedido != null)
                {
                    pedido.StatusPedido = StatusPedido.Entregue;
                    await _context.SaveChangesAsync();
                }
            }

            solicitacao.ObservacoesAdm = aprovarDto.Observacoes;
            await _solicitacaoTrocaRepository.AtualizarSolicitacaoAsync(solicitacao);

            var mensagem = aprovarDto.Aprovar 
                ? "Solicitação aprovada com sucesso" 
                : "Solicitação negada";

            return MapToResponseDTO(solicitacao, mensagem);
        }

        public async Task<ResponseSolicitacaoTrocaDTO> AtualizarStatusAsync(int solicitacaoId, StatusSolicitacaoTroca novoStatus, string? observacoes = null)
        {
            var solicitacao = await _solicitacaoTrocaRepository.AtualizarStatusAsync(solicitacaoId, novoStatus);
            
            if (solicitacao == null)
            {
                throw new ArgumentException("Solicitação não encontrada");
            }

            if (!string.IsNullOrEmpty(observacoes))
            {
                solicitacao.ObservacoesAdm = observacoes;
                await _solicitacaoTrocaRepository.AtualizarSolicitacaoAsync(solicitacao);
            }

            return MapToResponseDTO(solicitacao, $"Status atualizado para {novoStatus}");
        }

        public async Task<ResponseSolicitacaoTrocaDTO> DefinirEmTransporteAsync(int solicitacaoId)
        {
            var solicitacao = await _solicitacaoTrocaRepository.ObterSolicitacaoPorIdAsync(solicitacaoId);
            
            if (solicitacao == null)
            {
                throw new ArgumentException("Solicitação não encontrada");
            }

            if (solicitacao.Status != StatusSolicitacaoTroca.Aprovada)
            {
                throw new InvalidOperationException("Apenas solicitações aprovadas podem ser marcadas como em transporte");
            }

            solicitacao.Status = StatusSolicitacaoTroca.EmTransporte;
            solicitacao.ObservacoesAdm = "Produto em transporte para retorno";
            await _solicitacaoTrocaRepository.AtualizarSolicitacaoAsync(solicitacao);

            return MapToResponseDTO(solicitacao, "Status atualizado para Em Transporte");
        }

        public async Task<ResponseSolicitacaoTrocaDTO> ConfirmarRecebimentoAsync(int solicitacaoId)
        {
            var solicitacao = await _solicitacaoTrocaRepository.ObterSolicitacaoPorIdAsync(solicitacaoId);
            
            if (solicitacao == null)
            {
                throw new ArgumentException("Solicitação não encontrada");
            }

            if (solicitacao.Status != StatusSolicitacaoTroca.EmTransporte)
            {
                throw new InvalidOperationException("Apenas solicitações em transporte podem ter recebimento confirmado");
            }

            solicitacao.Status = StatusSolicitacaoTroca.ProdutoRecebido;
            solicitacao.DataRecebimento = DateTime.UtcNow;
            await _solicitacaoTrocaRepository.AtualizarSolicitacaoAsync(solicitacao);

            return MapToResponseDTO(solicitacao, "Recebimento do produto confirmado");
        }

        public async Task<ResponseSolicitacaoTrocaDTO> GerarCupomTrocaAsync(int solicitacaoId)
        {
            var solicitacao = await _solicitacaoTrocaRepository.ObterSolicitacaoPorIdAsync(solicitacaoId);
            
            if (solicitacao == null)
            {
                throw new ArgumentException("Solicitação não encontrada");
            }

            if (solicitacao.Status != StatusSolicitacaoTroca.ProdutoRecebido)
            {
                throw new InvalidOperationException("Apenas após confirmar o recebimento do produto o cupom pode ser gerado");
            }

            // Calcular valor do cupom baseado no item ou pedido completo
            decimal valorCupom = 0;
            if (solicitacao.TipoSolicitacao == TipoSolicitacao.Devolucao)
            {
                // Devolução completa: valor total do pedido
                var pedido = await _context.Pedidos.FindAsync(solicitacao.PedidoId);
                if (pedido != null)
                {
                    valorCupom = pedido.ValorTotal;
                }
            }
            else
            {
                // Troca de item específico: valor do item
                if (solicitacao.ItemPedidoId.HasValue)
                {
                    var item = await _context.ItensPedido.FindAsync(solicitacao.ItemPedidoId.Value);
                    if (item != null)
                    {
                        valorCupom = item.PrecoUnitario * item.Quantidade;
                    }
                }
            }

            if (valorCupom <= 0)
            {
                throw new InvalidOperationException("Não foi possível calcular o valor do cupom");
            }

            // Gerar nome único para o cupom
            string cupomNome;
            int tentativas = 0;
            do
            {
                cupomNome = $"TROCA{solicitacaoId}_{DateTime.UtcNow:yyyyMMddHHmmss}{tentativas}";
                tentativas++;
            } while (await _cupomRepository.ExisteCupomAsync(cupomNome) && tentativas < 100);

            if (tentativas >= 100)
            {
                throw new InvalidOperationException("Erro ao gerar nome único para o cupom");
            }

            // Criar cupom com o valor total da troca/devolução
            var cupom = new Cupom
            {
                Nome = cupomNome,
                Desconto = valorCupom,
                Utilizado = false,
                DataUtilizacao = null
            };

            await _cupomRepository.CriarCupomAsync(cupom);

            solicitacao.CupomId = cupom.Id;
            solicitacao.Cupom = cupom;
            solicitacao.ValorCupom = valorCupom;
            solicitacao.Status = StatusSolicitacaoTroca.CupomGerado;
            await _solicitacaoTrocaRepository.AtualizarSolicitacaoAsync(solicitacao);

            return MapToResponseDTO(solicitacao, $"Cupom de troca gerado com sucesso! Código: {cupom.Nome} | Valor: R$ {valorCupom:F2}");
        }

        public async Task<ResponseSolicitacaoTrocaDTO> FinalizarTrocaAsync(int solicitacaoId)
        {
            var solicitacao = await _solicitacaoTrocaRepository.ObterSolicitacaoPorIdAsync(solicitacaoId);
            
            if (solicitacao == null)
            {
                throw new ArgumentException("Solicitação não encontrada");
            }

            if (solicitacao.Status != StatusSolicitacaoTroca.CupomGerado)
            {
                throw new InvalidOperationException("Apenas solicitações com cupom gerado podem ser finalizadas");
            }

            solicitacao.Status = StatusSolicitacaoTroca.Finalizada;
            await _solicitacaoTrocaRepository.AtualizarSolicitacaoAsync(solicitacao);

            // Atualizar status do pedido para Trocado
            var pedido = await _context.Pedidos.FindAsync(solicitacao.PedidoId);
            if (pedido != null)
            {
                pedido.StatusPedido = StatusPedido.Trocado;
                await _context.SaveChangesAsync();
            }

            return MapToResponseDTO(solicitacao, "Troca/devolução finalizada com sucesso");
        }

        private ResponseSolicitacaoTrocaDTO MapToResponseDTO(SolicitacaoTroca solicitacao, string mensagem = "")
        {
            return new ResponseSolicitacaoTrocaDTO
            {
                Id = solicitacao.Id,
                PedidoId = solicitacao.PedidoId,
                ItemPedidoId = solicitacao.ItemPedidoId,
                NomeProduto = solicitacao.ItemPedido?.Produto?.Nome ?? "Pedido completo",
                ClienteId = solicitacao.ClienteId,
                ClienteNome = solicitacao.Cliente?.Nome ?? solicitacao.Pedido?.Cliente?.Nome ?? "Cliente não identificado",
                TipoSolicitacao = solicitacao.TipoSolicitacao,
                Status = solicitacao.Status,
                Motivo = solicitacao.Motivo,
                ObservacoesAdm = solicitacao.ObservacoesAdm,
                DataSolicitacao = solicitacao.DataSolicitacao,
                DataAprovacao = solicitacao.DataAprovacao,
                DataRecebimento = solicitacao.DataRecebimento,
                CupomId = solicitacao.CupomId,
                CupomNome = solicitacao.Cupom?.Nome,
                ValorCupom = solicitacao.ValorCupom,
                Mensagem = mensagem
            };
        }
    }
}

