using GoOnlineTodo.DataService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOnlineTodo.DataService.Data
{
    public interface IUnitOfWork
    {
        ITodoRepository TodoRepository { get; }
        Task<bool> CompleteAsync();
    }
}
