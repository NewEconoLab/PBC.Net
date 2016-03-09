using System;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;

namespace Handshake
{
    class Logger
    {
        private TextBox m_textBox;

        public Logger(TextBox textBox)
        {
            m_textBox = textBox;
        }

        public void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }

        private void threadWriteLine(string message)
        {
            m_textBox.Text = m_textBox.Text + message + Environment.NewLine;
        }
    }
}
