# BackupYukle

## Amaç

`BackupYukle`, bir klasördeki tüm MSSQL `.bak` yedek dosyalarını otomatik olarak, aynı isimli veritabanlarına hızlıca yüklemek için geliştirilmiş bir .NET Console uygulamasıdır. Özellikle test ve geliştirme ortamlarında toplu veritabanı yükleme işlemlerini kolaylaştırır.

## Özellikler

- Belirtilen klasördeki tüm `.bak` dosyalarını otomatik bulur ve yükler.
- Yükleme öncesi aynı isimli veritabanı varsa bağlantılarını sonlandırır ve siler.
- Yükleme sonrası başarılı olan yedek dosyasını siler.
- Hataları ve işlemleri detaylı olarak ekrana yazar.

## Kurulum

1. **Yapılandırma dosyasını düzenleyin:**
   `BackupYukle/appsettings.json` dosyasındaki ayarları kendi ortamınıza göre güncelleyin:

   ```json
   {
     "backupDirectory": "###",
     "connectionstring": {
       "datasource": "###",
       "userid": "###",
       "password": "###"
     }
   }
   ```

   - `backupDirectory`: `.bak` dosyalarının bulunduğu klasör.
   - `connectionstring`: SQL Server bağlantı bilgileri.

## Kullanım

1. **Onay isteği gelir, `Y` tuşuna basarak işlemi başlatın.**
   - Her `.bak` dosyası için:
     - Aynı isimli veritabanı varsa bağlantıları sonlandırılır ve silinir.
     - Yedek dosyası yüklenir.
     - Başarılı olursa dosya silinir.

## Notlar

- SQL Server'da yeterli yetkiniz olmalıdır (veritabanı silme/yükleme).
- Yedek dosyalarının isimleri, yüklenecek veritabanı adını belirler (ör: `TestDB.bak` → `TestDB` veya  `TestDB_20240101_000000.bak` → `TestDB`).
- Hatalar ekrana detaylı olarak yazılır, işlem sırasında takip edebilirsiniz.
- `appsettings.json` dosyasındaki şifre gibi bilgileri güvenli saklayınız.