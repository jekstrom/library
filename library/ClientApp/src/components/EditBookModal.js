import React, { Component } from 'react';
import { Button, Modal, FormGroup, FormControl, ControlLabel } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class EditBookModal extends Component {
  constructor(props) {
    super(props);

    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);

    this.state = {
      show: false,
      id: this.props.book.id,
      title: this.props.book.title,
      author: this.props.book.author,
      isbn: this.props.book.isbn,
      description: this.props.book.description,
      checkedOut: this.props.book.checkedOut
    };
  }

  setShow(show) {
    this.setState({show:show});
  }

  handleSubmit(e) {
    e.preventDefault();
    fetch('/api/book/' + this.state.id, {
      method: 'PUT',
      body: JSON.stringify({
        Title: this.state.title,
        Author: this.state.author,
        ISBN: this.state.isbn,
        Description: this.state.description,
        CheckedOut: this.state.checkedOut
      }),
      headers: {
        'Content-Type': 'application/json'
      }
    }).then(response => {
      response.json().then(d => this.props.onUpdatedBook(d)); 
      this.setState({show: false})
    });
  }

  getValidationState(field) {
    const length = this.state[field].length;
    if (length > 0) {
      return 'success';
    }
    else {
      return 'error';
    }
  }

  handleChange(key) {
    return function(e) {
      var state = {};
      state[key] = e.target.value;
      this.setState(state);
    }.bind(this);
  }

  render() {
    const handleClose = () => this.setShow(false);
    const handleShow = () => this.setShow(true);
    return (
      <div>
        <Button variant="primary" onClick={handleShow}>
          <FontAwesomeIcon icon="edit"/> Edit
        </Button>

        <Modal show={this.state.show} onHide={handleClose} animation={false}>
          <Modal.Header closeButton>
            <Modal.Title>{this.state.title}</Modal.Title>
          </Modal.Header>
          <Modal.Body>
          <form>
            <FormGroup
              controlId="bookFormTitle"
              validationState={this.getValidationState('title')}
            >
              <ControlLabel>Title</ControlLabel>
              <FormControl
                type="text"
                value={this.state.title}
                placeholder="Book title"
                onChange={this.handleChange("title")}
              />
              <FormControl.Feedback />
            </FormGroup>
            <FormGroup
              controlId="bookFormAuthor"
              validationState={this.getValidationState('author')}
            >
              <ControlLabel>Author</ControlLabel>
              <FormControl
                type="text"
                value={this.state.author}
                placeholder="Book Author"
                onChange={this.handleChange("author")}
              />
              <FormControl.Feedback />
            </FormGroup>
            <FormGroup
              controlId="bookFormISBN"
              validationState={this.getValidationState('isbn')}
            >
              <ControlLabel>ISBN</ControlLabel>
              <FormControl
                type="text"
                value={this.state.isbn}
                placeholder="Book ISBN"
                onChange={this.handleChange("isbn")}
              />
              <FormControl.Feedback />
            </FormGroup>
            <FormGroup controlId="bookFormDescription">
              <ControlLabel>Description</ControlLabel>
              <FormControl
                componentClass="textarea"
                value={this.state.description}
                placeholder="Book Description"
                onChange={this.handleChange("description")}
              />
              <FormControl.Feedback />
            </FormGroup>
          </form>
          </Modal.Body>
          <Modal.Footer>
            <Button variant="secondary" onClick={handleClose}>
              Close
            </Button>
            <Button variant="primary" onClick={this.handleSubmit}>
              Save Changes
            </Button>
          </Modal.Footer>
        </Modal>
      </div>
    );
  }
}