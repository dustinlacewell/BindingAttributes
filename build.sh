#!/run/current-system/sw/bin/env bash

mono .paket/paket.exe install
dotnet publish -o BindingAttributes/lib/netcoreapp2.1
tree
nuget pack BindingAttributes/BindingAttributes.nuspec -verbosity detailed
