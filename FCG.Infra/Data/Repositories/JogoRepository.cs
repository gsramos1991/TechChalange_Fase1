using FCG.Business.Interfaces;
using FCG.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.Repositories;

public class JogoRepository : IJogoRepository
{
    protected readonly ApplicationDbContext _db;
    public JogoRepository(ApplicationDbContext context) 
    { 
        _db = context;
    }

    public async Task<Jogo> Adicionar(Jogo produto)
    {
        _db.Jogo.Add(produto);
        await _db.SaveChangesAsync();
        return produto;
    }

    public async Task Atualizar(Jogo produto)
    {
        _db.Jogo.Update(produto);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Jogo>> ListarTodos()
    {
        return await _db.Jogo.AsNoTracking().OrderBy(p => p.Nome).ToListAsync();
    }
    public async Task<IEnumerable<Jogo>> ListarTodosComExcluidos()
    {
        return await _db.Jogo.IgnoreQueryFilters().AsNoTracking().OrderBy(p => p.Nome).ToListAsync();
    }

    public async Task<Jogo> ObterPorId(Guid id)
    {
        return await  _db.Jogo.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Remover(Guid id)
    {
        var produto = await _db.Jogo.FindAsync(id);

        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        produto.Excluir();
        await _db.SaveChangesAsync();
    }
}
