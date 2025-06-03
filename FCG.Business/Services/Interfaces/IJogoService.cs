using FCG.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Business.Services.Interfaces;
public interface IJogoService
{
    Task<Jogo> Adicionar(Jogo produto);
    Task Atualizar(Jogo produto);
    Task<IEnumerable<Jogo>> ListarTodos();
    Task<IEnumerable<Jogo>> ListarTodosComExcluidos();
    Task<Jogo> ObterPorId(Guid id);
    Task Remover(Guid id);
}

