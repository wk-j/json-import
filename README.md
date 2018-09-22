## Json Import

[![Build Status](https://dev.azure.com/wk-j/json-import/_apis/build/status/wk-j.json-import)](https://dev.azure.com/wk-j/json-import/_build/latest?definitionId=10)
[![NuGet](https://img.shields.io/nuget/v/wk.JsonImport.svg)](https://www.nuget.org/packages/wk.JsonImport)

Import json into database without predefined schema

## Installation

```
dotnet tool install -g wk.JsonImport
```

## Usage

```bash
wk-json-import \
    --type postgres \
    --connection "Host=localhost;User Id=postgres;Password=1234;Database=Test" \
    --table MyTable \
    resource/Hello.json
```