using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
    }

    public async Task<(List<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm = null, string? category = null)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm) || 
                                   p.Description.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var existingProduct = await GetByIdAsync(product.Id);
        
        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        existingProduct.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await GetByIdAsync(id);
        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<List<string>> GetAllCategoriesAsync()
    {
        return await _context.Products
            .Where(p => !string.IsNullOrEmpty(p.Category))
            .Select(p => p.Category)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Products.Where(p => p.Name == name);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task UpdateStockAsync(Guid id, int quantity)
    {
        var product = await GetByIdAsync(id);
        product.Stock = quantity;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
} 