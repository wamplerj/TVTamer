/****** Script for SelectTopNRows command from SSMS  ******/
INSERT INTO [DevTvTamer].[dbo].TvSeries
(
	   [SeriesId]
      ,[Name]
      ,[FirstAired]
      ,[AirsDayOfWeek]
      ,[AirsTimeOfDay]
      ,[Network]
      ,[Summary]
      ,[Status]
      ,[Rating]
	  ,LastUpdated
)
SELECT[SeriesId]
      ,[Name]
      ,[FirstAired]
      ,[AirsDayOfWeek]
      ,[AirsTimeOfDay]
      ,[Network]
      ,[Summary]
      ,[Status]
      ,[Rating]
	  ,'4/1/2015 12:00:00 AM'
  FROM [TvTamer].[dbo].[TvSeries]