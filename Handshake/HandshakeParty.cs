using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace Handshake
{
    abstract class HandshakeParty
    {
        protected enum PACKET_TYPE
        {
            PKT_TYPE_HELO,
            PKT_TYPE_CHLNG,
            PKT_TYPE_RESPONSE
        }

        private const ushort COMPANY_ID = 0xBEEF;
        protected const string SERVER_ROLE = "vendor";
        protected const string CLIENT_ROLE = "customer";

        protected Logger m_logger;
        protected HandshakeMember m_member;
        private BluetoothLEAdvertisementPublisher m_publisher;
        private BluetoothLEAdvertisementWatcher m_watcher;

        public delegate void AdvertisementDataHandler(byte[] manufacturerData);
        private AdvertisementDataHandler m_advHandler;

        public HandshakeParty(HandshakeMaster master, string role, Logger logger)
        {
            m_logger = logger;
            HandshakeMaster.MemberCredetials serverCredentials = master.issueCredentialsForClient(role);
            m_member = new HandshakeMember(serverCredentials.secret, serverCredentials.pseudonym, serverCredentials.role);
            m_logger.WriteLine(role + " pseudonym: " + m_member.getPseudonym());
        }

        abstract public void start();
        abstract public void stop();

        protected void startScanning(AdvertisementDataHandler handler)
        {
            m_advHandler = handler;
            m_watcher = new BluetoothLEAdvertisementWatcher();

            // we need Active Scanning to send a ScanRequest and receive ScanResponse
            m_watcher.ScanningMode = BluetoothLEScanningMode.Active;
            m_watcher.Received += watcherReceivedHandler;
            m_watcher.Start();
            m_logger.WriteLine("Started scanning");
        }

        protected void stopScanning()
        {
            if (null == m_watcher)
            {
                m_logger.WriteLine("Scanner is not active");
                return;
            }

            m_watcher.Stop();
            m_watcher.Received -= watcherReceivedHandler;
            m_logger.WriteLine("Stopped scanning");
        }

        protected void startPublisher(byte[] data)
        {
            BluetoothLEAdvertisement advertisement = new BluetoothLEAdvertisement();
            advertisement.ManufacturerData.Add(new BluetoothLEManufacturerData(COMPANY_ID, CryptographicBuffer.CreateFromByteArray(data)));
            m_publisher = new BluetoothLEAdvertisementPublisher(advertisement);
            m_publisher.StatusChanged += publisherStatusChangeHandler;
            m_publisher.Start();
        }

        protected void publisherStatusChangeHandler(BluetoothLEAdvertisementPublisher sender,
                                                    BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
        {
            m_logger.WriteLine("Advertisement publisher status changed to " + args.Status);
        }

        protected void stopPublisher()
        {
            if (null == m_publisher)
            {
                m_logger.WriteLine("Publisher is not active");
                return;
            }

            m_publisher.Stop();
            m_publisher.StatusChanged -= publisherStatusChangeHandler; // unregister handler
            m_publisher = null;
        }

        protected static byte[] getWriterData(DataWriter writer)
        {
            IBuffer ibuffer = writer.DetachBuffer();
            byte[] adData = new byte[ibuffer.Length];
            CryptographicBuffer.CopyToByteArray(ibuffer, out adData);
            return adData;
        }

        private void watcherReceivedHandler(BluetoothLEAdvertisementWatcher sender,
                                            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            // m_logger.WriteLine("Received advertisement from " + args.Advertisement.LocalName +
                // "(" + args.BluetoothAddress.ToString("X2") + ")");
            // m_logger.WriteLine("Advertisement type: " + args.AdvertisementType.ToString());

            // filter advertisement with our COMPANY_ID only and then call registered handler

            IReadOnlyList<BluetoothLEManufacturerData> dataList = args.Advertisement.GetManufacturerDataByCompanyId(COMPANY_ID);
            if (0 == dataList.Count)
            {
                // m_logger.WriteLine("No manufacturer data in advertisement for our company id");
                return;
            }

            Debug.Assert(null != m_advHandler);
            
            foreach (BluetoothLEManufacturerData data in dataList) {
                try
                {
                    // we got data from the expected advertising party - 
                    // stop scanning and advertising for now
                    stopPublisher();
                    stopScanning();

                    byte[] buffer = new byte[data.Data.Length];
                    CryptographicBuffer.CopyToByteArray(data.Data, out buffer);
                    
                    // handle the received data
                    m_advHandler(buffer);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
