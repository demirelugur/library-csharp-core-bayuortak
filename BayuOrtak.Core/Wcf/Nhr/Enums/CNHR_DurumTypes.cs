namespace BayuOrtak.Core.Wcf.Nhr.Enums
{
    using BayuOrtak.Core.Base;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Wcf.Nhr.Helper;
    using System;
    using System.ComponentModel;
    using Wcf_Nhr_personelinfo;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public class CNhr_DurumTypes : BaseEnum<CNhr_DurumTypes.Nhr_DurumTypes> // Not: 1) Enum değerleri [Portal2025].[dbo].[Ayarlar] tablosu içerisinde tanımlanmaktadır
    {
        [Flags]
        public enum Nhr_DurumTypes : byte
        {
            /// <summary>
            /// 35. Madde ile görevlendirilmiş
            /// </summary>
            [Description("35. Madde ile görevlendirilmiş")]
            madde35ilegiden = 1,
            /// <summary>
            /// İdari Görevli Gelen
            /// </summary>
            [Description("İdari Görevli Gelen")]
            idarigorevgelen = 2,
            /// <summary>
            /// Yabancı Uyruklu - Tam Zamanlı
            /// </summary>
            [Description("Yabancı Uyruklu - Tam Zamanlı")]
            yabanciuyruklutamzamanli = 4,
            /// <summary>
            /// Engelli
            /// </summary>
            [Description("Engelli")]
            engelli = 8
        }
        public static Nhr_DurumTypes? GetDurumTypes(wspersonel wspersonel) => wspersonel.GetDurumTypes();
        public static string GetDescriptionLocalizationValue(Nhr_DurumTypes value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "en")
            {
                switch (value)
                {
                    case Nhr_DurumTypes.madde35ilegiden: return "Assigned under Article 35";
                    case Nhr_DurumTypes.idarigorevgelen: return "Administrative Staff Incoming";
                    case Nhr_DurumTypes.yabanciuyruklutamzamanli: return "Foreign National - Full Time";
                    case Nhr_DurumTypes.engelli: return "Disabled";
                    default: throw _other.ThrowNotSupportedForEnum<Nhr_DurumTypes>();
                }
            }
            return value.GetDescription();
        }
    }
}