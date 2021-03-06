﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TvTamer.Core.Models
{
    public class TvSeries
    {

        public int Id { get; set; }
        public string TvDbSeriesId { get; set; }
        public string Name { get; set; }
        public DateTime FirstAired { get; set; }
        public DayOfWeek AirsDayOfWeek { get; set; }
        public string AirsTimeOfDay { get; set; }
        public string Network { get; set; }
        public string Summary { get; set; }
        public string Status { get; set; }
        public string Rating { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<TvEpisode> Episodes { get; set; } = new List<TvEpisode>();
        public List<string> Genres { get; set; }

        public List<AlternateName> AlternateNames { get; set; }

    }

    public class TvSeriesMap : EntityTypeConfiguration<TvSeries>
    {

        public TvSeriesMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.TvDbSeriesId).HasMaxLength(50).IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,new IndexAnnotation(new IndexAttribute("IX_TvDbSeriesId", 1) { IsUnique = true }));
            this.Property(t => t.Name).HasMaxLength(255).IsRequired();
            this.Property(t => t.FirstAired).IsRequired();
            this.Property(t => t.LastUpdated).IsRequired();

            HasMany(t => t.Episodes);
            HasMany(t => t.AlternateNames).WithOptional().Map( t => t.MapKey("SeriesId"));
        }

    }


}
