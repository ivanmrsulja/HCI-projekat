using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NapredneKontrole.Grafika
{
	/// <summary>
	/// Interaction logic for VideoPlayer.xaml
	/// </summary>
	public partial class VideoPlayer : Window
	{

		public double Duration { get; set; }

		public VideoPlayer()
		{
			InitializeComponent();

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += timer_Tick;
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			Duration = (mePlayer.NaturalDuration.HasTimeSpan ? mePlayer.NaturalDuration.TimeSpan : TimeSpan.FromMilliseconds(0)).TotalSeconds;
			if (mePlayer.Source != null)
			{
				if (mePlayer.NaturalDuration.HasTimeSpan)
					lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
			}
			else
				lblStatus.Content = "No file selected...";
			slider.Maximum = Duration;
			slider.Value = mePlayer.Position.TotalSeconds;
		}

		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{
			if ((int)mePlayer.Position.TotalSeconds == (int)Duration)
			{
				mePlayer.Position = TimeSpan.FromSeconds(0L);
			}
			mePlayer.Play();
		}

		private void btnPause_Click(object sender, RoutedEventArgs e)
		{
			mePlayer.Pause();
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			mePlayer.Stop();
		}

		private void Ubrzanje_Click(object sender, RoutedEventArgs e)
		{
			mePlayer.SpeedRatio = 1.5;
		}

		private void Wind_Click(object sender, RoutedEventArgs e)
		{
			mePlayer.Position = TimeSpan.FromSeconds((long)slider.Value);
			mePlayer.LoadedBehavior = MediaState.Manual;
			mePlayer.Play();
		}
	}
}