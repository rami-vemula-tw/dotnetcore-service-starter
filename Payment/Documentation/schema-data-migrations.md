# Datastore Migrations Approach
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
2. Generate SQL scripts through custom utilities from data files like CSV, flat files etc.


## Migration Strategy Recommendations
- It is highly recommended to opt for a migration strategy where human intervention factor is very minimal. 
- It is advisable to create EF Core migrations for schema changes where structural modifications needs to be deployed.
    - Dotnet EF Tools can be leveraged to automate incremental deployments.
- Stored procedures and functions should be written and versioned as SQL files. 
    - EF Core custom migrations should be leveraged to deploy these SQL Files. 
    - Whenever a SP or Function is changed, developer should create a custom migration and commit the migration to the source code repository. This way that migration can be deployed automatically through Dotnet EF Tools.
- Generation of SQL scripts from data files like CSV, flat file etc., is recommended for seed data migrations.
    - Custom utilities can be created with any choice of technology or language. These utilities can read the seed data from the data files, transform the data as per the target data structure and finally create the script file in a format which can be deployed to the target.
    - These script files should be versioned based on Release, Build and Date of creation.
    - Deployment platforms should be capable of picking up these scripts and deploy them to targets through deployment pipelines.
- SQL Scripts should be created by the developers whenever existing data store is changed (for example splitting a table into multiple tables based on Normalization forms).
    - EF Core custom migrations should be leveraged to deploy these SQL Files. This way that migration can be deployed automatically through Dotnet EF Tools.
- Every migration should have `Up` and `Down` strategies to support commit and rollback scenarios.

## Recommendations for deploying migrations
- In development environment, automatic deployment of migrations should be configured on application start. This will ensure exceptions or misleading behaviours are caught as early as possible. 
    - Automating of DB deployment on application start is possible with Dotnet EF Tools. 
    - If migrations cannot be executed in app start (for example, running custom seed data scripts), it is advised to create a bash/powershell script which will automate the process.
    - This custom script should keep track of SQL scripts it ran, so that it can always work with differential database updates.
- For production workloads, migrations should be deployed through CI/CD pipeline through Dotnet EF Tools. 
    - Custom tasks should be incorporated in the deployment pipeline to deploy the seed data scripts.
    - This will fecilitate error and hassle free deployments of distributed system where different applications are deployed to polygot environments. 
    - This will also helps in deploying the migrations to multiple regions.


# Code Sample for EF Core Migrations 

 [Dotnet ef migrations documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)

## Setting up environment to create database migrations
- Open the **Payment** folder in VS Code

- Make sure the Postgres Connection in **appsettings.json** file under **PaymentService** folder is your connection.

- Install **Dotnet EF Tools** by executing below command in **VS Code terminal** (under PaymentService).

```
$>> dotnet tool install --global dotnet-ef --version 3.1.3
```

- You might encounter below message after installing Dotnet tools. Make sure the path of **Dotnet tools** is set in **ZSH** Profile (or else **BASH** Profile based on the terminal configuration) as shown below.

```
Tools directory '/Users/your username/.dotnet/tools' is not currently on the PATH environment variable.
If you are using zsh, you can add it to your profile by running the following command:

cat << \EOF >> ~/.zprofile
# Add .NET Core SDK tools
export PATH="$PATH:/Users/your userna,e/.dotnet/tools"
EOF
And run zsh -l to make it available for current session.

You can only add it to the current session by running the following command:

export PATH="$PATH:/Users/your username/.dotnet/tools"

Reopen VS code
```

## How to create new migrations using Dotnet EF Tools

- Make changes (create or update data model) to the **PaymentDBContext**. 
- While in **PaymentService** folder, create a migration by executing below command.

```
dotnet ef migrations add MigrationName
```

## Execution of migrations using Dotnet EF Tools
While in **PaymentService** folder, migrations can be executed through below command.

```
dotnet ef database update
```

## (OPTIONAL) Data seeding through Dotnet EF Tools Migrations 
- Create the data script in **SQLScripts** folder under **Migrations** folder (of PaymentService). The naming convention which is followed is `{TableName}Data-{mmddyyyy}-{index (order in the day)}`. For example `BankInfoData-03312020-1.sql`

- **NOTE**: **SQLScripts** folder is marked with **CopyToOutputDirectory** to true in the **csproj** file.

- Create the migration as shown below. It is not necessary to give SqlFileName as the name of the migration. 
```
dotnet ef migrations add SqlFileName

For example `dotnet ef migrations add BankInfoData-03312020-1`
```

- Open generated Migration class and write below code to integrate the migration with SQL file.
```
using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentService.Migrations
{
    public partial class BankInfoData033120201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations/SQLScripts/BankInfoData-03312020-1.sql"); 
            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
```

- Test the migration in local by executing below command.
```
dotnet ef database update
```

# Code Sample for SQL Script based Seed data Migrations 

## To be implemented (mostly covered in CI/CD section)