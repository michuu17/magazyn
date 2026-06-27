using WarehouseApi.Models;

namespace WarehouseApi.Data;

public static class DataSeeder
{
    public static void Seed(WarehouseDbContext context)
    {
        if (context.Suppliers.Any())
        {
            return;
        }

        var supplierStalmet = new Supplier
        {
            Name = "Hurtownia Stalmet",
            Email = "kontakt@stalmet.pl",
            PhoneNumber = "601234567"
        };

        var supplierElektro = new Supplier
        {
            Name = "Dystrybucja Elektro",
            Email = "biuro@elektro-dystrybucja.pl",
            PhoneNumber = "602345678"
        };

        context.Suppliers.Add(supplierStalmet);
        context.Suppliers.Add(supplierElektro);
        context.SaveChanges();

        var products = new List<Product>
        {
            new Product { Sku = "STL-001", Name = "Sruba M8", Quantity = 500, UnitPrice = 0.45m, SupplierId = supplierStalmet.Id, CreatedAt = DateTime.UtcNow },
            new Product { Sku = "STL-002", Name = "Nakretka M8", Quantity = 480, UnitPrice = 0.30m, SupplierId = supplierStalmet.Id, CreatedAt = DateTime.UtcNow },
            new Product { Sku = "ELK-100", Name = "Przewod YDYp 3x1.5", Quantity = 120, UnitPrice = 3.20m, SupplierId = supplierElektro.Id, CreatedAt = DateTime.UtcNow },
            new Product { Sku = "ELK-101", Name = "Gniazdo podtynkowe", Quantity = 75, UnitPrice = 12.50m, SupplierId = supplierElektro.Id, CreatedAt = DateTime.UtcNow }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
