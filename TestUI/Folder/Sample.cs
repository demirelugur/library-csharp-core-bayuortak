namespace TestUI.Folder
{
    using BayuOrtak.Core.Attributes.DataAnnotations;
    using BayuOrtak.Core.Extensions;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using static BayuOrtak.Core.Enums.CKampusTypes;
    using static BayuOrtak.Core.Helper.GlobalConstants;
    internal class Sample // Not: Bu ihtimalin gelme durumu olmamasına rağmen ne olur olmaz yazılan kontrol
    {
        [Validation_Required]
        [DefaultValue_GuidEmpty]
        public Guid baglantiuploadid { get; set; }

        [Validation_Required]
        [Validation_RangePositiveInt32]
        [DefaultValue(0)]
        public int depid { get; set; }

        [Validation_Required]
        [EnumDataType(typeof(KampusTypes), ErrorMessage = _validationerrormessage.enumdatatype)]
        [DefaultValue(KampusTypes.baberti)]
        public KampusTypes kampustip { get; set; }

        [Validation_Required]
        [Validation_MinDateOnly]
        public DateOnly tarih { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public bool iskontrol => true;

        private static readonly Dictionary<string, string> DurumTypes = new Dictionary<string, string>() { { "tumu", "Tümü" }, { "basarili", "Başarılı" }, { "basarisiz", "Başarısız" } };
        private string _Durum;
        [Validation_Required]
        [Validation_Includes<string>(true, "tumu", "basarili", "basarisiz")]
        [DefaultValue("tumu")]
        public string durum { get { return _Durum; } set { _Durum = value; } }

        private string? _Search;
        [Validation_StringLength(30, 3)]
        public string? search
        {
            get
            {
                if (_Search == null) { return null; }
                return _Search.ToSeoFriendly();
            }
            set { _Search = value; }
        }

        [Validation_UrlHttp]
        [DefaultValue("")]
        public string? src_tr { get; set; }

        [Validation_Required]
        [Validation_StringLength(16, 16)]
        [Display(Name = "NHR Şifresi")]
        [DefaultValue("")]
        public string nhr_password { get; set; }
    }
}