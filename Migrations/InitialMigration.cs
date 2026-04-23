using FluentMigrator;

namespace UsersRoles.Migrations;

[Migration(20260422_001)] //Номер миграции выбран как дата модификации проекта.

//Класс миграции. Описывает, как создать начальную схему БД и наполнить её тестовыми данными.
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table("Roles")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable().Unique();

        Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("UserName").AsString(100).NotNullable()
            .WithColumn("UserSurname").AsString(100).NotNullable()
            .WithColumn("Email").AsString(255).NotNullable();

        Create.Table("UserRoles")
            .WithColumn("UserId").AsInt32().ForeignKey("Users", "Id").OnDelete(System.Data.Rule.Cascade)
            .WithColumn("RoleId").AsInt32().ForeignKey("Roles", "Id").OnDelete(System.Data.Rule.Cascade);

        Create.PrimaryKey("PK_UserRoles").OnTable("UserRoles").Columns("UserId", "RoleId");

        // Вставка тестовых данных.
        Insert.IntoTable("Roles").Row(new { Name = "Администратор" });
        Insert.IntoTable("Roles").Row(new { Name = "Модератор" });
        Insert.IntoTable("Roles").Row(new { Name = "Пользователь" });
        Insert.IntoTable("Roles").Row(new { Name = "Менеджер" });
        Insert.IntoTable("Roles").Row(new { Name = "Тестировщик" });

        Insert.IntoTable("Users").Row(new { UserName = "Антон", UserSurname = "Иванов", Email = "ant_ivanov@example.com" });
        Insert.IntoTable("Users").Row(new { UserName = "Мария", UserSurname = "Маркова", Email = "marymark@example.com" });
        Insert.IntoTable("Users").Row(new { UserName = "Сергей", UserSurname = "Петров", Email = "sergrey@example.com" });
        Insert.IntoTable("Users").Row(new { UserName = "Андрей", UserSurname = "Козлов", Email = "kozlov@example.com" });
        Insert.IntoTable("Users").Row(new { UserName = "Ольга", UserSurname = "Новикова", Email = "olganov@example.com" });
        Insert.IntoTable("Users").Row(new { UserName = "Алексей", UserSurname = "Морозов", Email = "alex.moroz@example.com" });
        Insert.IntoTable("Users").Row(new { UserName = "Илья", UserSurname = "Бледный", Email = "palemoon@example.com" });

        // Связи UserRoles.
        Execute.Sql(@"
            INSERT INTO UserRoles (UserId, RoleId) VALUES 
            (1, 1),
            (2, 2),
            (3, 3), 
            (2, 3),
            (4, 4),
            (4, 3),
            (5, 4),
            (6, 3),
            (6, 5),
            (7, 2),
            (7, 3),
            (7, 5);
        ");
    }

    public override void Down()
    {
        Delete.Table("UserRoles");
        Delete.Table("Users");
        Delete.Table("Roles");
    }
}