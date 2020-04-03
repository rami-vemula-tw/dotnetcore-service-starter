
- We are using Docker compose `3.7`, we should have Docker Engine `18.06.0+`. Find out docker version by using below command.
```
$>> docker --version
```

## Docker compose for Payment Service
- **docker-compose.yml** file can be found **Payment** folder.
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


## Some useful Docker commands

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

