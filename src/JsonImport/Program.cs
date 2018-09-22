using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicModel {

    class Program {

        static DynamicContext CreateContext(string connectionString, Type targetType) {

            var modelOptions = new ModelOptions {
                ModelType = targetType
            };

            var collection = new ServiceCollection();
            collection.AddLogging(builder => {
                builder.AddConsole();
            });

            collection.AddSingleton<ModelOptions>(modelOptions);
            collection.AddDbContext<DynamicContext>(builder => {
                builder.UseNpgsql(connectionString);
            });

            var provider = collection.BuildServiceProvider();
            var context = provider.GetService<DynamicContext>();
            return context;
        }

        static Dictionary<string, string> ToProperties(IDictionary<string, Object> dict) {
            return dict.ToDictionary(x => x.Key, x => x.Value.GetType().FullName);
        }

        static CommandLineOptions ParseArguments(IEnumerable<string> args) {
            var options = new CommandLineOptions();
            while (args.Count() > 1) {
                var key = args.ElementAt(0);
                var value = args.ElementAt(1);
                if (key is "--type") {
                    options.DbType = value;
                } else if (key is "--connection") {
                    options.ConnectionString = value;
                } else if (key is "--table") {
                    options.TableName = value;
                }
                args = args.Skip(2);
            }

            if (args.Count() != 0) {
                options.JsonFile = args.ElementAt(0);
            }

            return options;
        }

        static IEnumerable<IDictionary<string, Object>> GetData(string jsonFile) {
            var json = File.ReadAllText(jsonFile);
            var data = JsonConvert.DeserializeObject<ExpandoObject[]>(json);
            return data.Select(x => {
                dynamic item = x;
                return item as IDictionary<string, object>;
            });
        }

        static Type GenerateTargetType(IEnumerable<IDictionary<string, Object>> dict, string name) {
            var first = dict.First();
            var props = ToProperties(first);
            var type = ModelService.GenerateModelType(props, name);
            return type;
        }

        static void Main(string[] args) {
            var options = ParseArguments(args);
            var data = GetData(options.JsonFile).ToArray();
            var targetType = GenerateTargetType(data, options.TableName);
            var context = CreateContext(options.ConnectionString, targetType);
            var realData = ModelService.Map(data, targetType);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.AddRange(realData);
            context.SaveChanges();
        }
    }
}