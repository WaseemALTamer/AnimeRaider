using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;




namespace AnimeRaider.UI.Pages
{
    public class Player : Base
    {
        private LibVLC? _libVLC;
        private MediaPlayer? _mediaPlayer;

        public Player(Canvas? master) : base(master)
        {
            if (MainCanvas == null)
                return;

            //Task.Run(() =>
            //{
                // Initialize LibVLC (runs only once globally)
                //Core.Initialize();

                //var libVLC = new LibVLC();
                //var mediaPlayer = new MediaPlayer(libVLC);

                // Create VideoView on UI thread
                //Dispatcher.UIThread.Post(() =>
                //{
                //    var videoView = new VideoView
                //    {
                //        Width = 800,
                //        Height = 450,
                //        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                //        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                //        MediaPlayer = mediaPlayer
                //    };

                //    Canvas.SetLeft(videoView, 0);
                //    Canvas.SetTop(videoView, 0);
                //    MainCanvas.Children.Add(videoView);
                //});

                // Load and play media (still on background thread)
                //var media = new Media(libVLC, "http://127.0.0.1:8000/watch/ReZERO%20-Starting%20Life%20in%20Another%20World-/Episode%2010.mp4", FromType.FromLocation);
                //mediaPlayer.Play(media);
            //});

        }
    }
}
