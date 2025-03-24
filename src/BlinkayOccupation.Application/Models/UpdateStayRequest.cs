using System.ComponentModel.DataAnnotations;

namespace BlinkayOccupation.Application.Models
{
    public class UpdateStayRequest
    {
        [Required]
        public string StayId { get; set; }
        public string? EntryEventId { get; set; }
        public string? ExitEventId { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }
        [Range(1, 8)]
        public int? CaseId { get; set; }
        public List<string>? ParkingRightIds { get; set; }
    }
}
