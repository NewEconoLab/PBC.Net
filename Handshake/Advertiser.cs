using System;
using System.Text;
using System.IO;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Devices.Bluetooth.Advertisement;

namespace Handshake
{
    class Advertiser : HandshakeParty
    {
        public Advertiser(HandshakeMaster master, Logger logger)
            : base(master, SERVER_ROLE, logger)
        {

        }

        public override void start()
        {
            DataWriter writer = new DataWriter();
            writer.WriteByte((byte)PACKET_TYPE.PKT_TYPE_HELO);
            writer.WriteByte((byte) m_member.getPseudonym().Length);
            writer.WriteString(m_member.getPseudonym());
            byte[] adData = getWriterData(writer);
            startPublisher(adData);

            startScanning(respondToChallenge);
        }

        public override void stop()
        {
            stopPublisher();
            stopScanning();
        }

        private void respondToChallenge(byte[] data)
        {
            IBuffer buffer = CryptographicBuffer.CreateFromByteArray(data);
            DataReader reader = DataReader.FromBuffer(buffer);
            DataWriter writer = new DataWriter();

            // read packet type
            byte packetType = reader.ReadByte();
            m_logger.WriteLine("Packet type: " + packetType.ToString());

            // read pseudonym
            byte len = reader.ReadByte();
            string pseudonym = reader.ReadString(len);
            m_logger.WriteLine("Client pseudonym: " + pseudonym);

            byte[] sharedKey = m_member.computeSharedKeyServer(pseudonym, CLIENT_ROLE);
            m_logger.WriteLine("Computed shared key: " + BitConverter.ToString(sharedKey));

            // read challenge
            len = reader.ReadByte();
            byte[] encChallenge = new byte[len];
            reader.ReadBytes(encChallenge);

            byte[] decryptedChallenge = m_member.decryptChallenge(encChallenge, sharedKey);
            Int32 challenge = m_member.decodeChallenge(decryptedChallenge);
            m_logger.WriteLine("Received challenge: " + challenge.ToString());

            // set packet type
            writer.WriteByte((byte)PACKET_TYPE.PKT_TYPE_RESPONSE);

            writer.WriteInt32(challenge);
            byte[] adData = getWriterData(writer);

            // send back response
            startPublisher(adData);
        }
    }
}
