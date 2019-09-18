using System.Collections.Generic;

namespace Domain.Models
{
    public class LibraryUser : IDomainModel
    {
        public int Id { get; }
        public string Name { get; }
        public string Username { get; }
        public IReadOnlyCollection<string> Roles { get; }
        public IReadOnlyCollection<Book> BooksCheckedOut { get; }

        public LibraryUser(int id, string name, string username, IReadOnlyCollection<string> roles, IReadOnlyCollection<Book> booksCheckedOut)
        {
            Name = name;
            Username = username;
            Roles = roles;
            BooksCheckedOut = booksCheckedOut;
        }

        public LibraryUser(int id, string name, string username, IReadOnlyCollection<Book> booksCheckedOut)
        {
            Name = name;
            Username = username;
            BooksCheckedOut = booksCheckedOut;
        }
    }
}
