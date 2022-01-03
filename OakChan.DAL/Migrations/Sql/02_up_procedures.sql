CREATE PROCEDURE public.delete_posts(ids integer[])
 LANGUAGE plpgsql
AS $procedure$
	BEGIN
		DELETE FROM public."Posts" p WHERE p."Id" = ANY(ids);
	END;
$procedure$;

CREATE PROCEDURE public.delete_threads(ids integer[])
 LANGUAGE plpgsql
AS $procedure$
	BEGIN
		DELETE FROM public."Threads" t WHERE t."Id" = ANY(ids);
	END;
$procedure$;