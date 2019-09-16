# Library application

* Create, read update and delete books
* Update title, author ISBN, and description using a web interface
* Check in and out a book
* Track state changes for a book
* Generate a report that displays the current status of all books

## Architecture

### Models
* Book
* LibraryUser
* Report
* ReportUser

### Events
* BookCreated
* BookChanged
* BookDeleted
* BookCheckedIn
* BookCheckedOut

### TODO
* Add users table with roles
* Add authorization
* Generate nav based on current user's authorization
* Add book management pages
* Add book report page
* State changes
* Unit tests