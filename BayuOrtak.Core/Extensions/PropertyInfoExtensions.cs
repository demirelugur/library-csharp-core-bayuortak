namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Belirtilen özellik bilgilerinin haritalanmış bir özellik olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="propertyinfo">Kontrol edilecek özellik bilgisi.</param>
        /// <returns>Haritalanmış bir özellik ise <see langword="true"/>, değilse false <see langword="false"/>.</returns>
        public static bool IsMappedProperty(this PropertyInfo propertyinfo)
        {
            if (propertyinfo == null) { return false; }
            if (!propertyinfo.CanRead || !propertyinfo.CanWrite || _try.TryCustomAttribute(propertyinfo, out NotMappedAttribute _)) { return false; }
            if ((propertyinfo.GetMethod.IsVirtual || propertyinfo.SetMethod.IsVirtual) && (propertyinfo.PropertyType.IsMappedTable() || (propertyinfo.PropertyType.IsGenericType && propertyinfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) && propertyinfo.PropertyType.GenericTypeArguments[0].IsMappedTable()))) { return false; }
            return true;
        }
        /// <summary>
        /// Verilen özelliğin (<see cref="PropertyInfo"/>) birincil anahtar (Primary Key) olup olmadığını kontrol eder. Özelliğin, <see cref="KeyAttribute"/> ile işaretlenmiş olup olmadığını kontrol ederek birincil anahtar durumunu döndürür.
        /// </summary>
        /// <param name="propertyinfo">Kontrol edilecek özellik.</param>
        /// <returns>Özellik birincil anahtarsa <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsPK(this PropertyInfo propertyinfo) => propertyinfo != null && _try.TryCustomAttribute(propertyinfo, out KeyAttribute _);
        /// <summary>
        /// Verilen özelliğin (<see cref="PropertyInfo"/>) veritabanındaki sütun adını döndürür. Özellik <see cref="ColumnAttribute"/> ile işaretlenmişse, bu özniteliğin belirttiği sütun adını; aksi takdirde özelliğin adını döndürür.
        /// </summary>
        /// <param name="propertyinfo">Sütun adı alınacak özellik.</param>
        /// <returns>Özelliğin veritabanındaki sütun adı veya özellik adı.</returns>
        public static string GetColumnName(this PropertyInfo propertyinfo)
        {
            Guard.CheckNull(propertyinfo, nameof(propertyinfo));
            return _try.TryCustomAttribute(propertyinfo, out ColumnAttribute _ca) ? _ca.Name : propertyinfo.Name;
        }
        /// <summary>
        /// Verilen özelliğin (<see cref="PropertyInfo"/>) veritabanında nasıl oluşturulduğunu belirten <see cref="DatabaseGeneratedOption"/> değerini döndürür. Özellik <see cref="DatabaseGeneratedAttribute"/> ile işaretlenmişse, bu özniteliğin belirttiği seçeneği; aksi takdirde null döner.
        /// </summary>
        /// <param name="propertyinfo">Kontrol edilecek özellik.</param>
        /// <returns>DatabaseGeneratedOption değeri veya null.</returns>
        public static DatabaseGeneratedOption? GetDatabaseGeneratedOption(this PropertyInfo propertyinfo)
        {
            Guard.CheckNull(propertyinfo, nameof(propertyinfo));
            return _try.TryCustomAttribute(propertyinfo, out DatabaseGeneratedAttribute _dga) ? _dga.DatabaseGeneratedOption : null;
        }
    }
}