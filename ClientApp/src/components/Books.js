import React, { Component } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button } from 'react-bootstrap';
import { BookModal } from "./BookModal"
import { EditBookModal } from './EditBookModal';

export class Books extends Component {
  displayName = Books.name;

  constructor(props) {
    super(props);
    this.state = { books: [], userdata: [], loading: true };
    this.addNewBook = this.addNewBook.bind(this);
    this.onUpdatedBook = this.onUpdatedBook.bind(this);
    this.deleteBook = this.deleteBook.bind(this);
    this.checkoutBook = this.checkoutBook.bind(this);

    fetch('api/Login/GetGithubUserData')
    .then(response => response.json())
    .then(data => {
      this.setState({ userdata: data, loading: false });
    });

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
            {this.state.userdata.canEdit ? <th></th> : null}
            <th>Title</th>
            <th>Author</th>
            <th>ISBN</th>
            <th>Description</th>
            <th>Checked Out</th>
            {this.state.userdata.canEdit ? <th></th> : null}
          </tr>
        </thead>
        <tbody>
          {books.map(book =>
            <tr key={book.title}>
              {this.state.userdata.canEdit ? <td><EditBookModal book={book} onUpdatedBook={this.onUpdatedBook}/></td> : null}
              <td>{book.title}</td>
              <td>{book.author}</td>
              <td>{book.isbn}</td>
              <td>{book.description}</td>
              <td>{book.checkedOut ? 'yes' : this.state.userdata.canCheckOut ? <Button variant="info" onClick={this.checkoutBook(book.id)}>Check Out</Button> : null} </td>
              {this.state.userdata.canDelete ? <td><Button variant='warning' onClick={this.deleteBook(book.id)}><FontAwesomeIcon icon='trash' /> Delete</Button></td> : null}
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

  deleteBook(id) {
    var self = this;
    return function(e) {
      e.preventDefault();
      fetch('/api/book/' + id, {
        method: 'DELETE'
      }).then(response => {
        if (response) {
          self.setState({books: self.state.books.filter(b => b.id !== id)})
        }
      });
    }
  }

  checkoutBook(id) {
    var self = this;
    return function(e) {
      e.preventDefault();
      fetch('/api/book/' + id + '/checkout', {
        method: 'POST'
      }).then(response => {
        if (response) {
          var checkedOutBook = self.state.books.find(b => b.id === id);
          checkedOutBook.checkedOut = true;
          self.setState({books: self.state.books.filter(b => b.id !== id).concat([checkedOutBook])})
        }
      });
    }
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
