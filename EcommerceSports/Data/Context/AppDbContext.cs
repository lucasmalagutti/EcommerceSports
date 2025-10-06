
using Microsoft.EntityFrameworkCore;
using EcommerceSports.Models.Entity;



namespace EcommerceSports.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CartaoCredito> Cartoes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Telefone> Telefones { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Cupom> Cupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DtCadastro).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CadastroAtivo).HasDefaultValue(true);
                
                entity.Property(e => e.DtNasc).HasColumnType("timestamp with time zone");
                entity.Property(e => e.DtCadastro).HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Endereco>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Endereco)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Telefone>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Telefones)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartaoCredito>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Cartoes)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Categoria).IsRequired();
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.Descricao).IsRequired();
                entity.Property(e => e.Imagem);
            });

            modelBuilder.Entity<Cupom>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nome).IsRequired();
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DataPedido).HasColumnType("timestamp with time zone");
                entity.Property(e => e.StatusPedido).HasConversion<int>();
                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Pedidos)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItemPedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.Pedido)
                      .WithMany(p => p.Itens)
                      .HasForeignKey(e => e.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Produto)
                      .WithMany()
                      .HasForeignKey(e => e.ProdutoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DataTransacao).HasColumnType("timestamp with time zone");
                entity.Property(e => e.ValorTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ValorFrete);
                entity.HasOne(e => e.Pedido)
                      .WithOne(p => p.Transacao)
                      .HasForeignKey<Transacao>(e => e.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Endereco)
                      .WithMany()
                      .HasForeignKey(e => e.EnderecoId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Pagamentos)
                      .WithOne(p => p.Transacao)
                      .HasForeignKey(p => p.TransacaoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DataPagamento).HasColumnType("timestamp with time zone");
                entity.Property(e => e.Valor).HasColumnType("decimal(18,2)");
                entity.HasOne(e => e.Transacao)
                      .WithMany(t => t.Pagamentos)
                      .HasForeignKey(e => e.TransacaoId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Cartao)
                      .WithMany()
                      .HasForeignKey(e => e.CartaoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
