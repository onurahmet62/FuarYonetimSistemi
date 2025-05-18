using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.DTOs
{
    public class StandCreateDto
    {
        public string Name { get; set; }
        public string FairHall { get; set; }

        public double AreaSold { get; set; }
        public double AreaExchange { get; set; }
        public double ContractArea { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal SaleAmountWithoutVAT { get; set; }
        public decimal ElectricityConnectionFee { get; set; }
        public decimal ThirdPartyInsuranceShare { get; set; }
        public decimal StandSetupIncome { get; set; }
        public decimal SolidWasteFee { get; set; }
        public decimal AdvertisingIncome { get; set; }

        public decimal ContractAmountWithoutVAT { get; set; }
        public decimal VAT10Amount { get; set; }
        public decimal VAT20Amount { get; set; }
        public decimal StampTaxAmount { get; set; }

        public decimal TotalAmountWithVAT { get; set; }
        public decimal TotalReturnInvoice { get; set; }
        public decimal BarterInvoiceAmount { get; set; }

        public decimal CashCollection { get; set; }
        public decimal DocumentCollection { get; set; }
        public decimal Balance { get; set; }
        public decimal ReceivablesInLaw { get; set; }
        public decimal CollectibleBalance { get; set; }
        public decimal BarterAmount { get; set; }
        public decimal BarterBalance { get; set; }

        public DateTime? ActualDueDate { get; set; }
        public DateTime ContractDate { get; set; }

        public string? SalesRepresentative { get; set; }
        public string? Note { get; set; }

        public Guid ParticipantId { get; set; }
        public Guid FairId { get; set; }
    }

}
