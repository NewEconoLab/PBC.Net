using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using PbcProxy;
using Windows.Security.Cryptography;
using System.Diagnostics;

namespace Handshake
{
    class Utils
    {
        public const int IV_LENGTH = 0;
        private static readonly string ENC_ALG = SymmetricAlgorithmNames.AesCbcPkcs7; // crypto alg. to use for enc/dec
        
        public static byte[] encrypt(byte[] input, byte[] key)
        {
            var buffer = CryptographicBuffer.CreateFromByteArray(input);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(ENC_ALG);
            var symmetricKey = aes.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(key));
            
            // generate a random initialization vector
            // var ivBuffer = CryptographicBuffer.GenerateRandom(IV_LENGTH);

            var encryptedBuffer = CryptographicEngine.Encrypt(symmetricKey, buffer, null);
            byte[] encryptedByteArr = new byte[encryptedBuffer.Length];
            /*
            byte[] iv = new byte[IV_LENGTH];
            CryptographicBuffer.CopyToByteArray(ivBuffer, out iv);
            */
            CryptographicBuffer.CopyToByteArray(encryptedBuffer, out encryptedByteArr);

            /*
            byte[] outputBuffer = new byte[iv.Length + encryptedByteArr.Length];
            Buffer.BlockCopy(iv, 0, outputBuffer, 0, iv.Length);
            Buffer.BlockCopy(encryptedByteArr, 0, outputBuffer, iv.Length, encryptedByteArr.Length);
            */

            return encryptedByteArr;
        }

        public static byte[] decrypt(byte[] input, byte[] key)
        {
            /*
            byte[] iv = new byte[IV_LENGTH];
            */
            byte[] encryptedData = new byte[input.Length - IV_LENGTH];
            // Buffer.BlockCopy(input, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(input, IV_LENGTH, encryptedData, 0, encryptedData.Length);

            var encryptedBuffer = CryptographicBuffer.CreateFromByteArray(encryptedData);
            var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(ENC_ALG);
            var symmetricKey = aes.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(key));
            // var ivBuffer = CryptographicBuffer.CreateFromByteArray(iv);
            var decryptedBuffer = CryptographicEngine.Decrypt(symmetricKey, encryptedBuffer, null);
            byte[] outputBuffer = new byte[decryptedBuffer.Length];
            CryptographicBuffer.CopyToByteArray(decryptedBuffer, out outputBuffer);
            return outputBuffer;
        }

        public static void testPairings()
        {
            Debug.WriteLine("Running pairing test...");

            Pairing pairing = new Pairing();
            Debug.WriteLine("Pairing is " + (pairing.isSymmetric() ? "" : "not ") + "symmetric");

            Element g1 = pairing.elementFromHash(new G1(), "ABCDEF");
            byte[] buffer = g1.toBuffer();
            Debug.WriteLine(Convert.ToBase64String(buffer));

            Element g2 = pairing.elementFromHash(new G2(), "ADcsde");
            Element gt = pairing.apply(g1, g2);

            Debug.WriteLine("OK");
        }
    }
}
