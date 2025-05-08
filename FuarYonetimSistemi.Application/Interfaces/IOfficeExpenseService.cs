using FuarYonetimSistemi.Application.DTOs;
using FuarYonetimSistemi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Application.Interfaces
{
    public interface IOfficeExpenseService
    {
        Task<List<OfficeExpense>> GetAllExpensesAsync();
        Task<OfficeExpense> GetExpenseByIdAsync(Guid id);
        Task<OfficeExpense> CreateAsync(OfficeExpenseCreateDto dto);
        Task<OfficeExpense> DeleteExpenseAsync(Guid id);
        Task<List<OfficeExpenseType>> GetExpenseTypesAsync();
        Task<OfficeExpenseType> CreateExpenseTypeAsync(OfficeExpenseType expenseType);
        Task<OfficeExpenseType> DeleteExpenseTypeAsync(Guid id);
    }

}
