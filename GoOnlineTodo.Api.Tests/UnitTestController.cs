using Moq;
using GoOnlineTodo.DataService.Data;
using GoOnlineTodo.Entities.DbSet;
using GoOnlineTodo.Entities.DTOs;

namespace GoOnlineTodo.Api.Tests
{
    public class UnitTestController
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly List<Todo> _mockTodos;

        public UnitTestController() 
        {
            _unitOfWork = new Mock<IUnitOfWork>();
           _mockTodos = new List<Todo>
           {
               new Todo
               {
                   TodoId = Guid.NewGuid(),
                   ExpiryDateTime = DateTime.Now.AddDays(1),
                   Title = "Complete project documentation",
                   Description = "Finalize and upload the project documentation to the shared drive.",
                   PercentComplete = 50.0
               },
               new Todo
               {
                   TodoId = Guid.NewGuid(),
                   ExpiryDateTime = DateTime.Now.AddDays(2),
                   Title = "Prepare presentation",
                   Description = "Prepare the slides for the upcoming team meeting.",
                   PercentComplete = 30.0
               },
               new Todo
               {
                   TodoId = Guid.NewGuid(),
                   ExpiryDateTime = DateTime.Today,
                   Title = "Fix critical bug",
                   Description = "Resolve the issue causing crashes in the user authentication module.",
                   PercentComplete = 70.0
               },
               new Todo
               {
                   TodoId = Guid.NewGuid(),
                   ExpiryDateTime = DateTime.Now.AddDays(7),
                   Title = "Plan team outing",
                   Description = "Organize and finalize details for the team-building outing.",
                   PercentComplete = 10.0
               },
               new Todo
               {
                   TodoId = Guid.NewGuid(),
                   ExpiryDateTime = DateTime.Now.AddMonths(1),
                   Title = "Annual performance review",
                   Description = "Prepare notes and data for the upcoming performance review meeting.",
                   PercentComplete = 0.0
               }
           };
        }

        [Fact]
        public async void GetAllTodosAsync_ReturnsAllTodos()
        {
            _unitOfWork.Setup(u => u.TodoRepository.GetAllTodosAsync()).ReturnsAsync(_mockTodos);
            var result = await _unitOfWork.Object.TodoRepository.GetAllTodosAsync();
            Assert.NotNull(result);
            Assert.Equal(5, result.Count());
            Assert.Contains(result, todo => todo.Title == "Fix critical bug");
        }

