using System.Security.Cryptography;

namespace BusinessLogic.Security;

public static class RsaKeyLoader
{
    public static RSA LoadPrivateKey(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Contains("BEGIN", StringComparison.Ordinal))
        {
            var pemRsa = RSA.Create();
            pemRsa.ImportFromPem(value);
            return pemRsa;
        }

        var bytes = Convert.FromBase64String(value);
        try
        {
            var pkcs8Rsa = RSA.Create();
            pkcs8Rsa.ImportPkcs8PrivateKey(bytes, out _);
            return pkcs8Rsa;
        }
        catch (CryptographicException)
        {
            var pkcs1Rsa = RSA.Create();
            pkcs1Rsa.ImportRSAPrivateKey(bytes, out _);
            return pkcs1Rsa;
        }
    }

    public static RSA LoadPublicKey(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Contains("BEGIN", StringComparison.Ordinal))
        {
            var pemRsa = RSA.Create();
            pemRsa.ImportFromPem(value);
            return pemRsa;
        }

        var bytes = Convert.FromBase64String(value);
        try
        {
            var subjectPublicKeyRsa = RSA.Create();
            subjectPublicKeyRsa.ImportSubjectPublicKeyInfo(bytes, out _);
            return subjectPublicKeyRsa;
        }
        catch (CryptographicException)
        {
            var pkcs1Rsa = RSA.Create();
            pkcs1Rsa.ImportRSAPublicKey(bytes, out _);
            return pkcs1Rsa;
        }
    }
}
