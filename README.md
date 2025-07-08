# BayuOrtak.Core & BackupYukle

Bu depo, iki ana .NET projesi içerir:

- **BayuOrtak.Core**: T.C. Bayburt Üniversitesi için geliştirilmiş, kurumsal projelerde tekrar kullanılabilir doğrulama attribute'ları, yardımcı sınıflar, extension method'lar ve servis entegrasyonları içeren bir .NET Core kütüphanesidir. Local NuGet paketi olarak kullanılabilir.
- **BackupYukle**: MSSQL veritabanı yedeklerini (.bak dosyalarını) toplu ve hızlı şekilde yüklemek için hazırlanmış bir .NET Console uygulamasıdır. Özellikle test ve geliştirme ortamlarında veritabanı yükleme işlemlerini kolaylaştırır.

## Kısa Açıklamalar

### BayuOrtak.Core
- Gelişmiş doğrulama attribute'ları (TCKN, Sicil No, E-posta, ISBN, IP, MAC, Telefon, Tarih, URL, Pozitif Sayı vb.)
- Yardımcı sınıflar (e-posta gönderimi, şifreleme, dosya işlemleri, LDAP, Dapper, parola üretimi, SMTP ayarları vb.)
- Extension method'lar (koleksiyon, string, DateTime, IQueryable, object vb.)
- Üniversiteye özel WCF servis entegrasyonları (NHR, OGR, NVI, YÖKSİS)
- Enum ve standart sonuç tipleri

### BackupYukle
- Belirtilen klasördeki tüm `.bak` dosyalarını otomatik bulur ve yükler
- Yükleme öncesi aynı isimli veritabanı varsa bağlantılarını sonlandırır ve siler
- Yükleme sonrası başarılı olan yedek dosyasını siler
- Hataları ve işlemleri detaylı olarak ekrana yazar

Sorularınız ve katkılarınız için lütfen iletişime geçin veya pull request açın.