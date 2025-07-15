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
        /// <param name="propertyInfo">Kontrol edilecek özellik bilgisi.</param>
        /// <returns>Haritalanmış bir özellik ise <see langword="true"/>, değilse false <see langword="false"/>.</returns>
        public static bool IsMappedProperty(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) { return false; }
            if (!propertyInfo.CanRead || !propertyInfo.CanWrite || _try.TryCustomAttribute(propertyInfo, out NotMappedAttribute _)) { return false; }
            if ((propertyInfo.GetMethod.IsVirtual || propertyInfo.SetMethod.IsVirtual) && (propertyInfo.PropertyType.IsMappedTable() || (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) && propertyInfo.PropertyType.GenericTypeArguments[0].IsMappedTable()))) { return false; }
            return true;
        }
        /// <summary>
        /// Verilen özelliğin (<see cref="PropertyInfo"/>) birincil anahtar (Primary Key) olup olmadığını kontrol eder. Özelliğin, <see cref="KeyAttribute"/> ile işaretlenmiş olup olmadığını kontrol ederek birincil anahtar durumunu döndürür.
        /// </summary>
        /// <param name="propertyInfo">Kontrol edilecek özellik.</param>
        /// <returns>Özellik birincil anahtarsa <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsPK(this PropertyInfo propertyInfo) => propertyInfo != null && _try.TryCustomAttribute(propertyInfo, out KeyAttribute _);
        /// <summary>
        /// Verilen özelliğin (<see cref="PropertyInfo"/>) veritabanındaki sütun adını döndürür. Özellik <see cref="ColumnAttribute"/> ile işaretlenmişse, bu özniteliğin belirttiği sütun adını; aksi takdirde özelliğin adını döndürür.
        /// </summary>
        /// <param name="propertyInfo">Sütun adı alınacak özellik.</param>
        /// <returns>Özelliğin veritabanındaki sütun adı veya özellik adı.</returns>
        public static string GetColumnName(this PropertyInfo propertyInfo)
        {
            Guard.CheckNull(propertyInfo, nameof(propertyInfo));
            return _try.TryCustomAttribute(propertyInfo, out ColumnAttribute _ca) ? _ca.Name : propertyInfo.Name;
        }
        /// <summary>
        /// Verilen özelliğin (<see cref="PropertyInfo"/>) veritabanında nasıl oluşturulduğunu belirten <see cref="DatabaseGeneratedOption"/> değerini döndürür. Özellik <see cref="DatabaseGeneratedAttribute"/> ile işaretlenmişse, bu özniteliğin belirttiği seçeneği; aksi takdirde null döner.
        /// </summary>
        /// <param name="propertyInfo">Kontrol edilecek özellik.</param>
        /// <returns>DatabaseGeneratedOption değeri veya null.</returns>
        public static DatabaseGeneratedOption? GetDatabaseGeneratedOption(this PropertyInfo propertyInfo)
        {
            Guard.CheckNull(propertyInfo, nameof(propertyInfo));
            return _try.TryCustomAttribute(propertyInfo, out DatabaseGeneratedAttribute _dga) ? _dga.DatabaseGeneratedOption : null;
        }
    }
}