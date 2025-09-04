namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using BayuOrtak.Core.Extensions;
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
        public override bool Equals(object other) => Equals(other as FileSettingsHelper);
        public override int GetHashCode() => HashCode.Combine(ext, size, filecount);
        public bool Equals(FileSettingsHelper other) => (other != null && this.ext.IsEqual(other.ext) && this.size == other.size && this.filecount == other.filecount);
        #endregion
        private string[] _Ext;
        private long _Size;
        private byte _Filecount;
        [Validation_Required]
        [Validation_ArrayMinLength]
        [Display(Name = "Uzantı")]
        public string[] ext { get { return _Ext; } set { _Ext = (value ?? Array.Empty<string>()).Select(x => x.ToStringOrEmpty()).Where(x => x.Length > 1).Select(x => x[0] == '.' ? x : $".{x}").Select(x => x.ToLower()).OrderBy(x => x).Distinct().ToArray(); } }
        [Validation_Required]
        [Range(1, long.MaxValue, ErrorMessage = _validationerrormessage.range)]
        [DefaultValue(1048576)]
        [Display(Name = "Belge Boyutu")]
        public long size { get { return _Size; } set { _Size = value; } }
        [Validation_Required]
        [Range(1, byte.MaxValue, ErrorMessage = _validationerrormessage.range)]
        [DefaultValue(1)]
        [Display(Name = "Belge Sayısı")]
        public byte filecount { get { return _Filecount; } set { _Filecount = value; } }
        [JsonIgnore]
        [IgnoreDataMember]
        public string getformatsize => FormatSize(Convert.ToDouble(size));
        public bool gettryfileisexception(ICollection<IFormFile> files, string dil, out string[] errors) => TryFileisException(files, this, dil, out errors);
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
            var _j = 0;
            var _sz = filesizeunits.Length - 1;
            while (value > 1024 && _j < _sz) { value /= 1024; _j++; }
            return String.Join(" ", (Math.Ceiling(value * 100) / 100).ToString(), filesizeunits[_j]);
        }
        /// <summary>
        /// Verilen bir değeri ayrıntılı biçimde biçimler.
        /// </summary>
        /// <param name="value">Biçimlendirilecek boyut değeri.</param>
        public static string FormatSizeDetail(double value)
        {
            if (value < 0 || Double.IsNaN(value)) { return ""; }
            if (value < 1024 || value % 1024 == 0) { return FormatSize(value); }
            return $"{value.ToString()} {filesizeunits[0]} (~ {FormatSize(value)})";
        }
        /// <summary>
        /// Yükleme ayarlarına göre dosyaların geçerliliğini kontrol eder.
        /// </summary>
        /// <param name="files">Kontrol edilecek dosyaların koleksiyonu.</param>
        /// <param name="uploadsettings">Yükleme ayarları.</param>
        /// <param name="dil">İşlem sırasında kullanılacak dil (örneğin: &quot;tr&quot; veya &quot;en&quot;).</param>
        /// <param name="errors">Hata mesajlarını içeren bir dizi.</param>
        /// <returns>Geçersizlik durumu (true/false).</returns>
        public static bool TryFileisException(ICollection<IFormFile> files, FileSettingsHelper uploadsettings, string dil, out string[] errors)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            try
            {
                if (files.Count > 0)
                {
                    uploadsettings = uploadsettings ?? new FileSettingsHelper();
                    Guard.CheckEmpty(uploadsettings.ext, nameof(uploadsettings.ext));
                    Guard.CheckZeroOrNegative(uploadsettings.size, nameof(uploadsettings.size));
                    var _files = files.Select(file => new
                    {
                        file,
                        uzn = file.GetFileExtension()
                    }).Select(x => new
                    {
                        filename = x.file.FileName,
                        x.uzn,
                        size = x.file.Length,
                        check_uzn = uploadsettings.ext.Contains(x.uzn),
                        check_size = x.file.Length <= uploadsettings.size
                    }).ToArray();
                    if (_files.Length > uploadsettings.filecount)
                    {
                        if (dil == "en")
                        {
                            errors = new string[] {
                               "You have exceeded the maximum number of files allowed to upload!",
                               $"Maximum file count: {uploadsettings.filecount.ToString()}"
                            };
                        }
                        else
                        {
                            errors = new string[] {
                               "Yüklenecek maksimum dosya sayısını aştınız!",
                               $"Maksimum dosya sayısı: {uploadsettings.filecount.ToString()}"
                            };
                        }
                        return true;
                    }
                    if (_files.Any(x => !x.check_uzn))
                    {
                        if (dil == "en")
                        {
                            errors = new string[] {
                                "The file extensions are not compatible!",
                                $"Incompatible files: {string.Join(", ", _files.Where(x => !x.check_uzn).OrderBy(x => x.filename).Select(x => x.filename).ToArray())}",
                                $"Allowed extension types: {string.Join(", ", uploadsettings.ext)}"
                            };
                        }
                        else
                        {
                            errors = new string[] {
                               "Yüklenecek dosya uzantıları uyumsuzdur!",
                               $"Uyumsuz olan dosyalar: {string.Join(", ", _files.Where(x => !x.check_uzn).OrderBy(x => x.filename).Select(x => x.filename).ToArray())}",
                               $"İzin verilen uzantı türleri: {string.Join(", ", uploadsettings.ext)}"
                            };
                        }
                        return true;
                    }
                    if (_files.Any(x => !x.check_size))
                    {
                        if (dil == "en")
                        {
                            errors = new string[] {
                                "You have exceeded the allowed upload size for a single file!",
                                $"Files exceeding the size limit: {string.Join(", ", _files.Where(x => !x.check_size).OrderByDescending(x => x.size).ThenBy(x => x.filename).Select(x => string.Join(", ", x.filename, FormatSize(x.size))).ToArray())}",
                                $"Maximum allowed size for a single file: {uploadsettings.getformatsize}"
                            };
                        }
                        else
                        {
                            errors = new string[] {
                                "Tek bir dosya için izin verilen yükleme miktarını aştınız!",
                                $"Kapasite miktarı aşan dosyalar: {string.Join(", ", _files.Where(x => !x.check_size).OrderByDescending(x => x.size).ThenBy(x => x.filename).Select(x => string.Join(", ", x.filename, FormatSize(x.size))).ToArray())}",
                                $"Tek bir dosya için izin verilen maksimum boyut miktarı: {uploadsettings.getformatsize}"
                            };
                        }
                        return true;
                    }
                }
                errors = Array.Empty<string>();
                return false;
            }
            catch (Exception ex)
            {
                errors = ex.AllExceptionMessage();
                return true;
            }
        }
    }
}