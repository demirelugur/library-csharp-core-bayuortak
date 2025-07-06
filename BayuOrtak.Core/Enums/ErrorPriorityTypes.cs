namespace BayuOrtak.Core.Enums
{
    /// <summary>
    /// Hata kayitlarında hatanın seviyesini temsil eden enum
    /// </summary>
    public enum ErrorPriorityTypes : byte
    {
        normal = 0,
        low,
        high,
        catastrophicfailure
    }
}