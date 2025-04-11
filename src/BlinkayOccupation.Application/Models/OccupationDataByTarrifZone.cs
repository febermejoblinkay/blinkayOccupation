namespace BlinkayOccupation.Application.Models
{
    public class OccupationDataByTarrifZone
    {
        public ReferenceItem Zone { get; set; }
        public List<OccupationByTariff> Tariff { get; set; } = new List<OccupationByTariff>();
        public int TotalPaidOccupation { get; set; }
        public string TotalPaidOccupationPcg { get; set; }
        public int TotalRealOccupation { get; set; }
        public string TotalRealOccupationPcg { get; set; }
        public int TotalUnpaidOccupation { get; set; }
        public string TotalUnpaidOccupationPcg { get; set; }
        public int EntriesWithoutExitCount { get; set; } //UnpaidRO + PaidRO
        public int PaymentsWithEntry { get; set; }
        public int Capacity { get; set; }
    }
}
