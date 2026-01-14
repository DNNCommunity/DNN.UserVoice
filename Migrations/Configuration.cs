// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Migrations
{
    using DNN.Modules.UserVoice.Data;
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Configures Entity Framework Code First migrations for the UserVoice module database context.
    /// </summary>
    /// <remarks>This class is used by Entity Framework to manage database schema changes for the UserVoice
    /// module. It specifies migration settings and provides a Seed method for initializing or updating data after
    /// migrations are applied. This class is not intended to be used directly in application code.</remarks>
    internal sealed class Configuration : DbMigrationsConfiguration<ModuleDbContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = false;
            this.ContextKey = "DNN.Modules.UserVoice.Data.ModuleDbContext";
        }

        /// <inheritdoc/>
        protected override void Seed(ModuleDbContext context)
        {
            // This method will be called after migrating to the latest version.
            // You can use the DbSet<T>.AddOrUpdate() helper extension method
            // to avoid creating duplicate seed data.
        }
    }
}
