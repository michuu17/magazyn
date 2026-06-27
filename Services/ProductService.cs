using Microsoft.EntityFrameworkCore;
using WarehouseApi.Data;
using WarehouseApi.DTOs;
using WarehouseApi.Exceptions;
using WarehouseApi.Models;

namespace WarehouseApi.Services;

public class ProductService : IProductService
{
    private readonly WarehouseDbContext _context;

    public ProductService(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductResponseDto>> GetPagedAsync(PaginationQuery query)
    {
        var baseQuery = _context.Products
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Id);

        var totalItems = await baseQuery.CountAsync();

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                UnitPrice = p.UnitPrice,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier!.Name,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

        return new PagedResult<ProductResponseDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<ProductResponseDto> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            throw new NotFoundException($"Nie znaleziono produktu o id {id}.");
        }

        return MapToResponse(product);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == dto.SupplierId && !s.IsDeleted);

        if (supplier == null)
        {
            throw new NotFoundException($"Nie znaleziono dostawcy o id {dto.SupplierId}.");
        }

        var skuExists = await _context.Products
            .AnyAsync(p => !p.IsDeleted && p.Sku == dto.Sku);

        if (skuExists)
        {
            throw new DomainException($"Produkt o kodzie SKU {dto.Sku} juz istnieje w magazynie.");
        }

        var product = new Product
        {
            Sku = dto.Sku,
            Name = dto.Name,
            Description = dto.Description,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice,
            SupplierId = dto.SupplierId,
            Supplier = supplier,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto dto)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            throw new NotFoundException($"Nie znaleziono produktu o id {id}.");
        }

        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == dto.SupplierId && !s.IsDeleted);

        if (supplier == null)
        {
            throw new NotFoundException($"Nie znaleziono dostawcy o id {dto.SupplierId}.");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.UnitPrice = dto.UnitPrice;
        product.SupplierId = dto.SupplierId;
        product.Supplier = supplier;

        await _context.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            throw new NotFoundException($"Nie znaleziono produktu o id {id}.");
        }

        product.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task<ProductResponseDto> ReceiveStockAsync(int id, int quantity)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            throw new NotFoundException($"Nie znaleziono produktu o id {id}.");
        }

        product.Quantity += quantity;
        await _context.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task<ProductResponseDto> IssueStockAsync(int id, int quantity)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
        {
            throw new NotFoundException($"Nie znaleziono produktu o id {id}.");
        }

        if (quantity > product.Quantity)
        {
            throw new DomainException($"Brak wystarczajacej ilosci na stanie. Dostepne: {product.Quantity}, zadano: {quantity}.");
        }

        product.Quantity -= quantity;
        await _context.SaveChangesAsync();

        return MapToResponse(product);
    }

    private static ProductResponseDto MapToResponse(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            UnitPrice = product.UnitPrice,
            SupplierId = product.SupplierId,
            SupplierName = product.Supplier?.Name ?? string.Empty,
            CreatedAt = product.CreatedAt
        };
    }
}
