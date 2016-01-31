namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDownloadingStatus : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO dbo.DownloadStatuses (Id, Name) VALUES ('DOWN','Downloading')");
        }
        
        public override void Down()
        {
            Sql("UPDATE dbo.TvEpisodes SET DownloadStatus = 'WANT' WHERE DownloadStatus = 'DOWN'");
            Sql("DELETE FROM dbo.DownloadStatuses WHERE ID = 'DOWN'");
           
        }
    }
}
