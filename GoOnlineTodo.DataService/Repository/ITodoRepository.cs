using GoOnlineTodo.Entities.DbSet;
using GoOnlineTodo.Entities.DTOs;

namespace GoOnlineTodo.DataService.Repository
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetAllTodosAsync();
        Task<Todo?> GetTodoByIdAsync(Guid id);
        Task<Todo> InsertTodoAsync(TodoRequestDto todoDto);
        Task<bool> DeleteTodoAsync(Guid id);
        Task<bool> UpdateTodoAsync(Guid id, TodoRequestDto updatedTodoDto);
        // Probably should make a generic function which allows updating specific fields without having to make a concrete implementation for each field.
        Task<bool> UpdateCompletionAsync(Guid id, double completionPercentage);
        Task<IEnumerable<Todo>> GetTodosDueTodayAsync();
        Task<IEnumerable<Todo>> GetTodosDueTomorrowAsync();
        Task<IEnumerable<Todo>> GetTodosDueThisWeekAsync();
    }
}
