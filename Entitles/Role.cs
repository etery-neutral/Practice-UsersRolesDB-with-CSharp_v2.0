using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersRoles.Entities;

namespace UsersRoles.Entitles;


//Класс сущности Role.
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = new();
}
