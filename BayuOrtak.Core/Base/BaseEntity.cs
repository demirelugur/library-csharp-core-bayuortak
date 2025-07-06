namespace BayuOrtak.Core.Base
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    /// <summary>
    /// Genel varlık nesneleri için temel arayüzü tanımlar.
    /// Bu arayüz, varlıkların durumunu, ekleme, güncelleme ve silme bilgilerini içerir.
    /// </summary>
    /// <typeparam name="T">Kullanıcı kimliğini temsil eden tür (ör. int, Guid vb.).</typeparam>
    public interface IBaseEntity<T> where T : struct
    {
        /// <summary>
        /// Varlığın aktiflik durumunu belirtir. <see langword="true"/> ise varlık aktif, <see langword="false"/> ise pasiftir.
        /// </summary>
        bool Durum { get; set; }
        /// <summary>
        /// Varlığı ekleyen kullanıcının kimliğini temsil eder.
        /// </summary>
        T AddUser { get; set; }
        /// <summary>
        /// Varlığın eklendiği tarihi belirtir.
        /// </summary>
        DateTime AddDate { get; set; }
        /// <summary>
        /// Varlığı güncelleyen kullanıcının kimliğini temsil eder. Null olabilir.
        /// </summary>
        T? UptUser { get; set; }
        /// <summary>
        /// Varlığın güncellendiği tarihi belirtir. Null olabilir.
        /// </summary>
        DateTime? UptDate { get; set; }
        /// <summary>
        /// Varlığın silinip silinmediğini belirtir. <see langword="true"/> ise varlık silinmiştir.
        /// </summary>
        bool IsDeleted { get; set; }
        /// <summary>
        /// Varlığı silen kullanıcının kimliğini temsil eder. Null olabilir.
        /// </summary>
        T? DeleteUser { get; set; }
        /// <summary>
        /// Varlığın silindiği tarihi belirtir. Null olabilir.
        /// </summary>
        DateTime? DeleteDate { get; set; }
        /// <summary>
        /// Varlığın güncelleme bilgilerini ayarlar (güncelleyen kullanıcı ve güncelleme tarihi).
        /// </summary>
        /// <param name="uptuser">Güncellemeyi yapan kullanıcının kimliği.</param>
        void Set_UpdateInfo(T uptuser);
        /// <summary>
        /// Varlığın silme bilgilerini ayarlar (silme durumu, silen kullanıcı ve silme tarihi).
        /// </summary>
        /// <param name="deleteuser">Silmeyi yapan kullanıcının kimliği.</param>
        void Set_DeleteInfo(T deleteuser);
    }
    /// <summary>
    /// Genel varlık nesneleri için temel sınıf. IBaseEntity arayüzünü uygular ve IDisposable ile kaynak yönetimi sağlar.
    /// </summary>
    /// <typeparam name="T">Kullanıcı kimliğini temsil eden tür (ör. int, Guid vb.).</typeparam>
    public abstract class BaseEntity<T> : IBaseEntity<T>, IDisposable where T : struct
    {
        /// <summary>
        /// Nesnenin kaynaklarını serbest bırakır ve çöp toplayıcıyı bilgilendirir.
        /// </summary>
        public void Dispose()  { GC.SuppressFinalize(this); }
        private bool _durum;
        private T _addUser;
        private DateTime _addDate;
        private T? _uptUser;
        private DateTime? _uptDate;
        private bool _isDeleted;
        private T? _deleteUser;
        private DateTime? _deleteDate;
        /// <summary>
        /// Varlığın aktiflik durumunu belirtir. <see langword="true"/> ise varlık aktif, <see langword="false"/> ise pasiftir.
        /// </summary>
        public bool Durum { get { return _durum; } set { _durum = value; } }
        /// <summary>
        /// Varlığı ekleyen kullanıcının kimliğini temsil eder.
        /// </summary>
        public T AddUser { get { return _addUser; } set { _addUser = value; } }
        /// <summary>
        /// Varlığın eklendiği tarihi belirtir.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime AddDate { get { return _addDate; } set { _addDate = value; } }
        /// <summary>
        /// Varlığı güncelleyen kullanıcının kimliğini temsil eder. Null olabilir.
        /// </summary>
        public T? UptUser { get { return _uptUser; } set { _uptUser = value.NullIfOrDefault(); } }
        /// <summary>
        /// Varlığın güncellendiği tarihi belirtir. Null olabilir.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? UptDate { get { return _uptDate; } set { _uptDate = value.NullIfOrDefault(); } }
        /// <summary>
        /// Varlığın silinip silinmediğini belirtir. <see langword="true"/> ise varlık silinmiştir.
        /// </summary>
        public bool IsDeleted { get { return _isDeleted; } set { _isDeleted = value; } }
        /// <summary>
        /// Varlığı silen kullanıcının kimliğini temsil eder. Null olabilir.
        /// </summary>
        public T? DeleteUser { get { return _deleteUser; } set { _deleteUser = value.NullIfOrDefault(); } }
        /// <summary>
        /// Varlığın silindiği tarihi belirtir. Null olabilir.
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get { return _deleteDate; } set { _deleteDate = value.NullIfOrDefault(); } }
        /// <summary>
        /// Varlığın güncelleme bilgilerini ayarlar. Güncelleyen kullanıcıyı ve güncelleme tarihini kaydeder.
        /// </summary>
        /// <param name="uptuser">Güncellemeyi yapan kullanıcının kimliği.</param>
        public void Set_UpdateInfo(T uptuser)
        {
            this.UptUser = uptuser;
            this.UptDate = DateTime.Now;
        }
        /// <summary>
        /// Varlığın silme bilgilerini ayarlar. Silme durumunu, silen kullanıcıyı ve silme tarihini kaydeder.
        /// </summary>
        /// <param name="deleteuser">Silmeyi yapan kullanıcının kimliği.</param>
        public void Set_DeleteInfo(T deleteuser)
        {
            this.IsDeleted = true;
            this.DeleteUser = deleteuser;
            this.DeleteDate = DateTime.Now;
        }
    }
}