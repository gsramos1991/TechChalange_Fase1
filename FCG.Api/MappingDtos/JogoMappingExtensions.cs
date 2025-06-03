using FCG.Api.Dto;
using FCG.Business.Models;

namespace FCG.Api.MappingDtos;

public static class JogoMappingExtensions
{
    public static JogoDto ToDto(this Jogo produto)
    {
        if (produto == null) return null;

        return new JogoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Categoria = produto.Categoria,
            Preco = produto.Preco,
            DataLancamento = produto.DataLancamento
        };
    }


    public static Jogo ToDomain(this JogoDto dto)
    {
        if (dto == null) return null;

        return new Jogo(
            id: dto.Id,
            nome: dto.Nome,
            descricao: dto.Descricao,
            categoria: dto.Categoria,
            preco: dto.Preco,
            dataLancamento: dto.DataLancamento
        );
    }
}