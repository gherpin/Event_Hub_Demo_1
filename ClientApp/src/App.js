import React from 'react';
import { Router, Route } from 'react-router';
import Home from './components/Home';
import { createBrowserHistory } from 'history';

const defaultHistory = createBrowserHistory();

const App = ({ history }) => {
  
    return (
        <Router history={history || defaultHistory}>
            <Route exact path='/' component={Home} />
        </Router>
    );
}

export default App;
