
# Application Containarization Approach
Following are the major areas where migrations play crucial role in development and deployment life cycle.
1. Schema changes (specifically DDL statements like create/alter tables, indexes etc.) for both SQL/NoSQL environments.
2. Stored procedures and functions
3. Seed data (seeding master data like countries, zipcodes etc.)
4. Data transformation because of schema changes (for example, rstructuring a table into multiple tables etc.)
5. ETL Pipelines (migrating transactional data from on-prem to cloud etc.)

**NOTE:** ETL Migratons (#5 in the above list) is out of scope of this Starter kit.

The primary key factors which play a major role in selection of a migration strategy are as follows.
1. System topology (polygot)
2. Data store choices
3. Deployment strategies

In short, migrations strategies vary between different use cases. The migration strategy opted for an application which is dependent on NoSQL store cannot be leveraged for an application with a SQL backend.

At a strategy level, migrations can be created using one of the below options.
1. Dotnet EF Core migrations
2. SQL Scripts containing Stored procedires, Functions etc.
2. Generate SQL dacpac (or data dump for other data store types) through custom utilities from data files like CSV etc.


## Migration Strategy Recommendations
- It is highly recommended to opt for a migration strategy where human intervention factor is very minimal. 
- It is advisable to create EF Core migrations for schema changes where structural modifications are required.
- Stored procedures and functions should be written and versioned as SQL files. These scripts should always (whether changed or not) be executed in the deployment pipeline.
- Generation of SQL dacpack (or data dumps for other data store types) from different data files like CSV, flat file etc., is recommended for seed data migrations.
    - Custom utilities can be created with any choice of technology or language. These utilities can read the seed data from the data files, transform the data as per target data structure and finally create the data dump in a formatoly which can be deployed to the target.
    - These data dump files should be versioned based on Release, Build and Date of creation.
    - Deployment platforms should be capable of picking the data dumps and deploy them to targets through deployment pipelines.
- SQL Scripts should be created by the developers whenever existing data store is changed (for example splitting a table into multiple tables based on Normalization forms).
    - These scripts should be structured in folders based on Release, Build and Date of creation. So that they can be uniquely identified for incremental deployments between builds and releases.
- Every migration should have `Up` and `Down` strategies to support commit and rollback scenarios.

## Recommendations for deploying migrations
- In development environment, automatic deployment of migrations should be configured on application start. This will ensure exceptions or misleading behaviours are caught at the time of development. 
    - If migrations cannot be executd in app start (because of many reasons like heterogeneous application model), it is advised to create a bash/powershell script which will automate the process.
    - This custom script should keep track of SQL Scripts and migrations it ran, so that it can always work with differential database updates.
- For production workloads, migrations should be deployed through CI/CD pipeline. This will fecilitate error and hassle free deployments of distributed system where different applications are deployed to polygot environments. This will also helps in deploying the migrations to multiple regions.

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

