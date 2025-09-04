namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    using Bogus;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Sahte veri üretimi için kullanılan genel bir sınıf. Bogus kütüphanesini kullanarak farklı veri türlerinde özelleştirilebilir sahte veriler üretir.
    /// <list type="bullet">
    /// <item>
    /// String için özel işaretlenmiş property adları: <c>seo, nms, src, ipaddress, color, mac, tel, adres, ogrencino, ad, name, soyad, surname, kuladi, username, eposta, email</c>
    /// </item>
    /// <item>
    /// Int16(Short) için özel işaretlenmiş property adları: <c>dahili</c>
    /// </item>
    /// <item>
    /// Int64(Long) için özel işaretlenmiş property adları: <c>tckn, vkn</c>
    /// </item>
    /// </list>
    /// </summary>
    public sealed class BogusGenericFakeDataGenerator
    {
        private readonly Faker faker_en;
        private readonly string locale;
        private readonly float nullchange;
        private readonly int arrayminlength;
        private readonly int arraymaxlength;
        private byte minbyte = Byte.MinValue, maxbyte = Byte.MaxValue;
        private short shortminvalue = 0, shortmaxvalue = Int16.MaxValue;
        private int intminvalue = 0, intmaxvalue = Int32.MaxValue;
        private long longminvalue = 0, longmaxvalue = Int64.MaxValue;
        private decimal decimalminvalue = Decimal.Zero, decimalmaxvalue = Decimal.One;
        private DateTime? datetimebasdate = null, datetimebitdate = null;
        private DateOnly? dateonlybasdate = null, dateonlybitdate = null;
        /// <summary>
        /// Varsayılan yapılandırıcı
        /// </summary>
        /// <param name="locale">Kullanılacak yerel ayar (örneğin, &quot;tr&quot; için Türkçe, &quot;en&quot; için İngilizce).</param>
        /// <param name="nullchange">0 ile 1 arasında bir olasılık değeri (0: asla null, 1: her zaman null).</param>
        /// <param name="arrayminlength">Array türünde propertylerin minimum oluşabileceği eleman sayısı.</param>
        /// <param name="arraymaxlength">Array türünde propertylerin maksimum oluşabileceği eleman sayısı. Değer 0 olursa Array.Empty oluşur</param>
        public BogusGenericFakeDataGenerator(string locale = "tr", float nullchange = 0.25f, int arrayminlength = 0, int arraymaxlength = 10)
        {
            this.faker_en = new Faker("en");
            this.locale = locale;
            this.nullchange = (nullchange > 1 ? 1 : (nullchange < 0 ? 0 : nullchange));
            this.arrayminlength = arrayminlength > 0 ? arrayminlength : 0;
            this.arraymaxlength = arraymaxlength > 0 ? arraymaxlength : 0;
        }
        /// <summary>
        /// Byte türü için sahte veri üretiminde kullanılacak minimum ve maksimum değerleri belirler.
        /// </summary>
        /// <param name="minbyte">Minimum byte değeri.</param>
        /// <param name="maxbyte">Maksimum byte değeri.</param>
        public BogusGenericFakeDataGenerator WithByteRange(byte minbyte, byte maxbyte)
        {
            this.minbyte = minbyte;
            this.maxbyte = maxbyte;
            return this;
        }
        /// <summary>
        /// Short türü için sahte veri üretiminde kullanılacak minimum ve maksimum değerleri belirler.
        /// </summary>
        /// <param name="shortminvalue">Minimum short değeri.</param>
        /// <param name="shortmaxvalue">Maksimum short değeri.</param>
        public BogusGenericFakeDataGenerator WithShortRange(short shortminvalue, short shortmaxvalue)
        {
            this.shortminvalue = shortminvalue;
            this.shortmaxvalue = shortmaxvalue;
            return this;
        }
        /// <summary>
        /// Int türü için sahte veri üretiminde kullanılacak minimum ve maksimum değerleri belirler.
        /// </summary>
        /// <param name="intminvalue">Minimum int değeri.</param>
        /// <param name="intmaxvalue">Maksimum int değeri.</param>
        public BogusGenericFakeDataGenerator WithIntegerRange(int intminvalue, int intmaxvalue)
        {
            this.intminvalue = intminvalue;
            this.intmaxvalue = intmaxvalue;
            return this;
        }
        /// <summary>
        /// Long türü için sahte veri üretiminde kullanılacak minimum ve maksimum değerleri belirler.
        /// </summary>
        /// <param name="longminvalue">Minimum long değeri.</param>
        /// <param name="longmaxvalue">Maksimum long değeri.</param>
        public BogusGenericFakeDataGenerator WithLongRange(long longminvalue, long longmaxvalue)
        {
            this.longminvalue = longminvalue;
            this.longmaxvalue = longmaxvalue;
            return this;
        }
        /// <summary>
        /// Decimal türü için sahte veri üretiminde kullanılacak minimum ve maksimum değerleri belirler.
        /// </summary>
        /// <param name="decimalminvalue">Minimum decimal değeri.</param>
        /// <param name="decimalmaxvalue">Maksimum decimal değeri.</param>
        public BogusGenericFakeDataGenerator WithDecimalRange(decimal decimalminvalue, decimal decimalmaxvalue)
        {
            this.decimalminvalue = decimalminvalue;
            this.decimalmaxvalue = decimalmaxvalue;
            return this;
        }
        /// <summary>
        /// DateTime türü için sahte veri üretiminde kullanılacak başlangıç ve bitiş tarihlerini belirler.
        /// </summary>
        /// <param name="datetimebasdate">Başlangıç tarihi.</param>
        /// <param name="datetimebitdate">Bitiş tarihi.</param>
        public BogusGenericFakeDataGenerator WithDateTimeRange(DateTime datetimebasdate, DateTime datetimebitdate)
        {
            this.datetimebasdate = datetimebasdate;
            this.datetimebitdate = datetimebitdate;
            return this;
        }
        /// <summary>
        /// DateOnly türü için sahte veri üretiminde kullanılacak başlangıç ve bitiş tarihlerini belirler.
        /// </summary>
        /// <param name="dateonlybasdate">Başlangıç tarihi.</param>
        /// <param name="dateonlybitdate">Bitiş tarihi.</param>
        public BogusGenericFakeDataGenerator WithDateOnlyRange(DateOnly dateonlybasdate, DateOnly dateonlybitdate)
        {
            this.dateonlybasdate = dateonlybasdate;
            this.dateonlybitdate = dateonlybitdate;
            return this;
        }
        /// <summary>
        /// Belirtilen türde tek bir sahte veri nesnesi üretir.
        /// </summary>
        /// <typeparam name="T">Üretilecek nesnenin türü (sınıf olmalı).</typeparam>
        public T Generate<T>() where T : class => this.GenerateArray<T>(1)[0];
        /// <summary>
        /// Belirtilen türde ve sayıda sahte veri nesnesi dizisi üretir.
        /// </summary>
        /// <typeparam name="T">Üretilecek nesnelerin türü (sınıf olmalı).</typeparam>
        /// <param name="count">Üretilecek nesne sayısı.</param>
        public T[] GenerateArray<T>(int count) where T : class
        {
            if (count > 0) { return new Faker<T>(this.locale).CustomInstantiator(faker => (T)this.createfakeinstance("", typeof(T), faker)).Generate(count).ToArray(); }
            return Array.Empty<T>();
        }
        private string createuri() => this.faker_en.Internet.Url().TrimEnd('/');
        private string createmail() => this.faker_en.Internet.ExampleEmail().ToLower();
        private IPAddress createipadress() => this.faker_en.Internet.IpAddress().MapToIPv4();
        private string createnms(Faker faker) => String.Concat(faker.Person.FirstName, " ", faker.Person.LastName.ToUpper());
        private object createfakeinstance(string parametername, Type type, Faker faker)
        {
            if (_try.TryTypeIsNullable(type, out Type _underlying)) { return faker.Random.Bool(this.nullchange) ? null : this.createfakeinstance(parametername, _underlying, faker); }
            if (type == typeof(string))
            {
                if (parametername == "seo") { return this.createnms(faker).ToSeoFriendly(); }
                if (parametername == "nms") { return this.createnms(faker); }
                if (parametername == "src") { return this.createuri(); }
                if (parametername == "ipaddress") { return this.createipadress().ToString(); }
                if (parametername == "color") { return faker.Internet.Color().ToUpper(); }
                if (parametername == "mac") { return faker.Internet.Mac().ToUpper(); }
                if (parametername == "tel") { return faker.Phone.PhoneNumber("(5##) ###-####"); }
                if (parametername == "adres") { return faker.Address.FullAddress(); }
                if (parametername == "ogrencino") { return String.Concat((faker.Random.Int(_date.bayburtUniversityFoundationDate.Year, DateTime.Today.Year) - 2000).ToString().Replicate(2), faker.Random.ReplaceNumbers(new String('#', 7))); }
                if (parametername.Includes("ad", "name")) { return faker.Person.FirstName; }
                if (parametername.Includes("soyad", "surname")) { return faker.Person.LastName.ToUpper(); }
                if (parametername.Includes("kuladi", "username")) { return this.faker_en.Internet.UserName().ToLower(); }
                if (parametername.Includes("eposta", "email")) { return this.createmail(); }
                return faker.Commerce.ProductName();
            }
            if (type == typeof(byte)) { return faker.Random.Byte(this.minbyte, this.maxbyte); }
            if (type == typeof(short))
            {
                if (parametername == "dahili") { return faker.Random.Short(1000, 9999); }
                return faker.Random.Short(this.shortminvalue, this.shortmaxvalue);
            }
            if (type == typeof(int)) { return faker.Random.Int(this.intminvalue, this.intmaxvalue); }
            if (type == typeof(long))
            {
                if (parametername == "tckn") { return faker.Random.ArrayElement(new long[] { 10000000146, 19293160506, 35270291346, 35505252760, 37417041838, 48056596160, 57856397112, 66122384800, 69016478326, 75255184164, 78094801254, 78268733060, 79937798144, 81299907768, 86061923892, 88599002742, 89021372822, 93095299084, 93513339668, 94781067710 }); }
                if (parametername == "vkn") { return faker.Random.ArrayElement(new long[] { 602883151, 1266516393, 1775916611, 3085865484, 3641323334, 3749934537, 5056541626, 5252719378, 5498069343, 5613060112, 6000479747, 6501266542, 7267046912, 7915288675, 9142970393, 9152251176, 9205280623, 9217990731, 9292694379, 9734899775 }); }
                return faker.Random.Long(this.longminvalue, this.longmaxvalue);
            }
            if (type == typeof(bool)) { return faker.Random.Bool(); }
            if (type == typeof(decimal)) { return faker.Random.Decimal(this.decimalminvalue, this.decimalmaxvalue); }
            if (type == typeof(Guid)) { return faker.Random.Guid(); }
            if (type == typeof(DateTime)) { return ((this.datetimebasdate.HasValue && this.datetimebitdate.HasValue) ? faker.Date.Between(this.datetimebasdate.Value, this.datetimebitdate.Value) : faker.Date.Past()); }
            if (type == typeof(DateOnly)) { return ((this.dateonlybasdate.HasValue && this.dateonlybitdate.HasValue) ? faker.Date.BetweenDateOnly(this.dateonlybasdate.Value, this.dateonlybitdate.Value) : faker.Date.PastDateOnly()); }
            if (type == typeof(Uri)) { return new Uri(this.createuri()); }
            if (type == typeof(MailAddress)) { return new MailAddress(this.createmail(), this.createnms(faker)); }
            if (type == typeof(IPAddress)) { return this.createipadress(); }
            if (type.IsEnum) { return faker.PickRandom(Enum.GetValues(type).Cast<object>().ToArray()); }
            if (type.IsArray)
            {
                int i, _count = (this.arraymaxlength > 0 ? faker.Random.Int(this.arrayminlength, this.arraymaxlength) : 0);
                var _elemtype = type.GetElementType();
                var _array = Array.CreateInstance(_elemtype, _count);
                for (i = 0; i < _count; i++) { _array.SetValue(this.createfakeinstance(parametername, _elemtype, faker), i); }
                return _array;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var _keytype = type.GetGenericArguments()[0];
                var _valuetype = type.GetGenericArguments()[1];
                int i, _count = (this.arraymaxlength > 0 ? faker.Random.Int(this.arrayminlength, this.arraymaxlength) : 0);
                var _dict = (IDictionary)Activator.CreateInstance(type);
                for (i = 0; i < _count; i++)
                {
                    var _key = this.createfakeinstance(parametername, _keytype, faker);
                    if (_dict.Contains(_key)) { continue; }
                    _dict.Add(_key, this.createfakeinstance(parametername, _valuetype, faker));
                }
                return _dict;
            }
            if (type.IsClass)
            {
                var _ctor = type.GetConstructors().FirstOrDefault();
                if (_ctor == null) { throw new InvalidOperationException($"\"{type.FullName}\" için genel bir kurucu (Constructors) bulunamadı!"); }
                var _args = _ctor.GetParameters().Select(x => this.createfakeinstance(x.Name, x.ParameterType, faker)).ToArray();
                return _ctor.Invoke(_args);
            }
            return null;
        }
    }
}