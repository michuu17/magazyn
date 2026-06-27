using Microsoft.AspNetCore.Mvc;
using WarehouseApi.DTOs;
using WarehouseApi.Services;

namespace WarehouseApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    /// <summary>
    /// Zwraca liste aktywnych dostawcow.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SupplierResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SupplierResponseDto>>> GetAll()
    {
        var result = await _supplierService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Zwraca pojedynczego dostawce po jego identyfikatorze.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SupplierResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierResponseDto>> GetById(int id)
    {
        var result = await _supplierService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Dodaje nowego dostawce.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SupplierResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SupplierResponseDto>> Create([FromBody] SupplierCreateDto dto)
    {
        var result = await _supplierService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Aktualizuje dane dostawcy.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(SupplierResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierResponseDto>> Update(int id, [FromBody] SupplierUpdateDto dto)
    {
        var result = await _supplierService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Usuwa dostawce (usuniecie miekkie).
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        await _supplierService.DeleteAsync(id);
        return NoContent();
    }
}
