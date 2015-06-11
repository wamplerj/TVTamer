using System;
using System.Data.Entity.ModelConfiguration;

namespace TvTamer.Core.Models
{
    public class TvEpisode
    {
        public int Id { get; set; }
        public string SeriesName { get; set; }
        public string Title { get; set; }
        public int Season { get; set; }
        public int EpisodeNumber { get; set; }
        public string FileName { get; set; }
        public string Summary { get; set; }
        public DateTime FirstAired { get; set; }
        public string DownloadStatus { get; set; }

        public int SeriesId { get; set; }
        public TvSeries Series { get; set; }
    }

    public class TvEpisodeMap : EntityTypeConfiguration<TvEpisode>
    {

        public TvEpisodeMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Title).HasMaxLength(255).IsRequired();
            this.Property(t => t.Season).IsRequired();
            this.Property(t => t.EpisodeNumber).IsRequired();
            this.Property(t => t.FirstAired).IsRequired();
            this.Property(t => t.Summary);
            this.Property(t => t.DownloadStatus).IsRequired();

            HasRequired(c => c.Series);
        }

    }
}
