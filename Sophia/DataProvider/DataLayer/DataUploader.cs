using Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DataProvider.DataLayer
{
    public class DataUploader
    {
        public static async Task UploadXmlDataAsync(string procName, string xmlDataToUpload)
        {
            try
            {
                SqlParameter[] parameters = {new SqlParameter("@i_input", xmlDataToUpload)};
                await SqlProcExecuter.ExecProcAsync(procName, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task UploadCsvDataAsync(string tableName, DataTable dataTable)
        {
            var newRowCount = dataTable.Rows.Count;

            var connectionString = AppSettings.DbConnectionString;

            var rowCountInDb = await DbAdapter.GetTableRowsCount($"[dbo].{tableName}");

            await using var dbConnection = new SqlConnection(connectionString);
            await dbConnection.OpenAsync();

            if (newRowCount < rowCountInDb / 2)  // если новое количество вдвое меньше текущего останавливаем процесс обновления
            {
                throw new DataException("Некоректна кількість записів.");
            }

            await TruncateTableAsync($"[temp].{tableName}");
            await InsertDataToTmpTableAsync(tableName, dataTable, dbConnection);
            await TruncateTableAsync($"[dbo].{tableName}");
            await DataTransferFromTmpToDboAsync(tableName);
            await dbConnection.CloseAsync();
        }

        private static async Task TruncateTableAsync(string tableName)
        {
            var parameters = new []
            {
               new SqlParameter("@i_tableName", tableName)
            };
            await SqlProcExecuter.ExecProcAsync("[dbo].[proc_TruncateTable]", parameters);
        }

        private static async Task InsertDataToTmpTableAsync(string tableName, DataTable dataTable, SqlConnection dbConnection)
        {

            switch (tableName)
            {
                case "[OpenDataAppriserReestr]":
                    AppraiserMapping(tableName, dataTable, dbConnection);
                    string fieldForHash = "fio";
                    await SetHashAsync($"[dbo].{tableName}", fieldForHash);
                    break;
                case "[OpenDataPassportInvalidate]":
                    Mapping(tableName, dataTable, dbConnection);
                    break;
                case "[OpenDataPassportInvalidateAbroad]":
                    Mapping(tableName, dataTable, dbConnection);
                    break;
                default:
                    throw new DataException("Csv file upload. Invalid table name");
            }
        }

        private static async Task SetHashAsync(string tableName, string fieldForHash)
        {
            var parameters = new[]
            {
               new SqlParameter("@i_tableName", tableName),
               new SqlParameter("@i_field", fieldForHash)
            };
            await SqlProcExecuter.ExecProcAsync("[dbo].[proc_CreateHash]", parameters);
        }

        private static void AppraiserMapping(string tableName, DataTable dataTable, SqlConnection dbConnection)
        {
            using (SqlBulkCopy s = new SqlBulkCopy(dbConnection)) // заливаем данные в темповую таблицу
            {
                s.BulkCopyTimeout = 0;

                s.DestinationTableName = $"[temp].{tableName}";

                s.ColumnMappings.Add(dataTable.Columns[0].ToString(), "fio");  //ПРІЗВИЩЕ ІМ’Я ТА ПО - БАТЬКОВІ,, 
                s.ColumnMappings.Add(dataTable.Columns[1].ToString(), "registrationCertificateNumber"); //СВІДОЦТВО ПРО РЕЄСТРАЦІЮ(НОМЕР),
                s.ColumnMappings.Add(dataTable.Columns[2].ToString(), "registrationCertificateDate");   //СВІДОЦТВО ПРО РЕЄСТРАЦІЮ(ДАТА), 
                s.ColumnMappings.Add(dataTable.Columns[3].ToString(), "orderOnNumber"); //НАКАЗ ПРО ВКЛЮЧЕННЯ(НОМЕР), 
                s.ColumnMappings.Add(dataTable.Columns[4].ToString(), "orderOnDate");   //НАКАЗ ПРО ВКЛЮЧЕННЯ(ДАТА),
                s.ColumnMappings.Add(dataTable.Columns[5].ToString(), "orderOffNumber");    //НАКАЗ ПРО ВИКЛЮЧЕННЯ(НОМЕР),
                s.ColumnMappings.Add(dataTable.Columns[6].ToString(), "orderOffDate");  //НАКАЗ ПРО ВИКЛЮЧЕННЯ(ДАТА),
                s.ColumnMappings.Add(dataTable.Columns[7].ToString(), "specialization");    //НАПРЯМ ОЦІНКИ(СПЕЦІАЛІЗАЦІЯ),
                s.ColumnMappings.Add(dataTable.Columns[8].ToString(), "region");    //РЕГІОН,
                s.ColumnMappings.Add(dataTable.Columns[9].ToString(), "infoNumber");   //ІНФОРМАЦІЯ ЗГІДНО З ДОДАТКОМ 4 ДО НАКАЗУ ФОНДУ ВІД 19.12.2001 № 2355 ОТРИМАНА(НОМЕР),
                s.ColumnMappings.Add(dataTable.Columns[10].ToString(), "infoDate"); //ІНФОРМАЦІЯ ЗГІДНО З ДОДАТКОМ 4 ДО НАКАЗУ ФОНДУ ВІД 19.12.2001 № 2355 ОТРИМАНА(ДАТА)

                s.WriteToServer(dataTable);
            }
        }

        private static void Mapping(string tableName, DataTable dataTable, SqlConnection dbConnection)
        {
            var memory = GC.GetTotalMemory(true);
            using SqlBulkCopy s = new SqlBulkCopy(dbConnection)
            {
                BulkCopyTimeout = 0, DestinationTableName = $"[temp].{tableName}"
            };
            foreach (var column in dataTable.Columns)
            {
                s.ColumnMappings.Add(column.ToString().Trim(), column.ToString().Trim());
            }
            memory = GC.GetTotalMemory(true);
            s.WriteToServer(dataTable);
        }

        private static async Task DataTransferFromTmpToDboAsync(string tableName)
        {
            tableName = tableName.Replace("[", "").Replace("]", "");
            var parameters = new []
            {
               new SqlParameter("@i_sourceSchemaName", "temp"),
               new SqlParameter("@i_sourceTableName", tableName),
               new SqlParameter("@i_destSchemaName", "dbo"),
               new SqlParameter("@i_destTableName", tableName)
            };
            await SqlProcExecuter.ExecProcAsync("[dbo].[proc_TableDataTransfer]", parameters);
        }
    }
}
