using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Services;

namespace ProductService.API.Controllers;

[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ProductListResponseDTO>> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? category = null)
    {
        if (pageNumber < 1) return BadRequest("Page number must be greater than 0");
        if (pageSize < 1) return BadRequest("Page size must be greater than 0");
        if (pageSize > 50) return BadRequest("Page size cannot exceed 50");

        var result = await _productService.GetProductsAsync(pageNumber, pageSize, searchTerm, category);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseDTO>> GetProduct(Guid id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("categories")]
    public async Task<ActionResult<string[]>> GetCategories()
    {
        var categories = await _productService.GetAllCategoriesAsync();
        return Ok(categories);
    }
} 