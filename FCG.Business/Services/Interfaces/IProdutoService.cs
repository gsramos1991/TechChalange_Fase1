using FCG.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Business.Services.Interfaces;
public interface IProdutoService
{
    Task<Produto> Adicionar(Produto produto);
    Task Atualizar(Produto produto);
    Task<IEnumerable<Produto>> ListarTodos();
    Task<IEnumerable<Produto>> ListarTodosComExcluidos();
    Task<Produto> ObterPorId(Guid id);
    Task Remover(Guid id);
}

