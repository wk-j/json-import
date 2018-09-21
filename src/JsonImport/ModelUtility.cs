using System;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Linq;
using AutoMapper;

namespace DynamicModel {
    class ModelUtility {
        public static object[] Map(object[] source, Type targetType) {
            var type = source.ElementAt(0).GetType();
            Mapper.Initialize(cfg => cfg.CreateMap(type, targetType));

            return source.Select(x => {
                var targetValue = Activator.CreateInstance(targetType);
                return Mapper.Map(x, targetValue);
            }).ToArray();
        }

        public static Type GenerateModelType(Type source, string name) {
            var src = $@"
using DynamicModel;
using System;
public class {name}: ModelBase {{
    {
        source.GetProperties()
            .Select(x => $"public {x.PropertyType} {x.Name} {{ set;get; }}")
            .Aggregate((a, x) => $"{a}\n               {x}")
    }
}}
return typeof({name});
            ";

            Console.WriteLine(src);

            var reference = ScriptOptions.Default.WithReferences(Assembly.GetExecutingAssembly());
            var script = CSharpScript.Create(src, reference);
            var compile = script.Compile();
            var modelType = script.RunAsync().Result.ReturnValue as Type;
            return modelType;
        }
    }
}
