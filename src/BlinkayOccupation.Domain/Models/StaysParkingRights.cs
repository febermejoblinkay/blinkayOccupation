using BlinkayOccupation.Domain.Helpers;
using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class StaysParkingRights : IAuditable
{
    public string Id { get; set; } = null!;

    public string StayId { get; set; } = null!;

    public string? ParkingRightId { get; set; }

    public DateTime Created { get; set; }

    public bool Deleted { get; set; }

    public DateTime Updated { get; set; }

    public virtual ParkingRights? ParkingRight { get; set; }

    public virtual Stays Stay { get; set; } = null!;
}
