using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tournament.Core.Request;

public class PagedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public MetaData MetaData { get; set; }

    // Constructor to initialize the PagedList with items and metadata
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
            (
                totalCount: count,
                totalPages: (int)Math.Ceiling(count / (double)pageSize),
                pageSize: pageSize,
                currentPage: pageNumber
            );

        // Convert the items to a read-only list
        Items = new List<T>(items).AsReadOnly();
    }


    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
