using FCG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace FCG.Data.Mappings;

public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produto");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasColumnType("varchar(200)");

        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasColumnType("varchar(1000)");

        builder.Property(p => p.Categoria)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(p => p.Preco)
            .IsRequired()
            .HasColumnType("decimal(18,2)");  

        builder.Property(p => p.DataLancamento)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(p => p.DataCriacao)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(p => p.DataAtualizacao)
            .IsRequired(false)
            .HasColumnType("datetime");

        builder.Property(p => p.Excluido)
            .IsRequired(true)
            .HasColumnType("bit");
    }
}