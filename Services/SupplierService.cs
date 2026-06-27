using Microsoft.EntityFrameworkCore;
using WarehouseApi.Data;
using WarehouseApi.DTOs;
using WarehouseApi.Exceptions;
using WarehouseApi.Models;

namespace WarehouseApi.Services;

public class SupplierService : ISupplierService
{
    private readonly WarehouseDbContext _context;

    public SupplierService(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplierResponseDto>> GetAllAsync()
    {
        return await _context.Suppliers
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Id)
            .Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                ProductCount = s.Products.Count(p => !p.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<SupplierResponseDto> GetByIdAsync(int id)
    {
        var supplier = await _context.Suppliers
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                ProductCount = s.Products.Count(p => !p.IsDeleted)
            })
            .FirstOrDefaultAsync();

        if (supplier == null)
        {
            throw new NotFoundException($"Nie znaleziono dostawcy o id {id}.");
        }

        return supplier;
    }

    public async Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto)
    {
        var supplier = new Supplier
        {
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsDeleted = false
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return new SupplierResponseDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Email = supplier.Email,
            PhoneNumber = supplier.PhoneNumber,
            ProductCount = 0
        };
    }

    public async Task<SupplierResponseDto> UpdateAsync(int id, SupplierUpdateDto dto)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (supplier == null)
        {
            throw new NotFoundException($"Nie znaleziono dostawcy o id {id}.");
        }

        supplier.Name = dto.Name;
        supplier.Email = dto.Email;
        supplier.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();

        var productCount = await _context.Products
            .CountAsync(p => p.SupplierId == id && !p.IsDeleted);

        return new SupplierResponseDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Email = supplier.Email,
            PhoneNumber = supplier.PhoneNumber,
            ProductCount = productCount
        };
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (supplier == null)
        {
            throw new NotFoundException($"Nie znaleziono dostawcy o id {id}.");
        }

        var hasProducts = await _context.Products
            .AnyAsync(p => p.SupplierId == id && !p.IsDeleted);

        if (hasProducts)
        {
            throw new DomainException("Nie mozna usunac dostawcy, ktory ma przypisane produkty w magazynie.");
        }

        supplier.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
