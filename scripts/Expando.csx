#! "netcoreapp2.1"
#r "nuget:Newtonsoft.Json,11.0.2"
#r "nuget:AutoMapper,7.0.1"

using Newtonsoft.Json;
using AutoMapper;

var text = File.ReadAllText("resource/Hello.json");
var results = JsonConvert.DeserializeObject<ExpandoObject[]>(text);

dynamic result = results[0];
var dict = result as IDictionary<string, object>;

foreach (var (k, v) in dict) {
    Console.WriteLine($"{k} {v.GetType().FullName}");
}


class Data {
    public int Auto { set; get; }
    public int A { set; get; }
    public int B { set; get; }
    public DateTime Date { set; get; }
}

Mapper.Initialize(cfg => {
    cfg.CreateMap(typeof(IDictionary<string, object>), typeof(Data));
});

var data = Mapper.Map<Data>(dict);
Console.WriteLine(JsonConvert.SerializeObject(data));