
# External Configuration Approach

## Setting up the ENV variables
- The `.env` file is present in **Docker** directory under **Payment** folder.
- Update the following settings to reflect your local development environment.
    - ConnectionStrings_PaymentConnection_VAL : should be set with proper value if Vault is not used
    - PGADMIN_DEFAULT_PASSWORD_VAL
    - POSTGRES_PASSWORD_VAL
    - ElasticConfiguration_Enabled_VAL=true/false to enable/disable direct integration with Elastic Search (EK Logging).
    - LogstashConfiguration_Enabled_VAL=true/false to enable/disable integration with Logstash (ELK Logging)