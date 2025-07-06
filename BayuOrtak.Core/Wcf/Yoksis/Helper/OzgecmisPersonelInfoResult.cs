namespace BayuOrtak.Core.Wcf.Yoksis.Helper
{
    using BayuOrtak.Core.Extensions;
    using System.Linq;
    using Wcf_Yoksis_OzgecmisV2;
    /// <summary>
    /// Özgeçmiş personel bilgilerini temsil eden sonuç sınıfı.
    /// </summary>
    public class OzgecmisPersonelInfoResult
    {
        /// <summary>
        /// Sonuç durumunu belirten bir değer.
        /// </summary>
        public bool durum { get; }
        /// <summary>
        /// Bağlantı durumunu belirten bir değer. Eğer değer <see langword="false"/> ise ilgili personelin bilgilerine yöksiste ulaşılamıyor veya yetki yok anlamındadır
        /// </summary>
        public bool baglanti { get; }
        /// <summary>
        /// İletişim bilgilerini temsil eden nesne.
        /// </summary>
        public docIletisimPersonel iletisim { get; }
        /// <summary>
        /// Temel alan bilgilerini temsil eden nesne.
        /// </summary>
        public docTemelAlan temel { get; }
        /// <summary>
        /// Personelin YÖKSİS Sunucunda bağlantı kurulup kurulmadığını kontrol eder. 
        /// <code>(this.durum &amp;&amp; this.baglanti &amp;&amp; this.iletisim.KADRO_YERI.ToStringOrEmpty().ToEnumerable().Select(x =&gt; x.Split(&#39;/&#39;)).Select(x =&gt; x.Length &gt; 1 &amp;&amp; x[0].ToSeoFriendly() == &quot;bayburt-universitesi&quot;).FirstOrDefault());</code>
        /// </summary>
        /// <returns>Kadroda olup olmadığını belirten bir değer.</returns>
        public bool iskadrobayburtuniversitesi() => (this.durum && this.baglanti && this.iletisim.KADRO_YERI.ToStringOrEmpty().ToEnumerable().Select(x => x.Split('/')).Select(x => x.Length > 1 && x[0].ToSeoFriendly() == "bayburt-universitesi").FirstOrDefault());
        /// <summary>
        /// Özgeçmiş personel bilgi sonucunun varsayılan yapılandırıcısı.
        /// </summary>
        public OzgecmisPersonelInfoResult() : this(default, default, default, default) { }
        /// <summary>
        /// Özgeçmiş personel bilgi sonucunun belirli parametrelerle başlatılmasını sağlar.
        /// </summary>
        /// <param name="durum">Sonuç durumu.</param>
        /// <param name="baglanti">Bağlantı durumu.</param>
        /// <param name="iletisim">İletişim bilgileri.</param>
        /// <param name="temel">Temel alan bilgileri.</param>
        public OzgecmisPersonelInfoResult(bool durum, bool baglanti, docIletisimPersonel iletisim, docTemelAlan temel)
        {
            this.durum = durum;
            this.baglanti = baglanti;
            this.iletisim = iletisim ?? new docIletisimPersonel();
            this.temel = temel ?? new docTemelAlan();
        }
    }
}