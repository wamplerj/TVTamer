namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoggedEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventType = c.Int(nullable: false),
                        EventTime = c.DateTime(nullable: false),
                        Message = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TvEpisodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeriesName = c.String(),
                        Title = c.String(nullable: false, maxLength: 255),
                        Season = c.Int(nullable: false),
                        EpisodeNumber = c.Int(nullable: false),
                        FileName = c.String(),
                        Summary = c.String(),
                        FirstAired = c.DateTime(nullable: false),
                        SeriesId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TvSeries", t => t.SeriesId, cascadeDelete: true)
                .Index(t => t.SeriesId);
            
            CreateTable(
                "dbo.TvSeries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeriesId = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 255),
                        FirstAired = c.DateTime(nullable: false),
                        AirsDayOfWeek = c.Int(nullable: false),
                        AirsTimeOfDay = c.String(),
                        Network = c.String(),
                        Summary = c.String(),
                        Status = c.String(),
                        Rating = c.String(),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.SeriesId, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TvEpisodes", "SeriesId", "dbo.TvSeries");
            DropIndex("dbo.TvSeries", new[] { "SeriesId" });
            DropIndex("dbo.TvEpisodes", new[] { "SeriesId" });
            DropTable("dbo.TvSeries");
            DropTable("dbo.TvEpisodes");
            DropTable("dbo.LoggedEvents");
        }
    }
}
