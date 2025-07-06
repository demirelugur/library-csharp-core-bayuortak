namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Helper;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public static class IOExtensions
    {
        #region byte[]
        /// <summary>
        /// İkili verileri base64 biçiminde bir dizeye dönüştürür.
        /// </summary>
        /// <param name="bytes">Dönüştürülecek byte dizisi.</param>
        /// <param name="mimeType">Mime türü.</param>
        /// <returns>Base64 biçimindeki dize.</returns>
        public static string ToStringFromBinary(this byte[] bytes, string mimeType) => $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
        /// <summary>
        /// Verilen byte dizisini belirtilen fiziksel yola asenkron olarak yükler.
        /// </summary>
        public static async Task FileUploadAsync(this byte[] bytes, string physicallyPath, CancellationToken cancellationToken = default)
        {
            Guard.CheckEmpty(bytes, nameof(bytes));
            Guard.CheckEmpty(physicallyPath, nameof(physicallyPath));
            _file.DirectoryCreate(new FileInfo(physicallyPath).DirectoryName);
            using (var fs = new FileStream(physicallyPath, FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
            {
                await fs.WriteAsync(bytes.AsMemory(0, bytes.Length), cancellationToken);
                await fs.FlushAsync(cancellationToken);
                fs.Close();
            }
        }
        /// <summary>
        /// Bir byte dizisini IFormFile nesnesine dönüştürür.
        /// </summary>
        /// <param name="bytes">Dönüştürülecek byte dizisi.</param>
        /// <param name="fileName">Dosya adı.</param>
        /// <param name="contentType">Dosyanın içerik türü (MIME type).</param>
        /// <param name="name">Content-Disposition başlığının adı. Otomatik &quot;file&quot; değeri atanır.</param>
        /// <returns>IFormFile nesnesi.</returns>
        public static IFormFile ToIFormFile(this byte[] bytes, string fileName, string contentType, string name = "file")
        {
            Guard.CheckEmpty(bytes, nameof(bytes));
            Guard.CheckEmpty(fileName, nameof(fileName));
            Guard.CheckEmpty(contentType, nameof(contentType));
            Guard.CheckEmpty(name, nameof(name));
            using (var ms = new MemoryStream(bytes))
            {
                return new FormFile(ms, 0, bytes.Length, name, fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = contentType
                };
            }
        }
        /// <summary>
        /// İki dosya byte dizisinin eşit olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="file1">Karşılaştırılacak ilk byte dizisi.</param>
        /// <param name="file2">Karşılaştırılacak ikinci byte dizisi.</param>
        /// <returns>İki byte dizisi eşitse <see langword="true"/>, değilse <see langword="false"/> döner.</returns>
        public static bool IsFileBytesEqual(this byte[] file1, byte[] file2)
        {
            file1 = file1 ?? Array.Empty<byte>();
            file2 = file2 ?? Array.Empty<byte>();
            if (file1.Length == 0 && file2.Length == 0) { return true; }
            if (file1.Length != file2.Length) { return false; }
            int i, l = file1.Length;
            for (i = 0; i < l; i++) { if (file1[i] != file2[i]) { return false; } }
            return true;
        }
        #endregion
        #region IFormFile
        /// <summary>
        /// Verilen IFormFile nesnesinden dosya adını döner (uzantısız).
        /// </summary>
        /// <param name="file">IFormFile nesnesi.</param>
        /// <returns>Dosya adı (uzantısız).</returns>
        public static string GetFileName(this IFormFile file) => Path.GetFileNameWithoutExtension(file.FileName);
        /// <summary>
        /// Verilen IFormFile nesnesinden dosya uzantısını döner (ilk karater &quot;.&quot; olacak biçimde)
        /// </summary>
        /// <param name="file">IFormFile nesnesi.</param>
        /// <returns>Dosya uzantısı (ilk karater &quot;.&quot; olacak biçimde).</returns>
        public static string GetFileExtension(this IFormFile file) => Path.GetExtension(file.FileName).ToLower();
        /// <summary>
        /// Verilen IFormFile nesnesini belirtilen fiziksel yola asenkron olarak yükler.
        /// </summary>
        public static async Task FileUploadAsync(this IFormFile file, string physicallyPath, CancellationToken cancellationToken = default)
        {
            Guard.CheckNull(file, nameof(file));
            Guard.CheckEmpty(physicallyPath, nameof(physicallyPath));
            _file.DirectoryCreate(new FileInfo(physicallyPath).DirectoryName);
            using (var fs = new FileStream(physicallyPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await file.CopyToAsync(fs, cancellationToken);
            }
        }
        /// <summary>
        /// Bir IFormFile nesnesini byte dizisine dönüştürür.
        /// </summary>
        public static async Task<byte[]> ToByteArrayAsync(this IFormFile formFile, CancellationToken cancellationToken = default)
        {
            Guard.CheckNull(formFile, nameof(formFile));
            using (var ms = new MemoryStream())
            {
                await formFile.CopyToAsync(ms, cancellationToken);
                return ms.ToArray();
            }
        }
        #endregion
        #region Image
        /// <summary>
        /// Görüntüyü belirtilen formatta bayt dizisine dönüştürür.
        /// </summary>
        /// <param name="image">Dönüştürülecek görüntü.</param>
        /// <param name="format">Görüntünün kaydedileceği format.</param>
        /// <returns>Görüntünün bayt dizisi temsilini döndürür.</returns>
        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Görüntüyü belirtilen boyuta yeniden boyutlandırır.
        /// </summary>
        /// <param name="image">Yeniden boyutlandırılacak görüntü.</param>
        /// <param name="size">Hedef boyut.</param>
        /// <returns>Yeniden boyutlandırılmış Bitmap görüntüsünü döndürür.</returns>
        /// <exception cref="ArgumentException">Boyut parametresi geçersiz veya boş olduğunda fırlatılır.</exception>
        /// <remarks>Kullandığı yerde Dispose edilmelidir!</remarks>
        public static Bitmap Resize(this Image image, Size size)
        {
            if (size.IsEmpty) { throw new ArgumentException($"{nameof(size)} parametresi geçerli olmalıdır!", nameof(size)); }
            var bm = new Bitmap(size.Width, size.Height); // using kullanılırsa bitmap değerleri iletilmemekte
            using (var g = Graphics.FromImage(bm))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(image, 0, 0, size.Width, size.Height);
                return bm;
            }
        }
        /// <summary>
        /// Verilen yeni genişlik(width) değerine göre resmin yüksekliğini(height), en boy oranını koruyarak hesaplar.
        /// </summary>
        /// <param name="image">Orijinal resim nesnesi.</param>
        /// <param name="width">Yeni genişlik değeri (piksel cinsinden).</param>
        /// <returns>En boy oranı korunarak hesaplanan yeni yükseklik değeri (piksel cinsinden).</returns>
        public static int CalculateHeight(this Image image, int width) => Convert.ToInt32(image.Height * (Convert.ToSingle(width) / image.Width));
        /// <summary>
        /// Verilen yeni yükseklik(height) değerine göre resmin genişliğini(width), en boy oranını koruyarak hesaplar.
        /// </summary>
        /// <param name="image">Orijinal resim nesnesi.</param>
        /// <param name="height">Yeni yükseklik değeri (piksel cinsinden).</param>
        /// <returns>En boy oranı korunarak hesaplanan yeni genişlik değeri (piksel cinsinden).</returns>
        public static int CalculateWidth(this Image image, int height) => Convert.ToInt32(image.Width * (Convert.ToSingle(height) / image.Height));
        #endregion
        #region DirectoryInfo
        /// <summary>
        /// Belirtilen kaynak dizinini hedef dizine kopyalar.
        /// </summary>
        /// <param name="source">Kaynak dizini temsil eden DirectoryInfo nesnesi.</param>
        /// <param name="target">Hedef dizini temsil eden DirectoryInfo nesnesi.</param>
        public static void CopyAll(this DirectoryInfo source, DirectoryInfo target)
        {
            _file.DirectoryCreate(target.FullName);
            foreach (var item_file in source.GetFiles()) { item_file.CopyTo(Path.Combine(target.FullName, item_file.Name), true); }
            foreach (var item_dir in source.GetDirectories()) { item_dir.CopyAll(target.CreateSubdirectory(item_dir.Name)); }
        }
        #endregion
    }
}