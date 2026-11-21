# Oracle SQL files

Place Oracle-specific `.sql` files in this folder. At runtime this folder is resolved relative to the application executable as `Sql\Oracle`.

SQL files are read from disk for each operation. Replacing a file takes effect on the next operation; restarting the application is not required. An operation already in progress continues using the SQL text it has already loaded.

Use named bind parameters in Oracle SQL and provide their values through the SQL executor parameter dictionary. Do not concatenate user input into SQL text. Oracle command binding will be implemented with the Oracle provider when Oracle connectivity is enabled.
