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
  * To interact with most of the API you will need an approriate role on your user, which is managed in appsettings.json currently
* Set the correct ClientId and ClientSecret in appsettings.json
* Run `dotnet run` in the root directory
  * Run `dotnet library.dll` in the publish output directory for a production version
* Navigate to `http://localhost:<port>` or `https://localhost:<secure port>`

There is currently a version running at https://jamesekstrom.com/library/

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
  * Domain events in memory to drive behavior
  * Integration events on message bus to allow scaling and for integrating apps
* Unit tests
* Containerize
  * Host in kubernetes to manage scaling and network 
* Better authentication and authorization management
  * Make an admin interface for role management for users
  * Add additional authentication methods such as basic and Google
* Polish up UI
  * Use more colors - it is pretty drab right now
  * Add interface for each user for the books they have checked out ?
  * Add book detail page
  * Admins should be able to view who has the book checked out
* Use a centralized database over network to allow scaling
  * Possibly nosql db if reads are heavy
* Re-align domain models with DDD and CQRS patterns
  * Make an aggregate root entity (Book), and any related value objects
* Add event based logging (like datadog or ELK) or fluentd if in kubernetes
* Add fault tolerance to API calls
  * Retry mechanisms
  * Circuit breakers
* Add API endpoint versioning
* Add hyperlinks for REST responses to make it fully representational

#### Events
* BookCreated
* BookChanged
* BookDeleted
* BookCheckedIn
* BookCheckedOut
