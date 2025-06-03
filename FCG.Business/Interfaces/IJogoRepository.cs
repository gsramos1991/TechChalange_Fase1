using FCG.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Business.Interfaces;
public interface IJogoRepository
{
    Task<Jogo> Adicionar(Jogo produto);
    Task Atualizar(Jogo produto);
    Task Remover(Guid id);
    Task<Jogo> ObterPorId(Guid id); 
    Task<IEnumerable<Jogo>> ListarTodos();
    Task<IEnumerable<Jogo>> ListarTodosComExcluidos();
}
