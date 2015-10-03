SELECT 
                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], e.[DownloadStatus] AS [DownloadStatus], 
                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
                WHERE (DATEDIFF(day, e.[FirstAired], SysDateTime())) >= 0 AND N'WANT' = e.[DownloadStatus] ORDER BY e.[FirstAired] ASC

--ALL Wanted Downloads Without Time Constraint

--SELECT 
--                e.[Id] AS [Id],s.[Name] AS [SeriesName], e.[Title] AS [Title], e.[Season] AS [Season], e.[EpisodeNumber] AS [EpisodeNumber], 
--                e.[FileName] AS [FileName], e.[Summary] AS [Summary], e.[FirstAired] AS [FirstAired], e.[DownloadStatus] AS [DownloadStatus], 
--                e.[SeriesId] AS [SeriesId] FROM [dbo].[TvEpisodes] AS e	INNER JOIN [dbo].[TvSeries] s ON s.Id = e.SeriesId
--                WHERE  N'WANT' = e.[DownloadStatus] ORDER BY e.[FirstAired] ASC