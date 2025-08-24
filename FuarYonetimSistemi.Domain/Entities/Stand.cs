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

        public double? AreaSold { get; set; }

        public double? AreaExchange { get; set; }

        public double? ContractArea { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? SaleAmountWithoutVAT { get; set; }

        public decimal? ElectricityConnectionFee { get; set; }

        public decimal? ThirdPartyInsuranceShare { get; set; }

        public decimal? StandSetupIncome { get; set; }

        public decimal? SolidWasteFee { get; set; }

        public decimal? AdvertisingIncome { get; set; }

        public decimal? ContractAmountWithoutVAT { get; set; }

        public decimal? VAT10Amount { get; set; }

        public decimal? VAT20Amount { get; set; }

        public decimal? StampTaxAmount { get; set; }

        public decimal? TotalAmountWithVAT { get; set; }

        public decimal? TotalReturnInvoice { get; set; }

        public decimal? BarterInvoiceAmount { get; set; }

        public decimal? CashCollection { get; set; }

        public decimal? DocumentCollection { get; set; }

        public decimal? Balance { get; set; }

        public decimal? ReceivablesInLaw { get; set; }

        public decimal? CollectibleBalance { get; set; }

        public decimal? BarterAmount { get; set; }

        public decimal? BarterBalance { get; set; }

        public DateTime? ActualDueDate { get; set; }

        public DateTime? ContractDate { get; set; }

        // Sales Representative - User ile ilişki
        public Guid? SalesRepresentativeId { get; set; }

        [JsonIgnore]
        public User? SalesRepresentative { get; set; }

        [MaxLength(1000)]
        public string Note { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        // Foreign Keys
        public Guid? ParticipantId { get; set; }

        [JsonIgnore]
        public Participant? Participant { get; set; }

        public Guid? FairId { get; set; }

        [JsonIgnore]
        public Fair? Fair { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
