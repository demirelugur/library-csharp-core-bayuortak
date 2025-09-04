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
    public class CNhr_UnvanTypes : BaseEnum<CNhr_UnvanTypes.Nhr_UnvanTypes>
    {
        [Flags]
        public enum Nhr_UnvanTypes : byte
        {
            /// <summary>
            /// Akademik
            /// </summary>
            [Description("Akademik")]
            aka = 1,
            /// <summary>
            /// İdari
            /// </summary>
            [Description("İdari")]
            ida = 2,
            /// <summary>
            /// Sözleşmeli
            /// </summary>
            [Description("Sözleşmeli")]
            soz = 4,
            /// <summary>
            /// Diğer
            /// </summary>
            [Description("Diğer")]
            dig = 8
        }
        public static Nhr_UnvanTypes GetUnvanTypes(wspersonel wspersonel) => wspersonel.GetUnvanTypes();
        public static string GetDescriptionLocalizationValue(Nhr_UnvanTypes value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "en")
            {
                switch (value)
                {
                    case Nhr_UnvanTypes.aka: return "Academic";
                    case Nhr_UnvanTypes.ida: return "Administrative";
                    case Nhr_UnvanTypes.soz: return "Contract";
                    case Nhr_UnvanTypes.dig: return "Other";
                    default: throw _other.ThrowNotSupportedForEnum<Nhr_UnvanTypes>();
                }
            }
            return value.GetDescription();
        }
    }
}