        [Fact]
        public async void GetTodoByIdAsync_ReturnsSpecifiedTodo()
        {
            var targetTodo = _mockTodos[4];
            var targetTodoId = targetTodo!.TodoId;

            _unitOfWork
                .Setup(u => u.TodoRepository.GetTodoByIdAsync(targetTodoId))
                .ReturnsAsync(_mockTodos.FirstOrDefault(todo => todo.TodoId == targetTodoId));

            var result = await _unitOfWork.Object.TodoRepository.GetTodoByIdAsync(targetTodoId);
            Assert.NotNull(result);
            Assert.Equal(targetTodoId, result.TodoId);
            Assert.Equal(targetTodo.Title, result.Title);
            Assert.Equal(targetTodo.Description, result.Description);
            Assert.Equal(targetTodo.PercentComplete, result.PercentComplete);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsNull_WhenTodoNotFound()
        {
            var guidNotInList = Guid.NewGuid();

            _unitOfWork.Setup(u => u.TodoRepository.GetTodoByIdAsync(guidNotInList))
                       .ReturnsAsync(_mockTodos.FirstOrDefault(t => t.TodoId == guidNotInList));

            var result = await _unitOfWork.Object.TodoRepository.GetTodoByIdAsync(guidNotInList);

            Assert.Null(result); // Verify the result is null for a non-existing ID
        }

        [Fact]
        public async Task InsertTodoAsync_AddsTodo()
        {
            var todoDto = new TodoRequestDto
            {
                ExpiryDateTime = DateTime.Today.AddDays(1),
                Title = "Foo",
                Description = "Bar",
                PercentComplete = 90
            };

            _unitOfWork.Setup(u => u.TodoRepository.InsertTodoAsync(todoDto)).ReturnsAsync((TodoRequestDto todoDto) => {
                return new Todo
                {
                    TodoId = Guid.NewGuid(),
                    Title = todoDto.Title,
                    Description = todoDto.Description,
                    PercentComplete = todoDto.PercentComplete,
                    ExpiryDateTime = todoDto.ExpiryDateTime,
                };
            });

            var result = await _unitOfWork.Object.TodoRepository.InsertTodoAsync(todoDto);

            Assert.NotNull(result);
            Assert.Equal(todoDto.Title, result!.Title);
            Assert.Equal(todoDto.Description, result.Description);
            Assert.Equal(todoDto.ExpiryDateTime, result.ExpiryDateTime);
            Assert.Equal(todoDto.PercentComplete, result.PercentComplete);
        }

        [Fact]
        public async Task UpdateTodoAsync_UpdatesSpecifiedTodo()
        {
            var todoForUpdating = _mockTodos[0];
            var todoDto = new TodoRequestDto
            {
                ExpiryDateTime = DateTime.Today.AddDays(3),
                Title = "Updated",
                Description = "Updated",
                PercentComplete = 50
            };

            // Use a callback to update the mocked list
            _unitOfWork.Setup(u => u.TodoRepository.UpdateTodoAsync(todoForUpdating.TodoId, todoDto))
                .Callback<Guid, TodoRequestDto>((id, todoDto) =>
                {
                    var todo = _mockTodos.FirstOrDefault(todo => todo.TodoId == id);
                    todo!.ExpiryDateTime = todoDto.ExpiryDateTime;
                    todo.Title = todoDto.Title;
                    todo.Description = todoDto.Description;
                    todo.PercentComplete = todoDto.PercentComplete;
                })
                .ReturnsAsync(true);
                
            var result = await _unitOfWork.Object.TodoRepository.UpdateTodoAsync(todoForUpdating.TodoId, todoDto);
            Assert.True(result);
            Assert.Equal(todoDto.ExpiryDateTime, _mockTodos[0].ExpiryDateTime);
            Assert.Equal(todoDto.Title, _mockTodos[0].Title);
            Assert.Equal(todoDto.Description, _mockTodos[0].Description);
            Assert.Equal(todoDto.PercentComplete, _mockTodos[0].PercentComplete);
        }

        [Fact]
        public async Task DeleteTodoAsync_DeletesSpecifiedTodo()
        {
            var todoForDeleting = _mockTodos[0]; 
            var todoIdToDelete = todoForDeleting.TodoId;

            _unitOfWork.Setup(u => u.TodoRepository.DeleteTodoAsync(todoIdToDelete))
                .Callback<Guid>((id) =>
                {
                    var todoToDelete = _mockTodos.FirstOrDefault(t => t.TodoId == id);
                    if (todoToDelete != null)
                    {
                        _mockTodos.Remove(todoToDelete);
                    }
                })
                .ReturnsAsync(true);

            var result = await _unitOfWork.Object.TodoRepository.DeleteTodoAsync(todoIdToDelete);
    
            Assert.True(result); 
            Assert.DoesNotContain(todoForDeleting, _mockTodos);
        }

        [Fact]
        public async Task UpdateCompletionAsync_UpdatesPercentComplete()
        {       
            var todoForUpdating = _mockTodos[0]; 
            var todoIdToUpdate = todoForUpdating.TodoId;
            var newCompletionPercentage = 75.0;

            _unitOfWork.Setup(u => u.TodoRepository.UpdateCompletionAsync(todoIdToUpdate, newCompletionPercentage))
                .Callback<Guid, double>((id, completionPercentage) =>
                {
                    var todoToUpdate = _mockTodos.FirstOrDefault(t => t.TodoId == id);
                    if (todoToUpdate != null)
                    {
                        todoToUpdate.PercentComplete = completionPercentage;
                    }
                })
                .ReturnsAsync(true);

            _unitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(true);

            var result = await _unitOfWork.Object.TodoRepository.UpdateCompletionAsync(todoIdToUpdate, newCompletionPercentage);

            Assert.True(result); 
            Assert.Equal(newCompletionPercentage, todoForUpdating.PercentComplete);
        }

        [Fact]
        public async Task GetTodosDueTodayAsync_ReturnsTodosDueToday()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var expectedTodos = _mockTodos
                .Where(todo => todo.ExpiryDateTime >= DateTime.Today && todo.ExpiryDateTime < tomorrow)
                .Where(todo => todo.PercentComplete < 100);

            _unitOfWork.Setup(u => u.TodoRepository.GetTodosDueTodayAsync())
                .ReturnsAsync(expectedTodos);

            var result = await _unitOfWork.Object.TodoRepository.GetTodosDueTodayAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTodosDueTomorrowAsync_ReturnsTodosDueTomorrow()
        {
            var dayPastTomorrow = DateTime.Today.AddDays(2);
            var expectedTodos = _mockTodos
                .Where(todo => todo.ExpiryDateTime >= DateTime.Today && todo.ExpiryDateTime < dayPastTomorrow)
                .Where(todo => todo.PercentComplete < 100);

            _unitOfWork.Setup(u => u.TodoRepository.GetTodosDueTomorrowAsync())
                .ReturnsAsync(expectedTodos);

            var result = await _unitOfWork.Object.TodoRepository.GetTodosDueTomorrowAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTodosDueThisWeekAsync_ReturnsTodosDueThisWeek()
        {
            var today = DateTime.Today;
            // When we are checking for todos due today on a sunday we only care for that day, therefore use modulo 7
            var sunday = today.AddDays((7 - (int)today.DayOfWeek) % 7);

            var expectedTodos = _mockTodos
                .Where(todo => todo.ExpiryDateTime.Date >= today && todo.ExpiryDateTime.Date <= sunday)
                .Where(todo => todo.PercentComplete < 100);

            _unitOfWork.Setup(u => u.TodoRepository.GetTodosDueThisWeekAsync())
                .ReturnsAsync(expectedTodos);

            var result = await _unitOfWork.Object.TodoRepository.GetTodosDueThisWeekAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }
    }
}