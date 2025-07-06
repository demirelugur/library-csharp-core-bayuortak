namespace BayuOrtak.Core.Helper.Results
{
    /// <summary>
    /// İki farklı tür arasında sol dış birleştirme sonuçlarını temsil eden bir sınıf.
    /// Bu sınıf, sol (left) ve sağ (right) taraflarda bulunan verileri tutar.
    /// </summary>
    /// <typeparam name="T">Sol tarafın veri türü.</typeparam>
    /// <typeparam name="Y">Sağ tarafın veri türü.</typeparam>
    public sealed class LeftJoinResult<T, Y>
    {
        private T _Left;
        private bool _Righthasvalue;
        private Y _Right;
        /// <summary>
        /// Sol tarafın verisini alır veya ayarlar.
        /// </summary>
        public T left { get { return _Left; } set { _Left = value; } }
        /// <summary>
        /// Sağ tarafın var olup olmadığını gösterir.
        /// </summary>
        public bool righthasvalue { get { return _Righthasvalue; } set { _Righthasvalue = value; } }
        /// <summary>
        /// Sağ tarafın verisini alır veya ayarlar.
        /// </summary>
        public Y right { get { return _Right; } set { _Right = value; } }
    }
}