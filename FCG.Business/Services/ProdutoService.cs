using FCG.Business.Interfaces;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Business.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<Produto> Adicionar(Produto produto)
    {
        // Validações de negócio podem ser feitas aqui, se necessário
        return await _produtoRepository.Adicionar(produto);
    }

    public async Task Atualizar(Produto produto)
    {
        await _produtoRepository.Atualizar(produto);
    }

    public async Task<IEnumerable<Produto>> ListarTodos()
    {
        return await _produtoRepository.ListarTodos();
    }

    public async Task<IEnumerable<Produto>> ListarTodosComExcluidos()
    {
        return await _produtoRepository.ListarTodosComExcluidos();
    }


    public async Task<Produto> ObterPorId(Guid id)
    {
        return await _produtoRepository.ObterPorId(id);
    }

    public async Task Remover(Guid id)
    {
        await _produtoRepository.Remover(id);
    }
}
