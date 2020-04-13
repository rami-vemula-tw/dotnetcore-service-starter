#!/bin/bash

export VAULT_ADDR='http://127.0.0.1:8200'
unsealkey=NjH5HpWnXoVMXfv523bZCa5+4sYxTIEZPgWj9GQQ02A=
roottoken=s.PwiC7OTFW5vutReHuKg2wryo

if [ "$1" == "-i" ]
then
    brew install vault
    vault operator init -key-threshold=1 -key-shares=1
fi

if [ "$1" == "-c" ]
then
    vault operator unseal $unsealkey
    vault login $roottoken
    vault secrets enable -version=2 -path=paymentservice kv
    vault kv put paymentservice/settings ConnectionStrings:PaymentConnection="User ID=postgres;Password=admin123;Server=pg_con;Port=5432;Database=netpayments;"
    
    vault kv list paymentservice/
    vault kv get paymentservice/settings/

    vault auth enable approle
    vault policy fmt policies/policies.hcl
    vault policy write paymentservice policies/policies.hcl

    vault policy read paymentservice
    
    vault write auth/approle/role/paymentservice secret_id_ttl=525600m secret_id_num_uses=0 policies=default,paymentservice

    vault read auth/approle/role/paymentservice/role-id
    vault write -f auth/approle/role/paymentservice/secret-id
fi



