# Library application

* Create, read update and delete books
* Update title, author ISBN, and description using a web interface
* Check in and out a book
* Track state changes for a book
* Generate a report that displays the current status of all books

## Building
* Download [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core/2.1) if you do not have it 
* Download and install nodejs and npm
* Run `dotnet build` in the root directory
  * Run `dotnet publish -c Release` to build a production version

## Running
* If you want to log in with GitHub credentials, you will need to [create a GitHub OAuth application](https://developer.github.com/apps/building-oauth-apps/creating-an-oauth-app/)
* Set the correct ClientId and ClientSecret in appsettings.json
* Run `dotnet run` in the root directory
  * Run `dotnet library.dll` in the publish output directory for a production version
* Navigate to `http://localhost:<port>` or `https://localhost:<secure port>`

## Architecture

### Models
* Book
  * Domain model for book
  * Manages valid state transitions
* LibraryUser
  * Domain specific user DTO
* GitHubUser
  * OAuth user

### Technology
* dotnet and react for building
* SQLite for persistence

### TODO
* State change events
* Unit tests
* Containerize

#### Events
* BookCreated
* BookChanged
* BookDeleted
* BookCheckedIn
* BookCheckedOut
