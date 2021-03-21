using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WPFMediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        private const string TIME_FORMAT = @"hh\:mm\:ss";
        private bool _isplaying = false;
        private bool _isNewFileOpened = false;
        private TimeSpan _currentTime;
        private DispatcherTimer _timerVideoTime;

        public MainWindow()
        {
            InitializeComponent();
            ChangeIsPlaying(false);
        }

        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = @"Video files
                   (*.mpg; *.mpeg; *.avi; *.mp4)| *.mpg; *.mpeg; *.avi; *.mp4"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Source = new Uri(openFileDialog.FileName);
            }
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnMediaEnded()
        {
            mediaPlayer.Stop();
            ChangeIsPlaying(false);
        }

        private void ChangeIsPlaying(bool isPlaying)
        {
            _isplaying = isPlaying;

            if (_isplaying)
            {
                btnPlayPause.Content = "Pause";
                btnPlayPause.ToolTip = "Play the media";
                mediaPlayer.Play();
            }
            else
            {
                btnPlayPause.Content = "Play"; 
                btnPlayPause.Content = "Pause the media";
                mediaPlayer.Pause();
            }
        }

        private void MediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            OnMediaEnded();
        }

        private void MediaElement_OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OnMediaEnded();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Close();
        }
        
        private void PlayPauseClicked(object sender, RoutedEventArgs e)
        {
            ChangeIsPlaying(!_isplaying);
        }

        private void MyMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            _isNewFileOpened = true;
            ChangeIsPlaying(true);
            btnPlayPause.IsEnabled = true;

            // Create a timer that will update the counters and the time slider
            _timerVideoTime = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timerVideoTime.Tick += new EventHandler(Timer_Tick);
            _timerVideoTime.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                if (_isNewFileOpened)
                {
                    _isNewFileOpened = false;
                    playBackSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    _currentTime = mediaPlayer.NaturalDuration.TimeSpan;
                    totalTime.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(TIME_FORMAT);
                }
                if (_currentTime.TotalSeconds > 0)
                {
                    // Updating time slider
                    playBackSlider.Value = mediaPlayer.Position.TotalSeconds;
                    currentTime.Text = mediaPlayer.Position.ToString(TIME_FORMAT);
                }
            }
        }
    }
}