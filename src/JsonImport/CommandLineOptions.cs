namespace DynamicModel {
    class CommandLineOptions {
        public string ConnectionString { set; get; } = "";
        public string DbType { set; get; } = "postgres";
        public string JsonFile { set; get; } = "../../resource/Hello.json";
        public string TableName { set; get; } = "Table";
    }
}
