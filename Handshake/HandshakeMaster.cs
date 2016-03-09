using System;
using System.Diagnostics;
using System.Text;

using PbcProxy;

namespace Handshake
{
    class HandshakeMaster
    {
        // Character set for generating pseudonyms
        private static char[] CHAR_SET =
            "ABCDEFGHIJKLMNOPQRTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        // Pseudonym and corresponding secret handed to a member
        public class MemberCredetials
        {
            public String role;
            public String pseudonym;
            public Element secret; // member secret: T_A

            public MemberCredetials(string pseudonym, string role, Element secret)
            {
                this.role = role;
                this.pseudonym = pseudonym;
                this.secret = secret;
            }
        }

        private Pairing m_pairing;
        private Element m_masterSecret; // t

        public HandshakeMaster()
        {
            m_pairing = new Pairing();
            Debug.Assert(m_pairing.isSymmetric());
            m_masterSecret = m_pairing.getRandomElement(new Zn());
        }

        public MemberCredetials issueCredentialsForClient(string role)
        {
            return issueCredentials(role, new G2());
        }

        public MemberCredetials issueCredentialsForServer(string role)
        {
            return issueCredentials(role, new G1());
        }

        private MemberCredetials issueCredentials(string role, GroupIface g)
        {
            string pseudonym = getRandomString();

            Element hash = m_pairing.elementFromHash(g, pseudonym + "-" + role);

            Element secret = hash.powZn(m_masterSecret); // member's secret: T = H_1(pseudonym)^t
            return new MemberCredetials(pseudonym, role, secret);
        }

        private string getRandomString()
        {
            StringBuilder randomString = new StringBuilder();

            // TODO: we can use same seed in order to get same result
            // on two different devices for testing
            Random generator = new Random();
            for (int i = 0; i < 6; i++)
            {
                char randomChar = CHAR_SET[generator.Next(CHAR_SET.Length)];
                randomString.Append(randomChar);
            }

            return randomString.ToString();
        } // end of getRandomString
    }
}
