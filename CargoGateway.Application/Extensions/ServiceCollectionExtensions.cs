using Microsoft.Extensions.DependencyInjection;

namespace CargoGateway.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Регистрация сервисов Core будет здесь
        // Сейчас ничего не регистрируем, но оставляем метод для будущих расширений
        return services;
    }
}