using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductService.Domain.Entities;

namespace ProductService.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(Guid id);
    Task<(List<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, string? category = null);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<List<string>> GetAllCategoriesAsync();
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
    Task UpdateStockAsync(Guid id, int quantity);
} 