namespace EcommerceSports.Models.Enums
{
    public enum StatusSolicitacaoTroca
    {
        Pendente = 1,           // Solicitação criada, aguardando análise do administrador
        Aprovada = 2,           // Administrador aprovou a solicitação
        Negada = 3,             // Administrador negou a solicitação
        EmTransporte = 4,       // Produto está sendo transportado de volta
        ProdutoRecebido = 5,    // Administrador confirmou recebimento do produto
        CupomGerado = 6,        // Cupom de troca foi gerado
        Finalizada = 7          // Troca/devolução finalizada
    }
}

