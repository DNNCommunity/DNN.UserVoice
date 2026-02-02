// MIT License
// Copyright DNN Community
namespace DNN.Modules.UserVoice.Common.Extensions
{
    using DNN.Modules.UserVoice.Data.Repositories;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A collection of extension methods for enumerables.
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Orders the collection with an option to do it ascending or descending.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <typeparam name="TKey">The key to use for ordering.</typeparam>
        /// <param name="source">The source collection to order.</param>
        /// <param name="selector">A function to define the sorting expression.</param>
        /// <param name="descending">If ture, will sort in descending order instead of ascending.</param>
        /// <returns>A new enumerable sorted as specified.</returns>
        public static IOrderedQueryable<T> Order<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> selector, bool descending)
        {
            if (descending)
            {
                return source.OrderByDescending(selector);
            }

            return source.OrderBy(selector);
        }

        /// <summary>
        /// Asynchronously creates a paged list from the specified query by retrieving the items for the given page and
        /// page size.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source query.</typeparam>
        /// <param name="query">The source query to paginate. Must not be null.</param>
        /// <param name="page">The one-based index of the page to retrieve. Values less than 1 are treated as 1.</param>
        /// <param name="pageSize">The number of items per page. Values less than 1 are treated as 1.</param>
        /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a PagedList{T} with the items
        /// for the specified page and page size, along with pagination metadata.</returns>
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int page, int pageSize, CancellationToken token = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var total = await query.CountAsync(token);
            var skip = (page - 1) * pageSize;

            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(token);

            var pageCount = (total + pageSize - 1) / pageSize;

            return new PagedList<T>(items, page, pageSize, total, pageCount);
        }
    }
}