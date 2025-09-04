namespace BayuOrtak.Core.Interface
{
    using System.Linq;
    /// <summary>
    /// İlişkili koleksiyonlar için kayıt olup olmadığını kontrol etmek amacıyla oluşturulmuş arayüz.
    /// </summary>
    /// <typeparam name="TKey">İlişkilendirilmiş veri türü. İlgili tablodaki birincil anahtar (primary key) türü olmalıdır!</typeparam>
    public interface ICanBeDeletedIds<TKey> // Not: where TKey>: struct yazılmamalıdır. PK string de olabilir!
    {
        /// <summary>
        /// İlişkili kaydı olmayan birincil anahtar (primary key) değerlerinin tutulduğu özellik.
        /// </summary>
        IQueryable<TKey> GetCanBeDeletedIds { get; }
    }
}