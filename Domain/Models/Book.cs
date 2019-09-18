using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Book : IDomainModel
    {
        public int Id { get; }
        public string Title { get; }
        public string Author { get; }
        public string ISBN { get; }
        public string Description { get; }
        public bool CheckedOut { get; private set; }
        public DateTime Created { get; }
        public DateTime Updated { get; }
        public string CreatedBy { get; private set; }
        public string CheckedOutBy { get; private set; }


        public Book(int id, string title, string author, string isbn, string description)
        {
            Id = id;
            Title = title;
            Author = author;
            ISBN = isbn;
            Description = description;
        }

        public Book(int id, string title, string author, string isbn, string description, bool checkedOut, DateTime created, DateTime updated, string createdBy, string checkedOutBy)
        {
            Id = id;
            Title = title;
            Author = author;
            ISBN = isbn;
            Description = description;
            CheckedOut = checkedOut;
            Created = created;
            Updated = updated;
            CreatedBy = createdBy;
            CheckedOutBy = checkedOutBy;
        }

        public void SetCreatedBy(string username)
        {
            CreatedBy = username;
        }

        public void ResetSetCheckedOutBy(string username)
        {
            if (CheckedOutBy != username)
            {
                throw new Exception($"User {username} cannot check in book as it is checked out by another user.");
            }
            CheckedOutBy = null;
        }

        public Book CheckOut(string username)
        {
            if (CheckedOut)
            {
                throw new Exception("Cannot check out already checked out book.");
            }
            CheckedOut = true;
            CheckedOutBy = username;
            return this;
        }

        public Book CheckIn(string username)
        {
            if (!CheckedOut)
            {
                throw new Exception("Cannot check int already checked in book.");
            }

            if (!CheckedOutBy.Equals(username))
            {
                // TODO: make explicit exception types for domain state validation.
                throw new Exception($"{username} cannot check this book in as it is currently checked out by {CheckedOutBy}.");
            }

            CheckedOut = false;
            return this;
        }

        public override bool Equals(object obj)
        {
            Book other = obj as Book;
            return other != null
                && other.Author == Author
                && other.CheckedOut == CheckedOut
                && other.CheckedOutBy == CheckedOutBy
                && other.Created == Created
                && other.CreatedBy == CreatedBy
                && other.Description == Description
                && other.Id == Id
                && other.ISBN == ISBN
                && other.Title == Title
                && other.Updated == Updated;
        }
    }
}
