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
using Microsoft.EntityFrameworkCore;

namespace DynamicModel {
    partial class Program {

        static (DynamicContext, Type) CreateContext(string connectionString, string tableName, Type model) {
            var type = ModelUtility.GenerateModelType(model, tableName);
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
            return (context, type);
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

        static object[] GetDatas() {
            return new[] {
                new {
                    A = 100,
                    B = 100,
                    C = new DateTime(2018,1,2)
                },
                new {
                    A = 200,
                    B = 200,
                    C = new DateTime(2018,1,1)
                }
            };
        }

        static void Main(string[] args) {
            var options = ParseArguments(args);
            var json = File.ReadAllText(options.JsonFile);

            var datas = GetDatas();
            var type = datas[0].GetType();
            var (context, targetType) = CreateContext(options.ConnectionString, options.TableName, type);

            var realData = ModelUtility.Map(datas, targetType);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.AddRange(realData);
        }
    }
}