using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TvTamer.Core.Models;

namespace TvTamer.Web.Models
{
    public class CalendarViewModel
    {
        public IDictionary<DateTime, List<TvEpisode>> Episodes { get; set; } = new Dictionary<DateTime, List<TvEpisode>>();

    }
}
