using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class GithubUser
    {
        public string GitHubAvatar { get; }

        public string GitHubLogin { get; }

        public string GitHubName { get; }

        public string GitHubUrl { get; }

        public bool CanEdit { get; }

        public bool CanDelete { get; }

        public bool CanCheckOut { get; }

        public IReadOnlyCollection<Book> BooksCheckedOut { get; }

        public GithubUser(string avatar, string login, string name, string url, bool canEdit, bool canDelete, bool canCheckOut, IReadOnlyCollection<Book> booksCheckedOut)
        {
            GitHubAvatar = avatar;
            GitHubLogin = login;
            GitHubName = name;
            GitHubUrl = url;
            CanEdit = canEdit;
            CanDelete = canDelete;
            CanCheckOut = canCheckOut;
            BooksCheckedOut = booksCheckedOut;
        }
    }
}
