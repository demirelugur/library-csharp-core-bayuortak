namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    public sealed class StrongPasswordValid
    {
        /// <summary>
        /// Varsayılan güçlü şifre ayarlarını temsil eden örnek. Varsayılan minimum uzunluk 8, maksimum uzunluk 16, ardışık rakam, boşluk ve Türkçe karakter kontrolleri aktif durumdadır. <code>new StrongPasswordValid(8, 16, true, true, true);</code>
        /// </summary>
        public static readonly StrongPasswordValid Default = new StrongPasswordValid(8, 16, true, true, true);
        /// <summary>
        /// Şifrenin minimum uzunluğunu belirtir. Kullanıcı tarafından belirlenen minimum karakter sayısıdır.
        /// </summary>
        public int minimumlength { get; }
        /// <summary>
        /// Şifrenin maksimum uzunluğunu belirtir. Kullanıcı tarafından belirlenen maksimum karakter sayısıdır. Eğer belirtilmezse null değer alır.
        /// </summary>
        public int? maximumlength { get; }
        /// <summary>
        /// Şifre içinde ardışık üç rakam bulunup bulunmadığını kontrol eder. Eğer <see langword="true"/> ise, şifre kontrolü sırasında ardışık rakam kontrolü yapılacaktır.
        /// </summary>
        public bool isardisiksayi { get; }
        /// <summary>
        /// Şifre içinde boşluk karakterinin bulunup bulunmadığını kontrol eder. Eğer <see langword="true"/> ise, şifre kontrolü sırasında boşluk kontrolü yapılacaktır.
        /// </summary>
        public bool isbosluk { get; }
        /// <summary>
        /// Şifre içinde Türkçe karakterlerin bulunup bulunmadığını kontrol eder. Eğer <see langword="true"/> ise, Türkçe karakterler kontrol edilecektir.
        /// </summary>
        public bool isturkceharf { get; }
        /// <summary>
        /// Kullanıcının adını temsil eder. Şifre kontrolü sırasında, adın şifre içinde geçip geçmediği kontrol edilecektir.
        /// </summary>
        public string ad { get; }
        /// <summary>
        /// Kullanıcının soyadını temsil eder. Şifre kontrolü sırasında, soyadın şifre içinde geçip geçmediği kontrol edilecektir.
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
        /// <para>Uyarılar, geçersiz şifre durumlarına göre döner. Eğer şifre güçlü ise uyarılar boş bir dizi olarak dönecektir.</para>
        /// </summary>
        /// <param name="value">Kontrol edilecek şifre.</param>
        /// <param name="ad">Kullanıcının adı.</param>
        /// <param name="soyad">Kullanıcının soyadı.</param>
        /// <param name="dil">Dil kodu (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <param name="errors">Olumsuz durumlara ait uyarıları içeren dizi.</param>
        /// <returns>Geçerli bir şifre değilse <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
        public bool TryIsWarning(string value, string ad, string soyad, string dil, out string[] errors)
        {
            Guard.CheckEmpty(value, nameof(value));
            Guard.UnSupportLanguage(dil, nameof(dil));
            var _r = new List<string>();
            var _isen = dil == "en";
            if (!PasswordGenerator.IsStrongPassword(value, this.minimumlength))
            {
                if (_isen) { _r.Add($"The password must have a minimum of {this.minimumlength.ToString()} characters and contain at least 1 Uppercase Letter, 1 Lowercase Letter, 1 Number and 1 Punctuation mark!"); }
                else { _r.Add($"Şifre minimum {this.minimumlength.ToString()} karakter ve içerisinde en az 1 Büyük Harf, 1 Küçük Harf, 1 Rakam ve 1 Noktalama işareti olmalıdır!"); }
            }
            if (this.maximumlength.HasValue && value.Length > this.maximumlength.Value)
            {
                if (_isen) { _r.Add($"Password can be maximum {this.maximumlength.Value.ToString()} characters!"); }
                else { _r.Add($"Şifre maksimum {this.maximumlength.Value.ToString()} karakter olabilir!"); }
            }
            if (this.isardisiksayi && ardisiksayikontrol_private(value))
            {
                if (_isen) { _r.Add("The password must not contain 3 consecutive numbers! (123, 987 etc...)"); }
                else { _r.Add("Şifre içerisinde 3 ardışık sayı (123, 987 vb...) bulunmamalıdır!"); }
            }
            if (this.isbosluk && value.Contains(' '))
            {
                if (_isen) { _r.Add("There should be no empty characters in the password!"); }
                else { _r.Add("Şifre içerisinde boş karakter bulunmamalıdır!"); }
            }
            if (this.isturkceharf && value.Any(GlobalConstants.turkishcharacters.Contains))
            {
                var _t = String.Join(", ", GlobalConstants.turkishcharacters);
                if (_isen) { _r.Add($"The password must not contain any letters specific to the Turkish language! ({_t})"); }
                else { _r.Add($"Şifre içerisinde Türk diline özgü harf ({_t}) bulunmamalıdır!"); }
            }
            var _password_seo = value.ToSeoFriendly();
            if (adsoyadkontrol_private(_password_seo, ad))
            {
                if (_isen) { _r.Add("Your name(s) must not appear in the password!"); }
                else { _r.Add("Şifre içerisinde adınız/adlarınız geçmemelidir!"); }
            }
            if (adsoyadkontrol_private(_password_seo, soyad))
            {
                if (_isen) { _r.Add("Your surname(s) must not appear in the password!"); }
                else { _r.Add("Şifre içerisinde soyadınız/soyadlarınız geçmemelidir!"); }
            }
            errors = _r.ToArray();
            return _r.Count > 0;
        }
        private static bool ardisiksayikontrol_private(string password)
        {
            var _r = false;
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
                            _r = true;
                            break;
                        }
                    }
                }
            }
            return _r;
        }
        private static bool adsoyadkontrol_private(string passwordseo, string value)
        {
            var _r = false;
            var _values = value.ToStringOrEmpty().ToEnumerable().Select(x => (x == "" ? Array.Empty<string>() : x.Split(' ').Select(y => y.ToSeoFriendly()).Where(y => y != "").ToArray())).FirstOrDefault();
            if (_values.Length > 0)
            {
                foreach (var item in _values)
                {
                    if (passwordseo.Contains(item))
                    {
                        _r = true;
                        break;
                    }
                }
            }
            return _r;
        }
    }
}