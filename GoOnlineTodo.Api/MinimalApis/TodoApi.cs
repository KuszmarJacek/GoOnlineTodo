using FluentValidation;
using GoOnlineTodo.DataService.Data;
using GoOnlineTodo.Entities.DbSet;
using GoOnlineTodo.Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GoOnlineTodo.Api.MinimalApis
{
    public static class TodoApi
    {
        public static void MapTodoApi(this IEndpointRouteBuilder builder)
        {
            var todoGroupApiV1 = builder.MapGroup("/api/v1");

            todoGroupApiV1.MapGet("/todoitems", async (IUnitOfWork unitOfWork) =>
            {
                var result =  await unitOfWork.TodoRepository.GetAllTodosAsync();
                return TypedResults.Ok(result);
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint returns every todo in the database",
                Description = "If you have a lot of todos, consider using a solution with pagination. There is no sorting of returned todos."
            });

            todoGroupApiV1.MapGet("/todoitems/{id:guid}", async (Guid id, IUnitOfWork unitOfWork) =>
            {
                var todo = await unitOfWork.TodoRepository.GetTodoByIdAsync(id);
                if (todo == null)
                {
                    return Results.NotFound($"Todo with Id {id} was not found.");
                }

                return TypedResults.Ok(todo);
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint returns a singular todo",
                Description = ""
            });

            todoGroupApiV1.MapGet("/todoitems/due-today", async (IUnitOfWork unitOfWork) =>
            {
                var todosDueToday = await unitOfWork.TodoRepository.GetTodosDueTodayAsync();
                return TypedResults.Ok(todosDueToday);
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint returns all tasks due today",
                Description = "Returned todos are compared to the 00:00:00 of current day and the beginning of tomorrow. There is no sorting by time."
            }); 

            /* 
             * Get Todos that are due tomorrow
             * Next day refers to just the next day? Or inclusive with today's todos?
             * Below implementation includes today's and tomorrow's todos.
             */
            todoGroupApiV1.MapGet("/todoitems/due-tomorrow", async (IUnitOfWork unitOfWork) =>
            {
                var todosDueTomorrow = await unitOfWork.TodoRepository.GetTodosDueTomorrowAsync();
                return TypedResults.Ok(todosDueTomorrow);
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint returns all tasks due from today untill the end of tomorrow",
                Description = "Returned todos are compared to the 00:00:00 of current day and the end of tomorrow. There is no sorting by date and hour."
            });

            /* 
             * Get Todos that are due this week
             * Do we consider this week as in working days week where it's monday-friday or are weekends to be included as well?
             * This implementation considers sunday as the end of the week since some jobs do work weekends and having todos makes sens in that situation.
             */
            todoGroupApiV1.MapGet("/todoitems/due-this-week", async (IUnitOfWork unitOfWork) =>
            {
                var todosDueThisWeek = await unitOfWork.TodoRepository.GetTodosDueThisWeekAsync();
                return TypedResults.Ok(todosDueThisWeek);
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint returns all tasks due this week",
                Description = "Returned todos are compared to the 00:00:00 of current day and the end of sunday of this current week. " +
                    "In case of a sunday, it returns only the todos due on sunday. There is no sorting by date."
            });

            todoGroupApiV1.MapPost("/todoitems", async (IValidator<TodoRequestDto> validator, TodoRequestDto todoDto, IUnitOfWork unitOfWork) =>
            {
                var validationResult = await validator.ValidateAsync(todoDto);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var todo = await unitOfWork.TodoRepository.InsertTodoAsync(todoDto);
                await unitOfWork.CompleteAsync();
                return TypedResults.Created($"/todoitems/{todo.TodoId}", todo);
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint creates a todo",
                Description = "It take a todoDto without exposing unnecessary fields like Id, creationDate, status etc..."
            });

            todoGroupApiV1.MapPut("/todoitems/{id:guid}", async (IValidator<TodoRequestDto> validator, Guid id, TodoRequestDto updatedTodoDto, IUnitOfWork unitOfWork) =>
            {
                var validationResult = await validator.ValidateAsync(updatedTodoDto);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await unitOfWork.TodoRepository.UpdateTodoAsync(id, updatedTodoDto);

                if (!result)
                {
                    return TypedResults.NotFound($"Todo with Id: {id} was not found.");
                }

                await unitOfWork.CompleteAsync();
                return TypedResults.NoContent();
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint updates a todo",
                Description = "It updates the whole todo with the incoming DTOs values. " +
                    "If you need to only update a singular value, for example the completion percentage of a task, " +
                    "look for the change-completion-percentage endpoint or a similar one suited for you needs."
            });

            todoGroupApiV1.MapPatch("/todoitems/{id:guid}/change-completion-percentage", async (Guid id, double percentageCompleted, IUnitOfWork unitOfWork) =>
            {
                // Wrapping a single double, creating a custom validator and then using DI to inject the validator seems like overkill
                if (percentageCompleted < 0 || percentageCompleted > 100)
                {
                    return Results.BadRequest("PercentComplete must be between 0 and 100.");
                }

                var result = await unitOfWork.TodoRepository.UpdateCompletionAsync(id, percentageCompleted);

                if (!result)
                {
                    return Results.NotFound($"Todo with Id {id} was not found.");
                }

                await unitOfWork.CompleteAsync();
                return TypedResults.NoContent();
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint updates only the completion percentage of a todo",
                Description = ""
            });

            todoGroupApiV1.MapPatch("/todoitems/{id:guid}/mark-as-done", async (Guid id, IUnitOfWork unitOfWork) =>
            {
                var result = await unitOfWork.TodoRepository.UpdateCompletionAsync(id, 100);

                if (!result)
                {
                    return Results.NotFound($"Todo with Id {id} was not found.");
                }

                await unitOfWork.CompleteAsync();
                return TypedResults.NoContent();
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint marks the todo as done",
                Description = "It it essentially the same as the change-completion percentage endpoint, but automatically sets the value as 100"
            });

            todoGroupApiV1.MapDelete("/todoitems/{id:guid}", async (Guid id, IUnitOfWork unitOfWork) =>
            {
                var result = await unitOfWork.TodoRepository.DeleteTodoAsync(id);

                // If the Todo item is not found, return a NotFound response
                if (!result)
                {
                    return Results.NotFound($"Todo with Id {id} was not found.");
                }

                await unitOfWork.CompleteAsync(); ;

                return Results.NoContent();
            })
            .WithOpenApi(operation => new(operation)
            {
                Summary = "This endpoint deletes a TODO",
                Description = "It takes a single GUID id as it's parameter"
            });
        }
    }
}
