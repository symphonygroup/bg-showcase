# Cookbook Services

Set of services written for food processing domain, focused on showcasing the implementation of an enterprise service bus.

## Tech stack:
- .Net 6
- MassTransit - service bus
- RabbitMQ - message broker
- Seq (with Serilog) - structured logging
- OpenTelemetry (with Jaeger) - telemetry monitoring
- Hangfire - delayed messaging


## System overview  
&nbsp;
![High level architecture](./docs/Cookbook.diagram.png)



System consists of three services, as shown in the image above:

 - Cookbook API - an API that starts all of the processes and allows for interaction with the system through OpenAPI (swagger) page
 - Cookbook Inventory - a service that processes all requests related to ingredients
 - Cookbook Cooking - a service that handles actual processing of ingredients and the cooking process through a distributed transaction using a state machine.

 There are two MongoDB databases, one for each of the backend services, Inventory and Cooking. Each of them stores infomration only related to a part of the process or relevant to their internal processes.

 ## Starting the system

---
Note:   
[Docker](https://www.docker.com/products/docker-desktop/) is mandatory for running this system's dependencies.  
[.Net 6](https://dotnet.microsoft.com/en-us/download) is mandatory for running the services.

---

  By running `docker-compose up` in the `cookbook-docker-services` directory, all of the required services will be setup in your local docker. **Be mindful of the ports as all ports in the system are default ones.**

  *Currently there's no docker image for each of the services, this is on TODO list.*

  After starting up docker containers, navigate to each of the services directories and do a `dotnet run` command, this will start each service individually. When docker deployment is handled this step will be made much easier :)

 ## Relevant URLs

Integrations:
 - Seq logging: http://localhost/#/events
 - Jaeger telemetry: http://localhost:16686/search
 - RabbitMQ management: http://localhost:15672/#/

Services:

- API: 
  - OpenAPI - Swagger: https://localhost:7167/swagger
  - Health check - Ready: https://localhost:7167/health/ready
  - Health check - Live: https://localhost:7167/health/live
  - Scheduler - Hangfire: https://localhost:7167/hangfire
- Inventory:
  - Health check - Ready: https://localhost:7168/health/ready
  - Health check - Live: https://localhost:7168/health/live
  - Scheduler - Hangfire: https://localhost:7168/hangfire
- Cooking:
  - Health check - Ready: https://localhost:7169/health/ready
  - Health check - Live: https://localhost:7169/health/live
  - Scheduler - Hangfire: https://localhost:7169/hangfire


