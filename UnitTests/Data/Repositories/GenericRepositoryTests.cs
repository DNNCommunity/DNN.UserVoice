using DNN.Modules.UserVoice.Providers;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Data.Repositories
{
    public class GenericRepositoryTests : FakeDataContext
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public GenericRepositoryTests()
        {
            this.dateTimeProvider = Substitute.For<IDateTimeProvider>();
            this.dateTimeProvider.GetUtcNow().Returns(new DateTime(2022, 1, 1));
        }

        [Fact]
        public void GenericRepositoryConstructs()
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);

            Assert.NotNull(dataContext);
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task GenericRepositoryCreatesAndGetsById()
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            var expectedItem = new TestProduct() { Id = 1, Name = "Name" };
            await repository.CreateAsync(expectedItem);

            var returnedItem = await repository.GetByIdAsync(1);

            Assert.Equal(JsonConvert.SerializeObject(expectedItem), JsonConvert.SerializeObject(returnedItem));
        }

        [Fact]
        public async Task GenericRepositoryDeletes()
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            var item = new TestProduct() { Id = 1, Name = "Name" };
            await repository.CreateAsync(item);

            await repository.DeleteAsync(item.Id);

            Assert.Empty(await repository.GetAllAsync());
        }

        [Fact]
        public void GenericRepositoryGet()
        {
            var context = this.dataContext as TestDataContext;
            context.TestProducts.Add(new TestProduct() { Id = 1, Name = "Name" });
            context.SaveChanges();
            var repository = new ProductRepository(context, this.dateTimeProvider);

            var items = repository.Get();

            Assert.Equal(1, items.Count());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GenericRepositoryGetsAll(int iterations)
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            for (int i = 1; i <= iterations; i++)
            {
                var item = new TestProduct() { Id = i, Name = $"Name {i}" };
                await repository.CreateAsync(item);
            }

            var items = await repository.GetAllAsync();
            var count = items.Count();

            Assert.Equal(iterations, count);
        }

        [Fact]
        public async Task GenericRepositoryUpdates()
        {
            var createdTime = new DateTime(2022, 1, 1);
            this.dateTimeProvider.GetUtcNow().Returns(createdTime);
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            await repository.CreateAsync(new TestProduct() { Id = 1, Name = "Original Name" });
            var entity = await repository.GetByIdAsync(1);
            entity.Name = "New Name";
            var updatedTime = createdTime.AddDays(1);
            this.dateTimeProvider.GetUtcNow().Returns(updatedTime);

            await repository.UpdateAsync(entity);

            Assert.Equal("New Name", entity.Name);
            Assert.True(entity.UpdatedAt > entity.CreatedAt);
        }

        [Fact]
        public async Task GenericRepositoryCreate_ThrowsWithNullEntity()
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);

            Task create() => repository.CreateAsync(null);

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(create);
            Assert.Equal("entity", ex.ParamName);
        }

        [Fact]
        public async Task GenericRepositoryUpdate_ThrowsWithNullEntity()
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);

            Task update() => repository.UpdateAsync(null);

            var ex = await Assert.ThrowsAsync<ArgumentNullException>(update);
            Assert.Equal("entity", ex.ParamName);
        }

        [Fact]
        public async Task Repository_Create_DefaultAudit()
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            var item = new TestProduct() { Name = "Name" };

            await repository.CreateAsync(item);

            Assert.Equal(-1, item.CreatedByUserId);
            Assert.Equal(-1, item.UpdatedByUserId);
            Assert.True(item.CreatedAt == item.UpdatedAt);
            Assert.Equal(item.CreatedAt, this.dateTimeProvider.GetUtcNow());
            Assert.Equal(item.UpdatedAt, this.dateTimeProvider.GetUtcNow());
        }

        [Fact]
        public async Task Repository_Create_UsesUserId()
        {
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            var item = new TestProduct() { Name = "Name" };

            await repository.CreateAsync(item, 123);

            Assert.Equal(123, item.CreatedByUserId);
            Assert.Equal(123, item.UpdatedByUserId);
        }

        [Fact]
        public async Task Repository_Update_DefaultAudit()
        {
            var currentTime = this.dateTimeProvider.GetUtcNow();
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            var item = new TestProduct() { Name = "Name" };
            var id = await repository.CreateAsync(item);
            this.dateTimeProvider.GetUtcNow().Returns(currentTime.AddDays(1));
            var itemToUpdate = await repository.GetByIdAsync(id);
            itemToUpdate.Name = "New Name";

            await repository.UpdateAsync(itemToUpdate);

            Assert.True(itemToUpdate.CreatedAt < itemToUpdate.UpdatedAt);
            Assert.Equal(-1, itemToUpdate.CreatedByUserId);
            Assert.Equal(-1, itemToUpdate.UpdatedByUserId);
        }

        [Fact]
        public async Task Repository_Update_UsesUserId()
        {
            var now = this.dateTimeProvider.GetUtcNow();
            var repository = new ProductRepository(dataContext, this.dateTimeProvider);
            var item = new TestProduct() { Name = "Name" };
            await repository.CreateAsync(item);
            item.Name = "New Name";
            this.dateTimeProvider.GetUtcNow().Returns(now.AddDays(1));

            await repository.UpdateAsync(item, 123);

            Assert.True(item.CreatedAt < item.UpdatedAt);
            Assert.Equal(-1, item.CreatedByUserId);
            Assert.Equal(123, item.UpdatedByUserId);
        }

        /// <summary>
        /// Ensures that deleting an item id that does not exists simply does nothing.
        /// </summary>
        [Fact]
        public async Task Repository_DeleteMissing_DoesNotThrow()
        {
            var repository = new ProductRepository(this.dataContext, this.dateTimeProvider);

            await repository.DeleteAsync(1);
        }

        [Theory]
        [InlineData(1, 10, 10, 5)]
        [InlineData(3, 20, 10, 3)]
        [InlineData(0, 0, 1, 50)]
        public async Task GetPage_Pages(
            int page,
            int pageSize,
            int expectedItems,
            int expectedPages)
        {
            this.CreateProducts(100);
            var repository = new ProductRepository(this.dataContext, this.dateTimeProvider);

            var result = await repository.GetPageAsync(
                page,
                pageSize,
                filter: item => item.Name.ToUpper().Contains("test".ToUpper()),
                orderBy: item => item.Name);

            Assert.Equal(expectedItems, result.Items.Count());
            Assert.Equal(page == 0 ? 1 : page, result.Page);
            Assert.Equal(pageSize == 0 ? 1 : pageSize, result.PageSize);
            Assert.Equal(50, result.ResultCount);
            Assert.Equal(expectedPages, result.PageCount);
        }

        [Fact]
        public async Task Pages_With_Related_Entities()
        {
            var testConnection = Effort.DbConnectionFactory.CreateTransient();
            using (var context = new TestDataContext(testConnection))
            {
                for (int i = 0; i < 10; i++)
                {
                    var newCategory = new TestCategory { Name = $"Category {i}" };
                    context.TestCategories.Add(newCategory);
                    await context.SaveChangesAsync();
                    for (int j = 0; j < 10; j++)
                    {
                        var newProduct = new TestProduct
                        {
                            Name = $"Product {j}",
                            Category = newCategory,
                        };
                        context.TestProducts.Add(newProduct);
                    }
                    await context.SaveChangesAsync();
                }
                var repository = new ProductRepository(context, this.dateTimeProvider);

                var result = await repository.GetPageAsync(
                    1,
                    10,
                    orderBy: product => product.Name,
                    include: product => product.Category);
            }
        }

        private void CreateProducts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                this.testDataContext.TestProducts.Add(new TestProduct()
                {
                    Name = i % 2 == 0 ? $"Test Name {i}" : $"Name {i}",
                });
            }
            this.testDataContext.SaveChanges();
        }
    }
}
