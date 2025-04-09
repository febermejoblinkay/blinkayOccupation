namespace BlinkayOccupation.Application.Models
{
    public class CurrentParkingDataDto
    {
        public DateTime Date { get; set; }
        public ZoneDto Zone { get; set; }
        public TariffDto Tariff { get; set; }
        public InstallationDto Installation { get; set; }

        public int Sold { get; set; }
        public int OpenForSale { get; set; }
        public int Total { get; set; }
        public int Paid { get; set; }
        public int Unpaid { get; set; }
        public int Available { get; set; }
        public int Occupied { get; set; }
    }
}
