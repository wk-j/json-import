## Commands

```
dotnet run --project src/JsonImport/JsonImport.csproj \
    --type postgres \
    --connection "Host=localhost;User Id=postgres;Password=1234;Database=Test" \
    --table MyTable \
    resource/Hello.json
```