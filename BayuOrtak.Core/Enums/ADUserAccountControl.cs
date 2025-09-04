namespace BayuOrtak.Core.Enums
{
    using System;
    /// <summary>
    /// Active Directory kullanıcı hesap özelliklerini belirten enum değerleri. Bu enum, kullanıcı hesapları üzerinde çeşitli özellikleri ve durumları tanımlamak için kullanılır. Daha fazla bilgi için <see href="https://learn.microsoft.com/en-us/troubleshoot/windows-server/identity/useraccountcontrol-manipulate-account-properties">buraya</see> göz atabilirsiniz.
    /// </summary>
    [Flags]
    public enum ADUserAccountControl
    {
        /// <summary>Script olarak işaretlenmiş hesap.</summary>
        SCRIPT = 1,
        /// <summary>Hesap devre dışı.</summary>
        ACCOUNTDISABLE = 2,
        /// <summary>Ev dizini zorunlu.</summary>
        HOMEDIR_REQUIRED = 8,
        /// <summary>Hesap kilitlenmiş.</summary>
        LOCKOUT = 16,
        /// <summary>Şifre gerekli değil.</summary>
        PASSWD_NOTREQD = 32,
        /// <summary>Şifrenin değiştirilemeyeceği hesap.</summary>
        PASSWD_CANT_CHANGE = 64,
        /// <summary>Şifrelenmiş metinle şifreli hesap izni.</summary>
        ENCRYPTED_TEXT_PWD_ALLOWED = 128,
        /// <summary>Geçici olarak kopya hesap.</summary>
        TEMP_DUPLICATE_ACCOUNT = 256,
        /// <summary>Normal hesap.</summary>
        NORMAL_ACCOUNT = 512,
        /// <summary>İnter-domain güven hesabı.</summary>
        INTERDOMAIN_TRUST_ACCOUNT = 2048,
        /// <summary>Çalışma istasyonu güven hesabı.</summary>
        WORKSTATION_TRUST_ACCOUNT = 4096,
        /// <summary>Sunucu güven hesabı.</summary>
        SERVER_TRUST_ACCOUNT = 8192,
        /// <summary>Şifre süresiz olarak geçerli.</summary>
        DONT_EXPIRE_PASSWORD = 65536,
        /// <summary>MNS oturum açma hesabı.</summary>
        MNS_LOGON_ACCOUNT = 131072,
        /// <summary>Akıllı kart gerektirir.</summary>
        SMARTCARD_REQUIRED = 262144,
        /// <summary>Delegasyon için güvenilir.</summary>
        TRUSTED_FOR_DELEGATION = 524288,
        /// <summary>Delegasyon için güvenilmez.</summary>
        NOT_DELEGATED = 1048576,
        /// <summary>Sadece DES anahtarları kullanılsın.</summary>
        USE_DES_KEY_ONLY = 2097152,
        /// <summary>Ön yetkilendirme gerektirmiyor.</summary>
        DONT_REQ_PREAUTH = 4194304,
        /// <summary>Şifre süresi dolmuş.</summary>
        PASSWORD_EXPIRED = 8388608,
        /// <summary>Delegasyon için kimlik doğrulama yapmaya güvenilir.</summary>
        TRUSTED_TO_AUTH_FOR_DELEGATION = 16777216,
        /// <summary>Parçalı gizlilik hesabı.</summary>
        PARTIAL_SECRETS_ACCOUNT = 67108864
    }
}