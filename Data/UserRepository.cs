using Dapper;
using UsersRoles.Entities;

namespace UsersRoles.Data;


//Реализация интерфейса IUserRepository.
//Содержит конкретную логику запросов к БД.
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    //Метод, вызывающий всех пользователей и роли, в которых они состоят.
    public async Task<IEnumerable<User>> GetUsersWithRolesAsync()
    {
        const string query = @"
            SELECT u.Id, u.UserName, u.UserSurname, u.Email, r.Name as RoleName
            FROM Users u
            INNER JOIN UserRoles ur ON u.Id = ur.UserId
            INNER JOIN Roles r ON ur.RoleId = r.Id
            ORDER BY u.UserName, r.Name;";

        using var connection = _context.CreateConnection();
        var userDict = new Dictionary<int, User>();

        var result = await connection.QueryAsync<User, string, User>(
            query,
            (user, roleName) =>
            {
                if (!userDict.TryGetValue(user.Id, out var currentUser))
                {
                    currentUser = user;
                    currentUser.Roles = new List<string>();
                    userDict.Add(currentUser.Id, currentUser);
                }
                currentUser.Roles.Add(roleName);
                return currentUser;
            },
            splitOn: "RoleName"
        );

        return result.Distinct();
    }

    //Метод, вызывающий количество пользователей, которые входят в эту роль
    public async Task<IEnumerable<RoleWithCount>> GetRoleUserCountsAsync()
    {
        const string query = @"
            SELECT r.Name, COUNT(ur.UserId) as UserCount
            FROM Roles r
            LEFT JOIN UserRoles ur ON r.Id = ur.RoleId
            GROUP BY r.Id, r.Name
            ORDER BY r.Name;";

        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<RoleWithCount>(query);
    }

    //Проверка наличия таблиц в БД.
    public async Task<bool> TableExistsAsync(string tableName)
    {
        const string query = @"
            SELECT EXISTS (
                SELECT FROM information_schema.tables 
                WHERE table_name = @tableName
            );";

        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(query, new { tableName });
    }

    //Выполнение асинхронных запросов.
    public async Task ExecuteRawSqlAsync(string sql)
    {
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(sql);
    }
}