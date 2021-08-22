CREATE VIEW public.last_created_threads_per_board
AS 
SELECT t.* 
FROM "Boards" b
JOIN LATERAL (
	SELECT *
	FROM "Threads" t
	WHERE t."BoardKey" = b."Key"
		AND NOT t."IsReadOnly"
		AND NOT t."IsPinned" 
		AND t."PostsCount" > 0 
	ORDER BY t."Created" DESC 
	LIMIT 1) AS t ON b."Key" = t."BoardKey";


CREATE VIEW public.last_bumped_threads_per_board
AS 
SELECT t.* 
FROM "Boards" b
JOIN LATERAL (
	SELECT *
	FROM "Threads" t
	WHERE t."BoardKey" = b."Key"
		AND NOT t."IsReadOnly"
		AND NOT t."IsPinned" 
		AND t."PostsCount" > 1 
	ORDER BY t."LastBump" DESC 
	LIMIT 1) AS t ON b."Key" = t."BoardKey";