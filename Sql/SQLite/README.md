# SQLite SQL files

Place SQLite-specific `.sql` files in this folder. At runtime this folder is resolved relative to the application executable as `Sql\SQLite`.

SQL files are read from disk for each operation. Replacing a file takes effect on the next operation; restarting the application is not required. An operation already in progress continues using the SQL text it has already loaded.

Use named parameters such as `@userId` in SQLite SQL. Supply their values through the SQL executor parameter dictionary. Do not concatenate user input into SQL text.
