namespace BayuOrtak.Core.Interface
{
    using System.Linq;
    /// <summary>
    /// İlişkili koleksiyonlar için kayıt olup olmadığını kontrol etmek amacıyla oluşturulmuş arayüz.
    /// </summary>
    /// <typeparam name="T">İlişkilendirilmiş veri türü. İlgili tablodaki birincil anahtar (primary key) türü olmalıdır!</typeparam>
    public interface ICanBeDeletedIds<T> // Not: where T: struct yazılmamalıdır. PK string de olabilir!
    {
        /// <summary>
        /// İlişkili kaydı olmayan birincil anahtar (primary key) değerlerinin tutulduğu özellik.
        /// </summary>
        IQueryable<T> GetCanBeDeletedIds { get; }
    }
}