using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Handshake
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const bool USE_BLE_ADVERTISEMENT = true;
        private const uint RANDOM_SEED = 0;

        private HandshakeMaster m_master;
        private Scanner m_scanner;
        private Advertiser m_advertiser;

        private Logger m_logger;

        public MainPage()
        {
            this.InitializeComponent();

            m_logger = new Logger(this.textBox);

            // This one must be called to initialize the random function
            PbcProxy.PBC.init(RANDOM_SEED);

#if false
            // Test PBC
            PbcProxy.PBC.test();

            // Test pairing proxy
            Utils.testPairings();
#endif

            m_master = new HandshakeMaster();
            m_scanner = new Scanner(m_master, m_logger);
            m_advertiser = new Advertiser(m_master, m_logger);
        }

        private void StartStopAdvertising_Click(object sender, RoutedEventArgs e)
        {
            Button advertisingButton = (Button)sender;
            if (advertisingButton.Content.Equals("Stop Advertising"))
            {
                stopAdvertising(advertisingButton);
                return;
            }

            startAdvertising(advertisingButton);
        }

        private void stopAdvertising(Button advertisingButton)
        {
            m_advertiser.stop();
            advertisingButton.Content = "Start Advertising";
            m_logger.WriteLine("Stopped advertising");
        }

        private
        void startAdvertising(Button advertisingButton)
        {
            m_advertiser.start();
            advertisingButton.Content = "Stop Advertising";
            m_logger.WriteLine("Started as Advertiser");
        }

        private void StartStopScanning_Click(object sender, RoutedEventArgs e)
        {
            Button scanningButton = (Button)sender;
            if (scanningButton.Content.Equals("Stop Scanning"))
            {
                scanningButton.Content = "Start Scanning";
                m_scanner.stop();
                return;
            }

            m_scanner.start();
            scanningButton.Content = "Stop Scanning";
        }
    }
}
