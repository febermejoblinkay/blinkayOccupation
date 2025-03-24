using BlinkayOccupation.Domain.Models;

namespace BlinkayOccupation.Application.Models
{
    public record StayState(bool HasEntry, bool HasPayment, bool HasExit)
    {
        public override string ToString()
        {
            var entry = HasEntry ? "E" : "NE";
            var payment = HasPayment ? "P" : "NP";
            var exit = HasExit ? "S" : "NS";
            return $"{entry},{payment},{exit}";
        }

        public static StayState FromStay(
            Stays stay,
            bool hasPayment
        )
        {
            bool hasEntry = !string.IsNullOrWhiteSpace(stay.EntryEventId);
            bool hasExit = !string.IsNullOrWhiteSpace(stay.ExitEventId);
            return new StayState(hasEntry, hasPayment, hasExit);
        }

        public static StayState ToNewStay(
            Stays stay,
            bool hasPayment
        )
        {
            bool hasEntry = !string.IsNullOrWhiteSpace(stay.EntryEventId);
            bool hasExit = !string.IsNullOrWhiteSpace(stay.ExitEventId);
            return new StayState(hasEntry, hasPayment, hasExit);
        }

        public bool HasEntryEvent() => HasEntry;
        public bool HasPaymentRegistered() => HasPayment;
        public bool HasExitEvent() => HasExit;
    }
}
