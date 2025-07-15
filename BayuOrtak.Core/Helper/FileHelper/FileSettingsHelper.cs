namespace BayuOrtak.Core.Helper.FileHelper
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    public sealed class FileSettingsHelper : IEquatable<FileSettingsHelper>
    {
        #region Equals
        public override bool Equals(object other) => this.Equals(other as FileSettingsHelper);
        public override int GetHashCode() => HashCode.Combine(this.ext, this.size, this.filecount);
        public bool Equals(FileSettingsHelper other)
        {
            if (other is FileSettingsHelper _f) { return this.ext.IsEqual(_f.ext) && this.size == _f.size && this.filecount == _f.filecount; }
            return false;
        }
        #endregion
        private string[] _Ext;
        private long _Size;
        private byte _Filecount;
        [Validation_Required]
        [Validation_ArrayMinLength]
        [Display(Name = "Uzantı")]
        public string[] ext { get { return _Ext; } set { _Ext = (value ?? Array.Empty<string>()).Select(x => x.ToStringOrEmpty()).Where(x => x.Length > 1).Select(x => (x[0] == '.' ? x : $".{x}")).Select(x => x.ToLower()).OrderBy(x => x).Distinct().ToArray(); } }
        [Validation_Required]
        [Range(1, Int64.MaxValue, ErrorMessage = _validationerrormessage.range)]
        [DefaultValue(1048576)]
        [Display(Name = "Belge Boyutu")]
        public long size { get { return _Size; } set { _Size = value; } }
        [Validation_Required]
        [Range(1, Byte.MaxValue, ErrorMessage = _validationerrormessage.range)]
        [DefaultValue(1)]
        [Display(Name = "Belge Sayısı")]
        public byte filecount { get { return _Filecount; } set { _Filecount = value; } }
        [JsonIgnore]
        [IgnoreDataMember]
        public string getformatsize => FormatSize(Convert.ToDouble(size));
        public bool gettryfileisexception(ICollection<IFormFile> files, string dil, out string[] outvalues) => TryFileisException(files, this, dil, out outvalues);
        public FileSettingsHelper() : this(default, default, default) { }
        public FileSettingsHelper(string[] ext, long size, byte filecount)
        {
            this.ext = ext;
            this.size = size;
            this.filecount = filecount;
        }
        /// <summary>
        /// Verilen bir değeri uygun boyut biriminde biçimlendirir.
        /// </summary>
        /// <param name="value">Biçimlendirilecek boyut değeri.</param>
        /// <returns>Biçimlendirilmiş boyut.</returns>
        public static string FormatSize(double value)
        {
            if (value < 0 || Double.IsNaN(value)) { value = 0; }
            var j = 0;
            var sz = filesizeunits.Length - 1;
            while (value > 1024 && j < sz) { value /= 1024; j++; }
            return String.Join(" ", (Math.Ceiling(value * 100) / 100).ToString(), filesizeunits[j]);
        }
        /// <summary>
        /// Verilen bir değeri ayrıntılı biçimde biçimler.
        /// </summary>
        /// <param name="value">Biçimlendirilecek boyut değeri.</param>
        public static string FormatSizeDetail(double value)
        {
            if (value < 0 || Double.IsNaN(value)) { return ""; }
            if (value < 1024 || (value % 1024) == 0) { return FormatSize(value); }
            return $"{value.ToString()} {filesizeunits[0]} (~ {FormatSize(value)})";
        }
        /// <summary>
        /// Yükleme ayarlarına göre dosyaların geçerliliğini kontrol eder.
        /// </summary>
        /// <param name="files">Kontrol edilecek dosyaların koleksiyonu.</param>
        /// <param name="uploadSettings">Yükleme ayarları.</param>
        /// <param name="dil">İşlem sırasında kullanılacak dil (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <param name="outvalues">Hata mesajlarını içeren bir dizi.</param>
        /// <returns>Geçersizlik durumu (true/false).</returns>
        public static bool TryFileisException(ICollection<IFormFile> files, FileSettingsHelper uploadSettings, string dil, out string[] outvalues)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            try
            {
                if (files.Count > 0)
                {
                    uploadSettings = uploadSettings ?? new FileSettingsHelper();
                    Guard.CheckEmpty(uploadSettings.ext, nameof(uploadSettings.ext));
                    Guard.CheckZeroOrNegative(uploadSettings.size, nameof(uploadSettings.size));
                    var ie_files = files.Select(file => new
                    {
                        file,
                        uzn = file.GetFileExtension()
                    }).Select(x => new
                    {
                        filename = x.file.FileName,
                        x.uzn,
                        size = x.file.Length,
                        check_uzn = uploadSettings.ext.Contains(x.uzn),
                        check_size = x.file.Length <= uploadSettings.size
                    }).ToArray();
                    if (ie_files.Length > uploadSettings.filecount)
                    {
                        if (dil == "tr")
                        {
                            outvalues = new string[] {
                               "Yüklenecek maksimum dosya sayısını aştınız!",
                               $"Maksimum dosya sayısı: {uploadSettings.filecount.ToString()}"
                            };
                        }
                        else
                        {
                            outvalues = new string[] {
                               "You have exceeded the maximum number of files allowed to upload!",
                               $"Maximum file count: {uploadSettings.filecount.ToString()}"
                            };
                        }
                        return true;
                    }
                    if (ie_files.Any(x => !x.check_uzn))
                    {
                        if (dil == "tr")
                        {
                            outvalues = new string[] {
                               "Yüklenecek dosya uzantıları uyumsuzdur!",
                               $"Uyumsuz olan dosyalar: {String.Join(", ", ie_files.Where(x => !x.check_uzn).OrderBy(x => x.filename).Select(x => x.filename).ToArray())}",
                               $"İzin verilen uzantı türleri: {String.Join(", ", uploadSettings.ext)}"
                            };
                        }
                        else
                        {
                            outvalues = new string[] {
                                "The file extensions are not compatible!",
                                $"Incompatible files: {String.Join(", ", ie_files.Where(x => !x.check_uzn).OrderBy(x => x.filename).Select(x => x.filename).ToArray())}",
                                $"Allowed extension types: {String.Join(", ", uploadSettings.ext)}"
                            };
                        }
                        return true;
                    }
                    if (ie_files.Any(x => !x.check_size))
                    {
                        if (dil == "tr")
                        {
                            outvalues = new string[] {
                                "Tek bir dosya için izin verilen yükleme miktarını aştınız!",
                                $"Kapasite miktarı aşan dosyalar: {String.Join(", ", ie_files.Where(x => !x.check_size).OrderByDescending(x => x.size).ThenBy(x => x.filename).Select(x => String.Join(", ", x.filename, FormatSize(x.size))).ToArray())}",
                                $"Tek bir dosya için izin verilen maksimum boyut miktarı: {uploadSettings.getformatsize}"
                            };
                        }
                        else
                        {
                            outvalues = new string[] {
                                "You have exceeded the allowed upload size for a single file!",
                                $"Files exceeding the size limit: {String.Join(", ", ie_files.Where(x => !x.check_size).OrderByDescending(x => x.size).ThenBy(x => x.filename).Select(x => String.Join(", ", x.filename, FormatSize(x.size))).ToArray())}",
                                $"Maximum allowed size for a single file: {uploadSettings.getformatsize}"
                            };
                        }
                        return true;
                    }
                }
                outvalues = Array.Empty<string>();
                return false;
            }
            catch (Exception ex)
            {
                outvalues = ex.AllExceptionMessage();
                return true;
            }
        }
    }
}