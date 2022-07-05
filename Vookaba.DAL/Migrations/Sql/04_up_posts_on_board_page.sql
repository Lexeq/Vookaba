CREATE OR REPLACE FUNCTION posts_on_board_page(threadIds integer[], repCount integer)
  RETURNS SETOF "Posts"
  LANGUAGE sql
  AS $$
    SELECT p.*
    FROM "Threads" t,
    LATERAL(
    	SELECT *
        FROM "Posts" p0
        WHERE p0."IsOP" and p0."ThreadId" = t."Id"
        UNION(
            SELECT *
            FROM "Posts" p1
            WHERE p1."ThreadId" = t."Id"
            ORDER BY p1."Number" DESC
            LIMIT $2)
        ) AS p
	WHERE t."Id" = ANY($1);
$$ ROWS 40;