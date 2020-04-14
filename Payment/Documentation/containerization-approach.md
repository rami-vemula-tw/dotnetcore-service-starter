
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
- Docker recommends the use of volumes over the use of binds, as volumes are created and managed by docker and binds have a lot more potential of failure.
- Avoid manual configuration of services (like load balancers, security etc.) provided by Docker containers, offload this activity to orchestrators like Kubernetes.
- It is recommended to create `Docker Compose` of multi application deployments instead of working with individual Docker files.
    - Each application (typically a Docker compose service) should have its own Docker file. Docker compose should internally refer these docker files. 
- `Dockerfile` best practices can be found at this [official documentation](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/). The most important best practice is to keep the size of the image smaller.
    - Leverage build cache by adding the layers which are not often changing to the top of the dockerfile.
    - Use Multi-stage builds.
    - Run one application per container.
    - Keep (and copy) only required components in the dockerfile layers.
    - Build smallest image possible.
    - Tag images properly with Build/Release numbers.
    - Always use public images from authentic sources.
    - Create Multi-Arch images (based on infrastructure choices).
    - Run the container with least privileged user.
    - Use labels to specify metadata.
    - Add .dockerignore file to ignore files from build context.
    - Leverage Docker enterprise features for additional protection


## Recommendations for deploying Docker containers.
- When a change is committed to source control, use CI/CD pipeline to automatically build and tag a Docker image. Stored the images in a centralized registry like Docker Hub, Azure Container Registry etc.
    - Continuos Deployment
- Automate the process of signing the docker image using Docker Content Trust (DCT).
- Protect against vulnerabilities by using tools such as [Snyk](https://snyk.io/) in the build pipeline to continuously scan and monitor for vulnerabilities that may exist across all of the Docker image layers that are in use.
- Sensitive information (application secrets, SQL connection information, tokens etc.) should not be stored or mentioned in the docker compose or docker files.
    - The Developer (local environment) Secrets should be configured in `appsettings.Development.json` or in an `.env` file.
    - Production (or any higher environment) Secrets should be maintained at Key Vault like Azure Key Vault, Amazon Key Management Service, HashiCorp Vault etc. Both the Kay Vault and Application should be configured to have trust (or identity) through which application can read secrets which are provisioned in the vault.
    - The precedence of configuration is as follows. To give an example, the ConnectionString setting mentioned in appsettings.json file will be overridden by the same setting value which is mentioned in Key Vault.
        - appsettings.json
        - appsettings.{environment}.json
        - Environment variables
        - Key Vault configuration



# Containerization of Payment Service using Docker platform
- We are using Docker compose `3.7`, we should have Docker Engine `18.06.0+`. Find out docker version by using below command.
```
$>> docker --version
```

## Docker compose for Payment Service
**docker-compose** files can be found **Docker** folder under **Payment** folder.
- Docker compose for payment service is structured with below services.
    - Payment Service 
        - Dotnet Core WebAPI with `dockerfile` within the project
        - `.dockerignore` file is present to ignore certain files and folders from containerization.   
    - Postgres Database
    - PGAdmin
    - Logstash
    - Elastic Search
    - Kibana
    - Vault
- All the services are integrated on the same network bridge - paymentnetwork.
- Following volumes are used by respective services.
    - postgres-vol
    - pgadmin-vol
    - elasticsearch-vol
    - vault-vol


## Running Docker compose
Navigate to **Docker** folder (under **Payment** folder) and execute below commands in terminal.

### Set up the Configuration for Infrastructure by following [Setting up the ENV variables for Infrastructure](external-configuration.md#Setting-up-the-ENV-variables-for-Infrastructure).

### Setup the Infrastructure
```
$>> docker-compose -f docker-compose.network.yml -f docker-compose.infra.yml -f docker-compose.app.yml build

$>> docker-compose -f docker-compose.network.yml -f docker-compose.infra.yml up -d
```
- **NOTE:** docker-compose is executed in detached mode when the command is executed with `-d` switch.  

### Once infrastructure in ready, Set up the Configuration for Payment Service Application by following [Setting up the ENV variables for Application](external-configuration.md#Setting-up-the-ENV-variables-for-Application).


### Setup the Payment application
```
$>> docker-compose -f docker-compose.network.yml -f docker-compose.app.yml up -d  
```

- To check all the containers are up and running, execute the below command.
```
$>> docker ps -a
```

- Output would be like following table showing all containers and their current status.
```
CONTAINER ID        IMAGE                                                 COMMAND                  CREATED             STATUS              PORTS                                        NAMES
d44104b6cf0a        paymentservice:latest                                 "dotnet PaymentServi…"   12 minutes ago      Up 12 minutes       0.0.0.0:10001->8080/tcp                      ps_con
b29557cc90e6        vault                                                 "vault server -confi…"   13 minutes ago      Up 13 minutes       0.0.0.0:8200->8200/tcp                       v_con
5feb9a571a87        docker.elastic.co/kibana/kibana:7.6.1                 "/usr/local/bin/dumb…"   45 hours ago        Up 45 hours         0.0.0.0:5601->5601/tcp                       k_con
ea37caab0fdf        postgres                                              "docker-entrypoint.s…"   45 hours ago        Up 45 hours         0.0.0.0:5432->5432/tcp                       pg_con
1f4b4f30c297        docker.elastic.co/elasticsearch/elasticsearch:7.6.1   "/usr/local/bin/dock…"   45 hours ago        Up 45 hours         0.0.0.0:9200->9200/tcp, 9300/tcp             es_con
694d73166bbc        dpage/pgadmin4                                        "/entrypoint.sh"         45 hours ago        Up 45 hours         443/tcp, 0.0.0.0:5050->80/tcp                pga_con
00724c1ab231        docker_logstash                                       "/usr/local/bin/dock…"   45 hours ago        Up 45 hours         5044/tcp, 9600/tcp, 0.0.0.0:8080->8080/tcp   l_con
```

- **NOTE:** **Payment Service** container needs to be stopped and restarted after **Postgres** container is up and running. This step is required to run the migrations successfully on the postgres database.
```
$>> docker-compose -f docker-compose.network.yml -f docker-compose.app.yml up -d --force-recreate --no-deps --build paymentservice
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

- Build and run from multiple Docker compose files
```
$>> docker-compose -f docker-compose.network.yml -f docker-compose.infra.yml -f docker-compose.app.yml build

$>> docker-compose -f docker-compose.network.yml -f docker-compose.infra.yml up -d

$>> docker-compose -f docker-compose.network.yml -f docker-compose.app.yml up -d  
```

- Build and run a particular server in docker compose
```
$>> docker-compose up -d --force-recreate --no-deps --build servicename
```

- Build and run a particular server in docker compose (multiple yml files)
```
$>> docker-compose -f docker-compose.network.yml -f docker-compose.app.yml up -d --force-recreate --no-deps --build paymentservice
```
- List all Docker volume
```
$>> docker volume ls
```
- Remove a Docker volume
```
$>> docker volume rm volume-name
```


