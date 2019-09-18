using System.Collections.Generic;
using System.Linq;

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

        public override bool Equals(object obj)
        {
            var other = obj as LibraryUser;
            return other != null
                && (other.BooksCheckedOut == null && BooksCheckedOut == null || other.BooksCheckedOut.SequenceEqual(BooksCheckedOut))
                && other.Id == Id
                && other.Name == Name
                && (other.Roles == null && Roles == null || other.Roles.SequenceEqual(Roles))
                && other.Username == Username;
        }
    }
}
