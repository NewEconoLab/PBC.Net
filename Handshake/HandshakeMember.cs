using System;

using System.Diagnostics;
using PbcProxy;

namespace Handshake
{
    class HandshakeMember
    {
        private Element m_secret;
        private string m_pseudonym;
        private string m_role;

        private Pairing m_pairing;

        public HandshakeMember(Element secret, string pseudonym, string role)
        {
            m_secret = secret;
            m_pseudonym = pseudonym;
            m_role = role;
            m_pairing = new Pairing();
            Debug.Assert(m_pairing.isSymmetric());
        }

        public string getPseudonym()
        {
            return m_pseudonym;
        }

        public string getRole()
        {
            return m_role;
        }

        /// <summary>
        /// Obtain a random challenge to send to the other party
        /// </summary>
        /// <returns></returns>
        public byte[] getChallenge(byte[] key, out Int32 challenge)
        {
            challenge = new Random().Next();
            byte[] encryptedChallenge = Utils.encrypt(BitConverter.GetBytes(challenge), key);
            return encryptedChallenge;
        }

        /// <summary>
        /// Decrypt an encrypted buffer using the provided key
        /// </summary>
        /// <param name="encChallenge">Encrypted challenge buffer</param>
        /// <param name="key">Decryption key</param>
        /// <returns>Decrypted response buffer</returns>
        public byte[] decryptChallenge(byte[] encChallenge, byte[] key)
        {
            return Utils.decrypt(encChallenge, key);
        }

        public Int32 decodeChallenge(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Compute shared key for the handshake communication.
        /// </summary>
        /// <param name="pseudonym"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public byte[] computeSharedKeyClient(string pseudonym, string role)
        {
            string credential = pseudonym + "-" + role;
            Element credentialHash = m_pairing.elementFromHash(new G1(), credential);
            Element sharedKey = m_pairing.apply(credentialHash, m_secret);
            return sharedKey.toBuffer();
        }

        /// <summary>
        /// Compute shared key for the handshake communication.
        /// </summary>
        /// <param name="pseudonym"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public byte[] computeSharedKeyServer(string pseudonym, string role)
        {
            string credential = pseudonym + "-" + role;
            Element credentialHash = m_pairing.elementFromHash(new G2(), credential);
            Element sharedKey = m_pairing.apply(m_secret, credentialHash);
            return sharedKey.toBuffer();
        }

    } // end of HandshakeMember class
}
