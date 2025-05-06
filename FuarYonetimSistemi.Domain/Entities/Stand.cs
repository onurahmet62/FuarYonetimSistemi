using FuarYonetimSistemi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FuarYonetimSistemi.Domain.Entities
{
    public class Stand
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FairHall { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public double AreaSold { get; set; }

        [Range(0, double.MaxValue)]
        public double AreaExchange { get; set; }

        [Range(0, double.MaxValue)]
        public double ContractArea { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SaleAmountWithoutVAT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ElectricityConnectionFee { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ThirdPartyInsuranceShare { get; set; }

        [Range(0, double.MaxValue)]
        public decimal StandSetupIncome { get; set; }

        [Range(0, double.MaxValue)]
        public decimal SolidWasteFee { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AdvertisingIncome { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ContractAmountWithoutVAT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal VAT10Amount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal VAT20Amount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal StampTaxAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmountWithVAT { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalReturnInvoice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BarterInvoiceAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CashCollection { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DocumentCollection { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Balance { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ReceivablesInLaw { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CollectibleBalance { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BarterAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BarterBalance { get; set; }

        public DateTime? ActualDueDate { get; set; }

        public DateTime ContractDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string SalesRepresentative { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Note { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        // Foreign Keys
        [Required]
        public Guid ParticipantId { get; set; }

        [JsonIgnore]
        public Participant Participant { get; set; }

        [Required]
        public Guid FairId { get; set; }

        [JsonIgnore]
        public Fair Fair { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
