#!/run/current-system/sw/bin/env bash

mkdir source
nuget pack BindingAttributes/BindingAttributes.nuspec -verbosity detailed -build
mv *.nupkg source
