// MIT License
// Copyright DNN Community

using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Providers;

namespace DNN.Modules.UserVoice.Data.Repositories
{
    /// <inheritdoc cref="IItemRepository"/>
    internal class ItemRepository : Repository<Item>, IItemRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRepository"/> class.
        /// </summary>
        /// <param name="context">The underlying data-context to use.</param>
        /// <param name="dateTimeProvider">Provides information about dates and times.</param>
        public ItemRepository(ModuleDbContext context, IDateTimeProvider dateTimeProvider)
            : base(context, dateTimeProvider)
        {
        }
    }
}