namespace BackupYukle
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Helper.Results;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public class Program
    {
        private static void Main(string[] args) //private static readonly DateOnly projeBasDate = new DateOnly(2025, 7, 8);
        {
            Console.Write("Yapılacak işlemi onaylıyor musunuz? [Y/N] : ");
            var r = Console.ReadKey(false).Key;
            Console.WriteLine("");
            if (r == ConsoleKey.Y) { MainAsync(args).GetAwaiter().GetResult(); }
            for (var i = 5; i > 0; i--)
            {
                Console.WriteLine($"{i}. saniye kaldı");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Uygulama kapanıyor.");
            Environment.Exit(0);
        }
        private static string getconnectionstring(IConfigurationSection section) => _get.GetConnectionString(section["datasource"], "master", section["userid"], section["password"]);
        private static async Task MainAsync(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, true).Build();
            var backupDirectory = configuration["backupdirectory"];
            if (!Directory.Exists(backupDirectory))
            {
                Console.WriteLine($"\"{backupDirectory}\" dosya yolu bulunamadı!");
                return;
            }
            var _backupFiles = Directory.GetFiles(backupDirectory, "*.bak");
            if (_backupFiles.Length == 0)
            {
                Console.WriteLine("\".bak\" uzantılı bir belge bulunamadı!");
                return;
            }
            var _dap = new DapperHelper(new SqlConnection(getconnectionstring(configuration.GetSection("connectionstring"))), default);
            foreach (var item in _backupFiles)
            {
                var status = false;
                try
                {
                    await Task.Delay(1000);
                    var sw = Stopwatch.StartNew();
                    var database = Path.GetFileNameWithoutExtension(item).Split('_')[0];
                    Console.WriteLine($"{database}, Restore işlemine başlandı.");
                    Console.WriteLine($"{database}, Veritabanı üzerine kayıt var mı kontrol yapılıyor ve açık bağlantı(session) varsa bağlantılar da öldürülüyor.");
                    var _dy = (await _dap.QueryDynamicAsync(@"DECLARE @dbid SMALLINT = DB_ID(@database) 
                        IF EXISTS(SELECT D.* FROM [sys].[databases] AS D WHERE D.[database_id] = @dbid)
                        BEGIN
                          DECLARE @sql NVARCHAR(10), @sessionid SMALLINT, @sessionids VARCHAR(MAX) = ''
                          DECLARE session_cursor CURSOR FOR SELECT S.[session_id] FROM [sys].[dm_exec_sessions] AS S WHERE S.[database_id] = @dbid
                          OPEN session_cursor
                          FETCH NEXT FROM session_cursor INTO @sessionid
                          WHILE @@FETCH_STATUS = 0
                          BEGIN
                              SET @sessionids = CONCAT(@sessionids, ',', CONVERT(VARCHAR(5), @sessionid))
                              SET @sql = CONCAT('KILL', SPACE(1), CONVERT(VARCHAR(5), @sessionid))
                              EXEC sp_executesql @sql
                              FETCH NEXT FROM session_cursor INTO @sessionid
                          END
                          CLOSE session_cursor
                          DEALLOCATE session_cursor
                          SELECT CONVERT(BIT, 1) AS hasvalue, @sessionids AS sessionids
                        END
                        ELSE
                        BEGIN
                          SELECT CONVERT(BIT, 0) AS hasvalue, '' AS sessionids
                        END", new
                    {
                        database
                    }, null, CommandType.Text)).Select(x => new
                    {
                        hasvalue = (bool)x.hasvalue,
                        sessionids = (string)x.sessionids
                    }).Select(x => new
                    {
                        x.hasvalue,
                        sessionids = (x.sessionids.Length > 1 ? x.sessionids.Substring(1) : "")
                    }).FirstOrDefault();
                    if (_dy.hasvalue)
                    {
                        Console.WriteLine($"{database}, Var olan eski veritabanının sistemden kaydı silinmesi için komut iletiliyor.");
                        await Task.Delay(1000);
                        await _dap.ExecuteAsync($"EXEC [msdb].[dbo].[sp_delete_database_backuphistory] @database_name = N'{database}' DROP DATABASE [{database}]", null, null, CommandType.Text, default);
                        Console.WriteLine($"{database}, Eski veritabanı başarılı bir şekilde silindi.");
                        if (!_dy.sessionids.IsNullOrEmpty_string()) { Console.WriteLine($"{database}, Öldürülen bağlantılar(sessions): {_dy.sessionids}"); }
                    }
                    else { Console.WriteLine($"{database}, Veritabanı üzerine bir kayıt yok."); }
                    await Task.Delay(1000);
                    await _dap.ExecuteAsync(@"DECLARE @sql NVARCHAR(MAX) = 'RESTORE DATABASE [{database}] FROM DISK = N''{item}'' WITH FILE = 1, MOVE N''{database}'' TO N''' + CONVERT(NVARCHAR(MAX), SERVERPROPERTY('instancedefaultdatapath')) + '{database}.mdf'', MOVE N''{database}_log'' TO N''' + CONVERT(NVARCHAR(MAX), SERVERPROPERTY('instancedefaultlogpath')) + '{database}_log.ldf'', NOUNLOAD, STATS = 5' EXEC sp_executesql @sql".FormatVar(new
                    {
                        database,
                        item,
                    }), null, null, CommandType.Text, default);
                    Console.WriteLine($"{database}, Veritabanı başarıyla geri yüklendi. Süre: {new TimeSpanDiffResult(sw.StopThenGetElapsed()).FormatTimeSpan()}");
                    status = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"HATA: \"{item}\"");
                    foreach (var itemex in ex.AllExceptionMessage()) { Console.WriteLine($"HATA Detay: {itemex}"); }
                }
                finally { if (status) { _file.FileExiststhenDelete(item); } }
            }
            await _dap.EnsureConnectionCloseAsync();
        }
    }
}