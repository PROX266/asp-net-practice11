using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserLogin { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public byte[] UserPassword { get; set; } = null!;

    // Хеш пароля от PasswordHasher<User> (строка). Старый столбец UserPassword остаётся для MVC-сайта.
    public string? PasswordHash { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductReservation> Reservations { get; set; } = new List<ProductReservation>();

    public virtual Role Role { get; set; } = null!;
}
