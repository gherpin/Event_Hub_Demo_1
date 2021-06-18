import React from 'react';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useState, useEffect } from 'react'

import { setupSignalRConnection } from '../services/signalR';

const ProgressBar = ({ currentValue, eventName, backgroundStyle, animated, progressBarStyle }) => {

  const animatedStyle = animated ? 'progress-bar-animated' : '';
  const stripedStyle = progressBarStyle ? 'progress-bar-striped' : '';

  return (
    <div className="progress">
      <div className={'progress-bar ' + stripedStyle + ' ' + animatedStyle + ' ' + backgroundStyle} role="progressbar" style={{ width: currentValue +'%' }} aria-valuenow={currentValue} aria-valuemin="0" aria-valuemax="100">{eventName} - {currentValue}%</div>
    </div>
  )
};

const EventCard = ({ sourceName, receivedEvents }) => {

  const [eventSource] = useState(sourceName || 'Event Source');

  const determineProgressBarCompletion = receivedEvent => {
    
    let currentValue = 10;
    if (receivedEvent.serviceEvents["StartedProcessingEvent"]) {
      currentValue = 33;
    }
    if (receivedEvent.serviceEvents["ProcessingEvent"]) {
      currentValue = 66;
    }
    if (receivedEvent.serviceEvents["FinishedProcessingEvent"]) {
       currentValue = 100;
    } 
    return currentValue;
  }

  const determineBackGroundStyle = receivedEvent => {
  // (100 ms bg-success, 300ms bg-warning, 500ms bg-danger)
    let backgroundStyle = '';
    if (receivedEvent.serviceEvents["FinishedProcessingEvent"] && receivedEvent.serviceEvents["StartedProcessingEvent"]) {
      var ms = Math.abs(new Date(receivedEvent.serviceEvents["FinishedProcessingEvent"]) - new Date(receivedEvent.serviceEvents["StartedProcessingEvent"]));
      backgroundStyle = 'bg-success';
      backgroundStyle = ms > 300 ? 'bg-warning' : backgroundStyle;
      backgroundStyle = ms > 500 ? 'bg-danger' : backgroundStyle;
    }
    return backgroundStyle;
  }

  const eventProgressBars = receivedEvents.map((receivedEvent) => {

    let currentValue = determineProgressBarCompletion(receivedEvent);
    let animated = currentValue < 100 ? true : false;
    let progressBarStyle = currentValue < 100 ? true : false;
    let backgroundStyle = determineBackGroundStyle(receivedEvent);
    
    return (
      <ProgressBar key={receivedEvent.id} currentValue={currentValue} eventSource={receivedEvent.receivedEvent.eventSource} eventName={receivedEvent.receivedEvent.eventName} backgroundStyle={backgroundStyle} animated={animated} progressBarStyle={progressBarStyle} />
    );
  });

  return (
    <div className="card">
      <div className="card-header">{eventSource}</div>
      <div className="card-body">
        <h6 className="card-title">Last 20 Events</h6>
        {eventProgressBars}
        {eventProgressBars.length == 0 &&
          <p>No recent events</p>
        }
      </div>
      <div className="card-footer">
        <small className="text-muted">Last updated 3 mins ago</small>
      </div>
    </div>

  )
}


const Home = () => {

  const [hubConnection, setHubConnection] = useState({});
  // //This app just needs to be a dash board of the events being consumed by each publisher
  const [notificationServiceEvents, setNotificationServiceEvents] = useState([]);
  const [foodTruckManagementAPIEvents, setFoodTruckManagementAPIEvents] = useState([]);
  const [accountManagementAPIEvents, setAccountManagementAPIEvents] = useState([]);
  const [menuManagementAPIEvents, setMenuManagementAPIEvents] = useState([]);
  const [paymentManagementAPIEvents, setPaymentManagementAPIEvents] = useState([]);
  const [eventCounter, setEventCounter] = useState(0);

  const eventSources = {
    
    "Notification_Service": (event) => {
      setNotificationServiceEvents(notificationServiceEvents => {
        return updateEventArray(notificationServiceEvents, event);
      });
    },
    "FoodTruckManagement_API": (event) => {
      setFoodTruckManagementAPIEvents(foodTruckManagementAPIEvents => {
        return updateEventArray(foodTruckManagementAPIEvents, event)
      });
    },
    "AccountManagement_API": (event) => {
      setAccountManagementAPIEvents(accountManagementAPIEvents => {
        return updateEventArray(accountManagementAPIEvents, event);
      });
    },
    "MenuManagement_API": (event) => {
      setMenuManagementAPIEvents(menuManagementAPIEvents => {
        return updateEventArray(menuManagementAPIEvents, event);
      })
    },
    "PaymentManagement_API": (event) => {
      setPaymentManagementAPIEvents(paymentManagementAPIEvents => {
        return updateEventArray(paymentManagementAPIEvents, event);
      });
    }
    //Add additional items as event sources increases
    
  }

  const updateEventArray = (eventArray, event) => {

    //update item state in array if present.
    var index = eventArray.findIndex(item => item.id === event.id);
    if (index > -1) {
      eventArray.splice(index, 1, event);
      return [...eventArray];
    }
   
    if (eventArray.length >= 20) {
      eventArray.splice(0, 1);
    }
    return [...eventArray, event]
  }

  useEffect(() => {

    const createHubConnection = async () => {

      const hubConnection = new HubConnectionBuilder()
        .withUrl('/messagehub')
        .configureLogging(LogLevel.Information)
        .build();

      //Set Up events before starting hub
      hubConnection.on("eventReceived", e => {

        setEventCounter(eventCounter => eventCounter + 1);
        //In Summary Components Store cumulative number of events

        //Card will only show the last 20 events. so calculated 

        //rewrite to obtain from dicationary of strings and the setStatefunction
        var setEvents = eventSources[e.receivedEvent.eventSource]; //return setState function
        setEvents(e);
        

        //If Unknown eventSource send to another panel to display that
      });


      try {
        await hubConnection.start();
        console.log('Connection successful!')

        //Test Methods for development
        hubConnection.invoke("SimulateEvents", 1);

      } catch (err) {
        console.log(err);
      }
    
      setHubConnection(hubConnection);
    }
    createHubConnection();
  }, []);

 

  return (
    <>
      
      <Header hubConnection={hubConnection} eventCounter={eventCounter} />
      <div className="card-group">
        <EventCard sourceName="AccountManagement_API" receivedEvents={accountManagementAPIEvents} />
        <EventCard sourceName="FoodTruckManagement_API" receivedEvents={foodTruckManagementAPIEvents} />
        <EventCard sourceName="MenuManagement_API" receivedEvents={menuManagementAPIEvents} />
        <EventCard sourceName="PaymentManagement_API" receivedEvents={paymentManagementAPIEvents} />
        <EventCard sourceName="Notification_Service" receivedEvents={notificationServiceEvents}/> 
        </div>
    </>
    );
}

const Header = ({ hubConnection, eventCounter }) => {

  const simulateEvents = () => {
    hubConnection.invoke("SimulateEvents", 10);
  }

  return (
    <>
      <div className="row">
        <div className="col">
          <h1>Food Truck Notification Service</h1>
          <button onClick={simulateEvents} >Simulate Events</button>
          <h6>Events - {eventCounter} </h6>
        </div>
      </div>
    </>
  )
}

export default Home;