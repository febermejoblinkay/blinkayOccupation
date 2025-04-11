using System.Globalization;

namespace BlinkayOccupation.Application.Models
{
    public class OccupationByTariff
    {
        public string TariffId { get; set; }
        public string TariffName { get; set; }
        public int? PaidOccupation { get; set; }
        public string? PaidOccupationPcg { get; set; }
        public int? RealOccupation { get; set; }
        public string? RealOccupationPcg { get; set; }
        public int Capacity { get; set; }
    }
}
