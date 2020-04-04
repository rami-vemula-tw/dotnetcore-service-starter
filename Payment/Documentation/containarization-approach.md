
# Containerization Approach
Following are the major areas where containerization techniques play crucial role in development and deployment life cycle.
1. System Modernization Process
2. Polyglot Microservices Architectures
3. Distributed Systems Design
4. Cloud agnostic development and deployment strategies

There are many built in advantages with a container platform like Docker. Few of the notable advantages are as follows. 
1. Environment standardization
2. Efficient resource consumption
3. Highly scalable and resilient services
4. Application isolation
5. Consistency in Build and Release pipelines
6. Effective Configuration
7. Rapid Development
8. Quick onboarding new technical team members
9. Support for container orchestrators

There are many other advantages where containerization concepts deliver value compared to the traditional development methodologies. We can get more advantages from containers when we are trying to achieve Non-functional requirements ([Quality Attributes](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ee658094%28v%3dpandp.10%29)). 

## Docker Recommendations
- One should have proper understanding of Docker container topology to better plan the docker images for application (for example windows containers can only run on Windows machines, but linux containers can work on both Windows, Mac, Linux distros).
    - There can be performance issues when developing applications using docker on local machines because of the VM abstraction through which docker gets the access to host kernel .
    - Even though docker containers are light-weight compared with VMs, but they are less isolated with host OS and more prone to issues.
- Do not complicate the application development with Docker containers unless it is truly required.
    - If we are developing simple web application or API where we do not have critical integrations with different other applications, then containers can be an overkill.
    - Docker helps debugging of large scale systems, but to debug a simple API, docker might be adding extra complexity.
- Even though containers can be used to deliver stateful services with the help of volumes, it is recommended against to this development model in some cases.
    - Avoid maintaining databases and data stores in containers. It is advised to subscribe to cloud vendors who provide data stores as services in different subscription models.
    - In case of on-prem infrastructure, it still advisable to run databases on dedicated servers rather than in Docker containers. Hence reducing the complexity and pitfalls of loosing production data.
- Avoid storing application data on the container. If the container goes down, data goes down.
- Avoid manual configuration of services (like load balancers, security etc.) provided by Docker containers, offload this activity to orchestrators like Kubernetes.
- It is recommended to create `Docker Compose` of multi application deployments instead of working with individual Docker files.
    - Each application (typically a Docker compose service) should have its own Docker file. Docker compose should internally refer these docker files. 
- `Dockerfile` best practices can be found at this [official documentation](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/). The most important best practice is to keep the size of the image smaller.
    - Leverage build cache.
    - Use Multi-stage builds.
    - Run one application per container.
    - Build smallest image possible.
    - Tag images properly.
    - Always use public images from authentic sources.
    - Create Multi-Arch images (based on infrastructure choices).


## Recommendations for deploying Docker containers.



# Containarization of Payment Service using Docker platform
- We are using Docker compose `3.7`, we should have Docker Engine `18.06.0+`. Find out docker version by using below command.
```
$>> docker --version
```

## Docker compose for Payment Service
**docker-compose.yml** file can be found **Payment** folder.
- Docker compose for payment service is structured with below services.
    - Payment Service (Dotnet Core WebAPI)
    - Postgres Database
    - PGAdmin
    - Logstash
    - Elastic Search
    - Kibana
- All the services are integrated on the same network bridge - paymentnetwork.
- Following volumes are used by respective services.
    - postgres
    - pgadmin
    - elasticsearch-data

## Setting up Postgres Connection
- The Postgres Connection can be found under **paymentservice** of docker compose file. Update the connection with your connection.

## Running Docker compose
- Navigate to **Payment** folder and execute below commands in terminal.
```
$>> docker-compose build

$>> docker-compose up
```
- **NOTE:** docker-compose up can be executed in detached mode by appending the command with `-d` switch.  

- To check all the containers are up and running, execute the below command.
```
$>> docker ps -a
```

- Output would be like following table showing all containers and their current status.
```
CONTAINER ID        IMAGE                                                 COMMAND                  CREATED             STATUS              PORTS                                        NAMES
c50c13f3b3f7        paymentservice:latest                                 "dotnet PaymentServi…"   3 hours ago         Up 3 hours          0.0.0.0:10001->8080/tcp                      ps_con
e1b21eb8494b        payment_logstash                                      "/usr/local/bin/dock…"   4 hours ago         Up 3 hours          5044/tcp, 9600/tcp, 0.0.0.0:8080->8080/tcp   l_con
f3713bb619db        docker.elastic.co/kibana/kibana:7.6.1                 "/usr/local/bin/dumb…"   25 hours ago        Up 4 hours          0.0.0.0:5601->5601/tcp                       k_con
9947c812a3e9        docker.elastic.co/elasticsearch/elasticsearch:7.6.1   "/usr/local/bin/dock…"   25 hours ago        Up 4 hours          0.0.0.0:9200->9200/tcp, 9300/tcp             es_con
c5015e12b4f5        dpage/pgadmin4                                        "/entrypoint.sh"         27 hours ago        Up 4 hours          443/tcp, 0.0.0.0:5050->80/tcp                pga_con
948835ee9817        postgres                                              "docker-entrypoint.s…"   27 hours ago        Up 4 hours          0.0.0.0:5432->5432/tcp                       pg_con
```

- **NOTE:** **Payment Service** container needs to be stopped and restarted after **Postgres** container is up and running. This step is required to run the migrations successfully on the postgres database.
```
$>> docker-compose up -d --force-recreate --no-deps --build paymentservice 
```

## Executing migrations and running the app in Development environment
- Migrations will run automatically when **ASPNETCORE_ENVIRONMENT** variable is set to **Development** in **dockerfile** under **PaymentService** folder.


## Making sure application is up and running
- Do a **GET** to http://localhost:10001/api/payments , we should see the response `Payment Service Up ad running`.


## Setting up Seed Data
- Do a **POST** to http://localhost:10001/api/bankinfo
    - With `CorrelationId` set in **Headers** to any value like `2d390bf2-0589-46af-94d9-a3b9e5ce7607`
    - With JSON body `{ "bankCode": "HDFC", "url": "http://localhost:10002" }`

- We should get a 201 response 
    - With following body `{ "id": 1, "bankCode": "HDFC", "url": "http://localhost:10002" }`

- Do a **GET** to http://localhost:10001/api/bankinfo/1
    - With `CorrelationId` set in **Headers** to any value like `2d390bf2-0589-46af-94d9-a3b9e5ce7607`

- We should get a 201 response 
    - With following JSON `{ "id": 1, "bankCode": "HDFC", "url": "http://localhost:10002" }`

# Deployment of Payment Service Docker Containers

## To be implemented (mostly covered in CI/CD section)

# Some useful Docker commands

- Build a Docker images
```
$>> docker build -t imagename:tag .
```

- Run a container
```
$>> docker run -p hostport:containerport --name containername -d imagename:tag
```

- View all containers
```
$>> docker ps -a
```

- View all images
```
$>> docker images -a
```

- Stop and Delete a container
```
$>> docker stop containername
$>> docker rm containername
```

- Delete an image
```
$>> docker rmi imagename
```

- Delete all containers which are not currently running
```
$>> docker rm $(docker ps -a -q)
```

- Delete all images which are untagged 
```
$>> docker rmi $(docker images -a --filter dangling=true)
```

- Follow the logs of a container  
```
$>> docker logs --follow containername
```

- Bash into a container 
```
$>> docker exec -it containerName /bin/bash
```

- Build and run a particular server in docker compose
```
$>> docker-compose up -d --force-recreate --no-deps --build servicename
```

