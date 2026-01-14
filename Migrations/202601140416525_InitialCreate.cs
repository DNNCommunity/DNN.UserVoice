// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Represents the initial database migration for creating the DNN_UserVoice_Items table schema.
    /// </summary>
    public partial class InitialCreate : DbMigration
    {
        /// <inheritdoc/>
        public override void Up()
        {
            this.CreateTable(
                "dbo.DNN_UserVoice_Items",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 50),
                    Description = c.String(maxLength: 250),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                    CreatedByUserId = c.Int(nullable: false),
                    UpdatedByUserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        /// <inheritdoc/>
        public override void Down()
        {
            this.DropTable("dbo.DNN_UserVoice_Items");
        }
    }
}
