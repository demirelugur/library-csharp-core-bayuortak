namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Helper.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Net.Mail;
    using System.Reflection;
    using System.Web;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class OtherExtensions
    {
        /// <summary>
        /// Belirtilen varlığın (entity) bir veya daha fazla özelliğinin değiştirilip değiştirilmediğini kontrol eder.
        /// </summary>
        /// <typeparam name="T">Kontrol edilecek varlık türü.</typeparam>
        /// <param name="dbcontext">DbContext örneği.</param>
        /// <param name="entity">Değişiklik durumu kontrol edilecek varlık.</param>
        /// <param name="expressions">Kontrol edilecek özelliklerin ifadeleri.</param>
        /// <returns>Değiştirilmişse <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsModified<T>(this DbContext dbcontext, T entity, params Expression<Func<T, object>>[] expressions) where T : class
        {
            var _entry = dbcontext.Entry(entity);
            var _ie = typeof(T).GetProperties().Where(x => x.IsMappedProperty() && _entry.Property(x.Name).IsModified).ToArray();
            var _columns = (expressions ?? Array.Empty<Expression<Func<T, object>>>()).Select(x => x.GetExpressionName()).ToArray();
            if (_columns.Length == 0) { return _ie.Length > 0; }
            return _ie.Any(x => _columns.Contains(x.Name));
        }
        /// <summary>
        /// Belirli bir bileşik anahtar özelliği ile eski varlığın güncellenmesini sağlar.
        /// </summary>
        public static async Task<T> SetCompositeKeyAsync<T, CompositeKey>(this DbContext dbcontext, bool issavechanges, T oldentity, Expression<Func<T, CompositeKey>> compositekey, CompositeKey compositekeyvalue, string dil, CancellationToken cancellationtoken) where T : class, new()
        {
            var _t = typeof(T);
            var _tablename = _t.GetTableName(false);
            var _c = compositekey.GetExpressionName();
            var _props = _t.GetProperties().Where(x => x.IsMappedProperty()).Select(x => new
            {
                name = x.Name,
                setcolumn = x.Name == _c,
                iscompositekey = x.IsPK() && x.GetDatabaseGeneratedOption() == DatabaseGeneratedOption.None
            }).ToArray();
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (_props.Where(x => x.iscompositekey).Count() < 2)
            {
                if (dil == "en") { throw new KeyNotFoundException($"The \"{_tablename}\" table must contain at least 2 properties with \"{typeof(KeyAttribute).FullName}\" or \"{typeof(DatabaseGeneratedAttribute).FullName}\" attributes to continue processing!"); }
                throw new KeyNotFoundException($"İşleme devam edebilmek için \"{_tablename}\" tablosunda en az 2 özelliğin \"{typeof(KeyAttribute).FullName}\" veya \"{typeof(DatabaseGeneratedAttribute).FullName}\" içermesi gerekmektedir!");
            }
            if (_props.Any(x => x.setcolumn && x.iscompositekey))
            {
                var _newentity = new T();
                var _entry = dbcontext.Entry(oldentity);
                var _dbset = dbcontext.Set<T>();
                _dbset.Attach(oldentity);
                foreach (var item in _props.Select(x => new
                {
                    x.name,
                    x.setcolumn
                }).ToArray()) { _other.SetPropertyValue(_newentity, item.name, (item.setcolumn ? compositekeyvalue : _entry.Property(item.name).OriginalValue)); }
                await _dbset.AddAsync(_newentity, cancellationtoken);
                _dbset.Remove(oldentity);
                if (issavechanges) { await dbcontext.SaveChangesAsync(cancellationtoken); }
                return _newentity;
            }
            if (dil == "en") { throw new Exception($"The property \"{_c}\" in table \"{_tablename}\" must have either \"{typeof(KeyAttribute).FullName}\" or \"{typeof(DatabaseGeneratedAttribute).FullName}\" specified!"); }
            throw new Exception($"\"{_tablename}\" tablosundaki \"{_c}\" özelliğinde \"{typeof(KeyAttribute).FullName}\" veya \"{typeof(DatabaseGeneratedAttribute).FullName}\" belirtilmelidir!");
        }
        /// <summary>
        /// Bir decimal değeri, InvariantCulture kullanarak string&#39;e dönüştürür.
        /// </summary>
        /// <param name="value">Dönüştürülecek decimal değer.</param>
        /// <returns>Decimal değerin string temsilini döner.</returns>
        /// <remarks>Bu metodun ters işlemi için <code>Convert.ToDecimal(value, CultureInfo.InvariantCulture);</code> kullanılabilir.</remarks>
        public static string ToStringInvariantCulture(this decimal value) => value.ToString(CultureInfo.InvariantCulture);
        /// <summary>
        /// Verilen bir ondalık değeri belirtilen ondalık basamak sayısına yuvarlayarak string olarak döndürür. İsteğe bağlı olarak, yuvarlama yöntemi belirtilebilir.
        /// </summary>
        /// <param name="d">Yuvarlanacak <see cref="decimal"/> değeri.</param>
        /// <param name="decimals">Yuvarlanacak ondalık basamak sayısı (varsayılan olarak 2).</param>
        /// <param name="midpointrounding">Yuvarlama yöntemi (varsayılan olarak <see cref="MidpointRounding.AwayFromZero"/>).</param>
        /// <returns>Yuvarlanmış değerin string temsili.</returns>
        public static string ToRound(this decimal d, int decimals = 2, MidpointRounding midpointrounding = MidpointRounding.AwayFromZero) => Decimal.Round(d, decimals, midpointrounding).ToString();
        /// <summary>
        /// Verilen e-Posta adresinin T.C. Bayburt Üniversitesi&#39;ne ait olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="mailaddress">MailAddress nesnesi.</param>
        /// <returns>T.C. Bayburt Üniversitesi e-Postası ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsBayburtUniEPosta(this MailAddress mailaddress) => (mailaddress != null && mailaddress.Host == "bayburt.edu.tr");
        /// <summary>
        /// Verilen e-Posta adresindeki &quot;@&quot; karakterini &quot;[at]&quot; ile değiştirir.
        /// </summary>
        /// <param name="mailaddress">MailAddress nesnesi.</param>
        /// <returns>Dönüştürülmüş e-Posta adresi.</returns>
        /// <exception cref="ArgumentNullException">Verilen e-Posta adresi null ise fırlatılır.</exception>
        public static string ReplaceAT(this MailAddress mailaddress)
        {
            Guard.CheckNull(mailaddress, nameof(mailaddress));
            return mailaddress.Address.Replace("@", "[at]");
        }
        /// <summary>
        /// Verilen validation bağlamında bir özelliğin gerekli olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="validationcontext">ValidationContext nesnesi.</param>
        /// <returns>Gerekli ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsRequiredAttribute(this ValidationContext validationcontext)
        {
            if (validationcontext == null) { return false; }
            var pi = validationcontext.ObjectInstance.GetType().GetProperty(validationcontext.MemberName);
            if (pi == null) { return false; }
            return _try.TryCustomAttribute(pi, out RequiredAttribute _);
        }
        /// <summary>
        /// Verilen doğrulama bağlamına göre, belirtilen özelliğin değerini günceller. Eğer özellik yazılabilir durumdaysa, yeni değer atanır.
        /// </summary>
        /// <param name="validationcontext">Doğrulama işlemi sırasında bağlam bilgilerini içeren nesne.</param>
        /// <param name="value">Güncellenmek istenen yeni değer.</param>
        /// <exception cref="ArgumentNullException">Eğer <paramref name="validationcontext"/> null ise tetiklenir.</exception>
        public static void SetValidatePropertyValue(this ValidationContext validationcontext, object value)
        {
            if (validationcontext != null)
            {
                var _propertyinfo = validationcontext.ObjectType.GetProperty(validationcontext.MemberName);
                if (_propertyinfo != null && _propertyinfo.CanWrite) { _propertyinfo.SetValue(validationcontext.ObjectInstance, value); }
            }
        }
        /// <summary>
        /// Verilen sayının asal olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek pozitif tamsayı.</param>
        /// <returns>Asal ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsPrimeNumber(this ulong value)
        {
            if (value < 2) { return false; }
            if (value == 2) { return true; }
            if ((value % 2) == 0) { return false; }
            if (value == 5) { return true; }
            if ((value % 5) == 0) { return false; }
            ulong i, limit = Convert.ToUInt64(Math.Sqrt(value));
            for (i = 3; i <= limit; i += 2) { if ((value % i) == 0) { return false; } }
            return true;
        }
        /// <summary>
        /// Unix zaman damgasını, yerel bir <see cref="DateTime"/> değerine dönüştürür.
        /// </summary>
        /// <param name="gettime">Unix zaman damgası (milisaniye cinsinden).</param>
        /// <returns>Dönüştürülen yerel <see cref="DateTime"/> değeri.</returns>
        public static DateTime ToJsDate(this long gettime) => DateTime.UnixEpoch.AddMilliseconds(Convert.ToDouble(gettime)).ToLocalTime();
        /// <summary>
        /// Active Directory&#39;de kullanılan FILETIME formatındaki bir değeri (1 Ocak 1601&#39;den itibaren 100 nanosaniye cinsinden tick) UTC zaman diliminde bir DateTime nesnesine çevirir. 
        /// <para>
        /// Eğer filetime değeri 0 veya <see cref="Int64.MaxValue"/> ise, hesap süresiz kabul edilir ve <see cref="DateTime.MaxValue"/> döndürülür. Geçersiz bir filetime değeri durumunda null döner.
        /// </para>
        /// </summary>
        /// <param name="filetime">Çevrilecek 64 bitlik FILETIME değeri.</param>
        /// <returns>Başarılı olursa DateTime nesnesi, süresiz hesaplar için DateTime.MaxValue, geçersiz değerler için null.</returns>
        public static DateTime? ToFileTimeUTC(this long filetime)
        {
            if (filetime.Includes(0, Int64.MaxValue)) { return DateTime.MaxValue; }
            try { return DateTime.FromFileTimeUtc(filetime); }
            catch { return null; }
        }
        /// <summary>
        /// Belirtilen <see cref="ActionDescriptor"/> nesnesinin dönüş tipinin &quot;ActionResult&quot; veya &quot;IActionResult&quot; olup olmadığını ve &quot;public&quot; erişim seviyesinde olup olmadığını kontrol eder. Eğer dönüş tipi &quot;Task&lt;ActionResult&gt;&quot; veya &quot;Task&lt;IActionResult&gt;&quot; ise bunu da geçerli kabul eder.
        /// </summary>
        /// <param name="value">Kontrol edilecek <see cref="ActionDescriptor"/> nesnesi.</param>
        /// <returns>
        /// Metodun &quot;public&quot; erişim seviyesinde olması ve dönüş tipinin &quot;ActionResult&quot;, &quot;IActionResult&quot;, &quot;Task&lt;ActionResult&gt;&quot; veya &quot;Task&lt;IActionResult&gt;&quot; olması durumunda &quot;true&quot; döner; aksi takdirde &quot;false&quot; döner.
        /// </returns>
        public static bool IsPublicActionResult(this ActionDescriptor value)
        {
            if (value is ControllerActionDescriptor _cad)
            {
                var _methodinfo = _cad.MethodInfo;
                if (!_methodinfo.IsPublic) { return false; }
                var _returntypes = new Type[] { typeof(ActionResult), typeof(IActionResult) };
                if (_returntypes.Contains(_methodinfo.ReturnType)) { return true; }
                if (_methodinfo.ReturnType.IsGenericType && _methodinfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)) { return _returntypes.Contains(_methodinfo.ReturnType.GenericTypeArguments[0]); }
            }
            return false;
        }
        /// <summary>
        /// Verilen ifadenin adını alır.
        /// </summary>
        /// <param name="expression">İfade.</param>
        /// <returns>İfadenin adı.</returns>
        /// <exception cref="ArgumentException">İfade geçersiz ise fırlatılır.</exception>
        public static string GetExpressionName(this Expression expression)
        {
            string _r = "", r = "";
            if (expression is LambdaExpression _le) { expression = _le.Body; }
            if (expression is MemberExpression _me) { r = _me.Member.Name; }
            else if (expression is UnaryExpression _ue)
            {
                if (_ue.Operand is MemberExpression _ume) { r = _ume.Member.Name; }
                else if ((_ue.Operand is MethodCallExpression _umce) && (_umce.Object is ConstantExpression _uce) && _try.TryMemberDynamic(_uce.Value.ToDynamic(), nameof(MemberInfo.Name), out _r)) { r = _r.ToStringOrEmpty(); }
            }
            if (r == "") { throw new ArgumentException($"\"{expression.ToString()}\" değeri uyumsuzdur!", nameof(expression)); }
            return r;
        }
        /// <summary>
        /// Bir Guid&#39;i SQL UNIQUEIDENTIFIER biçimine dönüştürür.
        /// </summary>
        /// <param name="guid">Dönüştürülecek Guid.</param>
        /// <returns>Guid&#39;in SQL biçimindeki string temsilini döner.</returns>
        public static string ToSqlUniqueIdentifier(this Guid guid) => $"CONVERT(UNIQUEIDENTIFIER, '{guid.ToString().ToUpper()}')";
        /// <summary>
        /// Verilen <see cref="SqlDbType"/> enum değerini, SQL Server sistem tür kimliğine (<c>[system_type_id]</c>) dönüştürür. Bu kimlikler, SQL Server&#39;ın [sys].[types] sistem tablosunda bulunan ve her veri türü için benzersiz olan sayısal değerlerdir.
        /// </summary>
        /// <param name="type">Dönüştürülecek <see cref="SqlDbType"/> enum değeri.</param>
        /// <returns>SQL Server sistem tür kimliği (<c>[system_type_id]</c>) değeri</returns>
        /// <exception cref="NotSupportedException">Desteklenmeyen bir <see cref="SqlDbType"/> değeri verildiğinde fırlatılır.</exception>
        /// <remarks>SELECT [name], [system_type_id] FROM [sys].[types]</remarks>
        public static int ToSystemTypeId(this SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.Image: return 34;
                case SqlDbType.Text: return 35;
                case SqlDbType.UniqueIdentifier: return 36;
                case SqlDbType.Date: return 40;
                case SqlDbType.Time: return 41;
                case SqlDbType.DateTime2: return 42;
                case SqlDbType.DateTimeOffset: return 43;
                case SqlDbType.TinyInt: return 48;
                case SqlDbType.SmallInt: return 52;
                case SqlDbType.Int: return 56;
                case SqlDbType.SmallDateTime: return 58;
                case SqlDbType.Real: return 59;
                case SqlDbType.Money: return 60;
                case SqlDbType.DateTime: return 61;
                case SqlDbType.Float: return 62;
                case SqlDbType.NText: return 99;
                case SqlDbType.Bit: return 104;
                case SqlDbType.Decimal: return 106;
                case SqlDbType.SmallMoney: return 122;
                case SqlDbType.BigInt: return 127;
                case SqlDbType.VarBinary: return 165;
                case SqlDbType.VarChar: return 167;
                case SqlDbType.Binary: return 173;
                case SqlDbType.Char: return 175;
                case SqlDbType.Timestamp: return 189;
                case SqlDbType.NVarChar: return 231;
                case SqlDbType.NChar: return 239;
                case SqlDbType.Xml: return 241;
                default: throw _other.ThrowNotSupportedForEnum<SqlDbType>();
            }
        }
        /// <summary>
        /// Verilen <see cref="SqlDbType"/> enum değerini, ADO.NET&#39;in genel veri türü olan <see cref="DbType"/>&#39;a dönüştürür. SQL Server&#39;a özgü veri türlerini platform bağımsız <see cref="DbType"/> türlerine çevirir.
        /// </summary>
        /// <param name="type">Dönüştürülecek <see cref="SqlDbType"/> enum değeri.</param>
        /// <returns>Eşdeğer <see cref="DbType"/> enum değeri.</returns>
        /// <exception cref="NotSupportedException">Desteklenmeyen bir <see cref="SqlDbType"/> değeri verildiğinde fırlatılır.</exception>
        public static DbType ToDbType(this SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.UniqueIdentifier: return DbType.Guid;
                case SqlDbType.Date: return DbType.Date;
                case SqlDbType.Time: return DbType.Time;
                case SqlDbType.DateTime2: return DbType.DateTime2;
                case SqlDbType.DateTimeOffset: return DbType.DateTimeOffset;
                case SqlDbType.TinyInt: return DbType.Byte;
                case SqlDbType.SmallInt: return DbType.Int16;
                case SqlDbType.Int: return DbType.Int32;
                case SqlDbType.Real: return DbType.Single;
                case SqlDbType.Money: return DbType.Currency;
                case SqlDbType.DateTime: return DbType.DateTime;
                case SqlDbType.Float: return DbType.Double;
                case SqlDbType.Bit: return DbType.Boolean;
                case SqlDbType.Decimal: return DbType.Decimal;
                case SqlDbType.BigInt: return DbType.Int64;
                case SqlDbType.VarChar: return DbType.AnsiString;
                case SqlDbType.Binary: return DbType.Binary;
                case SqlDbType.Char: return DbType.AnsiStringFixedLength;
                case SqlDbType.NVarChar: return DbType.String;
                case SqlDbType.NChar: return DbType.StringFixedLength;
                case SqlDbType.Xml: return DbType.Xml;
                default: throw _other.ThrowNotSupportedForEnum<SqlDbType>();
            }
        }
        /// <summary>
        /// Verilen <see cref="DbType"/> enum değerini, SQL Server&#39;a özgü <see cref="SqlDbType"/> enum değerine dönüştürür. ADO.NET&#39;in genel veri türleri (<see cref="DbType"/>) ile SQL Server&#39;ın özel veri türleri (<see cref="SqlDbType"/>) arasında eşleme yapar.
        /// </summary>
        /// <param name="type">Dönüştürülecek <see cref="DbType"/> enum değeri.</param>
        /// <returns>Eşdeğer <see cref="SqlDbType"/> enum değeri.</returns>
        /// <exception cref="NotSupportedException">Desteklenmeyen bir <see cref="DbType"/> değeri verildiğinde fırlatılır.</exception>
        public static SqlDbType ToSqlDbType(this DbType type)
        {
            switch (type)
            {
                case DbType.Guid: return SqlDbType.UniqueIdentifier;
                case DbType.Date: return SqlDbType.Date;
                case DbType.Time: return SqlDbType.Time;
                case DbType.DateTime2: return SqlDbType.DateTime2;
                case DbType.DateTimeOffset: return SqlDbType.DateTimeOffset;
                case DbType.Byte: return SqlDbType.TinyInt;
                case DbType.Int16: return SqlDbType.SmallInt;
                case DbType.Int32: return SqlDbType.Int;
                case DbType.Single: return SqlDbType.Real;
                case DbType.Currency: return SqlDbType.Money;
                case DbType.DateTime: return SqlDbType.DateTime;
                case DbType.Double: return SqlDbType.Float;
                case DbType.Boolean: return SqlDbType.Bit;
                case DbType.Decimal: return SqlDbType.Decimal;
                case DbType.Int64: return SqlDbType.BigInt;
                case DbType.AnsiString: return SqlDbType.VarChar;
                case DbType.Binary: return SqlDbType.Binary;
                case DbType.AnsiStringFixedLength: return SqlDbType.Char;
                case DbType.String: return SqlDbType.NVarChar;
                case DbType.StringFixedLength: return SqlDbType.NChar;
                case DbType.Xml: return SqlDbType.Xml;
                default: throw _other.ThrowNotSupportedForEnum<DbType>();
            }
        }
        /// <summary>
        /// Bir <see cref="JToken"/> nesnesini belirtilen <typeparamref name="T"/> türündeki bir diziye dönüştürür. Eğer <see cref="JToken"/> null ise boş bir dizi döner, array türünde ise içindeki değerleri <typeparamref name="T"/> türüne çevirip dizi olarak döner. Diğer durumlarda bir istisna fırlatır.
        /// </summary>
        /// <typeparam name="T">Dönüştürülecek hedef veri türü.</typeparam>
        /// <param name="jtoken">Dönüştürülecek <see cref="JToken"/> nesnesi.</param>
        /// <returns><typeparamref name="T"/> türünden bir dizi.</returns>
        /// <exception cref="NotSupportedException"><see cref="JToken"/> türü null veya array değilse fırlatılır.</exception>
        public static T[] ToArrayFromJToken<T>(this JToken jtoken)
        {
            if (jtoken == null || jtoken.Type == JTokenType.Null) { return Array.Empty<T>(); }
            if (jtoken.Type == JTokenType.Array) { return jtoken.Select(x => x.Value<T>()).ToArray(); }
            throw new NotSupportedException($"\"{nameof(jtoken)}\" türü uyumsuzdur!");
        }
        /// <summary>
        /// <see cref="QueryString"/> içindeki belirtilen anahtarı alır ve uygun türde bir değere dönüştürür. Eğer anahtar bulunamazsa veya dönüştürme başarısız olursa, varsayılan değeri döner.
        /// </summary>
        /// <typeparam name="T">Dönüştürülecek hedef tür.</typeparam>
        /// <param name="querystring">İçinde sorgu parametrelerini barındıran <see cref="QueryString"/> nesnesi.</param>
        /// <param name="propertyname">Alınacak sorgu parametresinin adı (anahtar).</param>
        /// <returns>Başarılıysa sorgu parametresi uygun türe dönüştürülür, aksi halde varsayılan değer döner.</returns>
        public static T ToKeyValueParseOrDefault_querystring<T>(this QueryString querystring, string propertyname)
        {
            var _querydic = (querystring.HasValue ? HttpUtility.ParseQueryString(querystring.Value) : new NameValueCollection());
            propertyname = propertyname.ToStringOrEmpty();
            if (_querydic.AllKeys.Contains(propertyname)) { return _querydic[propertyname].ParseOrDefault<T>(); }
            return default;
        }
        /// <summary>
        /// Stopwatch&#39;ı durdurur ve geçen süreyi döner.
        /// </summary>
        /// <param name="stopwatch">Zamanlayıcı nesnesi.</param>
        /// <returns>Durdurulduktan sonra geçen süre.</returns>
        public static TimeSpan StopThenGetElapsed(this Stopwatch stopwatch)
        {
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
        /// <summary>
        /// <paramref name="data"/> verisini önbelleğe ekler ve başarılı işlem sonucunu (<see cref="IslemSonucResult{T}"/>) döndürür.
        /// </summary>
        /// <typeparam name="T">Önbelleğe eklenecek verinin tipi.</typeparam>
        /// <param name="memorycache">Önbellek nesnesi.</param>
        /// <param name="cachekey">Önbelleğe eklenecek değerin anahtarı.</param>
        /// <param name="data">Önbelleğe eklenecek veri.</param>
        /// <param name="timespan">Önbellekte tutulma süresi. Boş bırakılırsa varsayılan olarak 1 dakika kullanılır.</param>
        /// <returns>Başarılı işlem sonucunu temsil eden <see cref="IslemSonucResult{T}"/> nesnesi.</returns>
        public static IslemSonucResult<T> SetCacheAndReturnSuccess<T>(this IMemoryCache memorycache, object cachekey, T data, TimeSpan? timespan = null)
        {
            memorycache.Set(cachekey, data, timespan ?? TimeSpan.FromMinutes(1));
            return new(data, true, default);
        }
    }
}