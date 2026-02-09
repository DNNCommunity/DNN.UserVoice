// MIT License
// Copyright DNN Community

namespace DNN.Modules.UserVoice.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Represents the initial database migration for the UserVoice module, defining the creation and removal of core
    /// tables and relationships.
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
                .PrimaryKey(t => t.Id)
                .Index(t => t.ModuleId);

            this.CreateTable(
                "dbo.DNN_UserVoice_UserVotes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.Int(nullable: false),
                    IdeaId = c.Int(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    UpdatedAt = c.DateTime(nullable: false),
                    CreatedByUserId = c.Int(nullable: false),
                    UpdatedByUserId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DNN_UserVoice_Ideas", t => t.IdeaId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => new { t.IdeaId, t.UserId }, unique: true, name: "IX_UserVote_Idea_User");
        }

        /// <inheritdoc/>
        public override void Down()
        {
            this.DropForeignKey("dbo.DNN_UserVoice_UserVotes", "IdeaId", "dbo.DNN_UserVoice_Ideas");
            this.DropIndex("dbo.DNN_UserVoice_UserVotes", "IX_UserVote_Idea_User");
            this.DropIndex("dbo.DNN_UserVoice_UserVotes", new[] { "UserId" });
            this.DropIndex("dbo.DNN_UserVoice_Ideas", new[] { "ModuleId" });
            this.DropTable("dbo.DNN_UserVoice_UserVotes");
            this.DropTable("dbo.DNN_UserVoice_Ideas");
        }
    }
}
