using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersRoles.Entities;

//Класс сущности User.
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}