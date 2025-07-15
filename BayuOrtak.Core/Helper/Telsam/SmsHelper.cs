namespace BayuOrtak.Core.Helper.Telsam
{
    using BayuOrtak.Core.Extensions;
    using SmsApiNode;
    using SmsApiNode.Operations;
    using SmsApiNode.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public interface ISmsHelper
    {
        (long? messageid, Exception ex) Send(string message, List<long> nums, DateTime? sendingdate);
        (SmsReportResult[] data, Exception[] exs) Report(long messageid);
    }
    /// <summary>
    /// SMS gönderimi ve raporlama işlemlerini gerçekleştiren yardımcı sınıf.
    /// Bu sınıf, SMS mesajlarının içeriğini, gönderilecek numaraları ve gönderim tarihini saklar.
    /// </summary>
    /// <remarks>
    /// SMS hakkında daha fazla bilgi için
    /// <a href="https://sms.telsam.com.tr/api-docs/?id=eyJwcm9kdWN0IjoiVEVMU0FNIiwiZG9tYWluIjoic21zLnRlbHNhbS5jb20udHIiLCJyIjp0cnVlfQ==">buraya</a> tıklayarak resmi dokümantasyona ulaşabilirsiniz.
    /// </remarks>
    public sealed class SmsHelper : ISmsHelper
    {
        private readonly Messenger messenger;
        public SmsHelper(string host, string username, string password)
        {
            this.messenger = new Messenger(host, username, password);
        }
        /// <summary>
        /// SMS&#39;i gönderir.
        /// </summary>
        public (long? messageid, Exception ex) Send(string message, List<long> nums, DateTime? sendingdate)
        {
            message = this.quotationreplace(message);
            if (message.IsNullOrEmpty_string()) { return (default, new Exception($"\"{nameof(message)}\" boş geçilemez!")); }
            nums = (nums ?? new List<long>()).Where(x => x > 0).Distinct().ToList();
            if (nums.Count == 0) { return (default, new Exception($"\"{nameof(nums)}\" boş geçilemez!")); }
            try
            {
                var _r = this.messenger.SendMultiSms(new SendMultiSms
                {
                    Content = message,
                    Numbers = nums,
                    Sender = "BAYBURT UNI",
                    Encoding = 1,
                    Validity = 1440,
                    SendingDate = sendingdate.NullIfOrDefault()
                });
                if (_r.PackageId > 0) { return (_r.PackageId, default); }
                else { return (default, (_r.Err == null ? new Exception("SMS iletimi sırasında beklenmeyen bir hata meydana geldi. Yönetici ile iletişime geçiniz!") : this.setexception(_r.Err, null))); }
            }
            catch (Exception ex) { return (default, ex); }
        }
        /// <summary>
        /// Gönderilmiş bir SMS&#39;in raporunu alır.
        /// </summary>
        public (SmsReportResult[] data, Exception[] exs) Report(long messageid)
        {
            var _data = new List<SmsReportResult>();
            var _exs = new List<Exception>();
            try { this.getreport(messageid, 0, ref _data, ref _exs); }
            catch (Exception ex) { _exs = new List<Exception> { ex }; }
            return (_data.ToArray(), _exs.ToArray());
        }
        #region Private
        private const int pagesize = 1000;
        private string quotationreplace(string s)
        {
            s = s.ToStringOrEmpty();
            s = s.Replace(Convert.ToChar(8216).ToString(), "'"); // ‘, Left Single Quotation Mark, HttpUtility.HtmlEncode
            s = s.Replace(Convert.ToChar(8217).ToString(), "'"); // ’, Right Single Quotation Mark
            return s;
        }
        private Exception setexception(Err err, int? index)
        {
            var _dic = new Dictionary<string, object>() { { nameof(err.Status), err.Status } };
            if (!err.Code.IsNullOrEmpty_string()) { _dic.Add(nameof(err.Code), err.Code); }
            if (!err.Message.IsNullOrEmpty_string()) { _dic.Add(nameof(err.Message), err.Message); }
            if (index.HasValue) { _dic.Add("Index", index.Value); }
            return new Exception(String.Join(", ", _dic.Select(x => String.Join(": ", x.Key, x.Value.ToString())).ToArray()));
        }
        private void getreport(long messageid, int index, ref List<SmsReportResult> data, ref List<Exception> exs)
        {
            var _r = this.messenger.GetSmsReportDetail(new GetSmsReportDetail
            {
                PackageId = messageid,
                PageIndex = index,
                PageSize = pagesize
            });
            if (_r.Err == null)
            {
                var _rdis = _r.List ?? new List<ReportDetailItem>();
                if (_rdis.Count > 0) { data.AddRange(_rdis.Select(SmsReportResult.ToEntityFromObject).ToArray()); }
                if (_r.TotalCount > ((index + 1) * pagesize)) { this.getreport(messageid, index + 1, ref data, ref exs); }
            }
            else { exs.Add(this.setexception(_r.Err, index)); }
        }
        #endregion
    }
}