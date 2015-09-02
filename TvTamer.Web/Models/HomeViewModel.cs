﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TvTamer.Core.Models;

namespace TvTamer.Web.Models
{
    public class HomeViewModel
    {
        public List<TvEpisode> TodaysEpisodes { get; set; } = new List<TvEpisode>();
    }
}