namespace BayuOrtak.Core.Base
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    /// <summary>
    /// Genel varlık nesneleri için temel arayüzü tanımlar. Bu arayüz, varlıkların durumunu, ekleme, güncelleme ve silme bilgilerini içerir.
    /// </summary>
    /// <typeparam name="T">Kullanıcı kimliğini temsil eden tür (ör. int, Guid vb.).</typeparam>
    public interface IBaseEntity<T> where T : struct
    {
        bool Durum { get; set; }
        T AddUser { get; set; }
        DateTime AddDate { get; set; }
        T? UptUser { get; set; }
        DateTime? UptDate { get; set; }
        bool IsDeleted { get; set; }
        T? DeleteUser { get; set; }
        DateTime? DeleteDate { get; set; }
        void Set_UpdateInfo(T uptuser);
        void Set_DeleteInfo(T deleteuser);
    }
    /// <summary>
    /// Genel varlık nesneleri için temel sınıf. IBaseEntity arayüzünü uygular ve IDisposable ile kaynak yönetimi sağlar.
    /// </summary>
    /// <typeparam name="T">Kullanıcı kimliğini temsil eden tür (ör. int, Guid vb.).</typeparam>
    public abstract class BaseEntity<T> : IBaseEntity<T>, IDisposable where T : struct
    {
        public void Dispose()  { GC.SuppressFinalize(this); }
        private bool _durum;
        private T _addUser;
        private DateTime _addDate;
        private T? _uptUser;
        private DateTime? _uptDate;
        private bool _isDeleted;
        private T? _deleteUser;
        private DateTime? _deleteDate;
        public bool Durum { get { return _durum; } set { _durum = value; } }
        public T AddUser { get { return _addUser; } set { _addUser = value; } }
        [Column(TypeName = "datetime")]
        public DateTime AddDate { get { return _addDate; } set { _addDate = value; } }
        public T? UptUser { get { return _uptUser; } set { _uptUser = value.NullIfOrDefault(); } }
        [Column(TypeName = "datetime")]
        public DateTime? UptDate { get { return _uptDate; } set { _uptDate = value.NullIfOrDefault(); } }
        public bool IsDeleted { get { return _isDeleted; } set { _isDeleted = value; } }
        public T? DeleteUser { get { return _deleteUser; } set { _deleteUser = value.NullIfOrDefault(); } }
        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get { return _deleteDate; } set { _deleteDate = value.NullIfOrDefault(); } }
        public void Set_UpdateInfo(T uptuser)
        {
            this.UptUser = uptuser;
            this.UptDate = DateTime.Now;
        }
        public void Set_DeleteInfo(T deleteuser)
        {
            this.IsDeleted = true;
            this.DeleteUser = deleteuser;
            this.DeleteDate = DateTime.Now;
        }
    }
}