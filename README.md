# coredb
A foray into finding out what it takes to make a database. Unabashedly different with a new take on a query language.

# StorageEngine
The first part to research is the storage engine. Studies of MySQL have multiple storage engines - MyISAM, Federated and InnoDB. SQL Server has a very complex system that minimizes the number of files created.

My initial thought had been a primary file with database information. Databases might be organized by directory so that a 'Customers' database would exist under a 'Customers' directory. Several files may exist 
such as a database schema, table files, indexes, etc. The exact nature and organization of the file system is yet undetermined. I am looking to design a hybrid between the MySQL default _file per table_ and the 
default SQL Server _all in one_ architectures.

Requirements:
* [Data generation] tools to create tons of data.  
* Directory structure
* File architecture

# Directory Structure  
Structuring the directories and placement of files is simple. Studying several options I am going with this approach:
<p align="center">
    <img src="https://github.com/razorware/coredb/blob/master/images/directory_structure.png"
         alt="directory structure"
         title="CoreDB Directory Structure" />
</p>

[Data generation]: https://github.com/razorware/coredb/master/documentation/FakeData.md