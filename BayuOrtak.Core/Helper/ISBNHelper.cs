namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    using System;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public sealed class ISBNHelper
    {
        /// <summary>
        /// Verilen ISBN numarasını alarak bir ISBN nesnesi oluşturur.
        /// </summary>
        /// <param name="isbn">10 veya 13 karakterden oluşan ISBN numarası.</param>
        public ISBNHelper(string isbn) => this.setisbn(isbn);
        /// <summary>
        /// ISBN-10 biçimindeki kitap numarasını alır veya ayarlar. Ayarlama sırasında geçerli olup olmadığı kontrol edilir.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Verilen ISBN-10 numarası geçerli değilse bu istisna fırlatılır.
        /// </exception>
        public string Isbn10
        {
            get { return _isbn10; }
            set
            {
                if (TryIsValid(value, out string _c)) { this.setisbn(_c); }
                throw new NotSupportedException($"{_title.isbn} uyumsuzdur!");
            }
        }
        /// <summary>
        /// ISBN-13 biçimindeki kitap numarasını alır veya ayarlar. Ayarlama sırasında geçerli olup olmadığı kontrol edilir.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Verilen ISBN-13 numarası geçerli değilse bu istisna fırlatılır.
        /// </exception>
        public string Isbn13
        {
            get { return _isbn13; }
            set
            {
                if (TryIsValid(value, out string _c)) { this.setisbn(_c); }
                throw new NotSupportedException($"{_title.isbn} uyumsuzdur!");
            }
        }
        /// <summary>
        /// Bir ISBN-10 numarasını ISBN-13 biçimine dönüştürür. ISBN-10 numarasına 978 eklendikten sonra ISBN-13 kontrol hanesi hesaplanır.
        /// </summary>
        /// <param name="isbn">Dönüştürülecek ISBN-10 biçimindeki kitap numarası.</param>
        /// <returns>ISBN-13 biçimine dönüştürülmüş kitap numarası.</returns>
        /// <exception cref="ArgumentException">
        /// Verilen ISBN numarası 10 karakter uzunluğunda değilse bu istisna fırlatılır.
        /// </exception>
        public static string Convert10to13(string isbn)
        {
            string isbn13, isbn10 = cleanisbn(isbn);
            if (isbn10.Length == 10)
            {
                isbn13 = $"978{isbn10.Substring(0, 9)}";
                return String.Concat(isbn13, isbn13checksum(isbn13));
            }
            throw new ArgumentException($"{nameof(isbn)} değeri 10 karakterden oluşmalıdır!", nameof(isbn));
        }
        /// <summary>
        /// Bir ISBN-13 numarasını ISBN-10 biçimine dönüştürür. ISBN-13 numarasından ilk üç karakter atıldıktan sonra ISBN-10 kontrol hanesi hesaplanır.
        /// </summary>
        /// <param name="isbn">Dönüştürülecek ISBN-13 biçimindeki kitap numarası.</param>
        /// <returns>ISBN-10 biçimine dönüştürülmüş kitap numarası.</returns>
        /// <exception cref="ArgumentException">
        /// Verilen ISBN numarası 13 karakter uzunluğunda değilse bu istisna fırlatılır.
        /// </exception>
        public static string Convert13to10(string isbn)
        {
            string isbn10, isbn13 = cleanisbn(isbn);
            if (isbn13.Length == 13)
            {
                isbn10 = isbn13.Substring(3, 9);
                return String.Concat(isbn10, isbn10checksum(isbn10));
            }
            throw new ArgumentException($"{nameof(isbn)} değeri 13 karakterden oluşmalıdır!", nameof(isbn));
        }
        /// <summary>
        /// Verilen ISBN numarasının geçerli olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="isbn">Geçerliliği kontrol edilecek ISBN numarası.</param>
        /// <returns>ISBN numarası geçerliyse <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
        public static bool IsValid(string isbn) => TryIsValid(isbn, out _);
        /// <summary>
        /// Verilen ISBN numarasının geçerli olup olmadığını kontrol eder ve geçerli halini döner.
        /// </summary>
        /// <param name="isbn">Geçerliliği kontrol edilecek ISBN numarası.</param>
        /// <param name="correctisbn">Eğer geçerli bir ISBN ise düzeltilmiş ISBN numarasını döner.</param>
        /// <returns>ISBN numarası geçerliyse <see langword="true"/>, aksi takdirde <see langword="false"/> döner.</returns>
        public static bool TryIsValid(string isbn, out string correctisbn)
        {
            isbn = cleanisbn(isbn);
            if (isbn.Length == 10) { return validateisbn10(isbn, out correctisbn); }
            if (isbn.Length == 13) { return validateisbn13(isbn, out correctisbn); }
            correctisbn = "";
            return false;
        }
        #region Private
        private string _isbn10;
        private string _isbn13;
        private void setisbn(string isbn)
        {
            isbn = cleanisbn(isbn);
            if (isbn.Length == 10)
            {
                this._isbn10 = isbn;
                this._isbn13 = Convert10to13(isbn);
            }
            else if (isbn.Length == 13)
            {
                this._isbn10 = Convert13to10(isbn);
                this._isbn13 = isbn;
            }
            else
            {
                this._isbn10 = "";
                this._isbn13 = "";
            }
        }
        private static string cleanisbn(string isbn) => isbn.ToStringOrEmpty().Replace("-", "").Replace(" ", "");
        private static string isbn10checksum(string isbn)
        {
            int i, rem, sum = 0;
            for (i = 0; i < 9; i++) { sum += ((10 - i) * Convert.ToInt32(isbn[i].ToString())); }
            rem = sum % 11;
            return rem == 0 ? "0" : (rem == 1 ? "X" : (11 - rem).ToString());
        }
        private static string isbn13checksum(string isbn)
        {
            int i, rem, sum = 0;
            for (i = 0; i < 12; i++) { sum += (((i % 2 == 0) ? 1 : 3) * Convert.ToInt32(isbn[i].ToString())); }
            rem = sum % 10;
            return rem == 0 ? "0" : (10 - rem).ToString();
        }
        private static bool validateisbn10(string isbn, out string correctISBN)
        {
            correctISBN = String.Concat(isbn.Substring(0, 9), isbn10checksum(isbn));
            return (correctISBN == isbn);
        }
        private static bool validateisbn13(string isbn, out string correctISBN)
        {
            correctISBN = String.Concat(isbn.Substring(0, 12), isbn13checksum(isbn));
            return (correctISBN == isbn);
        }
        #endregion
    }
}