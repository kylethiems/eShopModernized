namespace eShopModern.Catalog.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using eShopModern.Catalog.Models;
using eShopModern.Catalog.ViewModel;

public class CatalogService : ICatalogService, IDisposable
{
    private readonly CatalogContext _context;
    private readonly CatalogSettings _settings;

    public CatalogService(CatalogContext context, IOptions<CatalogSettings>? settings = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _settings = settings?.Value ?? new CatalogSettings();
    }

    public PaginatedItemsViewModel<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex)
    {
        var totalItems = _context.CatalogItems.LongCount();
        var itemsOnPage = _context.CatalogItems
            .OrderBy(ci => ci.Id)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToList();

        return new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
    }

    public CatalogItem? FindCatalogItem(int id)
    {
        return _context.CatalogItems.Find(id);
    }

    public IEnumerable<CatalogType> GetCatalogTypes()
    {
        return _context.CatalogTypes.ToList();
    }

    public IEnumerable<CatalogBrand> GetCatalogBrands()
    {
        return _context.CatalogBrands.ToList();
    }

    public void CreateCatalogItem(CatalogItem catalogItem)
    {
        if (string.IsNullOrEmpty(catalogItem.PictureFileName))
        {
            catalogItem.PictureFileName = _settings.DefaultPictureName;
        }
        _context.CatalogItems.Add(catalogItem);
        _context.SaveChanges();
    }

    public void UpdateCatalogItem(CatalogItem catalogItem)
    {
        _context.CatalogItems.Update(catalogItem);
        _context.SaveChanges();
    }

    public void RemoveCatalogItem(CatalogItem catalogItem)
    {
        _context.CatalogItems.Remove(catalogItem);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

public interface ICatalogService : IDisposable
{
    PaginatedItemsViewModel<CatalogItem> GetCatalogItemsPaginated(int pageSize, int pageIndex);
    CatalogItem? FindCatalogItem(int id);
    IEnumerable<CatalogType> GetCatalogTypes();
    IEnumerable<CatalogBrand> GetCatalogBrands();
    void CreateCatalogItem(CatalogItem catalogItem);
    void UpdateCatalogItem(CatalogItem catalogItem);
    void RemoveCatalogItem(CatalogItem catalogItem);
}

public class CatalogSettings
{
    public string DefaultPictureName { get; set; } = "dummy.png";
}

public class CatalogContext : DbContext
{
    public virtual DbSet<CatalogItem> CatalogItems { get; set; } = null!;
    public virtual DbSet<CatalogBrand> CatalogBrands { get; set; } = null!;
    public virtual DbSet<CatalogType> CatalogTypes { get; set; } = null!;

    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogItem>().Property(ci => ci.Id).UseHiLo("catalog_hilo");
        modelBuilder.Entity<CatalogBrand>().Property(cb => cb.Id).UseHiLo("catalog_brand_hilo");
        modelBuilder.Entity<CatalogType>().Property(ct => ct.Id).UseHiLo("catalog_type_hilo");
    }
}
