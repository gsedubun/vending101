To run the application.
1. Create new database in sql server. set the name to whatever you want, be sure to update connection string appropriately.
2. Run generatedb.sql to create all the tables and it's values.
3. Change connectionstring in VendingData/Models/AppSettings.cs to connect the console app to database.