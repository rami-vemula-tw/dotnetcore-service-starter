- [dotnet ef migrations documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)

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

## Create new migrations

- Make changes to the **Model** and **PaymentDBContext**.
- While in **PaymentService** folder, create a migration by executing below command.

```
dotnet ef migrations add MigrationName
```

## Manual execution of migrations
- While in **PaymentService** folder, migrations can be executed manually through below command.

```
dotnet ef database update
```

## (OPTIONAL) Data seeding through migrations 
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