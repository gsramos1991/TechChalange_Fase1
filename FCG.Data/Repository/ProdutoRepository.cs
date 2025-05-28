using FCG.Business.Interfaces;
using FCG.Business.Models;
using FCG.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Data.Repository;
public class ProdutoRepository : IProdutoRepository
{
    protected readonly ApplicationDbContext _db;
    public ProdutoRepository(ApplicationDbContext context) 
    { 
        _db = context;
    }

    public async Task<Produto> Adicionar(Produto produto)
    {
        _db.Produto.Add(produto);
        await _db.SaveChangesAsync();
        return produto;
    }

    public async Task Atualizar(Produto produto)
    {
        _db.Produto.Update(produto);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Produto>> ListarTodos()
    {
        return await _db.Produto.AsNoTracking().OrderBy(p => p.Nome).ToListAsync();
    }
    public async Task<IEnumerable<Produto>> ListarTodosComExcluidos()
    {
        return await _db.Produto.IgnoreQueryFilters().AsNoTracking().OrderBy(p => p.Nome).ToListAsync();
    }

    public async Task<Produto> ObterPorId(Guid id)
    {
        return await  _db.Produto.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Remover(Guid id)
    {
        var produto = await _db.Produto.FindAsync(id);

        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.Excluir();
        await _db.SaveChangesAsync();
    }
}
