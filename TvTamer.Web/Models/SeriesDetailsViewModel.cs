using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using TvTamer.Core.Models;
using TvTamer.Web.Controllers;

namespace TvTamer.Web.Models
{
    public class SeriesDetailsViewModel
    {
        public TvSeries Series { get; set; }
        public List<int> Seasons { get; set; }
    }
}