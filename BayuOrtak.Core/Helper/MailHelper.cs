namespace BayuOrtak.Core.Helper
{
    using BayuOrtak.Core.Extensions;
    using System.Net.Mail;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public struct MailHelper
    {
        public string subject { get; }
        public string body { get; }
        public bool isbodyhtml { get; }
        public MailAddress[] tos { get; }
        public MailAddress[] ccs { get; }
        public MailAddress[] bccs { get; }
        public Attachment[] attachments { get; }
        public MailPriority priority { get; }
        public MailHelper() : this("", "", default, default, default, default, default, default) { }
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
        /// <summary>
        /// Yapılandırılmış <see cref="MailMessage"/> nesnesini kullanarak asenkron olarak e-Posta gönderir. Bu metod, belirtilen SMTP ayarlarını kullanarak e-Posta gönderim işlemini gerçekleştirir.
        /// </summary>
        public async Task<(bool statuswarning, Exception ex)> SendAsync(SmtpSettingsHelper smtp, CancellationToken cancellationtoken)
        {
            try
            {
                smtp = smtp ?? new SmtpSettingsHelper();
                Guard.CheckEmpty(this.subject, nameof(this.subject));
                Guard.CheckEmpty(this.body, nameof(this.body));
                Guard.CheckEmpty(this.tos, nameof(this.tos));
                Guard.CheckEnumDefined<MailPriority>(this.priority, nameof(this.priority));
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
                    await smtp.toSmtpClient().SendMailAsync(mm, cancellationtoken);
                    return (false, default);
                }
            }
            catch (Exception ex) { return (true, ex); }
        }
    }
}