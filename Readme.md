#Simple.Data.SqlAnywhere
Simple.Data.SqlAnywhere is an ADO provider for the Simple.Data framework by Mark Rendle.
http://github.com/markrendle/Simple.Data

##Supported Sql Anywhere version
* SQL Anywhere 12.0.1 - all tests passing.
* SQL Anywhere 11.0.1 - all tests passing.
* SQL Anywhere 10.0.0 - all tests passing.
* SQL Anywhere 9.0.2 - all tests passing.
* SQL Anywhere 8.0.3 - all tests passing.

##Genesis
The initial code & test assemblies were taken from the SQL Server code & test assemblies and therefore it should be pretty easy to transfer skills from SQL Server to SQL Anywhere.

##Optimizations
To maintain feature parity with Simple.Data.SqlServer a number of features are supported in sub-optimal ways such as paging on 8.0.3 & bulk inserts before 10.0.0.

##Requirements
iAnywhere.Data.SQLAnywhere.v4.0.dll version 12.0.1 from the SQL Anywhere installation - http://www.sybase.com/products/databasemanagement/sqlanywhere
