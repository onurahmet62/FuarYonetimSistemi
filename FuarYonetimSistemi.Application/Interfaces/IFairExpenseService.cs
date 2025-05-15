using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IFairExpenseService
    {
        Task<List<FairExpense>> GetFairExpensesAsync(Guid fairId);
        Task<FairExpense> CreateFairExpenseAsync(FairExpenseCreateDto dto);
        Task<FairExpense> DeleteFairExpenseAsync(Guid id);

        Task<List<FairExpenseType>> GetExpenseTypesAsync();
        Task<FairExpenseType> CreateExpenseTypeAsync(FairExpenseTypeCreateDto dto);
        Task<FairExpenseType> DeleteExpenseTypeAsync(Guid id);
        Task<(List<FairExpense> Expenses, int TotalCount)> GetFairExpensesFilteredAsync(FairExpenseFilterDto filterDto);

        Task<(List<FairExpenseType> ExpenseTypes, int TotalCount)> GetExpenseTypesFilteredAsync(FairExpenseTypeFilterDto filterDto);

    }
}
    