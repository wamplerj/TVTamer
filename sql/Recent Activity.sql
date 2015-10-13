ALTER PROCEDURE dbo.GetRecentActivity
AS
BEGIN

	CREATE TABLE #RecentActvity
	(
		SearchAttempts int,
		SearchFailures int,
		Downloads int,
		ProcessedEpisodes int,
		EventDay DateTime NOT NULL
	)

	CREATE TABLE #Events
	(
		EventCount int NOT NULL,
		EventDay DateTime NOT NULL
	)

	INSERT INTO #RecentActvity (SearchAttempts, EventDay)
	SELECT Count(EventType), dateadd(DAY,0, datediff(day,0, EventTime)) As EventDay FROM LoggedEvents
	WHERE DATEDIFF(DAY,EventTime,GETDATE()) <= 7 AND EventType = 'SEARCH'
	GROUP BY dateadd(DAY,0, datediff(day,0, EventTime))

	INSERT INTO #Events (EventCount, EventDay)
	SELECT Count(EventType), dateadd(DAY,0, datediff(day,0, EventTime)) As EventDay FROM LoggedEvents
	WHERE DATEDIFF(DAY,EventTime,GETDATE()) <= 7 AND EventType = 'SEARCHFAILED'
	GROUP BY dateadd(DAY,0, datediff(day,0, EventTime))

	UPDATE #RecentActvity
	SET SearchFailures = EventCount
	FROM #Events e
	INNER JOIN #RecentActvity r
	ON r.EventDay = e.EventDay

	TRUNCATE TABLE #Events

	INSERT INTO #Events (EventCount, EventDay)
	SELECT Count(EventType), dateadd(DAY,0, datediff(day,0, EventTime)) As EventDay FROM LoggedEvents
	WHERE DATEDIFF(DAY,EventTime,GETDATE()) <= 7 AND EventType = 'EPISODE'
	GROUP BY dateadd(DAY,0, datediff(day,0, EventTime))

	UPDATE #RecentActvity
	SET ProcessedEpisodes = EventCount
	FROM #Events e
	INNER JOIN #RecentActvity r
	ON r.EventDay = e.EventDay

	TRUNCATE TABLE #Events

	INSERT INTO #Events (EventCount, EventDay)
	SELECT Count(EventType), dateadd(DAY,0, datediff(day,0, EventTime)) As EventDay FROM LoggedEvents
	WHERE DATEDIFF(DAY,EventTime,GETDATE()) <= 7 AND EventType = 'DOWNLOAD'
	GROUP BY dateadd(DAY,0, datediff(day,0, EventTime))

	UPDATE #RecentActvity
	SET Downloads = EventCount
	FROM #Events e
	INNER JOIN #RecentActvity r
	ON r.EventDay = e.EventDay

	TRUNCATE TABLE #Events

	SELECT * FROM #RecentActvity ORDER BY EventDay

	DROP TABLE #Events
	DROP TABLE #RecentActvity
END
