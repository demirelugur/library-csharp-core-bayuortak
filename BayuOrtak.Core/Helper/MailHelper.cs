namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    using System.Net.Mail;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public struct MailHelper
    {
        /// <summary>
        /// e-Posta başlığı. 
        /// Bu alan, gönderilecek e-Postanın başlık kısmını temsil eder.
        /// </summary>
        public string subject { get; }
        /// <summary>
        /// e-Posta gövdesi. 
        /// Bu alan, gönderilecek e-Postanın metin içeriğini veya HTML biçiminde gövdesini temsil eder.
        /// </summary>
        public string body { get; }
        /// <summary>
        /// e-Postanın HTML biçiminde olup olmadığını belirten bayrak. 
        /// Eğer bu değer true ise, e-Posta HTML olarak gönderilir; aksi halde düz metin olarak gönderilir.
        /// </summary>
        public bool isbodyhtml { get; }
        /// <summary>
        /// e-Posta gönderilecek alıcılar. 
        /// Birden fazla alıcı belirlenebilir ve her biri <see cref="MailAddress"/> türündedir.
        /// </summary>
        public MailAddress[] tos { get; }
        /// <summary>
        /// e-Posta için ek alıcılar (CC). 
        /// Bu alan, e-Posta bir kopya olarak başka kişilere gönderilecekse kullanılır.
        /// </summary>
        public MailAddress[] ccs { get; }
        /// <summary>
        /// e-Posta için gizli alıcılar (BCC). 
        /// e-Postanın gizli bir kopyasını başka kişilere göndermek için kullanılır.
        /// </summary>
        public MailAddress[] bccs { get; }
        /// <summary>
        /// e-Postaya eklenen dosyalar. 
        /// Bu alan, gönderilecek e-Postaya eklenmiş dosyaların listesi olarak kullanılır.
        /// </summary>
        public Attachment[] attachments { get; }
        /// <summary>
        /// e-Posta önceliği. 
        /// Bu alan, e-Postanın normal, yüksek veya düşük öncelikli olup olmadığını belirler.
        /// </summary>
        public MailPriority priority { get; }
        /// <summary>
        /// Parametre verilmediği durumda varsayılan değerlerle bir <see cref="MailHelper"/> örneği oluşturur. 
        /// Bu kurucu, e-Posta başlığını, gövdesini boş olarak ve alıcıları boş bir dizi olarak ayarlar.
        /// </summary>
        public MailHelper() : this("", "", default, default, default, default, default, default) { }
        /// <summary>
        /// e-Posta başlığı, gövdesi, alıcıları ve diğer bilgileri belirterek bir <see cref="MailHelper"/> örneği oluşturur.
        /// e-Posta gönderimi için gerekli olan tüm bilgileri bu kurucuya parametre olarak verebilirsiniz.
        /// </summary>
        /// <param name="subject">e-Posta başlığı.</param>
        /// <param name="body">e-Posta gövdesi.</param>
        /// <param name="isbodyhtml">e-Posta gövdesinin HTML biçiminde olup olmadığını belirten bayrak.</param>
        /// <param name="tos">e-Posta alıcıları (To).</param>
        /// <param name="ccs">e-Posta için ek alıcılar (CC).</param>
        /// <param name="bccs">e-Posta için gizli alıcılar (BCC).</param>
        /// <param name="attachments">e-Postaya eklenmiş dosyalar.</param>
        /// <param name="priority">e-Posta önceliği.</param>
        public MailHelper(string subject, string body, bool isbodyhtml, MailAddress[] tos, MailAddress[] ccs, MailAddress[] bccs, Attachment[] attachments, MailPriority priority)
        {
            this.subject = subject.ToStringOrEmpty();
            this.body = body.ToStringOrEmpty();
            this.isbodyhtml = isbodyhtml;
            this.tos = tos ?? Array.Empty<MailAddress>();
            this.ccs = ccs ?? Array.Empty<MailAddress>();
            this.bccs = bccs ?? Array.Empty<MailAddress>();
            this.attachments = attachments ?? Array.Empty<Attachment>();
            this.priority = priority;
        }
        private void guardvalidation()
        {
            Guard.CheckEmpty(this.subject, nameof(this.subject));
            Guard.CheckEmpty(this.body, nameof(this.body));
            Guard.CheckEmpty(this.tos, nameof(this.tos));
            Guard.CheckEnumDefined<MailPriority>(this.priority, nameof(this.priority));
        }
        /// <summary>
        /// Yapılandırılmış <see cref="MailMessage"/> nesnesini kullanarak asenkron olarak e-Posta gönderir. Bu metod, belirtilen SMTP ayarlarını kullanarak e-Posta gönderim işlemini gerçekleştirir.
        /// </summary>
        public async Task<(bool statuswarning, Exception ex)> SendAsync(SmtpSettingsHelper smtp, CancellationToken cancellationToken)
        {
            try
            {
                smtp = smtp ?? new SmtpSettingsHelper();
                this.guardvalidation();
                if (_try.TryWarningValidateObject(smtp, out string[] _errors)) { throw _errors.ToNestedException(); }
                using (var mm = new MailMessage())
                {
                    mm.Subject = this.subject;
                    mm.Body = this.body;
                    mm.IsBodyHtml = this.isbodyhtml;
                    mm.Priority = this.priority;
                    mm.From = new MailAddress(smtp.email);
                    foreach (var item in this.tos) { mm.To.Add(item); }
                    foreach (var item in this.ccs) { mm.CC.Add(item); }
                    foreach (var item in this.bccs) { mm.Bcc.Add(item); }
                    foreach (var item in this.attachments) { mm.Attachments.Add(item); }
                    await smtp.toSmtpClient().SendMailAsync(mm, cancellationToken);
                    return (false, default);
                }
            }
            catch (Exception ex) { return (true, ex); }
        }
    }
}