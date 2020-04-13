path "paymentservice/" {
  capabilities = ["read"]
}

path "paymentservice/*" {
  capabilities = ["read"]
}
