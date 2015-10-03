namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoggedEventUpdates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LoggedEvents", "EventType", c => c.String(nullable: false, maxLength: 15));

            CreateTable(
            "dbo.AnalyticEventTypes",
            c => new
            {
                Id = c.String(nullable: false, maxLength: 15, fixedLength: false),
                Name = c.String(nullable: false, maxLength: 255),
            })
            .PrimaryKey(t => t.Id);

            Sql("INSERT INTO dbo.AnalyticEventTypes (Id, Name) VALUES ('DBUPDATE','Database Update')");
            Sql("INSERT INTO dbo.AnalyticEventTypes (Id, Name) VALUES ('DOWNLOADFAILED','Download Torrent Attempt Failed')");
            Sql("INSERT INTO dbo.AnalyticEventTypes (Id, Name) VALUES ('DOWNLOAD','Downloaded Torrent for Episode')");
            Sql("INSERT INTO dbo.AnalyticEventTypes (Id, Name) VALUES ('EPISODEFAILED','Episode Processing Failed')");
            Sql("INSERT INTO dbo.AnalyticEventTypes (Id, Name) VALUES ('EPISODE','Processed Downloaded Episode')");
            Sql("INSERT INTO dbo.AnalyticEventTypes (Id, Name) VALUES ('SEARCH','Episode Search Performed')");
            Sql("INSERT INTO dbo.AnalyticEventTypes (Id, Name) VALUES ('SEARCHFAILED','Episode Search Returned No Results')");

            CreateIndex("dbo.LoggedEvents", "EventType");
            AddForeignKey("dbo.LoggedEvents", "EventType", "dbo.AnalyticEventTypes", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.LoggedEvents", "EventType", "dbo.AnalyticEventTypes");
            DropIndex("dbo.LoggedEvents", new[] { "EventType" });

            AlterColumn("dbo.LoggedEvents", "EventType", c => c.Int(nullable: false));
            DropTable("dbo.AnalyticEventTypes");

        }
    }
}
