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
<p align="center">
   <table align="center">
      <tr>
         <td>
            <img src="https://github.com/razorware/coredb/blob/master/images/database_file_format.png"
                 alt=".db file structure"
                 title="Database .db File Structure" />
         </td>
         <td>
            .dat file structre
            <br>
            <br>
            image to be created and inserted
         </td>
      </tr>
   </table>
</p>
* .db
   * header (currently 128 bytes)
      * status
      * describes the file structure (file-per-table or master-file)
      * date saved
      * date modified
      * file and page specifications
         * page size
         * number of pages per file
      * page indexing
         * page type
   * pages
      * table schemas
      * other database information - security, permissions, access, roles, etc. - as determined

* .dat
   * table data
   * table indexes
   * foreign keys
   * other table specific information as determined



[Data generation]: https://github.com/razorware/coredb/blob/master/documentation/FakeData.md