using System;
using System.Collections.Generic;

namespace BlinkayOccupation.Domain.Models;

public partial class Attachments
{
    public string Id { get; set; } = null!;

    public string InstallationId { get; set; } = null!;

    public string DeviceId { get; set; } = null!;

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public string StorageId { get; set; } = null!;

    public int Direction { get; set; }

    public virtual InputDevices Device { get; set; } = null!;

    public virtual Installations Installation { get; set; } = null!;
}
