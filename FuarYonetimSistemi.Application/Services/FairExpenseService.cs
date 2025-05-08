using FuarYonetimSistemi.Application.Interfaces;
using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using FuarYonetimSistemi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Services
{
    public class FairExpenseService : IFairExpenseService
    {
        private readonly AppDbContext _context;

        public FairExpenseService(AppDbContext context)
        {
            _context = context;
        }

        // Fuar giderlerini listeleme
        public async Task<List<FairExpense>> GetFairExpensesAsync(Guid fairId)
        {
            return await _context.FairExpenses
                .Where(fe => fe.FairId == fairId && !fe.IsDeleted)
                .Include(fe => fe.ExpenseType)
                .ToListAsync();
        }

        // Fuar gideri oluşturma
        public async Task<FairExpense> CreateFairExpenseAsync(FairExpenseCreateDto dto)
        {
            var fair = await _context.Fairs.FindAsync(dto.FairId);
            if (fair == null)
                throw new KeyNotFoundException("Fuar bulunamadı.");

            var expenseType = await _context.FairExpenseTypes.FindAsync(dto.FairExpenseTypeId);
            if (expenseType == null)
                throw new KeyNotFoundException("Gider türü bulunamadı.");

            var expense = new FairExpense
            {
                Id = Guid.NewGuid(),
                FairId = dto.FairId,
                FairExpenseTypeId = dto.FairExpenseTypeId,
                AccountCode = dto.AccountCode,
                AnnualTarget = dto.AnnualTarget,
                AnnualActual = dto.AnnualActual,
                CurrentTarget = dto.CurrentTarget,
                CurrentActual = dto.CurrentActual,
                RealizedExpense = dto.RealizedExpense
            };

            _context.FairExpenses.Add(expense);
            await _context.SaveChangesAsync();

            return expense;
        }

        // Fuar gideri silme (soft delete)
        public async Task<FairExpense> DeleteFairExpenseAsync(Guid id)
        {
            var expense = await _context.FairExpenses.FindAsync(id);
            if (expense == null)
                throw new KeyNotFoundException("Fuar gideri bulunamadı.");

            expense.IsDeleted = true;
            await _context.SaveChangesAsync();

            return expense;
        }

        // Fuar gider türlerini listeleme
        public async Task<List<FairExpenseType>> GetExpenseTypesAsync()
        {
            return await _context.FairExpenseTypes
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        // Fuar gider türü oluşturma
        public async Task<FairExpenseType> CreateExpenseTypeAsync(FairExpenseTypeCreateDto dto)
        {
            var type = new FairExpenseType
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                IsDeleted = false
            };

            _context.FairExpenseTypes.Add(type);
            await _context.SaveChangesAsync();

            return type;
        }

        // Fuar gider türü silme (soft delete)
        public async Task<FairExpenseType> DeleteExpenseTypeAsync(Guid id)
        {
            var type = await _context.FairExpenseTypes.FindAsync(id);
            if (type == null || type.IsDeleted)
                throw new KeyNotFoundException("Gider türü bulunamadı.");

            type.IsDeleted = true;
            await _context.SaveChangesAsync();
            return type;
        }
    }
}
