import 'bootstrap/dist/css/bootstrap.min.css';

import './index.css';
import React from 'react';
import ReactDOM from 'react-dom';
import $ from 'jquery';
import Popper from 'popper.js';
import 'bootstrap/dist/js/bootstrap.bundle.min';
import App from './App';
import { unregister } from './registerServiceWorker';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

//Rename method to render {MicroFrontEnd}
window.renderNewFrontEnd = (containerId, history) => {
    ReactDOM.render(
        <App history={history} />,
        document.getElementById(containerId),
    );
    unregister();

};

//Rename method to unmount{MicroFrontEnd}
window.unmountNewFrontEnd = containerId => {
    ReactDOM.unmountComponentAtNode(document.getElementById(containerId));
};
