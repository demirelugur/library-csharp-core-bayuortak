namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    public sealed class StrongPasswordValid
    {
        /// <summary>
        /// Varsayılan güçlü şifre ayarlarını temsil eden örnek.
        /// Varsayılan minimum uzunluk 8, maksimum uzunluk 16, ardışık rakam, boşluk ve Türkçe karakter kontrolleri aktif durumdadır. <code>new StrongPasswordValid(8, 16, true, true, true);</code>
        /// </summary>
        public static readonly StrongPasswordValid Default = new StrongPasswordValid(8, 16, true, true, true);
        /// <summary>
        /// Şifrenin minimum uzunluğunu belirtir.
        /// Kullanıcı tarafından belirlenen minimum karakter sayısıdır.
        /// </summary>
        public int minimumlength { get; }
        /// <summary>
        /// Şifrenin maksimum uzunluğunu belirtir.
        /// Kullanıcı tarafından belirlenen maksimum karakter sayısıdır. Eğer belirtilmezse null değer alır.
        /// </summary>
        public int? maximumlength { get; }
        /// <summary>
        /// Şifre içinde ardışık üç rakam bulunup bulunmadığını kontrol eder.
        /// Eğer true ise, şifre kontrolü sırasında ardışık rakam kontrolü yapılacaktır.
        /// </summary>
        public bool isardisiksayi { get; }
        /// <summary>
        /// Şifre içinde boşluk karakterinin bulunup bulunmadığını kontrol eder.
        /// Eğer true ise, şifre kontrolü sırasında boşluk kontrolü yapılacaktır.
        /// </summary>
        public bool isbosluk { get; }
        /// <summary>
        /// Şifre içinde Türkçe karakterlerin bulunup bulunmadığını kontrol eder.
        /// Eğer true ise, Türkçe karakterler kontrol edilecektir.
        /// </summary>
        public bool isturkceharf { get; }
        /// <summary>
        /// Kullanıcının adını temsil eder.
        /// Şifre kontrolü sırasında, adın şifre içinde geçip geçmediği kontrol edilecektir.
        /// </summary>
        public string ad { get; }
        /// <summary>
        /// Kullanıcının soyadını temsil eder.
        /// Şifre kontrolü sırasında, soyadın şifre içinde geçip geçmediği kontrol edilecektir.
        /// </summary>
        public string soyad { get; }
        /// <summary>
        /// Yeni bir <see cref="StrongPasswordValid"/> örneği oluşturur.
        /// </summary>
        /// <param name="minimumlength">Şifrenin minimum uzunluğu.</param>
        /// <param name="maximumlength">Şifrenin maksimum uzunluğu. (0 veya negatifse null olarak ayarlanır)</param>
        /// <param name="isardisiksayi">Ardışık rakam kontrolü yapılacak mı?</param>
        /// <param name="isbosluk">Boşluk karakteri kontrolü yapılacak mı?</param>
        /// <param name="isturkceharf">Türkçe karakter kontrolü yapılacak mı?</param>
        public StrongPasswordValid(int minimumlength, int maximumlength, bool isardisiksayi, bool isbosluk, bool isturkceharf)
        {
            this.minimumlength = minimumlength;
            this.maximumlength = (maximumlength > 0 ? maximumlength : null);
            this.isardisiksayi = isardisiksayi;
            this.isbosluk = isbosluk;
            this.isturkceharf = isturkceharf;
        }
        /// <summary>
        /// Verilen şifre değerinin güçlü olup olmadığını kontrol eder ve olumsuz durumları bir uyarı dizisi olarak döner.
        /// <para>Şifre kontrolleri şunları içerir:</para>
        /// <list type="bullet">
        ///     <item><description>Minimum uzunluk kontrolü.</description></item>
        ///     <item><description>Maksimum uzunluk kontrolü.</description></item>
        ///     <item><description>Ardışık üç rakam kontrolü.</description></item>
        ///     <item><description>Boş karakter kontrolü.</description></item>
        ///     <item><description>Türkçe karakter kontrolü.</description></item>
        ///     <item><description>Doğum yılı kontrolü.</description></item>
        ///     <item><description>Ad ve Soyad kontrolü.</description></item>
        /// </list>
        /// <para>Uyarılar, geçersiz şifre durumlarına göre döner. 
        /// Eğer şifre güçlü ise uyarılar boş bir dizi olarak dönecektir.</para>
        /// </summary>
        /// <param name="value">Kontrol edilecek şifre.</param>
        /// <param name="dogumtarihyil">Kullanıcının doğum yılı.</param>
        /// <param name="ad">Kullanıcının adı.</param>
        /// <param name="soyad">Kullanıcının soyadı.</param>
        /// <param name="dil">Dil kodu (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <param name="warnings">Olumsuz durumlara ait uyarıları içeren dizi.</param>
        /// <returns>Geçerli bir şifre değilse <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
        public bool TryIsWarning(string value, int? dogumtarihyil, string ad, string soyad, string dil, out string[] warnings)
        {
            var r = new List<string>();
            Guard.CheckEmpty(value, nameof(value));
            Guard.UnSupportLanguage(dil, nameof(dil));
            var istr = dil == "tr";
            if (!PasswordGenerator.IsStrongPassword(value, this.minimumlength))
            {
                if (istr) { r.Add($"Şifre minimum {this.minimumlength.ToString()} karakter ve içerisinde en az 1 Büyük Harf, 1 Küçük Harf, 1 Rakam ve 1 Noktalama işareti olmalıdır!"); }
                else { r.Add($"The password must have a minimum of {this.minimumlength.ToString()} characters and contain at least 1 Uppercase Letter, 1 Lowercase Letter, 1 Number and 1 Punctuation mark!"); }
            }
            if (this.maximumlength.HasValue && value.Length > this.maximumlength.Value)
            {
                if (istr) { r.Add($"Şifre maksimum {this.maximumlength.Value.ToString()} karakter olabilir!"); }
                else { r.Add($"Password can be maximum {this.maximumlength.Value.ToString()} characters!"); }
            }
            if (this.isardisiksayi && ardisiksayikontrol_private(value))
            {
                if (istr) { r.Add("Şifre içerisinde 3 ardışık sayı (123, 987 vb...) bulunmamalıdır!"); }
                else { r.Add("The password must not contain 3 consecutive numbers! (123, 987 etc...)"); }
            }
            if (this.isbosluk && value.Contains(' '))
            {
                if (istr) { r.Add("Şifre içerisinde boş karakter bulunmamalıdır!"); }
                else { r.Add("There should be no empty characters in the password!"); }
            }
            if (this.isturkceharf && value.Any(GlobalConstants.turkishcharacters.Contains))
            {
                var _t = String.Join(", ", GlobalConstants.turkishcharacters);
                if (istr) { r.Add($"Şifre içerisinde Türk diline özgü harf ({_t}) bulunmamalıdır!"); }
                else { r.Add($"The password must not contain any letters specific to the Turkish language! ({_t})"); }
            }
            if (dogumtarihyil.HasValue && value.Contains(dogumtarihyil.Value.ToString()))
            {
                if (istr) { r.Add("Şifre içerisinde doğum tarih yılınız geçmemelidir!"); }
                else { r.Add("The password must not contain your date and year of birth!"); }
            }
            var password_seo = value.ToSeoFriendly();
            if (adsoyadkontrol_private(password_seo, ad))
            {
                if (istr) { r.Add("Şifre içerisinde adınız/adlarınız geçmemelidir!"); }
                else { r.Add("Your name(s) must not appear in the password!"); }
            }
            if (adsoyadkontrol_private(password_seo, soyad))
            {
                if (istr) { r.Add("Şifre içerisinde soyadınız/soyadlarınız geçmemelidir!"); }
                else { r.Add("Your surname(s) must not appear in the password!"); }
            }
            warnings = r.ToArray();
            return r.Count > 0;
        }
        private static bool ardisiksayikontrol_private(string password)
        {
            var r = false;
            if (password.Length > 2)
            {
                int i, sayi1, sayi2, sayi3, l = password.Length - 2;
                for (i = 0; i < l; i++)
                {
                    if (Char.IsDigit(password[i]) && Char.IsDigit(password[i + 1]) && Char.IsDigit(password[i + 2]))
                    {
                        sayi1 = (password[i] - '0');
                        sayi2 = (password[i + 1] - '0');
                        sayi3 = (password[i + 2] - '0');
                        if ((sayi2 == (sayi1 + 1) && sayi3 == (sayi2 + 1)) || (sayi2 == (sayi1 - 1) && sayi3 == (sayi2 - 1)))
                        {
                            r = true;
                            break;
                        }
                    }
                }
            }
            return r;
        }
        private static bool adsoyadkontrol_private(string password_seo, string value)
        {
            var r = false;
            var values = value.ToStringOrEmpty().ToEnumerable().Select(x => (x == "" ? Array.Empty<string>() : x.Split(' ').Select(y => y.ToSeoFriendly()).Where(y => y != "").ToArray())).FirstOrDefault();
            if (values.Length > 0)
            {
                foreach (var itemValue in values)
                {
                    if (password_seo.Contains(itemValue))
                    {
                        r = true;
                        break;
                    }
                }
            }
            return r;
        }
    }
}