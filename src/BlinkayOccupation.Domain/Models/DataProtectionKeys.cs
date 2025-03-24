using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class DataProtectionKeys
{
    public int Id { get; set; }

    public string? FriendlyName { get; set; }

    public string? Xml { get; set; }
}
