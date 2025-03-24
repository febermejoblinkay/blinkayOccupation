using BlinkayOccupation.Domain.Helpers;
using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Users
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    /// <summary>
    /// Asigna y hashea la contraseña antes de almacenarla.
    /// </summary>
    public void SetPassword(string password)
    {
        Password = PasswordHasher.HashPassword(password);
    }

    /// <summary>
    /// Verifica si la contraseña ingresada es válida.
    /// </summary>
    public bool IsValidPassword(string password)
    {
        return PasswordHasher.VerifyPassword(password, Password);
    }
}
