--create sequence for numbering posts on the board 
CREATE FUNCTION public.create_num_sequence()
RETURNS trigger AS $$
DECLARE
 seq_name text := NEW."Key" || '_nums_seq';
BEGIN
    EXECUTE format('CREATE SEQUENCE IF NOT EXISTS public.%I', seq_name);
    RETURN NEW;
END; 
$$ LANGUAGE plpgsql;

CREATE TRIGGER create_num_sequence
AFTER INSERT ON public."Boards"
FOR EACH ROW
EXECUTE FUNCTION create_num_sequence();

------------------------------------------------------

--drop sequence for numbering posts on the board 
CREATE FUNCTION public.drop_num_sequence()
RETURNS trigger AS $$
DECLARE
 seq_name text := OLD."Key" || '_nums_seq';
BEGIN
    EXECUTE format('DROP SEQUENCE IF EXISTS public.%I', seq_name);
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER drop_num_sequence
AFTER DELETE ON public."Boards"
FOR EACH ROW
EXECUTE FUNCTION drop_num_sequence();

------------------------------------------------------

--rename the sequence if the board has renamed
CREATE FUNCTION public.rename_num_sequence()
RETURNS TRIGGER
AS $$
DECLARE
	new_seq text  :=  NEW."Key" || '_nums_seq';
	old_seq text  :=  OLD."Key" || '_nums_seq';
BEGIN
	IF (OLD."Key" <> NEW."Key") THEN 
		EXECUTE format('ALTER SEQUENCE public.%I RENAME TO %I', old_seq, new_seq );
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER rename_num_sequence
AFTER UPDATE ON public."Boards"
FOR EACH ROW
EXECUTE FUNCTION rename_num_sequence();

------------------------------------------------------

--delete all sequences
CREATE FUNCTION public.drop_all_sequences()
RETURNS TRIGGER
AS $$
DECLARE 
	b_key TEXT;
BEGIN
    FOR b_key IN
        SELECT b."Key"
        FROM public."Boards" b
    LOOP
        EXECUTE format('DROP SEQUENCE IF EXISTS public.%I', b_key || '_nums_seq');
    END LOOP;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER delete_num_sequences_on_truncate
BEFORE TRUNCATE ON public."Boards"
EXECUTE FUNCTION public.drop_all_sequences();

------------------------------------------------------

--set post number on the board to Posts.Number
CREATE FUNCTION public.set_post_number()
RETURNS TRIGGER AS $$
BEGIN
 NEW."Number" = nextval(
  (SELECT t."BoardKey"
  FROM public."Threads" t
        WHERE t."Id" = NEW."ThreadId") || '_nums_seq');
 RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_post_number
BEFORE INSERT ON public."Posts"
FOR EACH ROW
EXECUTE FUNCTION set_post_number();

------------------------------------------------------

--set thread last bump
--last bump should be updated if the new post has no SAGE and thread not in bump limit
CREATE FUNCTION public.update_thread_columns()
RETURNS TRIGGER AS $$
BEGIN
    IF(TG_OP = 'INSERT') THEN
        --increace post count and set lasthit
        UPDATE "Threads"
        SET
        	"PostsCount" = "PostsCount" + 1,
        	"LastHit" = NEW."Created"
        WHERE "Id" = NEW."ThreadId";
        --set last bump
        IF (NOT NEW."IsSaged"
            AND(SELECT
                (SELECT count(*) 
                FROM "Posts" p
                WHERE p."ThreadId" = NEW."ThreadId") <= b."BumpLimit" 
            FROM public."Boards" b
            WHERE b."Key" = (SELECT t."BoardKey" FROM "Threads" t WHERE t."Id" = NEW."ThreadId"))) 
        THEN
            UPDATE "Threads"
            SET "LastBump" = NEW."Created"
            WHERE "Id" = NEW."ThreadId";
        END IF;
        RETURN NEW;
    ELSIF (TG_OP = 'DELETE') THEN
        UPDATE "Threads"
        SET"PostsCount" = "PostsCount" - 1
        WHERE "Id" = NEW."ThreadId";
       	RETURN OLD;
    END IF;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_threads
AFTER INSERT OR DELETE ON public."Posts"
FOR EACH ROW
EXECUTE FUNCTION public.update_thread_columns();

------------------------------------------------------

CREATE OR REPLACE FUNCTION public.update_posts_with_attachmentns_count()
RETURNS TRIGGER AS $$
BEGIN
    IF(TG_OP = 'INSERT') THEN 
    	IF(NOT EXISTS (SELECT 1 FROM "Attachment" a WHERE a."PostId" = NEW."PostId")) THEN
        	UPDATE "Threads"
	        SET "PostsWithAttachmentnsCount" = "PostsWithAttachmentnsCount" + 1
    	    WHERE "Id" = (
        	    SELECT p."ThreadId"
            	FROM "Posts" p 
	            WHERE p."Id" = NEW."PostId"
    	        );
       	END IF;
     	RETURN NEW;
     ELSIF(TG_OP = 'DELETE') THEN
		IF (SELECT count(*) = 1 FROM "Attachment" a WHERE a."PostId" = OLD."PostId" LIMIT 2) THEN
			UPDATE "Threads"
	    	SET "PostsWithAttachmentnsCount" = "PostsWithAttachmentnsCount" - 1
			WHERE "Id" = (
        		SELECT p."ThreadId"
            	FROM "Posts" p 
	        	WHERE p."Id" = OLD."PostId");
	    END IF;
     	RETURN OLD;
     END IF;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER attachment_insert_or_delete
BEFORE INSERT OR DELETE ON public."Attachment"
FOR EACH ROW
EXECUTE FUNCTION public.update_posts_with_attachmentns_count();

------------------------------------------------------
