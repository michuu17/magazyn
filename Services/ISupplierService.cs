using WarehouseApi.DTOs;

namespace WarehouseApi.Services;

public interface ISupplierService
{
    Task<List<SupplierResponseDto>> GetAllAsync();
    Task<SupplierResponseDto> GetByIdAsync(int id);
    Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto);
    Task<SupplierResponseDto> UpdateAsync(int id, SupplierUpdateDto dto);
    Task DeleteAsync(int id);
}
