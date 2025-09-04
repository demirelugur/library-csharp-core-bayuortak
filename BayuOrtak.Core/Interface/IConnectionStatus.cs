namespace BayuOrtak.Core.Interface
{
    /// <summary>
    /// Web Servis Endpoint Addres bağlantılarının aktif olup olmadığını kontrol eden interface
    /// </summary>
    public interface IConnectionStatus
    {
        Task<(bool statuswarning, string error)> IsConnectionStatusAsync(TimeSpan timeout, string dil, CancellationToken cancellationtoken);
    }
}