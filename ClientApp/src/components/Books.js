import React, { Component } from 'react';
import { BookModal } from "./BookModal"

export class Books extends Component {
  displayName = Books.name;

  constructor(props) {
    super(props);
    this.state = { books: [], loading: true };
    this.addNewBook = this.addNewBook.bind(this);

    fetch('api/Book')
      .then(response => response.json())
      .then(data => {
        this.setState({ books: data, loading: false });
      });
  }

  static renderBooksTable(books) {
    return (
      <table className='table'>
        <thead>
          <tr>
            <th>Title</th>
            <th>Author</th>
            <th>ISBN</th>
            <th>Description</th>
            <th>Checked Out</th>
          </tr>
        </thead>
        <tbody>
          {books.map(book =>
            <tr key={book.title}>
              <td>{book.author}</td>
              <td>{book.isbn}</td>
              <td>{book.description}</td>
              <td>{!book.checkedIn}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  addNewBook(data) {
    var newBook = {
      title: data.title,
      author: data.author,
      isbn: data.isbn,
      description: data.description,
      checkedIn: !data.checkedOut
    }
    this.setState({books: this.state.books.concat([newBook])})
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Books.renderBooksTable(this.state.books);

    return (
      <div>
        <h1>Books</h1>

        {contents}

        <div>
          <BookModal buttonText="Add new book" onNewBook={this.addNewBook}/>
        </div>
      </div>
    );
  }
}
