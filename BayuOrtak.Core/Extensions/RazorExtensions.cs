namespace BayuOrtak.Core.Extensions
{
    using BayuOrtak.Core.Enums;
    using BayuOrtak.Core.Helper;
    using BayuOrtak.Core.Helper.FileHelper;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Text.Encodings.Web;
    using static BayuOrtak.Core.Helper.OrtakTools;
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static class RazorExtensions
    {
        /// <summary>
        /// Verilen model nesnesini JSON biçiminde HTML içerik olarak döndürür.
        /// Model, JSON biçimine dönüştürülerek Razor görünümünde kullanılmak üzere işlenir.
        /// </summary>
        public static IHtmlContent RenderJson(this IHtmlHelper htmlhelper, object model)
        {
            Guard.CheckNull(model, nameof(model));
            return htmlhelper.Raw(_to.ToJSON(model));
        }
        /// <summary>
        /// Belirtilen HTML özniteliğini, duruma bağlı olarak ekler veya boş döndürür.
        /// <list type="bullet">
        ///   <item><description><c><see cref="ElementStateTypes.Checked"/></c> için örnek: &lt;input type=&quot;radio&quot; value=&quot;1&quot; id=&quot;durum1&quot; name=&quot;durum&quot; @Html.SetHtmlAttribute(item.durum, ElementStateTypes.Checked) /&gt;</description></item>
        ///   <item><description><c><see cref="ElementStateTypes.Selected"/></c> için örnek: &lt;!option value=&quot;1&quot; @Html.SetHtmlAttribute(item.durum)&gt;Göster&lt;/!option&gt;</description></item>
        ///   <item><description><c><see cref="ElementStateTypes.Disabled"/></c> için örnek: &lt;input type=&quot;radio&quot; value=&quot;1&quot; id=&quot;durum1&quot; name=&quot;durum&quot; @Html.SetHtmlAttribute(item.durum, ElementStateTypes.Disabled) /&gt;</description></item>
        /// </list>
        /// </summary>
        /// <param name="htmlhelper">HTML oluşturucu yardımcı nesnesi.</param>
        /// <param name="value">Özniteliğin eklenip eklenmeyeceğini belirten boolean değer.</param>
        /// <param name="state">Eklenmek istenen HTML özniteliği.</param>
        /// <returns>Öznitelik atanmış bir HTML içeriği veya boş bir string.</returns>
        public static IHtmlContent SetHtmlAttribute(this IHtmlHelper htmlhelper, bool value, ElementStateTypes state = ElementStateTypes.Selected)
        {
            if (value)
            {
                var _t = state.ToString().ToLower();
                return new HtmlString($"{_t}=\"{_t}\"");
            }
            return HtmlString.Empty;
        }
        /// <summary>
        /// Versiyon bilgisi içeren bir JavaScript veya CSS kaynağı için HTML etiketini oluşturur.
        /// Dosya yolu, versiyon ve opsiyonel HTML özellikleri dikkate alınarak ilgili etiket döndürülür.
        /// </summary>
        public static IHtmlContent GenerateVersionedResourceTag(this IHtmlHelper htmlhelper, bool isdebug, Version version, string filepath, object htmlattributes = null)
        {
            filepath = filepath.ToStringOrEmpty();
            if (filepath == "") { return HtmlString.Empty; }
            if (filepath[0] == '~') { filepath = filepath.Substring(1); }
            if (filepath[0] != '/') { filepath = String.Concat("/", filepath); }
            var extension = Path.GetExtension(filepath);
            if (!extension.Includes(".js", ".css")) { return HtmlString.Empty; }
            var base64Version = Convert.ToBase64String(Encoding.UTF8.GetBytes((version ?? new Version("0.0.0.1")).ToString())).Replace("=", "");
            TagBuilder tb;
            if (extension == ".css")
            {
                tb = new TagBuilder("link");
                tb.Attributes.Add("href", $"{filepath}?v={base64Version}");
                tb.Attributes.Add("rel", "stylesheet");
                tb.Attributes.Add("type", "text/css");
                tb.TagRenderMode = TagRenderMode.SelfClosing;
            }
            else
            {
                tb = new TagBuilder("script");
                tb.Attributes.Add("src", $"{filepath}?v={base64Version}");
                tb.Attributes.Add("type", "module");
                tb.TagRenderMode = TagRenderMode.Normal;
            }
            if (htmlattributes != null) { tb.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlattributes), true); }
            if (!isdebug)
            {
                var minifiedPath = filepath.Replace(extension, $".min{extension}");
                if (extension == ".css") { tb.MergeAttribute("href", $"{minifiedPath}?v={base64Version}", true); }
                else { tb.MergeAttribute("src", $"{minifiedPath}?v={base64Version}", true); }
            }
            using (var sw = new StringWriter())
            {
                tb.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }
        /// <summary>
        /// Uzun tam sayı değerler için bir input alanı oluşturur.
        /// ID, isim, değer ve diğer HTML özellikleri ayarlanarak Razor görünümünde kullanılmak üzere döndürülür.
        /// </summary>
        public static IHtmlContent InputField_forlong(this IHtmlHelper htmlhelper, string name, long value = 0, object htmlattributes = null)
        {
            var tb = new TagBuilder("input");
            tb.Attributes.Add("id", name);
            tb.Attributes.Add("min", "0");
            tb.Attributes.Add("max", Int32.MaxValue.ToString());
            tb.Attributes.Add("step", "1");
            tb.Attributes.Add("onblur", "__InputField.long(this);");
            if (htmlattributes != null) { tb.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlattributes), true); }
            tb.MergeAttribute("name", name, true);
            tb.MergeAttribute("value", value.ToString(), true);
            tb.MergeAttribute("type", "number", true);
            tb.TagRenderMode = TagRenderMode.SelfClosing;
            using (var sw = new StringWriter())
            {
                tb.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }
        /// <summary>
        /// Ondalık sayı değerler için bir input alanı oluşturur.
        /// ID, isim, değer ve diğer HTML özellikleri ayarlanarak Razor görünümünde kullanılmak üzere döndürülür.
        /// </summary>
        public static IHtmlContent InputField_fordecimal(this IHtmlHelper htmlhelper, string name, decimal value = Decimal.Zero, object htmlattributes = null)
        {
            var tb = new TagBuilder("input");
            tb.Attributes.Add("id", name);
            tb.Attributes.Add("min", "0");
            tb.Attributes.Add("max", Int32.MaxValue.ToString());
            tb.Attributes.Add("step", "0.01");
            tb.Attributes.Add("onblur", "__InputField.decimal(this);");
            if (htmlattributes != null) { tb.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlattributes), true); }
            tb.MergeAttribute("name", name, true);
            tb.MergeAttribute("value", value.ToString("0.##", CultureInfo.InvariantCulture), true);
            tb.MergeAttribute("type", "number", true);
            tb.TagRenderMode = TagRenderMode.SelfClosing;
            using (var sw = new StringWriter())
            {
                tb.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }
        /// <summary>
        /// Dosya yükleme alanı oluşturur. Kabul edilen dosya türleri, maksimum dosya boyutu ve dosya sayısı gibi özellikleri ayarlanabilir.
        /// </summary>
        public static IHtmlContent InputField_forfile(this IHtmlHelper htmlhelper, FileSettingsHelper properties, string name, object htmlattributes = null)
        {
            Guard.CheckNull(properties, nameof(properties));
            var tb = new TagBuilder("input");
            tb.Attributes.Add("id", name);
            tb.Attributes.Add("accept", String.Join(",", properties.ext)); // htmlattributes yukarısında yazılmasının sebebi gerektiğinde accept: image/* gibi kayıt set edilebilmesi için
            if (htmlattributes != null) { tb.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlattributes), true); }
            tb.MergeAttribute("data-size", properties.size.ToString(), true);
            if (properties.filecount > 1)
            {
                tb.MergeAttribute("data-filecount", properties.filecount.ToString(), true);
                tb.MergeAttribute("multiple", "multiple", true);
            }
            tb.MergeAttribute("name", name, true);
            tb.MergeAttribute("type", "file", true);
            tb.TagRenderMode = TagRenderMode.SelfClosing;
            using (var sw = new StringWriter())
            {
                tb.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }
        /// <summary>
        /// Inputmask kütüphanesini kullanarak Türkiye telefon numarası biçiminde uygun bir input alanı oluşturur.
        /// Girilen değerin telefon numarası biçimine uygun olması için maske uygular.
        /// </summary>
        public static IHtmlContent InputMask_forphonenumbertr(this IHtmlHelper htmlhelper, string name, string value = "", object htmlattributes = null)
        {
            var tb = new TagBuilder("input");
            tb.Attributes.Add("id", name); // '(501) 234-5678'.replace(/[()\-\s]/g, '') -> 5012345678
            if (htmlattributes != null) { tb.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlattributes), true); }
            checkClassInputMask_private(tb);
            tb.MergeAttribute("data-mask", "(999) 999-9999", true);
            value = value.BeautifyPhoneNumberTR();
            if (value != "") { tb.MergeAttribute("value", value, true); }
            tb.MergeAttribute("type", "text", true);
            tb.MergeAttribute("name", name, true);
            tb.TagRenderMode = TagRenderMode.SelfClosing;
            using (var sw = new StringWriter())
            {
                tb.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }
        /// <summary>
        /// Inputmask kütüphanesini kullanarak pozitif sayı girişi için maskeleme özelliğine sahip bir input alanı oluşturur.
        /// Tekrar sayısı ve isteğe bağlı olarak maske özellikleri yapılandırılabilir.
        /// </summary>
        public static IHtmlContent InputMask_forpositivenumber(this IHtmlHelper htmlhelper, string name, int maskcount, string value = "", object htmlattributes = null)
        {
            var tb = new TagBuilder("input");
            tb.Attributes.Add("id", name); // '12__'.replace(/\_/g, '') ->  12
            if (htmlattributes != null) { tb.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlattributes), true); }
            checkClassInputMask_private(tb);
            tb.MergeAttribute("data-mask", new String('9', maskcount), true);
            if (UInt64.TryParse(value, out ulong _v) && _v > 0) { tb.MergeAttribute("value", _v.ToString(), true); }
            tb.MergeAttribute("type", "text", true);
            tb.MergeAttribute("name", name, true);
            tb.TagRenderMode = TagRenderMode.SelfClosing;
            using (var sw = new StringWriter())
            {
                tb.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }
        private static void checkClassInputMask_private(TagBuilder tb)
        {
            if (tb.Attributes.TryGetValue("class", out string _class))
            {
                var _c = _class.Split(' ').Where(x => !x.IsNullOrEmpty_string()).ToList();
                if (!_c.Contains("input-mask"))
                {
                    _c.Add("input-mask");
                    tb.MergeAttribute("class", String.Join(" ", _c), true);
                }
            }
            else { tb.Attributes.Add("class", "input-mask"); }
        }
    }
}