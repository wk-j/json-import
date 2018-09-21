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

    class Info {
        public Type TargetType { set; get; }
        public DynamicContext Context { set; get; }
    }

    partial class Program {

        static Info CreateContext<T>(string connectionString, string tableName) {
            var type = ModelService.GenerateModelType(typeof(T), tableName);
            var modelOptions = new ModelOptions {
                ModelType = type,
                TypeName = tableName
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
            return new Info {
                Context = context,
                TargetType = type
            };
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

        static void Main(string[] args) {
            var options = ParseArguments(args);
            var json = File.ReadAllText(options.JsonFile);
            var data = JsonConvert.DeserializeObject<ExpandoObject[]>(json);
            dynamic item = data[0];
            var dict = item as Dictionary<string, object>;

            var info = CreateContext<Dictionary<string, Object>>(options.ConnectionString, options.TableName);
            var context = info.Context;
            var targetType = info.TargetType;
            var realData = ModelService.Map(data, targetType);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.AddRange(realData);
            context.SaveChanges();
        }
    }
}