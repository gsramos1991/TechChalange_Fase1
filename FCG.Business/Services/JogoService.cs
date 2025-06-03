using FCG.Business.Interfaces;
using FCG.Business.Models;
using FCG.Business.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Business.Services;

public class JogoService : IJogoService
{
    private readonly IJogoRepository _produtoRepository;

    public JogoService(IJogoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<Jogo> Adicionar(Jogo produto)
    {
        return await _produtoRepository.Adicionar(produto);
    }

    public async Task Atualizar(Jogo produto)
    {
        await _produtoRepository.Atualizar(produto);
    }

    public async Task<IEnumerable<Jogo>> ListarTodos()
    {
        return await _produtoRepository.ListarTodos();
    }

    public async Task<IEnumerable<Jogo>> ListarTodosComExcluidos()
    {
        return await _produtoRepository.ListarTodosComExcluidos();
    }


    public async Task<Jogo> ObterPorId(Guid id)
    {
        return await _produtoRepository.ObterPorId(id);
    }

    public async Task Remover(Guid id)
    {
        await _produtoRepository.Remover(id);
    }
}
