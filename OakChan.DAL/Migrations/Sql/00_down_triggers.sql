
DROP TRIGGER IF EXISTS ON public."Boards" create_num_sequence CASCADE;
DROP FUNCTION IF EXISTS public.create_num_sequence() CASCADE;

DROP TRIGGER IF EXISTS ON public."Boards" drop_num_sequence CASCADE;
DROP FUNCTION IF EXISTS public.drop_num_sequence() CASCADE;

DROP TRIGGER IF EXISTS ON public."Boards" rename_num_sequence CASCADE;
DROP FUNCTION IF EXISTS public.rename_num_sequence() CASCADE;

DROP TRIGGER IF EXISTS ON public."Boards" delete_num_sequences_on_truncate CASCADE;
DROP FUNCTION IF EXISTS public.drop_all_sequences() CASCADE;

DROP TRIGGER IF EXISTS ON public."Posts" set_post_number CASCADE;
DROP FUNCTION IF EXISTS public.set_post_number() CASCADE;

DROP TRIGGER IF EXISTS ON public."Posts" update_threads CASCADE;
DROP FUNCTION IF EXISTS public.update_thread_columns() CASCADE;

DROP TRIGGER IF EXISTS ON public."Attachment" attachment_insert_or_delete CASCADE;
DROP FUNCTION IF EXISTS public.update_posts_with_attachmentns_count() CASCADE;

