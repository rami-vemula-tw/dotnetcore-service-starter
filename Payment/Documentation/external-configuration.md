
# External Configuration Approach

Configuration in ASP.NET Core can be cascaded through different sources. Any variable defined in two sources will be configured with the order of precedence of source declaration. In below code snippet, the precedence of configuration is as follows.
 - JSON Configuration files
    - appsettings.json
    - appsettings.{environment}.json
 - Environment Variables
 - Key Vault Configuration

```
Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    config.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
    config.AddEnvironmentVariables();

    var baseConfig = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build();

    if (baseConfig.GetValue<bool>("Vault:Enabled"))
    {
        var url = baseConfig.GetValue<string>("Vault:URL");
        var roleId = baseConfig.GetValue<string>("Vault:RoleId");
        var secretId = baseConfig.GetValue<string>("Vault:SecretId");
        config.AddVault(url, roleId, secretId);
    }
})
```


# Recommendations
- Build a cascading configuration to ensure proper overriding of default values.
    - Cascading configuration helps in supporting multiple development topologies like containerization, local debugging etc.
- Make sure proper conventions are followed while creating and resolving configuration.
- Store non-secure and immutable configuration in configuration files like appsettings.json.
- Always store confidential secrets, certificates and passwords in secure vault like Hashicorp vault.
- Use centralized configuration server to store non-secure and environment specific configuration information like Spring Cloud Configuration.
    - Centralized configuration is very helpful in distributed application scenarios where changes to the configuration can be propagated without application downtime.
- Always prefer Managed service providers like different cloud vendors to provide and support configuration infrastructure.
    - Manager services are reliable and resilient.
- Create proper access policies and identities to ensure security of configuration values.
    - Protection using Managed service identities through service principals is a recommended approach.
- Choosing different configuration providers like files, vault, environment variables and combination of providers should be dependent on the type and scale of the system.
    - Operations and maintenance of the applications plays crucial role in choosing configuration providers.






running locally, docker, production
convetions with : __







# Setting up the configuration through configuration files

- Configuration is loaded into two files - `appsettings.json` and `appsettings.{environment}.json` in `JSON` format. 
- Configuration from both the files are loaded using `AddJsonFile` extension of `IConfigurationBuilder` in `ConfigureAppConfiguration` extension of `IHostBuilder` under `Program.cs`.
- Config values can be categorized into multiple sections.
- Configuration sections can be mapped to the strongly types classes `ConfigureServices` method of `Startup.cs`. Create `AppSettings` class in `Infrastructure/Configuration` folder and map it to AppSettings section as shown below.

```
services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
```


# Setting up the configuration through ENV Variables

- Configuration can be set in the `Environment` variables on the machine where application is hosted. 
- Application is configured to use environment variables using `AddEnvironmentVariables` of `IConfigurationBuilder` in `ConfigureAppConfiguration` extension of `IHostBuilder` under `Program.cs`.

## Setting up the ENV variables for Infrastructure
- The `.env` file is present in **Docker** directory under **Payment** folder.
- Update the following settings to reflect your local development environment.
    - **PGADMIN_DEFAULT_EMAIL_VAL**
        - Valid email (used at the time of PgAdmin setup) using which we can access PGAdmin portal.
    - **PGADMIN_DEFAULT_PASSWORD_VAL**
        - Valid password (used at the time of PgAdmin setup) using which we can access PGAdmin portal.
    - **POSTGRES_USER_VAL**
        - Valid username (used at the time of Postgres server setup) for the root user.
    - **POSTGRES_PASSWORD_VAL**
        - Valid password (used at the time of Postgres server setup) using which we can access Postgres server using postgres user.
    - Following provide the configuration values for ELK stack containers.
        - LS_JAVA_OPTS_VAL=-Xmx256m -Xms256m
        - xpack_monitoring_enabled_VAL=true
        - xpack_watcher_enabled_VAL=false
        - ES_JAVA_OPTS_VAL=-Xms512m -Xmx512m
        - discovery_type_VAL=single-node


## Setting up the ENV variables for Application
- Update the following settings to reflect your local development environment.
    - **ConnectionStrings_PaymentConnection_VAL**
        - This value should be set with Postgres connection details. Setting up this can be ignore if Vault used.
    - **ElasticConfiguration_Uri_VAL**
        - Value of the Elastic Search endpoint typically the docker compose service name with port like http://elasticsearch:9200.
    - **ElasticConfiguration_Enabled_VAL**
        - True/false to enable/disable direct integration with Elastic Search (EK Logging).
    - **LogstashConfiguration_Uri_VAL**
        - Value of the Logstash endpoint typically the docker compose service name with port like tcp://logstash:8080.
    - **LogstashConfiguration_Enabled_VAL**
        - True/false to enable/disable integration with Logstash (ELK Logging)
    - **Vault_Enabled_VAL**
        - True/False to enable Vault integration.
    - **Vault_URL_VAL**
        - The URL for the vault, something like http://vault:8200 where domain is the vault service name in the docker compose.
    - **Vault_RoleId_VAL**
        - The role id through which vault can be accessed (for example : c8533179-7f9b-507a-7e72-5c56db93ded6). This can be obtained from the steps followed at [Key Vault Integration](key-vault-integration.md).
    - **Vault_SecretId_VAL**
        - The secret id through which vault can be accessed (for example : cdb04f9f-0d43-934a-3405-6582cbe4321a). This can be obtained from the steps followed at [Key Vault Integration](key-vault-integration.md).
    - ASP.NET Core variables are set with below values.
        - ASPNETCORE_ENVIRONMENT_VAL
            - Value can be Development, Production etc. Development value is required while running Docker compose in local.
        - ASPNETCORE_URLS_VAL
            - The internal container port where payment service is hosted (it should be http://*:8080).


# Setting up the configuration through centralized configuration provider (Spring Cloud Config Server)

### Instructions to setup the configuration at Spring Cloud Configuration for Payment Service Application can be found at [Spring Cloud Configuration Integration documentation](centralized-configuration.md).

# Setting up the secrets through Hashicorp Vault

### Instructions to setup the secrets at Hashicorp vault for Payment Service Application can be found at [Hashicorp Key Vault Integration documentation](key-vault-integration.md).