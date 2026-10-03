using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using eShopModern.Catalog.Models;
using eShopModern.Catalog.Services;
using eShopModern.Catalog.ViewModel;

namespace eShopModern.Catalog.Tests;

public class CatalogServiceTests
{
    private CatalogContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new CatalogContext(options);
    }

    [Fact]
    public void GetCatalogItemsPaginated_ReturnsCorrectPagination()
    {
        // Arrange
        using var context = CreateContext(nameof(GetCatalogItemsPaginated_ReturnsCorrectPagination));
        context.CatalogItems.AddRange(
            new CatalogItem { Id = 1, Name = "Item1", Price = 10.0m },
            new CatalogItem { Id = 2, Name = "Item2", Price = 20.0m },
            new CatalogItem { Id = 3, Name = "Item3", Price = 30.0m }
        );
        context.SaveChanges();

        var service = new CatalogService(context);

        // Act
        var result = service.GetCatalogItemsPaginated(2, 0);

        // Assert
        Assert.Equal(0, result.PageIndex);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.Count);
        Assert.Equal(2, result.Data.Count());
        Assert.Equal("Item1", result.Data.First().Name);
        Assert.Equal("Item2", result.Data.Skip(1).First().Name);
    }

    [Fact]
    public void GetCatalogItemsPaginated_SecondPage_ReturnsRemainingItems()
    {
        // Arrange
        using var context = CreateContext(nameof(GetCatalogItemsPaginated_SecondPage_ReturnsRemainingItems));
        context.CatalogItems.AddRange(
            new CatalogItem { Id = 1, Name = "Item1", Price = 10.0m },
            new CatalogItem { Id = 2, Name = "Item2", Price = 20.0m },
            new CatalogItem { Id = 3, Name = "Item3", Price = 30.0m }
        );
        context.SaveChanges();

        var service = new CatalogService(context);

        // Act
        var result = service.GetCatalogItemsPaginated(2, 1);

        // Assert
        Assert.Equal(1, result.PageIndex);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(3, result.Count);
        Assert.Single(result.Data);
        Assert.Equal("Item3", result.Data.First().Name);
    }

    [Fact]
    public void FindCatalogItem_ExistingId_ReturnsMatchingItem()
    {
        // Arrange
        using var context = CreateContext(nameof(FindCatalogItem_ExistingId_ReturnsMatchingItem));
        var item = new CatalogItem { Id = 42, Name = "TargetItem", Price = 99.99m };
        context.CatalogItems.Add(item);
        context.SaveChanges();

        var service = new CatalogService(context);

        // Act
        var result = service.FindCatalogItem(42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("TargetItem", result.Name);
    }

    [Fact]
    public void FindCatalogItem_NonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = CreateContext(nameof(FindCatalogItem_NonExistentId_ReturnsNull));
        var service = new CatalogService(context);

        // Act
        var result = service.FindCatalogItem(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetCatalogTypes_ReturnsAllTypes()
    {
        // Arrange
        using var context = CreateContext(nameof(GetCatalogTypes_ReturnsAllTypes));
        context.CatalogTypes.AddRange(
            new CatalogType { Id = 1, Type = "Mug" },
            new CatalogType { Id = 2, Type = "T-Shirt" }
        );
        context.SaveChanges();

        var service = new CatalogService(context);

        // Act
        var result = service.GetCatalogTypes();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetCatalogBrands_ReturnsAllBrands()
    {
        // Arrange
        using var context = CreateContext(nameof(GetCatalogBrands_ReturnsAllBrands));
        context.CatalogBrands.AddRange(
            new CatalogBrand { Id = 1, Brand = "Azure" },
            new CatalogBrand { Id = 2, Brand = ".NET" }
        );
        context.SaveChanges();

        var service = new CatalogService(context);

        // Act
        var result = service.GetCatalogBrands();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void CreateCatalogItem_ValidItem_AddsAndSaves()
    {
        // Arrange
        using var context = CreateContext(nameof(CreateCatalogItem_ValidItem_AddsAndSaves));
        var service = new CatalogService(context);
        var newItem = new CatalogItem { Id = 10, Name = "NewItem", Price = 15.0m };

        // Act
        service.CreateCatalogItem(newItem);

        // Assert
        var savedItem = context.CatalogItems.Find(10);
        Assert.NotNull(savedItem);
        Assert.Equal("NewItem", savedItem.Name);
    }

    [Fact]
    public void UpdateCatalogItem_ValidItem_UpdatesAndSaves()
    {
        // Arrange
        using var context = CreateContext(nameof(UpdateCatalogItem_ValidItem_UpdatesAndSaves));
        var originalItem = new CatalogItem { Id = 10, Name = "OriginalItem", Price = 20.0m };
        context.CatalogItems.Add(originalItem);
        context.SaveChanges();

        var service = new CatalogService(context);
        originalItem.Name = "UpdatedItem";
        originalItem.Price = 25.0m;

        // Act
        service.UpdateCatalogItem(originalItem);

        // Assert
        var updated = context.CatalogItems.Find(10);
        Assert.NotNull(updated);
        Assert.Equal("UpdatedItem", updated.Name);
        Assert.Equal(25.0m, updated.Price);
    }

    [Fact]
    public void RemoveCatalogItem_ValidItem_RemovesAndSaves()
    {
        // Arrange
        using var context = CreateContext(nameof(RemoveCatalogItem_ValidItem_RemovesAndSaves));
        var item = new CatalogItem { Id = 10, Name = "ItemToRemove" };
        context.CatalogItems.Add(item);
        context.SaveChanges();

        var service = new CatalogService(context);

        // Act
        service.RemoveCatalogItem(item);

        // Assert
        var removed = context.CatalogItems.Find(10);
        Assert.Null(removed);
    }
}
