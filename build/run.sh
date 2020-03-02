#!/usr/bin/env bash

cd $(dirname $0)
dotnet run --project ../samples/Web/Web.csproj
