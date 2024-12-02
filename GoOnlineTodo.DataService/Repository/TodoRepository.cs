using GoOnlineTodo.DataService.Data;
using GoOnlineTodo.Entities.DbSet;
using GoOnlineTodo.Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOnlineTodo.DataService.Repository
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        internal DbSet<Todo> _todoSet;
        public TodoRepository(AppDbContext context, ILogger logger)
        {
            _logger = logger;
            _context = context;
            _todoSet = _context.Set<Todo>();
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {   
            try
            {
                return await _todoSet.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<Todo?> GetTodoByIdAsync(Guid id)
        {
            try
            {
                var result = await _todoSet.FindAsync(id);
                if (result == null)
                {
                    return null;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<IEnumerable<Todo>> GetTodosDueThisWeekAsync()
        {
            try
            {
                var today = DateTime.Today;
                // When we are checking for todos due today on a sunday we only care for that day, therefore use modulo 7
                var sunday = today.AddDays((7 - (int)today.DayOfWeek) % 7);

                return await _todoSet
                    .AsNoTracking()
                    .Where(todo => todo.ExpiryDateTime.Date >= today && todo.ExpiryDateTime.Date <= sunday)
                    .Where(todo => todo.PercentComplete < 100)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<IEnumerable<Todo>> GetTodosDueTodayAsync()
        {
            try
            {
                var tomorrow = DateTime.Today.AddDays(1);
                return await _todoSet
                    .AsNoTracking()
                    .Where(todo => todo.ExpiryDateTime >= DateTime.Today && todo.ExpiryDateTime < tomorrow)
                    .Where(todo => todo.PercentComplete < 100)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<IEnumerable<Todo>> GetTodosDueTomorrowAsync()
        {
            try
            {
                var dayPastTomorrow = DateTime.Today.AddDays(2);
                return await _todoSet
                    .AsNoTracking()
                    .Where(todo => todo.ExpiryDateTime >= DateTime.Today && todo.ExpiryDateTime < dayPastTomorrow)
                    .Where(todo => todo.PercentComplete < 100)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<Todo> InsertTodoAsync(TodoRequestDto todoDto)
        {
            try
            {
                var todo = new Todo
                {
                    ExpiryDateTime = todoDto.ExpiryDateTime,
                    Title = todoDto.Title,
                    Description = todoDto.Description,
                    PercentComplete = todoDto.PercentComplete
                };
                await _todoSet.AddAsync(todo);
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<bool> UpdateTodoAsync(Guid id, TodoRequestDto updatedTodoDto)
        {
            try
            {
                var todo = await _todoSet.FindAsync(id);
                if (todo == null)
                {
                    return false;
                }

                todo.ExpiryDateTime = updatedTodoDto.ExpiryDateTime;
                todo.Title = updatedTodoDto.Title;
                todo.Description = updatedTodoDto.Description;
                todo.PercentComplete = updatedTodoDto.PercentComplete;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<bool> UpdateCompletionAsync(Guid id, double completionPercentage)
        {
            try
            {
                var todo = await _todoSet.FindAsync(id);

                if (todo == null)
                {
                    return false;
                }

                todo.PercentComplete = completionPercentage;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }

        public async Task<bool> DeleteTodoAsync(Guid id)
        {
            try
            {
                var todo = await _todoSet.FindAsync(id);

                if (todo == null)
                {
                    return false;
                }

                _todoSet.Remove(todo);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} All function error", typeof(TodoRepository));
                throw;
            }
        }
    }
}
