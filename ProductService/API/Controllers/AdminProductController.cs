using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Services;

namespace ProductService.API.Controllers;

[ApiController]
[Route("admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductController : ControllerBase
{
    private readonly IProductService _productService;

    public AdminProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDTO>> CreateProduct([FromBody] CreateProductDTO createProductDto)
    {
        try
        {
            if (createProductDto.Price < 0)
                return BadRequest("Price cannot be negative");

            if (createProductDto.Stock < 0)
                return BadRequest("Stock cannot be negative");

            var product = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
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

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(Guid id, [FromBody] UpdateProductDTO updateProductDto)
    {
        try
        {
            if (updateProductDto.Price.HasValue && updateProductDto.Price.Value < 0)
                return BadRequest("Price cannot be negative");

            if (updateProductDto.Stock.HasValue && updateProductDto.Stock.Value < 0)
                return BadRequest("Stock cannot be negative");

            var product = await _productService.UpdateProductAsync(id, updateProductDto);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}/stock")]
    public async Task<ActionResult> UpdateStock(Guid id, [FromBody] int quantity)
    {
        try
        {
            await _productService.UpdateProductStockAsync(id, quantity);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
} 