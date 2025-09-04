namespace BayuOrtak.Core.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Verilen istisnanın en içteki (inner) istisnasını döner.
        /// </summary>
        /// <param name="exception">İşlem yapılacak istisna.</param>
        /// <returns>En içteki istisna.</returns>
        public static Exception InnerEx(this Exception exception) => (exception.InnerException == null ? exception : exception.InnerException.InnerEx());
        /// <summary>
        /// Verilen bir istisna (exception) nesnesine göre uygun HTTP durum kodunu döndüren bir genişletme yöntemidir. Belirli istisna türleri için önceden tanımlı HTTP durum kodları eşleştirilir; eşleşme bulunamazsa varsayılan durum kodu döndürülür.
        /// </summary>
        /// <param name="exception">HTTP durum kodunun belirleneceği istisna nesnesi.</param>
        /// <param name="defaultvalue">Eşleşen bir durum kodu bulunamazsa döndürülecek varsayılan HTTP durum kodu (varsayılan olarak <see cref="HttpStatusCode.InternalServerError"/>).</param>
        /// <returns>İstisna türüne karşılık gelen <see cref="HttpStatusCode"/> değeri.</returns>
        /// <remarks>
        /// Bu yöntem, aşağıdaki istisna türlerini destekler:
        /// <list type="bullet">
        /// <item><description><see cref="UnauthorizedAccessException"/>: <see cref="HttpStatusCode.Unauthorized"/></description></item>
        /// <item><description><see cref="ArgumentException"/>: <see cref="HttpStatusCode.BadRequest"/></description></item>
        /// <item><description><see cref="TimeoutException"/>: <see cref="HttpStatusCode.RequestTimeout"/></description></item>
        /// <item><description><see cref="InvalidOperationException"/>: <see cref="HttpStatusCode.Conflict"/></description></item>
        /// <item><description><see cref="HttpRequestException"/> (StatusCode mevcutsa): İlgili durum kodu</description></item>
        /// <item><description><see cref="WebException"/> (HttpWebResponse mevcutsa): İlgili durum kodu</description></item>
        /// </list>
        /// </remarks>
        public static HttpStatusCode GetHttpStatusCode(this Exception exception, HttpStatusCode defaultvalue = HttpStatusCode.InternalServerError)
        {
            if (exception is UnauthorizedAccessException) { return HttpStatusCode.Unauthorized; }
            if (exception is ArgumentException) { return HttpStatusCode.BadRequest; }
            if (exception is TimeoutException) { return HttpStatusCode.RequestTimeout; }
            if (exception is InvalidOperationException) { return HttpStatusCode.Conflict; }
            if (exception is HttpRequestException _hre && _hre.StatusCode.HasValue) { return _hre.StatusCode.Value; }
            if (exception is WebException _we && _we.Response is HttpWebResponse _hwr) { return _hwr.StatusCode; }
            return defaultvalue;
        }
        /// <summary>
        /// Belirtilen hatanın ve varsa iç içe geçmiş tüm hata nesnelerinin bir yığın (Stack) olarak döndürülmesini sağlar. Bu yöntem, hata zincirindeki tüm Exception nesnelerini elde etmenize olanak tanır.
        /// </summary>
        /// <param name="exception">Kök hata (Exception) nesnesi.</param>
        /// <returns>Exception nesnelerinden oluşan bir yığın (Stack).</returns>
        public static Stack<Exception> AllException(this Exception exception)
        {
            var _r = new Stack<Exception>();
            do
            {
                _r.Push(exception);
                exception = exception.InnerException;
            } while (exception != null);
            return _r;
        }
        /// <summary>
        /// Belirtilen hatanın ve varsa iç içe geçmiş tüm hata nesnelerinin mesajlarını bir dizi (string[]) olarak döndürür. Bu yöntem, hata zincirindeki her bir Exception nesnesinin mesajına erişim sağlar.
        /// </summary>
        /// <param name="exception">Kök hata (Exception) nesnesi.</param>
        /// <returns>Hata mesajlarından oluşan bir string dizisi.</returns>
        public static string[] AllExceptionMessage(this Exception exception) => exception.AllException().Select(x => x.Message).ToArray();
        /// <summary>
        /// <see cref="DbUpdateException"/> durumunda varlık doğrulama hatalarını döner.
        /// </summary>
        /// <param name="exception">İşlem yapılacak istisna.</param>
        /// <returns>Varlık adı ve hata mesajı çiftlerini içeren dizi.</returns>
        public static (string propertyname, string errormessage)[] GetDbEntityValidationException(this Exception exception)
        {
            try
            {
                if (exception is DbUpdateException _due)
                {
                    var _r = new List<(string, string)>();
                    foreach (var entry in _due.Entries)
                    {
                        var _vrs = new List<ValidationResult>();
                        Validator.TryValidateObject(entry.Entity, new ValidationContext(entry.Entity), _vrs, true);
                        foreach (var validationResult in _vrs) { foreach (var memberName in validationResult.MemberNames) { _r.Add((memberName, validationResult.ErrorMessage)); } }
                    }
                    return _r.ToArray();
                }
                return Array.Empty<(string, string)>();
            }
            catch { return Array.Empty<(string, string)>(); }
        }
    }
}