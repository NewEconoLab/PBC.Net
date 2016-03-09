using System;
using System.Text;
using System.IO;
using Windows.Storage.Streams;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Security.Cryptography;

namespace Handshake
{
    class Scanner : HandshakeParty
    {
        private int m_challenge;

        public Scanner(HandshakeMaster master, Logger logger)
            : base(master, CLIENT_ROLE, logger)
        {
            
        }

        public override void start()
        {
            startScanning(sendChallenge);
        }

        public override void stop()
        {
            stopPublisher();
            stopScanning();
        }

        private async void sendChallenge(byte[] data)
        {
            IBuffer buffer = CryptographicBuffer.CreateFromByteArray(data);
            DataReader reader = DataReader.FromBuffer(buffer);
            DataWriter writer = new DataWriter();

            // read packet type
            byte packetType = reader.ReadByte();
            m_logger.WriteLine("Packet type: " + packetType);

            // read pseudonym
            byte len = reader.ReadByte();
            string pseudonym = reader.ReadString(len);
            m_logger.WriteLine("Server pseudonym: " + pseudonym);

            byte[] sharedKey = m_member.computeSharedKeyClient(pseudonym, HandshakeParty.SERVER_ROLE);
            m_logger.WriteLine("Computed shared key: " + BitConverter.ToString(sharedKey));

            // set packet type and write pseudonym
            writer.WriteByte((byte)PACKET_TYPE.PKT_TYPE_CHLNG);
            writer.WriteByte((byte)m_member.getPseudonym().Length);
            writer.WriteString(m_member.getPseudonym());

            // prepare challenge
            byte[] encChallenge = m_member.getChallenge(sharedKey, out m_challenge);
            m_logger.WriteLine("Sending challenge " + m_challenge.ToString() + " to other party");

            // send challenge length
            len = (byte)encChallenge.Length;
            writer.WriteByte(len);

            // send challenge
            writer.WriteBytes(encChallenge);
            byte[] adData = getWriterData(writer);
            startPublisher(adData);

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(3));

            // wait for response
            startScanning(handleResponse);
        }

        private void handleResponse(byte[] data)
        {
            IBuffer buffer = CryptographicBuffer.CreateFromByteArray(data);
            DataReader reader = DataReader.FromBuffer(buffer);

            byte packetType = reader.ReadByte();
            m_logger.WriteLine("Packet type: " + packetType.ToString());

            Int32 response = reader.ReadInt32();

            if (response == m_challenge)
            {
                m_logger.WriteLine("Handshake SUCCESS. Response = " + response.ToString());
            }
            else {
                m_logger.WriteLine("Handshake FAIL. Response = " + response.ToString());
            }
        }
    }
}
