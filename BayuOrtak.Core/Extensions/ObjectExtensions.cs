namespace BayuOrtak.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class ObjectExtensions
    {
        /// <summary>
        /// Belirtilen nesneyi bir string&#39;e dönüştürür. Eğer nesne null ise, boş bir string döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <returns>Nesnenin string temsilini veya null ise boş bir string.</returns>
        public static string ToStringOrEmpty(this object value)
        {
            if (value == null) { return ""; }
            return Convert.ToString(value).Trim();
        }
        /// <summary>
        /// Belirtilen nesneyi byte&#39;a dönüştürür. Dönüşüm başarısız olursa varsayılan değeri döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <param name="defaultvalue">Dönüşüm başarısız olursa dönecek varsayılan byte değeri.</param>
        /// <returns>Dönüştürülmüş byte değeri.</returns>
        public static byte ToByte(this object value, byte defaultvalue = 0)
        {
            if (value == null) { return defaultvalue; }
            try { return Convert.ToByte(value); }
            catch { return defaultvalue; }
        }
        /// <summary>
        /// Belirtilen nesneyi short&#39;a dönüştürür. Dönüşüm başarısız olursa varsayılan değeri döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <param name="defaultvalue">Dönüşüm başarısız olursa dönecek varsayılan short değeri.</param>
        /// <returns>Dönüştürülmüş short değeri.</returns>
        public static short ToInt16(this object value, short defaultvalue = 0)
        {
            if (value == null) { return defaultvalue; }
            try { return Convert.ToInt16(value); }
            catch { return defaultvalue; }
        }
        /// <summary>
        /// Belirtilen nesneyi short&#39;a dönüştürür. Dönüşüm başarısız olursa varsayılan değeri döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <param name="defaultvalue">Dönüşüm başarısız olursa dönecek varsayılan short değeri.</param>
        /// <returns>Dönüştürülmüş short değeri.</returns>
        public static short ToShort(this object value, short defaultvalue = 0) => value.ToInt16(defaultvalue);
        /// <summary>
        /// Belirtilen nesneyi int&#39;e dönüştürür. Dönüşüm başarısız olursa varsayılan değeri döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <param name="defaultvalue">Dönüşüm başarısız olursa dönecek varsayılan int değeri.</param>
        /// <returns>Dönüştürülmüş int değeri.</returns>
        public static int ToInt32(this object value, int defaultvalue = 0)
        {
            if (value == null) { return defaultvalue; }
            try { return Convert.ToInt32(value); }
            catch { return defaultvalue; }
        }
        /// <summary>
        /// Belirtilen nesneyi long&#39;a dönüştürür. Dönüşüm başarısız olursa varsayılan değeri döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <param name="defaultvalue">Dönüşüm başarısız olursa dönecek varsayılan long değeri.</param>
        /// <returns>Dönüştürülmüş long değeri.</returns>
        public static long ToInt64(this object value, long defaultvalue = 0)
        {
            if (value == null) { return defaultvalue; }
            try { return Convert.ToInt64(value); }
            catch { return defaultvalue; }
        }
        /// <summary>
        /// Belirtilen nesneyi long&#39;a dönüştürür. Dönüşüm başarısız olursa varsayılan değeri döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <param name="defaultvalue">Dönüşüm başarısız olursa dönecek varsayılan long değeri.</param>
        /// <returns>Dönüştürülmüş long değeri.</returns>
        public static long ToLong(this object value, long defaultvalue = 0) => value.ToInt64(defaultvalue);
        /// <summary>
        /// Bir <see cref="object"/> türündeki değeri, <see cref="decimal"/> türüne dönüştürür. Eğer dönüşüm başarısız olursa, belirtilen varsayılan değeri döner.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne değeri.</param>
        /// <param name="defaultvalue">Dönüşüm başarısız olduğunda dönecek varsayılan <see cref="decimal"/> değeri. Varsayılan olarak sıfırdır.</param>
        /// <returns>Belirtilen nesnenin <see cref="decimal"/> karşılığını veya dönüşüm başarısızsa varsayılan değeri döner.</returns>
        public static decimal ToDecimal(this object value, decimal defaultvalue = Decimal.Zero)
        {
            if (value == null) { return defaultvalue; }
            return Convert.ToString(value).ParseOrDefault<decimal?>() ?? defaultvalue;
        }
        /// <summary>
        /// Bir nesneyi dinamik bir nesneye (<see cref="ExpandoObject"/>) dönüştürür. Dönüştürülen nesne, içindeki tüm özellik adları ve değerleriyle birlikte dinamik bir yapı sunar.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <returns>Dinamik bir nesne olarak temsil edilen <see cref="ExpandoObject"/>.</returns>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> e = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType())) { e.Add(property.Name, property.GetValue(value)); }
            return e as ExpandoObject;
        }
        /// <summary>
        /// Verilen nesneyi string olarak dönüştürür, null ise varsayılan bir değer döndürür. Eğer nesne string türündeyse, belirlenen varsayılan değeri kontrol eder. Diğer nesneleri JSON biçimine çevirir ve URI güvenli olacak şekilde kodlar.
        /// </summary>
        /// <param name="value">Dönüştürülecek nesne.</param>
        /// <param name="defaultvalue">Nesne null ise döndürülecek varsayılan değer.</param>
        /// <returns>URI güvenli string döndürür.</returns>
        public static string ToUriEscaped(this object value, string defaultvalue = "{}")
        {
            string _v;
            if (value == null) { _v = defaultvalue; }
            else if (value is String _s) { _v = _s.CoalesceOrDefault(defaultvalue); }
            else { _v = _to.ToJSON(value); }
            return Uri.EscapeDataString(_v);
        }
    }
}