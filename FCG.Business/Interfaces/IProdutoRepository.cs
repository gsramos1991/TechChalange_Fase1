using FCG.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Business.Interfaces;
public interface IProdutoRepository
{
    Task<Produto> Adicionar(Produto produto);
    Task Atualizar(Produto produto);
    Task Remover(Guid id);
    Task<Produto> ObterPorId(Guid id); 
    Task<IEnumerable<Produto>> ListarTodos();
    Task<IEnumerable<Produto>> ListarTodosComExcluidos();
}
