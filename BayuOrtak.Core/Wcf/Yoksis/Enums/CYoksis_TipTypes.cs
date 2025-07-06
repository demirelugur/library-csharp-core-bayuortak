namespace BayuOrtak.Core.Wcf.Yoksis.Enums
{
    using BayuOrtak.Core.Base;
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper;
    using System.ComponentModel;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public class CYoksis_TipTypes : BaseEnum<CYoksis_TipTypes.Yoksis_TipTypes>
    {
        public enum Yoksis_TipTypes : byte
        {
            [Description("Öğrenim Bilgileri")]
            ogrenimbilgileri = 1,
            [Description("Kitap")]
            kitap,
            [Description("Makale")]
            makale,
            [Description("Bildiri")]
            bildiri,
            [Description("Proje")]
            proje,
            [Description("Ders")]
            ders,
            [Description("Lisansüstü Tez Danışmanlığı")]
            lisansustutezdanismanligi,
            [Description("Ödül")]
            odul,
            [Description("Patent")]
            patent,
            [Description("Üyelik")]
            uyelik,
            [Description("Sanat ve Tasarım Etkinlikleri")]
            sanatvetasarimetkinlikleri,
            [Description("İdari Görev")]
            idarigorev,
            [Description("Üniversite Dışı Deneyim")]
            universitedisideneyim,
            [Description("Editör")]
            editor,
            [Description("Yabancı Dil")]
            yabancidil,
            [Description("Araştırma, Kurs, Sertifika v.b.")]
            arastirmakurssertifika,
            [Description("Tasarım")]
            tasarim,
            [Description("Atıf")]
            atif
        }
        /// <summary>
        /// <paramref name="tur"/> değerleri
        /// <para>- 1: Yayınlar (Makale, Bildiri, Kitap)</para>
        /// <para>- 2: Sanatsal Faaliyetler</para>
        /// <para>- 3: Projeler</para>
        /// <para>- 5: Patentler</para>
        /// <para>- 8: Tez</para>
        /// </summary>
        public static bool TryCitationTur(int? tur, string eseradi, out Yoksis_TipTypes _outvalue)
        {
            _outvalue = default;
            if (!tur.HasValue) { return false; }
            var _s = eseradi.ToSeoFriendly();
            return (tur.Value, _s) switch
            {
                (1, "makale") => assign(out _outvalue, Yoksis_TipTypes.makale),
                (1, "bildiri") => assign(out _outvalue, Yoksis_TipTypes.bildiri),
                (1, "kitap") => assign(out _outvalue, Yoksis_TipTypes.kitap),
                (2, "") => assign(out _outvalue, Yoksis_TipTypes.sanatvetasarimetkinlikleri),
                (3, "") => assign(out _outvalue, Yoksis_TipTypes.proje),
                (5, "") => assign(out _outvalue, Yoksis_TipTypes.patent),
                (8, "tez") => assign(out _outvalue, Yoksis_TipTypes.lisansustutezdanismanligi),
                _ => false
            };
        }
        private static bool assign(out Yoksis_TipTypes _outvalue, Yoksis_TipTypes value)
        {
            _outvalue = value;
            return true;
        }
        public static string GetDescriptionLocalizationValue(Yoksis_TipTypes value, string dil)
        {
            Guard.UnSupportLanguage(dil, nameof(dil));
            if (dil == "tr") { return value.GetDescription(); }
            switch (value)
            {
                case Yoksis_TipTypes.ogrenimbilgileri: return "Education Information";
                case Yoksis_TipTypes.kitap: return "Book";
                case Yoksis_TipTypes.makale: return "Article";
                case Yoksis_TipTypes.bildiri: return "Campus";
                case Yoksis_TipTypes.proje: return "Project";
                case Yoksis_TipTypes.ders: return "Lecture";
                case Yoksis_TipTypes.lisansustutezdanismanligi: return "Graduate Thesis Consultancy";
                case Yoksis_TipTypes.odul: return "Award";
                case Yoksis_TipTypes.patent: return "Patent";
                case Yoksis_TipTypes.uyelik: return "Membership";
                case Yoksis_TipTypes.sanatvetasarimetkinlikleri: return "Arts Activity";
                case Yoksis_TipTypes.idarigorev: return "Administrative Duty";
                case Yoksis_TipTypes.universitedisideneyim: return "Non-University Experience";
                case Yoksis_TipTypes.editor: return "Editor";
                case Yoksis_TipTypes.yabancidil: return "Foreign Lang Exam";
                case Yoksis_TipTypes.arastirmakurssertifika: return "Research, Course, Certificate etc.";
                case Yoksis_TipTypes.tasarim: return "Design";
                case Yoksis_TipTypes.atif: return "Citation";
                default: throw _other.ThrowNotSupportedForEnum<Yoksis_TipTypes>();
            }
        }
    }
}