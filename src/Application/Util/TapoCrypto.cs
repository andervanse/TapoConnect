﻿using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Tapo.Application.Util;
public class RsaKeyPair
{
    public RsaKeyPair(string publicKey, string privateKey)
    {
        PublicKey = publicKey;
        PrivateKey = privateKey;
    }

    public string PublicKey { get; }
    public string PrivateKey { get; }
}

public static class TapoCrypto
{
    public static string UuidV4()
    {
        return Guid.NewGuid().ToString();
    }

    public static string Base64Encode(string plainText)
    {
        ArgumentNullException.ThrowIfNull(plainText, nameof(plainText));

        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        ArgumentNullException.ThrowIfNull(base64EncodedData, nameof(base64EncodedData));

        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static byte[] Sha1Hash(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));

        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(bytes);

        return hash;
    }

    public static byte[] Sha256Hash(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));

        using var sha1 = SHA256.Create();
        var hash = sha1.ComputeHash(bytes);

        return hash;
    }

    private static SymmetricAlgorithm GetCryptoAlgorithm(byte[] key, byte[] iv)
    {
        using SymmetricAlgorithm cryptor = Aes.Create();

        cryptor.Mode = CipherMode.CBC;
        cryptor.Padding = PaddingMode.PKCS7;
        cryptor.KeySize = 128;
        cryptor.BlockSize = 128;

        cryptor.Key = key ?? throw new ArgumentNullException(nameof(key));
        cryptor.IV = iv ?? throw new ArgumentNullException(nameof(iv));

        return cryptor;
    }

    public static byte[] Encrypt(string data, byte[] key, byte[] iv)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(iv, nameof(iv));

        var bytes = Encoding.UTF8.GetBytes(data);
        var cryptor = GetCryptoAlgorithm(key, iv);
        var cipher = cryptor.CreateEncryptor(key, iv);
        byte[] encrypted = cipher.TransformFinalBlock(bytes, 0, data.Length);
        return encrypted;
    }

    public static byte[] Decrypt(byte[] bytes, byte[] key, byte[] iv)
    {
        ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(iv, nameof(iv));

        var cryptor = GetCryptoAlgorithm(key, iv);
        var cipher = cryptor.CreateDecryptor(key, iv);
        return cipher.TransformFinalBlock(bytes, 0, bytes.Length);
    }

    public static RsaKeyPair GenerateKeyPair(string password, int length)
    {
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        //Create Random
        CryptoApiRandomGenerator randomGenerator = new();

        //RSAKeyPairGenerator generates the RSA keypair based on the random number and strength of the key required
        RsaKeyPairGenerator rsaKeyPairGen = new();
        rsaKeyPairGen.Init(new KeyGenerationParameters(new SecureRandom(randomGenerator), length));
        AsymmetricCipherKeyPair keyPair = rsaKeyPairGen.GenerateKeyPair();

        //Extracting the private key from the pair
        RsaKeyParameters Privatekey = (RsaKeyParameters)keyPair.Private;

        //Extracting the public key from the pair
        RsaKeyParameters Publickey = (RsaKeyParameters)keyPair.Public;

        //Creating public key in pem format\
        TextWriter pubtxtWriter = new StringWriter();
        PemWriter pubpemWriter = new(pubtxtWriter);
        pubpemWriter.WriteObject(Publickey);
        pubpemWriter.Writer.Flush();

        //now save the follwing string variable into a file. that's our public key
        string? publicKeyPem = pubtxtWriter.ToString();
        if (publicKeyPem == null)
        {
            throw new NullReferenceException($"PemWriter failed to create PEM.");
        }
        else
        {
            //encrypted Private Key
            //give desired password, with good strength
            AsymmetricKeyParameter privateKey = Privatekey;
            StringWriter sw = new();
            PemWriter pw = new(sw);
            pw.WriteObject(privateKey, "AES-256-CBC", password.ToCharArray(), new SecureRandom());
            pw.Writer.Close();
            pw.Writer.Flush();

            //now save the follwing string variable into a file. that's our "Encrypted private key"
            string privateKeyPem = sw.ToString();

            return new RsaKeyPair(publicKeyPem, privateKeyPem);
        }
    }

    public static byte[] DecryptWithPrivateKeyAndPassword(string base64Input, string privateKey, string password)
    {
        ArgumentNullException.ThrowIfNull(base64Input, nameof(base64Input));
        ArgumentNullException.ThrowIfNull(privateKey, nameof(privateKey));
        ArgumentNullException.ThrowIfNull(password, nameof(password));

        var bytesToDecrypt = Convert.FromBase64String(base64Input);

        //get a stream from the string
        AsymmetricCipherKeyPair keyPair;
        var decryptEngine = new Pkcs1Encoding(new RsaEngine());

        using (var txtreader = new StringReader(privateKey))
        {
            var obj = new PemReader(txtreader, new PasswordFinder(password)).ReadObject();
            keyPair = (AsymmetricCipherKeyPair)obj;

            decryptEngine.Init(false, keyPair.Private);
        }

        var bytesDecrypted = decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);

        return bytesDecrypted;
    }

    public static byte[] GenerateRandomBytes(int length = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[length];
        rng.GetBytes(randomBytes);
        return randomBytes;
    }

    private class PasswordFinder: IPasswordFinder
    {
        private readonly string password;

        public PasswordFinder(string password)
        {
            this.password = password;
        }

        public char[] GetPassword()
        {
            return password.ToCharArray();
        }
    }
}
