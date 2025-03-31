namespace BlinkayOccupation.Application.Exceptions
{
    public class ParkingEventsNotFoundException : Exception
    {
        public ParkingEventsNotFoundException() : base("No parking events found.")
        {
        }

        public ParkingEventsNotFoundException(string message) : base(message)
        {
        }

        public ParkingEventsNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ParkingRightsNotFoundException : Exception
    {
        public ParkingRightsNotFoundException() : base("No parking rights found.")
        {
        }

        public ParkingRightsNotFoundException(string message) : base(message)
        {
        }

        public ParkingRightsNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class InstallationNotFoundException : Exception
    {
        public InstallationNotFoundException() : base("Installation not found.")
        {
        }

        public InstallationNotFoundException(string message) : base(message)
        {
        }

        public InstallationNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ZoneNotFoundException : Exception
    {
        public ZoneNotFoundException() : base("Zone not found.")
        {
        }

        public ZoneNotFoundException(string message) : base(message)
        {
        }

        public ZoneNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class StayNotFoundException : Exception
    {
        public StayNotFoundException() : base("Stay not found.")
        {
        }

        public StayNotFoundException(string message) : base(message)
        {
        }

        public StayNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ParkingRightsNoValidEndDateException : Exception
    {
        public ParkingRightsNoValidEndDateException() : base("The Parking right has no validTo date defined.")
        {
        }

        public ParkingRightsNoValidEndDateException(string message) : base(message)
        {
        }

        public ParkingRightsNoValidEndDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class LastReceivedEventDateException : Exception
    {
        public LastReceivedEventDateException() : base("Last received event date can't be before internal event date.")
        {
        }

        public LastReceivedEventDateException(string message) : base(message)
        {
        }

        public LastReceivedEventDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
