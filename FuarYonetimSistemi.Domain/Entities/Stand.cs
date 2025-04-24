using FuarYonetimSistemi.Domain.Enums;

namespace FuarYonetimSistemi.Domain.Entities;

public class Stand
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string FairHall { get; set; }
    public decimal Price { get; set; }
    public double Area { get; set; }
    public string Description { get; set; }
    public string PaymentStatus { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountRemaining { get; set; }
    public DateTime? DueDate { get; set; }

    // Foreign key ve Navigation
    public Guid ParticipantId { get; set; }
    public Participant Participant { get; set; }

    public Guid FairId { get; set; }
    public Fair Fair { get; set; }

    // Yeni: Bu standın ödemeleri
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
