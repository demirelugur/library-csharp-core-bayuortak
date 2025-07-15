namespace BayuOrtak.Core.Helper.FileHelper
{
    using BayuOrtak.Core.Extensions;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using static BayuOrtak.Core.Enums.CRetMesaj;
    using static BayuOrtak.Core.Helper.OrtakTools;
    public sealed class FileUploadHelper
    {
        private readonly HashSet<string> _delDirectoryFiles = new HashSet<string>();
        private readonly HashSet<string> _delFiles = new HashSet<string>();
        private readonly Dictionary<string, object> _addFiles = new Dictionary<string, object>();
        /// <summary>
        /// Silinmesi gereken klasörlerin fiziksel yolunu ekler
        /// </summary>
        /// <param name="physicallyDirectoryPaths">Silinmesi gereken klasörün fiziksel yolu.</param>
        public void RemoveDirectory(params string[] physicallyDirectoryPaths) => this._delDirectoryFiles.AddRangeOptimized(physicallyDirectoryPaths ?? Array.Empty<string>());
        /// <summary>
        /// Silinmesi gereken bir dosyanın fiziksel yolunu ekler.
        /// </summary>
        /// <param name="physicallyPaths">Silinmesi gereken dosyanın fiziksel yolu.</param>
        public void RemoveFile(params string[] physicallyPaths) => this._delFiles.AddRangeOptimized(physicallyPaths ?? Array.Empty<string>());
        /// <summary>
        /// Eklenmesi gereken bir dosyanın fiziksel yolunu ve dosya nesnesini ekler.
        /// </summary>
        /// <param name="physicallyPath">Eklenmesi gereken dosyanın fiziksel yolu.</param>
        /// <param name="file">Eklenmesi gereken dosya nesnesi.</param>
        public void Add(string physicallyPath, IFormFile file) => this._addFiles.Upsert(physicallyPath, file);
        /// <summary>
        /// Eklenmesi gereken bir dosyanın fiziksel yolunu ve bayt dizisini ekler.
        /// </summary>
        /// <param name="physicallyPath">Eklenmesi gereken dosyanın fiziksel yolu.</param>
        /// <param name="bytes">Eklenmesi gereken dosyanın bayt dizisi.</param>
        public void Add(string physicallyPath, byte[] bytes) => this._addFiles.Upsert(physicallyPath, bytes);
        /// <summary>
        /// Belirtilen dosyalar yüklenmeden önce, varsa önce silinmesi gereken klasörler ve ardından silinmesi gereken dosyalar kaldırılır.
        /// </summary>
        public async Task ProcessFileUploadsAndDeletionsAsync(CancellationToken cancellationToken)
        {
            var delFileAny = this._delFiles.Count > 0;
            var addFileAny = this._addFiles.Count > 0;
            if (this._delDirectoryFiles.Count > 0)
            {
                foreach (var item in this._delDirectoryFiles) { _file.DirectoryExiststhenDelete(item, true); }
                if (delFileAny || addFileAny) { await Task.Delay(300, cancellationToken); }
            }
            if (delFileAny)
            {
                foreach (var itemDeleteFile in this._delFiles) { _file.FileExiststhenDelete(itemDeleteFile); }
                if (addFileAny) { await Task.Delay(300, cancellationToken); }
            }
            if (addFileAny)
            {
                foreach (var itemAddFile in this._addFiles)
                {
                    if (itemAddFile.Value is IFormFile _f) { await _f.FileUploadAsync(itemAddFile.Key, cancellationToken); }
                    else if (itemAddFile.Value is byte[] _b) { await _b.FileUploadAsync(itemAddFile.Key, cancellationToken); }
                    else { throw new Exception(RetMesaj.hata.GetDescription()); } // Not: Bu ihtimalin gelme durumu olmamasına rağmen ne olur olmaz yazılan kontrol
                }
            }
        }
    }
}