// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Data.Repositories
{
    using DNN.Modules.UserVoice.Data.Entities;
    using DNN.Modules.UserVoice.Providers;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides common generic data access methods for entities.
    /// </summary>
    /// <typeparam name="T">The type of the entities.</typeparam>
    public abstract class Repository<T> : IRepository<T>
        where T : BaseEntity
    {
        private readonly ModuleDbContext context;
        private readonly DbSet<T> entities;
        private readonly IDateTimeProvider dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The module database context.</param>
        /// <param name="dateTimeProvider">Provides date and time information.</param>
        public Repository(
            ModuleDbContext context,
            IDateTimeProvider dateTimeProvider)
        {
            this.context = context;
            this.entities = context.Set<T>();
            this.dateTimeProvider = dateTimeProvider;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
        {
            return await this.entities.ToListAsync(token);
        }

        /// <inheritdoc/>
        public IQueryable<T> Get()
        {
            return this.entities;
        }

        /// <inheritdoc/>
        public async Task<T> GetByIdAsync(int id, CancellationToken token = default)
        {
            return await this.entities.FindAsync(token, id);
        }

        /// <inheritdoc/>
        public virtual async Task<int> CreateAsync(T entity, int userId = -1, CancellationToken token = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.CreatedAt = this.dateTimeProvider.GetUtcNow();
            entity.CreatedByUserId = userId;
            entity.UpdatedAt = this.dateTimeProvider.GetUtcNow();
            entity.UpdatedByUserId = userId;
            this.entities.Add(entity);
            await this.context.SaveChangesAsync(token);
            return entity.Id;
        }

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(T entity, int userId = -1, CancellationToken token = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.UpdatedAt = this.dateTimeProvider.GetUtcNow();
            entity.UpdatedByUserId = userId;

            this.entities.Attach(entity);
            this.context.Entry(entity).State = EntityState.Modified;
            await this.context.SaveChangesAsync(token);
        }

        /// <inheritdoc/>
        public virtual async Task DeleteAsync(int id, CancellationToken token = default)
        {
            T entity = await this.entities.FindAsync(token, id);
            if (entity is null)
            {
                return;
            }

            this.entities.Remove(entity);
            await this.context.SaveChangesAsync(token);
        }
    }
}