namespace eShopModern.Catalog.ViewModel;

using System.Collections.Generic;

public record PaginatedItemsViewModel<TEntity>(int PageIndex, int PageSize, long Count, IEnumerable<TEntity> Data);
