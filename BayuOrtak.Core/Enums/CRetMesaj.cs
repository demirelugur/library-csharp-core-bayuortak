namespace BayuOrtak.Core.Enums
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System.ComponentModel;
    using static BayuOrtak.Core.Helper.OrtakTools;
    /// <summary>
    /// Geri dönüş mesajlarını temsil eden sınıf.
    /// </summary>
    public sealed class CRetMesaj
    {
        /// <summary>
        /// Geri dönüş mesajlarını temsil eden enum.
        /// </summary>
        public enum RetMesaj : byte
        {
            /// <summary>İsteğiniz başarılı bir şekilde sonuçlandı.</summary>
            [Description("İsteğiniz başarılı bir şekilde sonuçlandı.")]
            basari = 1,
            /// <summary>İşlem sırasında beklenmeyen bir sonuç meydana geldi! Yönetici ile iletişime geçiniz.</summary>
            [Description("İşlem sırasında beklenmeyen bir sonuç meydana geldi! Yönetici ile iletişime geçiniz.")]
            hata,
            /// <summary>Parametrelere uyumlu kayıt bulunamadı!</summary>
            [Description("Parametrelere uyumlu kayıt bulunamadı!")]
            kayityok,
            /// <summary>Girilen değer tarih biçimine uygun değildir! Kontrol ediniz.</summary>
            [Description("Girilen değer tarih biçimine uygun değildir! Kontrol ediniz.")]
            tarih,
            /// <summary>Metin içinde \"yasaklı\" kelimeler geçmektedir! Yönetici ile iletişime geçiniz.</summary>
            [Description("Metin içinde \"yasaklı\" kelimeler geçmektedir! Yönetici ile iletişime geçiniz.")]
            unethical,
            /// <summary>İşlem için yetkiniz bulunmamaktadır! Yönetici ile iletişime geçiniz.</summary>
            [Description("İşlem için yetkiniz bulunmamaktadır! Yönetici ile iletişime geçiniz.")]
            unauthority,
            /// <summary>Sunucu bilgisayar ile iletişim kurulamıyor! Yönetici ile iletişime geçiniz.</summary>
            [Description("Sunucu bilgisayar ile iletişim kurulamıyor! Yönetici ile iletişime geçiniz.")]
            unconnection,
            /// <summary>Girilebilecek maksimum karakter sınırı aşıldı! Yönetici ile iletişime geçiniz.</summary>
            [Description("Girilebilecek maksimum karakter sınırı aşıldı! Yönetici ile iletişime geçiniz.")]
            maxlength
        }
        /// <summary>
        /// Verilen geri dönüş mesajı için yerelleştirilmiş açıklama değerini döndürür.
        /// </summary>
        /// <param name="value">Geri dönüş mesajı enum değeri.</param>
        /// <param name="dil">Dil kodu; Türkçe için, &quot;en&quot; İngilizce için.</param>
        /// <returns>Yerelleştirilmiş mesajı döndürür.</returns>
        /// <exception cref="NotSupportedException">
        /// Eğer <paramref name="value"/> geçerli bir <see cref="RetMesaj"/> değeri değilse 
        /// veya desteklenmeyen bir dil girildiyse fırlatılır.
        /// </exception>
        public static string GetDescriptionLocalizationValue(RetMesaj value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "en")
            {
                switch (value)
                {
                    case RetMesaj.basari: return "Your request has been completed successfully.";
                    case RetMesaj.hata: return "An unexpected result occurred during the process! Contact the administrator.";
                    case RetMesaj.kayityok: return "No records matching the parameters were found.";
                    case RetMesaj.tarih: return "The entered value does not comply with the date format! Please check.";
                    case RetMesaj.unethical: return "\"Prohibited\" words appear in the text! Contact the administrator.";
                    case RetMesaj.unauthority: return "You are not authorized for the transaction! Contact the administrator.";
                    case RetMesaj.unconnection: return "Cannot communicate with the server computer! Contact the administrator.";
                    case RetMesaj.maxlength: return "The maximum character limit that can be entered has been exceeded! Contact the administrator.";
                    default: throw _other.ThrowNotSupportedForEnum<RetMesaj>();
                }
            }
            return value.GetDescription();
        }
    }
}