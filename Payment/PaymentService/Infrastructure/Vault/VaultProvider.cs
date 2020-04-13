using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.Commons;

namespace PaymentService.Infrastructure.Vault
{
    public static class VaultProvider
    {
        public static IConfigurationBuilder AddVault(this IConfigurationBuilder builder, string vaultUrl, string roleId, string secretId)
        {
            var buildConfig = builder.Build();
            var vaultClientSettings = new VaultClientSettings(vaultUrl, new AppRoleAuthMethodInfo(roleId, secretId));
            var vaultClient = new VaultClient(vaultClientSettings);

            var secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("settings/", null, "paymentservice/").Result;
            var dataDictionary = secret.Data.Data.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString()));
            return builder.AddInMemoryCollection(dataDictionary);
        }
    }
}