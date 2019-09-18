import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Nav, Navbar, NavItem, Image } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { LinkContainer } from 'react-router-bootstrap';
import './NavMenu.css';

var userStyle = {
  color: 'white',
  float: 'right',
  margin: '10px'
};

export class NavMenu extends Component {
  displayName = NavMenu.name

  constructor(props) {
    super (props);
    this.state = { userdata: [], loading: true };

    fetch('api/Login/GetGithubUserData')
      .then(response => response.json())
      .then(data => {
        this.setState({ userdata: data, loading: false });
      });
  }

  static renderUserData(userdata) {
    return (
      <div style={userStyle}>
        <a href={userdata.gitHubUrl} target="_blank">
          <Image src={userdata.gitHubAvatar} rounded width="30" height="30"/> {userdata.gitHubName}
        </a>
      </div>
    );
  }

  render() {
    let contents = this.state.loading
      ? <div></div>
      : NavMenu.renderUserData(this.state.userdata);

    return (
      <Navbar inverse fixedTop fluid collapseOnSelect>
        <Navbar.Header>
          <Navbar.Brand>
            <div>
              <Link to={'/'}>library</Link>
            </div>
          </Navbar.Brand>
          {contents}
        </Navbar.Header>
        <Navbar.Collapse>
          <Nav>
            <LinkContainer to={'/'} exact>
              <NavItem>
                <FontAwesomeIcon icon='home' /> Home
              </NavItem>
            </LinkContainer>
            <LinkContainer to={'/books'}>
              <NavItem>
                <FontAwesomeIcon icon='book' /> Books
              </NavItem>
            </LinkContainer>
          </Nav>
        </Navbar.Collapse>
      </Navbar>
    );
  }
}
