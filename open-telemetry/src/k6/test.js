import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
    // Key configurations for Stress in this section
    stages: [
      { duration: '1m', target: 20 }, // traffic ramp-up from 1 to a higher 200 users over 10 minutes.
      { duration: '5m', target: 20 }, // stay at higher 200 users for 30 minutes
      { duration: '1m', target: 0 }, // ramp-down to 0 users
    ],
  };

export default function () {
    const params = {
        headers: {
          'Content-Type': 'application/json',
        },
      };
    // const baseUrl = 'http://Signal-ApiSe-ehW3GDbnmirh-60462493.eu-central-1.elb.amazonaws.com';
    const baseUrl = 'http://localhost:45080';
    http.post(baseUrl + '/customers', 
        JSON.stringify({
            "name": "test 1",
            "email": "test email"
        }), params);
    http.get(baseUrl + '/customers/1');
  
    // Sleep for 1 second to simulate real-world usage
    sleep(0.1);
  }