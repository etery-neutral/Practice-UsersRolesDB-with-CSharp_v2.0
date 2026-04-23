using UsersRoles.Entities;

namespace UsersRoles.Data;


//Интерфейс репозитория с описанием методов класса, работающего с данными пользователей.
public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersWithRolesAsync();
    Task<IEnumerable<RoleWithCount>> GetRoleUserCountsAsync();
    Task<bool> TableExistsAsync(string tableName);
    Task ExecuteRawSqlAsync(string sql);
}


//Класс результата второго запроса.
public class RoleWithCount
{
    public string Name { get; set; }
    public int UserCount { get; set; }
}