namespace BayuOrtak.Core.Attributes.DataAnnotations
{
    using System;
    using System.ComponentModel;
    /// <summary>
    /// <see cref="DefaultValueAttribute"/> sınıfını genişleten ve varsayılan değer olarak <see cref="Guid.Empty"/> kullanan özel bir nitelik.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class DefaultValue_GuidEmptyAttribute : DefaultValueAttribute
    {
        /// <summary>
        /// Yeni bir <see cref="DefaultValue_GuidEmptyAttribute"/> örneği oluşturur.
        /// Varsayılan değeri <see cref="Guid.Empty"/> olarak ayarlar.
        /// </summary>
        public DefaultValue_GuidEmptyAttribute() : base(typeof(Guid), Guid.Empty.ToString()) { }
    }
}