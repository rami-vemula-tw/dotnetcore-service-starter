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

## Executing migrations and running the app in Development environment
- Migrations will run automatically when **ASPNETCORE_ENVIRONMENT** variable is set to **Development** (in **LaunchSettings.json**). For production workloads, migrations should be deployed through CI/CD. 
- Instructions for manual execution of migrations can be found at **ef-migrations file** in **Payment** folder.

```
$>> dotnet run
```

## Setting up Seed Data
- Do a **POST** to http://localhost:5000/api/bankinfo with following JSON `{ "bankCode": "HDFC", "url": "http://localhost:8082" }`
- You should get a 201 response with following body `{ "id": 1, "bankCode": "HDFC", "url": "http://localhost:8082" }`
- Do a **GET** to http://localhost:5000/api/bankinfo/1 with following JSON `{ "id": 1, "bankCode": "HDFC", "url": "http://localhost:8082" }`
