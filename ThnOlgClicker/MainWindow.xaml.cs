using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ThnOlgClicker
{
    public partial class MainWindow : Window
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        private int sleepInterval = 1000;

        private bool isRunning = false;
        private Thread thread;

        public MainWindow()
        {
            InitializeComponent();
            Init();
            UpdateUi();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Start();

            UpdateUi();
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Stop();

            UpdateUi();
        }

        private void TimeoutSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sleepInterval = (int)e.NewValue;
            UpdateUi();
        }

        private void Init()
        {
            this.timeoutSlider.Minimum = 2;
            this.timeoutSlider.Maximum = 15;
            this.timeoutSlider.SmallChange = 1;
            this.timeoutSlider.LargeChange = 5;
            this.timeoutSlider.IsSnapToTickEnabled = true;
            this.timeoutSlider.TickFrequency = 1;
            this.timeoutSlider.Value = sleepInterval;
        }

        private void Start()
        {
            if (this.thread != null)
            {
                return;
            }

            this.isRunning = true;
            thread = new Thread(ThreadMethod);
            this.thread.Start();
        }

        private void Stop()
        {
            if (this.thread == null)
            {
                return;
            }

            this.isRunning = false;
            this.thread.Interrupt();
            this.thread.Join();
            this.thread = null;
        }

        private void UpdateUi()
        {
            timeoutLabel.Content = sleepInterval.ToString("# s");
            progressBar.Maximum = sleepInterval;

            if (this.isRunning == true)
            {
                this.timerLabel.Content = "Enabled";
            }
            else
            {
                this.timerLabel.Content = "Disabled";
            }
        }

        private void ThreadMethod()
        {
            DateTime dateTime = DateTime.Now;

            while (this.isRunning)
            {
                int age = (int)(DateTime.Now - dateTime).TotalMilliseconds;

                if (age >= sleepInterval * 1000)
                {
                    dateTime = DateTime.Now;
                    DoMouseClick();
                }

                Dispatcher.Invoke(() => this.progressBar.Value = (age / 1000));

                try
                {
                    Thread.Sleep(500);
                }
                catch (ThreadInterruptedException ignored)
                {

                }

            }
        }

        private void DoMouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 1, 1, 0, 0);
        }
    }
}
