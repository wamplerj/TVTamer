namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EpisodeStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DownloadStatuses",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 5, fixedLength: false),
                    Name = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.Id);

            Sql("INSERT INTO dbo.DownloadStatuses (Id, Name) VALUES ('WANT','Wanted')");
            Sql("INSERT INTO dbo.DownloadStatuses (Id, Name) VALUES ('SKIP','Skipped')");
            Sql("INSERT INTO dbo.DownloadStatuses (Id, Name) VALUES ('HAVE','Already Downloaded')");

            AddColumn("dbo.TvEpisodes", "DownloadStatus", c => c.String(defaultValue: "SKIP", maxLength: 5, nullable: false));
            CreateIndex("dbo.TvEpisodes", "DownloadStatus");
            AddForeignKey("dbo.TvEpisodes", "DownloadStatus", "dbo.DownloadStatuses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TvEpisodes", "DownloadStatus", "dbo.DownloadStatuses");
            DropIndex("dbo.TvEpisodes", new[] { "DownloadStatus" });
            DropColumn("dbo.TvEpisodes", "DownloadStatus");
            DropTable("dbo.DownloadStatuses");
        }
    }
}
