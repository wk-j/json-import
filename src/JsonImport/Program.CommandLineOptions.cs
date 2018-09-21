namespace DynamicModel {
    partial class Program {
        class CommandLineOptions {
            public string ConnectionString { set; get; } = "";
            public string DbType { set; get; } = "postgres";
            public string JsonFile { set; get; } = "";
            public string TableName { set; get; } = "Table";
        }
    }
}
