namespace BayuOrtak.Core.Helper.Telsam
{
    using BayuOrtak.Core.Extensions;
    using BayuOrtak.Core.Helper.Telsam.Enums;
    using Microsoft.AspNetCore.Http;
    using SmsApiNode.Operations;
    public sealed class SmsReportResult : IEquatable<SmsReportResult>
    {
        #region Equals
        public override bool Equals(object other) => this.Equals(other as SmsReportResult);
        public override int GetHashCode() => HashCode.Combine(this.error, this.state, this.target, this.setstate);
        public bool Equals(SmsReportResult other)
        {
            if (other is SmsReportResult _ssr) { return this.error == _ssr.error && this.state == _ssr.state && this.target == _ssr.target && this.setstate == _ssr.setstate; }
            return false;
        }
        #endregion
        public string error { get; set; }
        public StateTypes? state { get; set; }
        public string target { get; set; }
        public SetStateTypes? setstate { get; set; }
        public SmsReportResult() : this("", default, "", default) { }
        public SmsReportResult(string error, StateTypes? state, string target, SetStateTypes? setstate)
        {
            this.error = error;
            this.state = state;
            this.target = target;
            this.setstate = setstate;
        }
        /// <summary>
        /// value için tanımlanan nesneler: SmsReportResult, ReportDetailItem, IFormCollection, AnonymousObjectClass
        /// </summary>
        public static SmsReportResult ToEntityFromObject(object value)
        {
            if (value == null) { return new SmsReportResult(); }
            if (value is SmsReportResult _srr) { return _srr; }
            if (value is ReportDetailItem _rdi)
            {
                return ToEntityFromObject(new
                {
                    error = _rdi.Error.ToStringOrEmpty(),
                    state = ((Int32.TryParse(_rdi.State, out int _i) && Enum.IsDefined((StateTypes)_i)) ? (StateTypes?)_i : null),
                    target = _rdi.Target.ToStringOrEmpty(),
                    setstate = (Enum.IsDefined((SetStateTypes)_rdi.SetState) ? (SetStateTypes?)_rdi.SetState : null)
                });
            }
            if (value is IFormCollection _form)
            {
                return ToEntityFromObject(new
                {
                    error = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(error)) ?? "",
                    state = _form.ToKeyValueParseOrDefault_formcollection<StateTypes?>(nameof(state)),
                    target = _form.ToKeyValueParseOrDefault_formcollection<string>(nameof(target)) ?? "",
                    setstate = _form.ToKeyValueParseOrDefault_formcollection<SetStateTypes?>(nameof(setstate))
                });
            }
            return value.ToEnumerable().Select(x => x.ToDynamic()).Select(x => new SmsReportResult((string)x.error, (StateTypes?)x.state, (string)x.target, (SetStateTypes?)x.setstate)).FirstOrDefault();
        }
    }
}