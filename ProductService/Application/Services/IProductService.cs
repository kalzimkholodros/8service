using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductService.Application.DTOs;

namespace ProductService.Application.Services;

public interface IProductService
{
    Task<ProductResponseDTO> GetProductByIdAsync(Guid id);
    Task<ProductListResponseDTO> GetProductsAsync(int pageNumber, int pageSize, string? searchTerm = null, string? category = null);
    Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO createProductDto);
    Task<ProductResponseDTO> UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto);
    Task DeleteProductAsync(Guid id);
    Task<List<string>> GetAllCategoriesAsync();
    Task UpdateProductStockAsync(Guid id, int quantity);
} 