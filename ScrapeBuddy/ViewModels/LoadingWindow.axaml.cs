using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;

namespace ScrapeBuddy
{
    /// <summary>
    /// Represents a window that displays a loading screen with a progress bar.
    /// </summary>
    public partial class LoadingWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingWindow"/> class.
        /// </summary>
        public LoadingWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Simulates the initialization of the application, updating the progress bar and status text.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitApp()
        {
            var start = DateTime.Now.Ticks;
            var time = start;
            var progressValue = 0;

            // Increment progress value over the course of one second
            while ((time - start) < TimeSpan.TicksPerSecond)
            {
                progressValue++;
                Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
                await Task.Delay(25);
                time = DateTime.Now.Ticks;
            }

            start = time;
            Dispatcher.UIThread.Post(() => LoadingText.Text = "Initializing Application Settings...");

            // Increment progress value over the course of two seconds while displaying status
            var limit = TimeSpan.TicksPerSecond * 2;
            while ((time - start) < limit)
            {
                progressValue += 1;
                Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
                await Task.Delay(50);
                time = DateTime.Now.Ticks;
            }

            Dispatcher.UIThread.Post(() => LoadingText.Text = "Preparing App...");

            // Increment progress to 100% in small steps
            while (progressValue < 100)
            {
                progressValue += 1;
                Dispatcher.UIThread.Post(() => ProgressBar1.Value = progressValue);
                await Task.Delay(10);
            }
        }
    }
}
