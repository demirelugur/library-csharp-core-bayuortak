namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Helper.Results;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class TypeExtensions
    {
        /// <summary>
        /// Belirtilen türün nullable olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="type">Kontrol edilecek tür.</param>
        /// <returns>Nullable ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsNullable(this Type type) => _try.TryTypeIsNullable(type, out _);
        /// <summary>
        /// Belirtilen türün özel bir sınıf olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="type">Kontrol edilecek tür.</param>
        /// <returns>Özel sınıf ise <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsCustomClass(this Type type) => (type != null && type.IsClass && type != typeof(string) && !type.IsArray && !typeof(Delegate).IsAssignableFrom(type) && !type.IsInterface);
        /// <summary>
        /// Belirtilen tür için tablo adını döner. İsteğe bağlı olarak köşeli parantezleri kullanabilir.
        /// </summary>
        /// <param name="type">Tablo adını alınacak tür.</param>
        /// <param name="isSquareBrackets">Köşeli parantezlerin kullanılıp kullanılmayacağı.</param>
        /// <param name="schemaDefaultValue">Varsayılan şema adı.</param>
        /// <returns>Tablo adı.</returns>
        public static string GetTableName(this Type type, bool isSquareBrackets, string schemaDefaultValue = "dbo")
        {
            Guard.CheckNull(type, nameof(type));
            if (type.IsCustomClass())
            {
                string ret;
                if (_try.TryCustomAttribute(type, out TableAttribute _ta))
                {
                    var schema = _ta.Schema.ToStringOrEmpty();
                    var names = _ta.Name.ToStringOrEmpty().Split('.').Where(x => x != "").ToArray();
                    if (names.Length == 2 && schema == "") { ret = String.Join(".", names); }
                    else if (names.Length == 1) { ret = String.Join(".", (schema == "" ? schemaDefaultValue : schema), names[0]); }
                    else if (names.Length == 0 && schema != "") { ret = String.Join(".", schema, type.Name); } // Not: Bu ihtimalin gelme durumu olmamasına rağmen ne olur olmaz yazılan kontrol
                    else { ret = String.Join(".", schemaDefaultValue, type.Name); }
                }
                else { ret = String.Join(".", schemaDefaultValue, type.Name); }
                return (isSquareBrackets ? String.Join(".", ret.Split('.').Select(x => $"[{x}]").ToArray()) : ret);
            }
            throw new NotSupportedException($"{nameof(type)} tipi class olmalıdır!");
        }
        /// <summary>
        /// Belirtilen türün denetleyici adı döner.
        /// </summary>
        /// <param name="type">Denetleyici adı alınacak tür. Tür <see cref="Controller"/> sınıfından miras almalıdır</param>
        /// <returns>Denetleyici adı.</returns>
        public static string GetControllerName(this Type type)
        {
            Guard.CheckNull(type, nameof(type));
            return (typeof(Controller).IsAssignableFrom(type) ? type.Name.Replace(nameof(Controller), "") : "");
        }
        /// <summary>
        /// Belirtilen türün tüm özelliklerini döner.
        /// </summary>
        /// <param name="type">Özellikleri alınacak tür.</param>
        /// <returns>Tüm özelliklerin dizisi.</returns>
        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            Guard.CheckNull(type, nameof(type));
            return (type.IsInterface ? type.GetInterfaces().Concat(new Type[] { type }).SelectMany(x => x.GetProperties()).ToArray() : type.GetProperties());
        }
        /// <summary>
        /// Belirtilen tür için açıklama döner.
        /// </summary>
        /// <param name="type">Açıklaması alınacak tür.</param>
        /// <returns>Açıklama metni.</returns>
        public static string GetDescription(this Type type)
        {
            Guard.CheckNull(type, nameof(type));
            return _try.TryCustomAttribute(type, out DescriptionAttribute _de) ? _de.Description : "";
        }
        /// <summary>
        /// Belirtilen türü enum dizisine dönüştürür.
        /// </summary>
        /// <param name="type">Enum türü.</param>
        /// <returns>Enum sonuçları dizisi.</returns>
        public static EnumResult[] ToEnumArray(this Type type) => Enum.GetNames(type).Select((enumName, i) => new EnumResult(Convert.ToInt64(Enum.GetValues(type).GetValue(i)), enumName, _try.TryCustomAttribute(type.GetField(enumName), out DescriptionAttribute _outvalue) ? _outvalue.Description : "")).ToArray();
        /// <summary>
        /// Verilen türün (Type) bir tabloya eşlendiğini kontrol eder. Türün, <see cref="TableAttribute"/> ile işaretlenmiş olup olmadığını kontrol ederek tabloya eşlenip eşlenmediğini döndürür.
        /// </summary>
        /// <param name="type">Kontrol edilecek tür.</param>
        /// <returns>Tür bir tabloya eşlenmişse <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsMappedTable(this Type type) => type != null && _try.TryCustomAttribute(type, out TableAttribute _);
    }
}