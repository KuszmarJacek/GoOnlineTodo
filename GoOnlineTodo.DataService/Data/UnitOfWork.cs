using GoOnlineTodo.DataService.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOnlineTodo.DataService.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public readonly AppDbContext _context;
        public ITodoRepository TodoRepository { get; }

        public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            var logger = loggerFactory.CreateLogger("logs");
            TodoRepository = new TodoRepository(_context, logger);
        }

        public async Task<bool> CompleteAsync()
        {
            var result = await _context.SaveChangesAsync();
            // if more than 0 success, else fail
            return result > 0;
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
