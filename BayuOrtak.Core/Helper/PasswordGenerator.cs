namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    using System.Text;
    using System.Text.RegularExpressions;
    public sealed class PasswordGenerator
    {
        /// <summary>
        /// Varsayılan şifre oluşturucu örneği. 
        /// <code>new PasswordGenerator(&quot;ABCDEFGHIJKLMNOPQRSTUVWXYZ&quot;, &quot;abcdefghijklmnopqrstuvwxyz&quot;, &quot;0123456789&quot;, &quot;!@#$%^*()_+[]{}|;:,.?&quot;);</code>
        /// </summary>
        public static readonly PasswordGenerator Default = new PasswordGenerator("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz", "0123456789", "!@#$%^*()_+[]{}|;:,.?");
        private string uppercaseChars { get; }
        private string lowercaseChars { get; }
        private string digits { get; }
        private string punctuations { get; }
        private string allChars => String.Join("", this.uppercaseChars, this.lowercaseChars, this.digits, this.punctuations);
        /// <summary>
        /// PasswordGenerator sınıfının bir örneğini başlatır.
        /// </summary>
        /// <param name="uppercaseChars">Büyük harf karakterleri.</param>
        /// <param name="lowercaseChars">Küçük harf karakterleri.</param>
        /// <param name="digits">Rakam karakterleri.</param>
        /// <param name="punctuations">Özel karakterler.</param>
        public PasswordGenerator(string uppercaseChars, string lowercaseChars, string digits, string punctuations)
        {
            this.uppercaseChars = uppercaseChars.ToStringOrEmpty();
            this.lowercaseChars = lowercaseChars.ToStringOrEmpty();
            this.digits = digits.ToStringOrEmpty();
            this.punctuations = punctuations.ToStringOrEmpty();
        }
        private void validation()
        {
            Guard.CheckEmpty(this.uppercaseChars, nameof(this.uppercaseChars));
            Guard.CheckEmpty(this.lowercaseChars, nameof(this.lowercaseChars));
            Guard.CheckEmpty(this.digits, nameof(this.digits));
            Guard.CheckEmpty(this.punctuations, nameof(this.punctuations));
        }
        /// <summary>
        /// 8 ile 16 karakter arasında rastgele bir dize oluşturur. Dize, belirli uzunluk koşullarına göre büyük harfler, küçük harfler,
        /// rakamlar ve noktalama işaretleri içerir. Dize uzunluğu 4&#39;ün katı ise her karakter türünden eşit sayıda karakter eklenir.
        /// Aksi takdirde, tüm karakter türlerinden en az birer tane olacak şekilde rastgele tamamlanır.
        /// </summary>
        /// <remarks>
        /// - Uzunluk 4&#39;ün katı olduğunda, büyük harf, küçük harf, rakam ve noktalama işaretlerinden eşit sayıda karakter içerir.
        /// - Uzunluk 4&#39;ün katı değilse, her türden en az bir karakter eklenir ve kalan karakterler rastgele dağıtılır.
        /// - Sonuç dize, karakter sırası karıştırılarak döndürülür.
        /// </remarks>
        /// <returns>Rastgele oluşturulmuş [8 - 16] karakter uzunluğunda bir dize.</returns>
        public string Generate()
        {
            int i, minvalue = 4, length = Random.Shared.Next(minvalue * 2, (minvalue * 4) + 1);
            this.validation();
            var sb = new StringBuilder();
            if (length % minvalue == 0) { set_private(sb, length / minvalue); }
            else
            {
                set_private(sb, 1);
                for (i = minvalue; i < length; i++) { sb.Append(this.allChars[Random.Shared.Next(this.allChars.Length)]); }
            }
            return new String(sb.ToString().ToCharArray().Shuffle().ToArray());
        }
        private void set_private(StringBuilder sb, int count)
        {
            int i;
            foreach (var item in new string[] { this.uppercaseChars, this.lowercaseChars, this.digits, this.punctuations }) { for (i = 0; i < count; i++) { sb.Append(item[Random.Shared.Next(item.Length)]); } }
        }
        /// <summary>
        /// Belirtilen uzunlukta rastgele bir karakter dizisi oluşturur.
        /// </summary>
        /// <param name="length">Oluşturulacak karakter dizisinin uzunluğu.</param>
        /// <param name="element">
        /// Rastgele seçim yapılacak karakter kümesi. 
        /// Varsayılan olarak büyük harfler, küçük harfler ve sayılar kullanılır (&quot;ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&quot;).
        /// </param>
        /// <returns>Belirtilen uzunlukta, verilen karakter kümesinden rastgele seçilmiş karakterlerden oluşan bir string döner.</returns>
        public static string GenerateRandomChars(int length, string element = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            Guard.CheckZeroOrNegative(length, nameof(length));
            Guard.CheckEmpty(element, nameof(element));
            return new String(Enumerable.Repeat(element, length).Select(x => x[Random.Shared.Next(x.Length)]).ToArray());
        }
        /// <summary>
        /// Verilen şifre değerinin belirtilen minimum uzunluğa sahip olup olmadığını
        /// ve çeşitli karmaşıklık gereksinimlerini karşılayıp karşılamadığını kontrol eder.
        /// Şifre en az bir rakam, küçük harf, büyük harf ve özel karakter içermelidir.
        /// </summary>
        /// <param name="value">Doğrulanacak şifre değeri</param>
        /// <param name="minimumLength">Şifrenin minimum uzunluk gereksinimi. Default değeri 8&#39;dir</param>
        /// <returns>Şifre güçlü ise <see langword="true"/>, değilse <see langword="false"/> döner</returns>
        public static bool IsStrongPassword(string value, int minimumLength = 8)
        {
            value = value.ToStringOrEmpty();
            var r = value.Length >= minimumLength;
            if (r) { r = Regex.IsMatch(value, @"[\d]"); }
            if (r) { r = Regex.IsMatch(value, @"[a-z]"); }
            if (r) { r = Regex.IsMatch(value, @"[A-Z]"); }
            if (r) { r = Regex.IsMatch(value, @"[!@#$%^&*()_+\-=\[\]{}|;:',.<>?]"); }
            return r;
        }
    }
}