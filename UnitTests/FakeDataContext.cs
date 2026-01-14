using DNN.Modules.UserVoice.Data;
using DNN.Modules.UserVoice.Data.Entities;
using DNN.Modules.UserVoice.Data.Repositories;
using DNN.Modules.UserVoice.Providers;
using Effort.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;

namespace UnitTests
{
    /// <summary>
    /// Provides a fake data context for testing.
    /// </summary>
    public class FakeDataContext : IDisposable
    {
        public EffortConnection connection;
        public TestDataContext testDataContext;
        public ModuleDbContext dataContext => testDataContext;

        private bool _disposed = false;

        public FakeDataContext()
        {
            this.connection = Effort.DbConnectionFactory.CreateTransient();
            this.testDataContext = new TestDataContext(this.connection);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                this.testDataContext.Dispose();
                this.connection.Dispose();
            }

            this.testDataContext = null;
            this.connection = null;

            _disposed = true;
        }
    }

    public class TestDataContext : ModuleDbContext
    {
        public TestDataContext(DbConnection connection)
            : base(connection)
        {
        }

        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<TestProduct> TestProducts { get; set; }
    }

    public class TestCategory : BaseEntity
    {
        public TestCategory()
        {
            this.Products = new HashSet<TestProduct>();
        }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<TestProduct> Products { get; set; }
    }

    public class TestProduct : BaseEntity
    {
        public string Name { get; set; }

        public virtual TestCategory Category { get; set; }
    }

    public class ProductRepository : Repository<TestProduct>
    {
        public ProductRepository(ModuleDbContext context, IDateTimeProvider dateTimeProvider)
            : base(context, dateTimeProvider)
        {
        }
    }
}
