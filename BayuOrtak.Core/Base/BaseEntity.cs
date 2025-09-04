namespace BayuOrtak.Core.Base
{
    using BayuOrtak.Core.Extensions;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    /// <summary>
    /// Genel varlık nesneleri için temel arayüzü tanımlar. Bu arayüz, varlıkların durumunu, ekleme, güncelleme ve silme bilgilerini içerir.
    /// </summary>
    /// <typeparam name="TKey">Kullanıcı kimliğini temsil eden tür (ör. int, Guid vb.).</typeparam>
    public interface IBaseEntity<TKey> where TKey : struct
    {
        bool Durum { get; set; }
        TKey AddUser { get; set; }
        DateTime AddDate { get; set; }
        TKey? UptUser { get; set; }
        DateTime? UptDate { get; set; }
        bool IsDeleted { get; set; }
        TKey? DeleteUser { get; set; }
        DateTime? DeleteDate { get; set; }
        void Set_UpdateInfo(TKey uptuser);
        void Set_DeleteInfo(TKey deleteuser);
    }
    /// <summary>
    /// Genel varlık nesneleri için temel sınıf. IBaseEntity arayüzünü uygular ve IDisposable ile kaynak yönetimi sağlar.
    /// </summary>
    /// <typeparam name="TKey">Kullanıcı kimliğini temsil eden tür (ör. int, Guid vb.).</typeparam>
    public abstract class BaseEntity<TKey> : IBaseEntity<TKey>, IDisposable where TKey : struct
    {
        public void Dispose() { GC.SuppressFinalize(this); }
        private bool _durum;
        private TKey _addUser;
        private DateTime _addDate;
        private TKey? _uptUser;
        private DateTime? _uptDate;
        private bool _isDeleted;
        private TKey? _deleteUser;
        private DateTime? _deleteDate;
        public bool Durum { get { return _durum; } set { _durum = value; } }
        public TKey AddUser { get { return _addUser; } set { _addUser = value; } }
        [Column(TypeName = "datetime")]
        public DateTime AddDate { get { return _addDate; } set { _addDate = value; } }
        public TKey? UptUser { get { return _uptUser; } set { _uptUser = value.NullIfOrDefault(); } }
        [Column(TypeName = "datetime")]
        public DateTime? UptDate { get { return _uptDate; } set { _uptDate = value.NullIfOrDefault(); } }
        public bool IsDeleted { get { return _isDeleted; } set { _isDeleted = value; } }
        public TKey? DeleteUser { get { return _deleteUser; } set { _deleteUser = value.NullIfOrDefault(); } }
        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get { return _deleteDate; } set { _deleteDate = value.NullIfOrDefault(); } }
        public void Set_UpdateInfo(TKey uptuser)
        {
            this.UptUser = uptuser;
            this.UptDate = DateTime.Now;
        }
        public void Set_DeleteInfo(TKey deleteuser)
        {
            this.IsDeleted = true;
            this.DeleteUser = deleteuser;
            this.DeleteDate = DateTime.Now;
        }
    }
}