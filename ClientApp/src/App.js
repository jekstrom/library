import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Books } from './components/Books';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faHome, faBook, faEdit } from '@fortawesome/free-solid-svg-icons';

library.add(faHome, faBook, faEdit);

export default class App extends Component {
  displayName = App.name;

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/books' component={Books} />
        <Route path='/fetchdata' component={FetchData} />
      </Layout>
    );
  }
}
