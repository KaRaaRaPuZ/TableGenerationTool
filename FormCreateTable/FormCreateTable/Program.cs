internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: CsvToSql <csv_file_path> <database_type>");
            return;
        }

        string csvFilePath = args[0];
        string databaseType = args[1].ToLower();

        if (!File.Exists(csvFilePath))
        {
            Console.WriteLine("CSV file does not exist.");
            return;
        }

        string[] csvHeader;
        using (var reader = new StreamReader(csvFilePath))
        {
            csvHeader = reader.ReadLine().Split(',');
        }

        string createTableStatement = GenerateCreateTableStatement(csvHeader, databaseType);
        if (createTableStatement == null)
        {
            Console.WriteLine("Unsupported database type.");
            return;
        }

        string csvFilenameWithoutExtension = Path.GetFileNameWithoutExtension(csvFilePath);
        string outputFilePath = Path.Combine(Path.GetDirectoryName(csvFilePath), "CREATETABLE_"+csvFilenameWithoutExtension+".sql");
        File.WriteAllText(outputFilePath, createTableStatement);

        Console.WriteLine($"SQL CREATE TABLE script generated: {outputFilePath}");
    }

    static string GenerateCreateTableStatement(string[] columns, string dbType)
    {
        switch (dbType)
        {
            case "tsql":
                return GenerateTsqlCreateTable(columns);
            case "oracle":
                return GenerateOracleCreateTable(columns);
            case "postgres":
                return GeneratePostgresCreateTable(columns);
            default:
                return null;
        }
    }

    static string GenerateTsqlCreateTable(string[] columns)
    {
        var columnDefinitions = columns.Select(col => $"[{col}] NVARCHAR(200)");
        return $"CREATE TABLE MyTable (\n    {string.Join(",\n    ", columnDefinitions)}\n);";
    }

    static string GenerateOracleCreateTable(string[] columns)
    {
        var columnDefinitions = columns.Select(col => $"\"{col}\" VARCHAR2(4000)");
        return $"CREATE TABLE MyTable (\n    {string.Join(",\n    ", columnDefinitions)}\n);";
    }

    static string GeneratePostgresCreateTable(string[] columns)
    {
        var columnDefinitions = columns.Select(col => $"\"{col}\" TEXT");
        return $"CREATE TABLE MyTable (\n    {string.Join(",\n    ", columnDefinitions)}\n);";
    }
}