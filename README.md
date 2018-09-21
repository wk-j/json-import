## Json Import

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