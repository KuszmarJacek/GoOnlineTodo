using GoOnlineTodo.Entities.Validators;
using FluentValidation;
using GoOnlineTodo.Entities.DTOs;

namespace GoOnlineTodo.Api.Extensions
{
    public static class ValidationServiceExtension
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<TodoRequestDto>, TodoRequestValidator>();
            return services;
        }
    }
}
