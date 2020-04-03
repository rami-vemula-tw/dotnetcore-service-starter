# Getting Started
## Prerequisites
- [Dotnet SDK v3.1.3](https://dotnet.microsoft.com/download/dotnet-core/3.1)

- Postgres database

- Docker

- Visual Studio Code


## Getting the code
```
$>> git clone git@github.com:rami-vemula-tw/dotnetcore-service-starter.git

$>> cd payment/

$>> dotnet restore

$>> dotnet build
```

## Setting up Postgres Connection
- The Postgres Connection can be found at **appsettings.json** file of **PaymentService** folder. Update the connection with your connection.
```
  "ConnectionStrings": {
    "PaymentConnection": "User ID =postgres;Password=admin123;Server=localhost;Port=5432;Database=netpayments;"
  }
```

## Executing migrations and running the app in Development environment
- Migrations will run automatically when **ASPNETCORE_ENVIRONMENT** variable is set to **Development** 
  - In **LaunchSettings.json**, while running using **dotnet run**. 
  - In **launch.json** (under **.vscode** folder), while running using **VS Code Debugger**. 
- For production workloads, migrations should be deployed through CI/CD. 

```
$>> dotnet run
```

## Making sure application is up and running
- Do a **GET** to http://localhost:5000/api/payments , we should see the response `Payment Service Up ad running`.


## Setting up Seed Data
- Do a **POST** to http://localhost:5000/api/bankinfo
    - With `CorrelationId` set in **Headers** to any value like `2d390bf2-0589-46af-94d9-a3b9e5ce7607`
    - With JSON body `{ "bankCode": "HDFC", "url": "http://localhost:10002" }`

- We should get a 201 response 
    - With following body `{ "id": 1, "bankCode": "HDFC", "url": "http://localhost:10002" }`

- Do a **GET** to http://localhost:5000/api/bankinfo/1
    - With `CorrelationId` set in **Headers** to any value like `2d390bf2-0589-46af-94d9-a3b9e5ce7607`

- We should get a 201 response 
    - With following JSON `{ "id": 1, "bankCode": "HDFC", "url": "http://localhost:10002" }`

# Other Key Concepts

## Create and Run Migrations Manually (including data seed migrations)
- Instructions for manual execution of migrations can be found at [EF Migrations](Payment/ef-migrations.md) file in **Payment** folder.

## Running the application using Docker ecosystem
- Payment Service is integrated with Docker Compose. Detailed instructions can be found at [Docker Compose](Payment/docker-compose.md) file in **Payment** folder.


