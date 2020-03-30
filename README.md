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


## Setting up environment to run database migrations
Open the **Payment** folder in VS Code

- Update the Postgres Connection in **appsettings.json** file under **PaymentService** folder.

- Install **Dotnet EF Tools** by executing below command in **VS Code terminal** (under PaymentService).

```
$>> dotnet tool install --global dotnet-ef --version 3.1.3
```

- You might encounter below message after installing Dotnet tools. Make sure the path to **Dotnet tools** is set in **ZSH** Profile (or else **BASH** Profile based on the terminal configuration) as shown below.

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

## Executing migrations

- Navigate to **PaymentService** folder and execute the migrations

```
dotnet ef database update
```

- **NOTE:** [dotnet ef migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
