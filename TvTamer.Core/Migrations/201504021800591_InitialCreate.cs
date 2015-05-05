namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TvSeries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeriesId = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 255),
                        FirstAired = c.DateTime(nullable: false),
                        Summary = c.String(),
                        Status = c.String(),
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
                        Series_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TvSeries", t => t.Series_Id, cascadeDelete: true)
                .Index(t => t.Series_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TvEpisodes", "Series_Id", "dbo.TvSeries");
            DropIndex("dbo.TvEpisodes", new[] { "Series_Id" });
            DropTable("dbo.TvEpisodes");
            DropTable("dbo.TvSeries");
        }
    }
}
