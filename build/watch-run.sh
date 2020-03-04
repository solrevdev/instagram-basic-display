#!/usr/bin/env bash

cd $(dirname $0)
dotnet watch --project ../samples/Web/Web.csproj run
