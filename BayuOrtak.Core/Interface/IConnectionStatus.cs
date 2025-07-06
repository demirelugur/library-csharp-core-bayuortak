namespace BayuOrtak.Core.Interface
{
    /// <summary>
    /// Web Servis Endpoint Addres bağlantılarının aktif olup olmadığını kontrol eden interface
    /// </summary>
    public interface IConnectionStatus
    {
        Task<bool> IsConnectionStatusAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}