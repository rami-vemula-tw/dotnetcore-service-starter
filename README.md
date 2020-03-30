# Getting Started
## Prerequisites
- [Dotnet SDK v3.1.3](https://dotnet.microsoft.com/download/dotnet-core/3.1)

- Postgres database

- Docker

- Visual Studio Code


## Getting the code
```
$>> mkdir dotnetcore-service-starter

$>> git init

$>> git remote add origin git@github.com:rami-vemula-tw/dotnetcore-service-starter.git

$>> git pull origin master

$>> cd payment/

$>> dotnet restore

$>> dotnet build
```

## Executing migrations and running the app in Development environment
- Migrations will run automatically when **ASPNETCORE_ENVIRONMENT** variable is set to **Development**. (in **LaunchSettings.json**) For production workloads, migrations should be deployed through CI/CD. 
- Instructions for manual execution of migrations can be found at **ef-migrations file** in **Payment** folder.

```
$>> dotnet run
```