namespace BayuOrtak.Core.Helper.Results
{
    using BayuOrtak.Core.Extensions;
    using static BayuOrtak.Core.Enums.CRetMesaj;
    /// <summary>
    /// API response dönüşleri için oluşturulmuş yardımcı class
    /// </summary>
    public class IslemSonucResult
    {
        public static readonly IslemSonucResult set_Success = new IslemSonucResult(true, default);
        public bool status { get; set; }
        public string[] errors { get; set; }
        public IslemSonucResult() : this(default, default) { }
        public IslemSonucResult(bool status, string[] errors)
        {
            this.status = status;
            this.errors = (status ? Array.Empty<string>() : (errors.IsNullOrEmpty_collection() ? new string[] { RetMesaj.hata.GetDescription() } : errors));
        }
        public static IslemSonucResult set_Failed(params string[] errors) => new IslemSonucResult(false, errors);
    }
    /// <summary>
    /// API response dönüşleri için oluşturulmuş yardımcı class
    /// </summary>
    public class IslemSonucResult<T> : IslemSonucResult
    {
        public T response { get; set; }
        public IslemSonucResult() : this(default, default, default) { }
        public IslemSonucResult(T response, bool status, string[] errors) : base(status, errors)
        {
            this.response = (status ? response : this.getcustomdefault());
        }
        private T getcustomdefault()
        {
            var _t = typeof(T);
            if (_t == typeof(string)) { return (T)(object)String.Empty; }
            if (_t.IsArray) { return (T)(object)Array.CreateInstance(_t.GetElementType(), 0); }
            if (_t.IsGenericType && _t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) { return (T)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(_t.GetGenericArguments())); }
            return default(T);
        }
    }
}