using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentMigrator.Runner;
using UsersRoles.Data;
using UsersRoles.Entities;

//Подключение к базе данных с помощью файла appsettings.json, в котором храняться входные данные.
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Ошибка: строка подключения не найдена.");
    return;
}


// Настройка внедрения зависимостей.
var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddSingleton<AppDbContext>(_ => new AppDbContext(connectionString));
services.AddScoped<IUserRepository, UserRepository>();


// Добавляем FluentMigrator для создания/обновления схемы базы данных.
services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(Program).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

var serviceProvider = services.BuildServiceProvider();


// Получаем логгер и репозиторий.
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var userRepo = serviceProvider.GetRequiredService<IUserRepository>();

try
{
    // Проверяем наличие таблиц и запускаем миграцию.
    var tableExists = await userRepo.TableExistsAsync("users");
    if (!tableExists)
    {
        logger.LogInformation("Таблицы не найдены. Запуск миграций...");
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        logger.LogInformation("Миграции успешно применены.\n");
    }
    else
    {
        logger.LogInformation("Таблицы уже существуют.\n");
    }

    //Выполнение первой задачи.
    logger.LogInformation("Задача 1: Все пользователи и их роли");
    var users = await userRepo.GetUsersWithRolesAsync();
    foreach (var user in users)
    {
        Console.WriteLine($"Пользователь: {user.UserName} {user.UserSurname} ({user.Email})");
        Console.WriteLine($"  Роли: {string.Join(", ", user.Roles)}");
        Console.WriteLine();
    }

    //Выполнение второй задачи.
    logger.LogInformation("\nЗадача 2: Количество пользователей в каждой роли");
    var roleCounts = await userRepo.GetRoleUserCountsAsync();
    foreach (var rc in roleCounts)
    {
        Console.WriteLine($"Роль: {rc.Name}, Количество пользователей: {rc.UserCount}");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Произошла ошибка при выполнении.");
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();