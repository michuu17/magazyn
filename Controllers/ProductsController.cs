using Microsoft.AspNetCore.Mvc;
using WarehouseApi.DTOs;
using WarehouseApi.Services;

namespace WarehouseApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Zwraca produkty z magazynu z podzialem na strony.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductResponseDto>>> GetAll([FromQuery] PaginationQuery query)
    {
        var result = await _productService.GetPagedAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Zwraca pojedynczy produkt po jego identyfikatorze.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDto>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Dodaje nowy produkt do magazynu.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] ProductCreateDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Aktualizuje dane istniejacego produktu.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDto>> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Usuwa produkt z magazynu (usuniecie miekkie).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Przyjmuje okreslona liczbe sztuk produktu na stan.
    /// </summary>
    [HttpPost("{id:int}/receive")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDto>> Receive(int id, [FromBody] StockOperationDto dto)
    {
        var result = await _productService.ReceiveStockAsync(id, dto.Quantity);
        return Ok(result);
    }

    /// <summary>
    /// Wydaje okreslona liczbe sztuk produktu ze stanu.
    /// </summary>
    [HttpPost("{id:int}/issue")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponseDto>> Issue(int id, [FromBody] StockOperationDto dto)
    {
        var result = await _productService.IssueStockAsync(id, dto.Quantity);
        return Ok(result);
    }
}
