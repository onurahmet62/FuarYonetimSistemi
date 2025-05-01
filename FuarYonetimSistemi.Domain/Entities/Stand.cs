using FuarYonetimSistemi.Domain.Enums;
using System.Text.Json.Serialization;

namespace FuarYonetimSistemi.Domain.Entities;

public class Stand
{
    public Guid Id { get; set; }
    public string Name { get; set; } // Stand Adı
    public string FairHall { get; set; }
    public double AreaSold { get; set; } // Gerçekleşen Satış m2
    public double AreaExchange { get; set; } // Gerçekleşen Satış Takas m2
    public double ContractArea { get; set; } // Gerçekleşen Sözleşme m2
    public decimal UnitPrice { get; set; } // Birim Fiyatı

    public decimal SaleAmountWithoutVAT { get; set; } // Gerçekleşen Satış (KDV'siz)
    public decimal ElectricityConnectionFee { get; set; } // Elektrik Bağlantı Bedeli
    public decimal ThirdPartyInsuranceShare { get; set; } // 3. Şahıs Sigorta Payı
    public decimal StandSetupIncome { get; set; } // Stand Kurma Geliri (KDV'siz)
    public decimal SolidWasteFee { get; set; } // Katı Atık Bedeli (KDV'siz)
    public decimal AdvertisingIncome { get; set; } // Reklam, Sponsor, Katolog Geliri (KDV'siz)

    public decimal ContractAmountWithoutVAT { get; set; } // Sözleşme Tutarı (KDV'siz)
    public decimal VAT10Amount { get; set; } // KDV %10
    public decimal VAT20Amount { get; set; } // KDV %20
    public decimal StampTaxAmount { get; set; } // Damga Vergisi (KDV'siz)

    public decimal TotalAmountWithVAT { get; set; } // Toplam KDV'li Tutar
    public decimal TotalReturnInvoice { get; set; } // Toplam İade Faturası
    public decimal BarterInvoiceAmount { get; set; } // Takas Fatura Tutarı

    public decimal CashCollection { get; set; } // Nakit Tahsilat / Havale
    public decimal DocumentCollection { get; set; } // Evrak Tahsilatı
    public decimal Balance { get; set; } // Bakiye
    public decimal ReceivablesInLaw { get; set; } // Hukuktaki Alacaklar
    public decimal CollectibleBalance { get; set; } // Tahsil Edilebilir Bakiye
    public decimal BarterAmount { get; set; } // Takas Takas Tutarı
    public decimal BarterBalance { get; set; } // Takas Takas Bakiye

    public DateTime? ActualDueDate { get; set; } // Gerçekleşen Vade
    public DateTime ContractDate { get; set; } // Sözleşme Tarihi

    public string SalesRepresentative { get; set; } // Satış Temsilcisi
    public string Note { get; set; } // Not
    public bool IsDeleted { get; set; }

    // Foreign key ve Navigation
    public Guid ParticipantId { get; set; }
    [JsonIgnore]
    public Participant Participant { get; set; }

    public Guid FairId { get; set; }
    [JsonIgnore]
    public Fair Fair { get; set; }

    // Bu standın ödeme kayıtları
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
