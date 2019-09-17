import React, { Component } from 'react';
import { BookModal } from "./BookModal"
import { EditBookModal } from './EditBookModal';

export class Books extends Component {
  displayName = Books.name;

  constructor(props) {
    super(props);
    this.state = { books: [], loading: true };
    this.addNewBook = this.addNewBook.bind(this);
    this.onUpdatedBook = this.onUpdatedBook.bind(this);

    fetch('api/Book')
      .then(response => response.json())
      .then(data => {
        this.setState({ books: data, loading: false });
      });
  }

  renderBooksTable(books) {
    return (
      <table className='table'>
        <thead>
          <tr>
            <th></th>
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
              <td><EditBookModal book={book} onUpdatedBook={this.onUpdatedBook}/></td>
              <td>{book.title}</td>
              <td>{book.author}</td>
              <td>{book.isbn}</td>
              <td>{book.description}</td>
              <td>{book.checkedOut ? 'yes' : 'no'}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  addNewBook(data) {
    var newBook = {
      id: data.id,
      title: data.title,
      author: data.author,
      isbn: data.isbn,
      description: data.description,
      checkedOut: data.checkedOut
    }
    this.setState({books: this.state.books.concat([newBook])})
  }

  onUpdatedBook(data) {
    var updatedBook = {
      title: data.title,
      author: data.author,
      isbn: data.isbn,
      description: data.description,
      checkedOut: data.checkedOut
    }
    this.setState({books: this.state.books.filter(b => b.id !== data.id).concat([updatedBook])})
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : this.renderBooksTable(this.state.books);

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
