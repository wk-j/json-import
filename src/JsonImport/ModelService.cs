using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Dynamic;

namespace DynamicModel {
    class ModelService {
        public static object[] Map(IDictionary<string, object>[] source, Type targetType) {
            Mapper.Initialize(cfg => {
                cfg.CreateMap(typeof(Dictionary<string, object>), targetType);
            });

            return source.Select(x => {
                var targetValue = Activator.CreateInstance(targetType);
                var rs = Mapper.Map(x, targetValue);
                return rs;
            }).ToArray();
        }

        public static Type GenerateModelType(Dictionary<string, string> props, string name) {
            var src = $@"
                using DynamicModel;
                using System;
                public class {name}: ModelBase {{
                    {
                        props.Select(kv => $"public {kv.Value} {kv.Key} {{ set;get; }}")
                            .Aggregate((a, x) => $"{a}\n\t\t{x}")
                    }
                }}
                return typeof({name});
            ";
            var reference = ScriptOptions.Default.WithReferences(Assembly.GetExecutingAssembly());
            var script = CSharpScript.Create(src, reference);
            var compile = script.Compile();
            var modelType = script.RunAsync().Result.ReturnValue as Type;
            return modelType;
        }
    }
}
