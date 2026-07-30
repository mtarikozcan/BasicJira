path "secret/data/basicjira" {
  capabilities = ["create", "read", "update", "patch"]
}

path "secret/data/basicjira/*" {
  capabilities = ["create", "read", "update", "patch"]
}

path "secret/metadata/basicjira" {
  capabilities = ["read", "list"]
}

path "secret/metadata/basicjira/*" {
  capabilities = ["read", "list"]
}