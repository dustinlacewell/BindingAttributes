#!/run/current-system/sw/bin/env bash

mono .paket/paket.exe install
dotnet build -o .
nuget pack BindingAttributes/BindingAttributes.nuspec -verbosity detailed
