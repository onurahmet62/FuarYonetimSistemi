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
    public class OfficeExpenseService : IOfficeExpenseService
    {
        private readonly AppDbContext _context;

        public OfficeExpenseService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<OfficeExpense>> GetAllExpensesAsync()
        {
            return await _context.OfficeExpenses
                .Where(x => !x.IsDeleted)
                .Include(x => x.ExpenseType)
                .ToListAsync();
        }

        public async Task<OfficeExpense> GetExpenseByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid expense ID.", nameof(id));

            var expense = await _context.OfficeExpenses
                .Include(x => x.ExpenseType)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (expense == null)
                throw new KeyNotFoundException($"Expense with ID {id} not found.");

            return expense;
        }

        public async Task<OfficeExpense> CreateAsync(OfficeExpenseCreateDto dto)
        {
            if (dto.OfficeExpenseTypeId == Guid.Empty || dto.Amount <= 0 || dto.Date == default)
                throw new ArgumentException("Geçersiz gider bilgileri.");

            var exists = await _context.OfficeExpenseTypes

                .AnyAsync(x => x.Id == dto.OfficeExpenseTypeId && !x.IsDeleted);

            if (!exists)
                throw new KeyNotFoundException("Gider tipi bulunamadı.");

            var expense = new OfficeExpense
            {
                Id = Guid.NewGuid(),
                OfficeExpenseTypeId = dto.OfficeExpenseTypeId,
                Date = dto.Date,
                Amount = dto.Amount,
                Description = dto.Description?.Trim(),
                IsDeleted = false
            };

            _context.OfficeExpenses.Add(expense);
            await _context.SaveChangesAsync();

            return expense;
        }


        public async Task<OfficeExpense> DeleteExpenseAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid expense ID.", nameof(id));

            var expense = await _context.OfficeExpenses.FindAsync(id);

            if (expense == null || expense.IsDeleted)
                throw new KeyNotFoundException("Expense not found or already deleted.");

            expense.IsDeleted = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error or handle as needed
                throw new InvalidOperationException("Error while deleting expense.", ex);
            }

            return expense;
        }

        public async Task<List<OfficeExpenseType>> GetExpenseTypesAsync()
        {
            return await _context.OfficeExpenseTypes.ToListAsync();
        }

        public async Task<OfficeExpenseType> CreateExpenseTypeAsync(OfficeExpenseType expenseType)
        {
            if (expenseType == null)
                throw new ArgumentNullException(nameof(expenseType));

            expenseType.Id = Guid.NewGuid();
            _context.OfficeExpenseTypes.Add(expenseType);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error or handle as needed
                throw new InvalidOperationException("Error while creating expense type.", ex);
            }

            return expenseType;
        }

        public async Task<OfficeExpenseType> DeleteExpenseTypeAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid expense type ID.", nameof(id));

            var expenseType = await _context.OfficeExpenseTypes.FindAsync(id);

            if (expenseType == null || expenseType.IsDeleted)
                throw new KeyNotFoundException("Expense type not found or already deleted.");

            expenseType.IsDeleted = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log error or handle as needed
                throw new InvalidOperationException("Error while deleting expense type.", ex);
            }

            return expenseType;
        }
    }
}
