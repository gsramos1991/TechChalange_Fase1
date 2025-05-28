using FCG.Api.Dto;
using FCG.Business.Models;

namespace FCG.Api.Extensions;

public static class ProdutoMappingExtensions
{
    public static ProdutoDto ToDto(this Produto produto)
    {
        if (produto == null) return null;

        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Categoria = produto.Categoria,
            Preco = produto.Preco,
            DataLancamento = produto.DataLancamento
        };
    }


    public static Produto ToDomain(this ProdutoDto dto)
    {
        if (dto == null) return null;

        return new Produto(
            id: dto.Id,
            nome: dto.Nome,
            descricao: dto.Descricao,
            categoria: dto.Categoria,
            preco: dto.Preco,
            dataLancamento: dto.DataLancamento
        );
    }
}