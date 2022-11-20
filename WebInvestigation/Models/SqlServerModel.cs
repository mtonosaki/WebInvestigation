// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using System.Collections.Generic;

namespace WebInvestigation.Models {
    public class SqlServerModel {
        public static readonly SqlServerModel Default = new SqlServerModel {
            ConnectionString = "Server=tcp:<your sqlserver here>.database.windows.net,1433;Initial Catalog=<your database name here>;Persist Security Info=False;User ID=<your sql server account here>;Password=<the password here>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
            Sql = "SELECT * FROM sys.objects WHERE type = 'U';",
        };
        public bool SkipSql { get; set; }
        public string ConnectionString { get; set; }
        public string Sql { get; set; }
        public string LatestSql { get; set; }
        public string ErrorMessage { get; set; }
        public List<List<string>> Table { get; set; }
    }
}
