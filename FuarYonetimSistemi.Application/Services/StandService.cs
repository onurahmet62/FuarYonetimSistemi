using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    public class StandService : IStandService
    {
        private readonly AppDbContext _context;

        public StandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Stand> AddAsync(StandCreateDto standCreateDto)
        {
            var participant = await _context.Participants.FindAsync(standCreateDto.ParticipantId);
            var fair = await _context.Fairs.FindAsync(standCreateDto.FairId);

            if (participant == null || fair == null)
                return null;

            var stand = new Stand
            {
                Name = standCreateDto.Name,
                FairHall = standCreateDto.FairHall,
                AreaSold = standCreateDto.AreaSold,
                AreaExchange = standCreateDto.AreaExchange,
                ContractArea = standCreateDto.ContractArea,
                UnitPrice = standCreateDto.UnitPrice,
                SaleAmountWithoutVAT = standCreateDto.SaleAmountWithoutVAT,
                ElectricityConnectionFee = standCreateDto.ElectricityConnectionFee,
                ThirdPartyInsuranceShare = standCreateDto.ThirdPartyInsuranceShare,
                StandSetupIncome = standCreateDto.StandSetupIncome,
                SolidWasteFee = standCreateDto.SolidWasteFee,
                AdvertisingIncome = standCreateDto.AdvertisingIncome,
                ContractAmountWithoutVAT = standCreateDto.ContractAmountWithoutVAT,
                VAT10Amount = standCreateDto.VAT10Amount,
                VAT20Amount = standCreateDto.VAT20Amount,
                StampTaxAmount = standCreateDto.StampTaxAmount,
                TotalAmountWithVAT = standCreateDto.TotalAmountWithVAT,
                TotalReturnInvoice = standCreateDto.TotalReturnInvoice,
                BarterInvoiceAmount = standCreateDto.BarterInvoiceAmount,
                CashCollection = standCreateDto.CashCollection,
                DocumentCollection = standCreateDto.DocumentCollection,
                Balance = standCreateDto.Balance,
                ReceivablesInLaw = standCreateDto.ReceivablesInLaw,
                CollectibleBalance = standCreateDto.CollectibleBalance,
                BarterAmount = standCreateDto.BarterAmount,
                BarterBalance = standCreateDto.BarterBalance,
                ActualDueDate = standCreateDto.ActualDueDate,
                ContractDate = standCreateDto.ContractDate,
                SalesRepresentative = standCreateDto.SalesRepresentative ?? string.Empty,
                Note = standCreateDto.Note ?? string.Empty, // Fix for CS8601: Assign a default value if null
                ParticipantId = standCreateDto.ParticipantId,
                FairId = standCreateDto.FairId
            };

            _context.Stands.Add(stand);
            await _context.SaveChangesAsync();
            return stand;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var stand = await _context.Stands.FindAsync(id);
            if (stand == null)
                return false;

            stand.IsDeleted = true;
            _context.Stands.Update(stand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Stand>> FilterAsync(string searchTerm)
        {
            return await _context.Stands
                .Include(s => s.Participant)
                .Include(s => s.Fair)
                .Where(s => s.Name.Contains(searchTerm) || s.FairHall.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Stand>> GetAllAsync()
        {
            return await _context.Stands
                .Include(s => s.Participant)
                .Include(s => s.Fair)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stand>> GetByFairIdAsync(Guid fairId)
        {
            return await _context.Stands
                .Include(s => s.Participant)
                .Include(s => s.Fair)
                .Where(s => s.FairId == fairId)
                .ToListAsync();
        }

        public async Task<Stand> GetByIdAsync(Guid id)
        {
            return await _context.Stands
                .Include(s => s.Participant)
                .Include(s => s.Fair)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Stand>> GetByParticipantIdAsync(Guid participantId)
        {
            return await _context.Stands
                .Include(s => s.Participant)
                .Include(s => s.Fair)
                .Where(s => s.ParticipantId == participantId)
                .ToListAsync();
        }

        public async Task<Stand> UpdateAsync(Guid id, Stand updatedStand)
        {
            var stand = await _context.Stands.FindAsync(id);
            if (stand == null)
                return null;

            stand.Name = updatedStand.Name;
            stand.FairHall = updatedStand.FairHall;
            stand.AreaSold = updatedStand.AreaSold;
            stand.AreaExchange = updatedStand.AreaExchange;
            stand.ContractArea = updatedStand.ContractArea;
            stand.UnitPrice = updatedStand.UnitPrice;
            stand.SaleAmountWithoutVAT = updatedStand.SaleAmountWithoutVAT;
            stand.ElectricityConnectionFee = updatedStand.ElectricityConnectionFee;
            stand.ThirdPartyInsuranceShare = updatedStand.ThirdPartyInsuranceShare;
            stand.StandSetupIncome = updatedStand.StandSetupIncome;
            stand.SolidWasteFee = updatedStand.SolidWasteFee;
            stand.AdvertisingIncome = updatedStand.AdvertisingIncome;
            stand.ContractAmountWithoutVAT = updatedStand.ContractAmountWithoutVAT;
            stand.VAT10Amount = updatedStand.VAT10Amount;
            stand.VAT20Amount = updatedStand.VAT20Amount;
            stand.StampTaxAmount = updatedStand.StampTaxAmount;
            stand.TotalAmountWithVAT = updatedStand.TotalAmountWithVAT;
            stand.TotalReturnInvoice = updatedStand.TotalReturnInvoice;
            stand.BarterInvoiceAmount = updatedStand.BarterInvoiceAmount;
            stand.CashCollection = updatedStand.CashCollection;
            stand.DocumentCollection = updatedStand.DocumentCollection;
            stand.Balance = updatedStand.Balance;
            stand.ReceivablesInLaw = updatedStand.ReceivablesInLaw;
            stand.CollectibleBalance = updatedStand.CollectibleBalance;
            stand.BarterAmount = updatedStand.BarterAmount;
            stand.BarterBalance = updatedStand.BarterBalance;
            stand.ActualDueDate = updatedStand.ActualDueDate;
            stand.ContractDate = updatedStand.ContractDate;
            stand.SalesRepresentative = updatedStand.SalesRepresentative;
            stand.Note = updatedStand.Note;

            await _context.SaveChangesAsync();
            return stand;
        }

        public async Task<IEnumerable<Stand>> GetSortedAsync(StandFilterRequestDto filter)
        {
            IQueryable<Stand> query = _context.Stands
                .Include(s => s.Participant)
                .Include(s => s.Fair);

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(s => s.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.FairHall))
                query = query.Where(s => s.FairHall.Contains(filter.FairHall));

            if (filter.ContractDate.HasValue)
                if (filter.ContractDate.HasValue)
                    query = query.Where(s => s.ContractDate.HasValue && s.ContractDate.Value.Date == filter.ContractDate.Value.Date);

            if (filter.ActualDueDate.HasValue)
                query = query.Where(s => s.ActualDueDate.HasValue && s.ActualDueDate.Value.Date == filter.ActualDueDate.Value.Date);
              

            if (filter.ActualDueDate.HasValue)
                query = query.Where(s => s.ActualDueDate.HasValue && s.ActualDueDate.Value.Date == filter.ActualDueDate.Value.Date);

            if (filter.ParticipantId.HasValue)
                query = query.Where(s => s.ParticipantId == filter.ParticipantId.Value);

            if (filter.FairId.HasValue)
                query = query.Where(s => s.FairId == filter.FairId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SalesRepresentative))
                query = query.Where(s => s.SalesRepresentative.Contains(filter.SalesRepresentative));

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "name" => filter.IsDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                    "fairhall" => filter.IsDescending ? query.OrderByDescending(s => s.FairHall) : query.OrderBy(s => s.FairHall),
                    "contractdate" => filter.IsDescending ? query.OrderByDescending(s => s.ContractDate) : query.OrderBy(s => s.ContractDate),
                    "actualduedate" => filter.IsDescending ? query.OrderByDescending(s => s.ActualDueDate) : query.OrderBy(s => s.ActualDueDate),
                    "participantid" => filter.IsDescending ? query.OrderByDescending(s => s.ParticipantId) : query.OrderBy(s => s.ParticipantId),
                    "fairid" => filter.IsDescending ? query.OrderByDescending(s => s.FairId) : query.OrderBy(s => s.FairId),
                    "salesrepresentative" => filter.IsDescending ? query.OrderByDescending(s => s.SalesRepresentative) : query.OrderBy(s => s.SalesRepresentative),
                    _ => query
                };
            }

            return await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stand>> GetStandsDueInDaysAsync(int days)
        {
            var targetDate = DateTime.Today.AddDays(days);

            return await _context.Stands
                .Include(s => s.Participant)
                .Include(s => s.Fair)
                .Where(s => s.ActualDueDate.HasValue && s.ActualDueDate.Value.Date <= targetDate)
                .ToListAsync();
        }
    }
}
