# coredb
A foray into finding out what it takes to make a database. Unabashedly different with a new take on a query language.

An immediate need exists for someone to initiate and maintain the wiki. All core information will be somewhat organized in the documentation folder.


# StorageEngine
The first part to research is the storage engine. Studies of MySQL have multiple storage engines - MyISAM, Federated and InnoDB. SQL Server has a very complex system that minimizes the number of files created.

My initial thought had been a primary file with database information. Databases might be organized by directory so that a 'Customers' database would exist under a 'Customers' directory. Several files may exist 
such as a database schema, table files, indexes, etc. The exact nature and organization of the file system is yet undetermined. I am looking to design a hybrid between the MySQL default _file per table_ and the 
default SQL Server _all in one_ architectures.

Requirements:
* [Data generation] tools to create tons of data.  
* Directory structure
* File architecture

## Directory Structure  
Structuring the directories and placement of files seems simple. Studying several options I am beginning with this general approach:
<p align="center">
    <img src="https://github.com/razorware/coredb/blob/master/images/directory_structure.png"
         alt="directory structure"
         title="coreDb Directory Structure" />
</p>

## File Architecture  
In the proposed directory structure, there are a minimum of 2 files:
<table style="margin: 0px auto;">
   <tr>
      <td>
         <img src="https://github.com/razorware/coredb/blob/master/images/database_file_format.png"
               alt=".db file structure"
               title="Database .db File Structure" />
      </td>
      <td align="center">
         .dat file structure
         <br>
         image to be created
      </td>
   </tr>
</table> 

More on [file structure here].



[Data generation]: https://github.com/razorware/coredb/blob/master/documentation/FakeData.md
[file structure here]: https://github.com/razorware/coredb/blob/master/documentation/FileStructure.md