// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Initializes the initial database schema for the UserVoice module.
    /// </summary>
    public partial class InitialCreate : DbMigration
    {
        /// <inheritdoc/>
        public override void Up()
        {
            this.CreateTable(
                "dbo.DNN_UserVoice_Ideas",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ModuleId = c.Int(nullable: false),
                    Title = c.String(nullable: false, maxLength: 200),
                    Description = c.String(nullable: false, maxLength: 2000),
                    DeletedOn = c.DateTime(),
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
            this.DropTable("dbo.DNN_UserVoice_Ideas");
        }
    }
}
