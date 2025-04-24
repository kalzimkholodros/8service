using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;
using ProductService.Domain.Repositories;

namespace ProductService.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDTO> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return MapToProductResponseDTO(product);
    }

    public async Task<ProductListResponseDTO> GetProductsAsync(int pageNumber, int pageSize, string? searchTerm = null, string? category = null)
    {
        var (products, totalCount) = await _productRepository.GetAllAsync(pageNumber, pageSize, searchTerm, category);

        return new ProductListResponseDTO
        {
            Products = products.Select(MapToProductResponseDTO).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO createProductDto)
    {
        if (!await _productRepository.IsNameUniqueAsync(createProductDto.Name))
        {
            throw new InvalidOperationException($"A product with the name '{createProductDto.Name}' already exists.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            Stock = createProductDto.Stock,
            Category = createProductDto.Category,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.CreateAsync(product);
        return MapToProductResponseDTO(product);
    }

    public async Task<ProductResponseDTO> UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);

        if (updateProductDto.Name != null && 
            updateProductDto.Name != existingProduct.Name && 
            !await _productRepository.IsNameUniqueAsync(updateProductDto.Name, id))
        {
            throw new InvalidOperationException($"A product with the name '{updateProductDto.Name}' already exists.");
        }

        if (updateProductDto.Name != null) existingProduct.Name = updateProductDto.Name;
        if (updateProductDto.Description != null) existingProduct.Description = updateProductDto.Description;
        if (updateProductDto.Price.HasValue) existingProduct.Price = updateProductDto.Price.Value;
        if (updateProductDto.Stock.HasValue) existingProduct.Stock = updateProductDto.Stock.Value;
        if (updateProductDto.Category != null) existingProduct.Category = updateProductDto.Category;
        if (updateProductDto.IsActive.HasValue) existingProduct.IsActive = updateProductDto.IsActive.Value;

        await _productRepository.UpdateAsync(existingProduct);
        return MapToProductResponseDTO(existingProduct);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }

    public async Task<List<string>> GetAllCategoriesAsync()
    {
        return await _productRepository.GetAllCategoriesAsync();
    }

    public async Task UpdateProductStockAsync(Guid id, int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(quantity));
        }

        await _productRepository.UpdateStockAsync(id, quantity);
    }

    private static ProductResponseDTO MapToProductResponseDTO(Product product)
    {
        return new ProductResponseDTO
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
} 