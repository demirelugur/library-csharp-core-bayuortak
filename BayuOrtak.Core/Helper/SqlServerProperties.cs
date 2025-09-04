namespace BayuOrtak.Core.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public sealed class SqlServerProperties : IDisposable
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        /// <summary>
        /// SQL Server ürün versiyonunu alır veya ayarlar.
        /// </summary>
        public string? productversion { get; set; }
        /// <summary>
        /// SQL Server ürün seviyesini alır veya ayarlar.
        /// </summary>
        public string? productlevel { get; set; }
        /// <summary>
        /// SQL Server sürümünü alır veya ayarlar.
        /// </summary>
        public string? edition { get; set; }
        /// <summary>
        /// SQL Server&#39;ın CLR sürümünü alır veya ayarlar.
        /// </summary>
        public string? buildclrversion { get; set; }
        /// <summary>
        /// SQL Server&#39;ın kullandığı sıralama (collation) ayarını alır veya ayarlar.
        /// </summary>
        public string? collation { get; set; }
        /// <summary>
        /// SQL Server sunucu adını alır veya ayarlar.
        /// </summary>
        public string? servername { get; set; }
        /// <summary>
        /// SQL Server örnek adını alır veya ayarlar.
        /// </summary>
        public string? instancename { get; set; }
        /// <summary>
        /// SQL Server örneğinin varsayılan veri dosyası yolunu alır veya ayarlar.
        /// </summary>
        public string? instancedefaultdatapath { get; set; }
        /// <summary>
        /// SQL Server örneğinin varsayılan log dosyası yolunu alır veya ayarlar.
        /// </summary>
        public string? instancedefaultlogpath { get; set; }
        /// <summary>
        /// SQL Server için LCID (Locale Identifier) değerini alır veya ayarlar.
        /// </summary>
        public int? lcid { get; set; }
        public SqlServerProperties() : this("", "", "", "", "", "", "", "", "", default) { }
        public SqlServerProperties(string productversion, string productlevel, string edition, string buildclrversion, string collation, string servername, string instancename, string instancedefaultdatapath, string instancedefaultlogpath, int? lcid)
        {
            this.productversion = productversion;
            this.productlevel = productlevel;
            this.edition = edition;
            this.buildclrversion = buildclrversion;
            this.collation = collation;
            this.servername = servername;
            this.instancename = instancename;
            this.instancedefaultdatapath = instancedefaultdatapath;
            this.instancedefaultlogpath = instancedefaultlogpath;
            this.lcid = lcid;
        }
        /// <summary>
        /// SQL Server özelliklerini almak için gerekli SQL sorgusunu oluşturur. Bu sorgu, sınıfın özelliklerini temsil eden SQL Server sistem işlevlerini kullanır.
        /// <code>
        /// Örnek Sorgu:
        /// SELECT SERVERPROPERTY(&#39;productversion&#39;) AS [productversion],
        ///        SERVERPROPERTY(&#39;productlevel&#39;) AS [productlevel],
        ///        SERVERPROPERTY(&#39;edition&#39;) AS [edition],
        ///        SERVERPROPERTY(&#39;buildclrversion&#39;) AS [buildclrversion],
        ///        SERVERPROPERTY(&#39;collation&#39;) AS [collation],
        ///        SERVERPROPERTY(&#39;servername&#39;) AS [servername],
        ///        SERVERPROPERTY(&#39;instancename&#39;) AS [instancename],
        ///        SERVERPROPERTY(&#39;instancedefaultdatapath&#39;) AS [instancedefaultdatapath],
        ///        SERVERPROPERTY(&#39;instancedefaultlogpath&#39;) AS [instancedefaultlogpath],
        ///        SERVERPROPERTY(&#39;lcid&#39;) AS [lcid]
        /// </code>
        /// </summary>
        /// <returns>SQL Server&#39;dan alınacak özellikleri içeren bir SQL sorgusu.</returns>
        public static string query()
        {
            var _r = new List<string>();
            foreach (var item in typeof(SqlServerProperties).GetProperties().Select(x => x.Name).ToArray()) { _r.Add($"SERVERPROPERTY('{item}') AS [{item}]"); }
            return String.Concat("SELECT ", String.Join(", ", _r));
        }
    }
}