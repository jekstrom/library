import React, { Component } from 'react';

export class Home extends Component {
  displayName = Home.name

  render() {
    return (
      <div>
        <h1>Library</h1>
        <p>Welcome to the Library</p>
        <div>
          <a href="Login/Login">Log In with GitHub</a>
        </div>

        <p>If you are an authenticated user, you can manage books.</p>

      </div>
    );
  }
}
