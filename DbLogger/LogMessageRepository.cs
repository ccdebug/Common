using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace DbLogger
{
    public class LogMessageRepository
    {
        public int Insert(LogMessage logMessage)
        {
            string tableName = TableUtils.GetTableName(logMessage.IsHandle);

            string sql = 
string.Format(@"INSERT INTO Ticket_Log4net_Database.dbo.{0}(ikey, username, logtime, clientip, module, orderno, logtype, content, ServerIP, KeyWord)
VALUES(@ikey, @username, @logtime, @clientip, @module, @orderno, @logtype, @content, @ServerIP, @KeyWord)", tableName);

            using (var conn = new SqlConnection(ConnectionStrings.LogDb))
            {
                return conn.Execute(sql, logMessage);
            }
        }

        public DateTime GetDbServerTime()
        {
            const string sql = "SELECT GETDATE()";
            using (var conn = new SqlConnection(ConnectionStrings.LogDb))
            {
                return conn.ExecuteScalar<DateTime>(sql);
            }
        }
    }
}
