namespace BayuOrtak.Core.Interface
{
    using BayuOrtak.Core.Base;
    /// <summary>
    /// Veritabanında silinmemiş kayıtları listelemek için oluşturulmuş interfacetir. <see cref="IBaseEntity{TKey}.IsDeleted"/> değeri <see langword="false"/> olan kayıtları temsil eder.
    /// </summary>
    public interface IDbSetAlive<T, TKey> where T: IBaseEntity<TKey> where TKey : struct
    {
        IQueryable<T> DbSetAlive { get; }
    }
}