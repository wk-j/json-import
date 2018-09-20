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

        class CommandLineOptions {
            public string ConnectionString { set; get; } = "";
            public string DbType { set; get; } = "postgres";
            public string JsonFile { set; get; } = "";
            public string TableName { set; get; } = "Table";
        }

        static DynamicContext CreateContext(string connectionString, string tableName, Type model) {
            var modelOptions = new ModelOptions {
                ModelType = model,
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
            return context;
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
            var obj = JArray.Parse(json);
            var obj0 = obj[0];

            // var type = obj[0].GetType();
            // var context = CreateContext(options.ConnectionString, options.TableName, x.GetType());
            // context.Database.EnsureDeleted();
            // context.Database.EnsureCreated();
        }
    }
}
