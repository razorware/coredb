
* .db
   * header (currently 128 bytes)  
      * status flag
      * format: file-per-table or master-file
      * date modified
      * date saved
      * specifications
         * page size
         * number of pages
      * page indexing
         * page type
         * page count
   * pages
      * master page (required page every .db file)
         * .dat file name(s)
         * table schemas
      * other database information: security, permissions, access, roles (as determined)

* .dat
   * table data
   * table indexes
   * foreign keys
   * other table specific information (as determined)