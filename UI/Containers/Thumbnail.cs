using System.Text.RegularExpressions;
using AnimeRaider.Structures;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using AnimeRaider.Setting;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using System.IO;
using SkiaSharp;
using Avalonia;
using System;
using Avalonia.Interactivity;











namespace AnimeRaider.UI.Containers
{



    public class Thumbnail : Border
    {


        private Canvas? Master;


        private Animations.Transations.EaseInOut? HoverTranslationPlayIcone;
        private Animations.Transations.EaseInOut? HoverTranslationCover;
        private Animations.Transations.EaseInOut? PostionTranslation;
        private Animations.Transations.Uniform? ShowHideTransition;


        private TextBlock? EpisodeNumber;
        private TextBlock? EpisodeDuration;

        private Common.Status? EpisodeStatus;

        private ContextMenu? RightClickMenu;
        private MenuItem MenuItemComplete = new MenuItem { Header = "Complete"};
        private MenuItem MenuItemBookmarked = new MenuItem { Header = "Bookmark"};


        private Canvas? _MainCanvas;

        public Canvas? MainCanvas
        {
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        private Episode? _Episode;
        public Episode? Episode
        {
            get { return _Episode; }
            set { _Episode = value; }
        }


        private Image? _Cover;
        public Image? Cover{
            get { return _Cover; }
            set { _Cover = value; }
        }

        private Image? _PlayIcone;
        public Image? PlayIcone{
            get { return _PlayIcone; }
            set { _PlayIcone = value; }
        }


        private Bitmap? _CoverBitmap;
        public Bitmap? CoverBitmap
        {
            get { return _CoverBitmap; }
            set { _CoverBitmap = value; }
        }




        public Thumbnail(Canvas? master, Episode? episode){
            Master = master;
            Episode = episode;

            Background = Themes.Poster;
            ClipToBounds = true;
            IsVisible = false;
            Opacity = 0;

            CornerRadius = new CornerRadius(Config.CornerRadius);


            MainCanvas = new Canvas();


            ShowHideTransition = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration / 2,
                Trigger = ShowHideSetOpeicity,
            };



            HoverTranslationPlayIcone = new Animations.Transations.EaseInOut{
                StartingValue = 0,
                EndingValue = 0.7,
                CurrentValue = 1,
                Duration = Config.TransitionHover * 3,
                Trigger = SetPlayIconeOpacity
            };

            PointerEntered += HoverTranslationPlayIcone.TranslateForward;
            PointerExited += HoverTranslationPlayIcone.TranslateBackward;




            HoverTranslationCover = new Animations.Transations.EaseInOut{
                StartingValue = 1,
                EndingValue = 1.1,
                CurrentValue = 1,
                Duration = Config.TransitionHover * 3,
                Trigger = SetCoverSize
            };
            PointerEntered += HoverTranslationCover.TranslateForward;
            PointerExited += HoverTranslationCover.TranslateBackward;


            PointerReleased += OnClick;

            Cover = new Image{
                Stretch = Stretch.Uniform,
            };

            MainCanvas.Children.Add(Cover);
            //Loaded += OnLoaded; // instead  of loading  the  content when we create
            // the poseter we load the content when we show the
            // content 




            PlayIcone = new Image{
                Stretch = Stretch.Uniform,
                Width = 150,
                Height = 150,
                Opacity = 0,
            };
            MainCanvas.Children.Add(PlayIcone);

            Assets.AddAwaitedActions(() => {
                PlayIcone.Source = Assets.PlayBitmap;
            });


            EpisodeNumber = new TextBlock
            {
                Text = "None",
                Height = 25,
                Width = 50,
                FontSize = 25
            };
            MainCanvas.Children.Add(EpisodeNumber);


            EpisodeDuration = new TextBlock{
                Text = "None",
                Height = 25,
                Width = 300,
                FontSize = 25
            };
            MainCanvas.Children.Add(EpisodeDuration);

            EpisodeStatus = new Common.Status();
            MainCanvas.Children.Add(EpisodeStatus);



            RightClickMenu = new ContextMenu {
                ItemsSource = new[] {
                    MenuItemComplete,
                    MenuItemBookmarked,
                }
            };


            ContextMenu = RightClickMenu;


            if (Master != null){
                Master.SizeChanged += OnSizeChanged;
                OnSizeChanged(); // trigger the function so we can set the dimentions
            }

            Child = MainCanvas;

        }


        bool _loaded = false;
        private async void OnLoaded(object? sender = null, object? e = null)
        {
            if (SharedData.TargetedData.Series == null) return;

            if (_loaded){
                return;
            }

            _loaded = true;

            if (Episode == null ||
                Episode.Thumbnail == null ||
                Cover == null) return;

            if (EpisodeNumber != null) {
                EpisodeNumber.Text = ExtractLastNumberString(Episode.Name);
            }

            if (EpisodeDuration != null){
                EpisodeDuration.Text = SecondsToHMS(Episode.Duration);
            }



            SetStatus();





            // this is responsible for the thumbnail image
            int targetWidth = (int)(Width * 1.4);  // capture UI property on UI thread
            // Run the image download and resizing off the UI thread
            MemoryStream? imageStream = await Task.Run(async () => {
                SKBitmap? loaded = await Network.Requester.GetImage(Episode.Thumbnail);
                if (loaded == null) return null;

                MemoryStream? resizedStream = SKImageToDarkenBitmap(loaded);
                return resizedStream;
            });

            if (imageStream == null) return;

            // Set the Avalonia image source on the UI thread
            using (imageStream)
            {

                CoverBitmap = new Bitmap(imageStream);
                Cover.Source = CoverBitmap;

                if (MainCanvas != null){
                    Cover.Width = MainCanvas.Width;
                    Cover.Height = MainCanvas.Height;
                }
            }

            // just in case the using doesnt dispose of the stream
            imageStream.Dispose();
        }


        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (Master != null){
                Width = 360;
                Height = 202;


                if (MainCanvas != null) {
                    MainCanvas.Width = Width;
                    MainCanvas.Height = Height;


                    if (PlayIcone != null){
                        Canvas.SetLeft(PlayIcone, (MainCanvas.Width - PlayIcone.Width) / 2);
                        Canvas.SetTop(PlayIcone, (MainCanvas.Height - PlayIcone.Height) / 2);
                    }


                    if (Cover != null){
                        Canvas.SetLeft(Cover, (MainCanvas.Width - Cover.Width) / 2);
                        Canvas.SetTop(Cover, (MainCanvas.Height - Cover.Height) / 2);
                    }

                    if (EpisodeNumber != null) {
                        Canvas.SetLeft(EpisodeNumber, MainCanvas.Width - EpisodeNumber.Width);
                        Canvas.SetTop(EpisodeNumber, MainCanvas.Height - EpisodeNumber.Height - 15);
                    }


                    if (EpisodeDuration != null){
                        Canvas.SetLeft(EpisodeDuration, 10);
                        Canvas.SetTop(EpisodeDuration, MainCanvas.Height - EpisodeDuration.Height - 15);
                    }

                    if (EpisodeStatus != null) {
                        Canvas.SetLeft(EpisodeStatus, MainCanvas.Width - EpisodeStatus.Width - 25);
                        Canvas.SetTop(EpisodeStatus,25);
                    }

                }
            }
        }



        public void Show()
        {
            if (Opacity == 1) return;
            OnLoaded();

            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();
        }

        public void Hide()
        {
            if (Opacity == 0) return;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();
        }

        private bool IsKill = false;
        public void Kill(){
            if (ShowHideTransition != null){
                ShowHideTransition.TranslateBackward();
                IsKill = true;
            }
        }

        public void ShowHideSetOpeicity(double Value)
        {

            //if (HoverTranslation != null &&
            //    Value <= HoverTranslation.CurrentValue) {
            //    Opacity = Value;
            //}

            Opacity = Value;

            if (Opacity == 0 &&
                IsKill == true &&
                ShowHideTransition != null &&
                ShowHideTransition.FunctionRunning == false &&
                Master != null)
            {

                // this if statment just checks if the user want to kill this object and removes it
                // from its parent

                Master.Children.Remove(this);
                Dispose();



            }
        }

        public void OnHoverSetCoverOpacity(double Value)
        {


            //if (Background is SolidColorBrush solidBrush)
            //{
            //   var originalColor = solidBrush.Color;
            //    byte newAlpha = (byte)(255 * Value);
            //    var fadedColor = Color.FromArgb(newAlpha, originalColor.R, originalColor.G, originalColor.B);
            //    Background = new SolidColorBrush(fadedColor);
            //}

            if (Cover != null)
            {
                Cover.Opacity = Value;
                return;
            }
            //if (HoverTranslation != null) HoverTranslation.Reset();
        }


        public void OnHoverSetMainCanvasSize(double Value)
        {
            if (MainCanvas != null){
                MainCanvas.Width = Width * Value;
                MainCanvas.Height = Height * Value;
            }
        }


        public void SetPlayIconeOpacity(double value) {

            if (PlayIcone != null) { 
                PlayIcone.Opacity = value;
            }
        }

        public void SetCoverSize(double value) {

            if (Cover == null) return;

            Cover.Width = Width * value;
            Cover.Height = Height * value;

            if (MainCanvas != null) {
                Canvas.SetLeft(Cover, (MainCanvas.Width - Cover.Width) / 2);
                Canvas.SetTop(Cover, (MainCanvas.Height - Cover.Height) / 2);
            }


        }



        private Vector InitialPos;
        private Vector FinalPos;
        public void SetPostionTranslate(double Xpos, double Ypos)
        {

            if (PostionTranslation != null && PostionTranslation.FunctionRunning)
            {
                FinalPos = new Vector(Xpos, Ypos);
                return;
            };

            if (Canvas.GetLeft(this) == Xpos && Canvas.GetTop(this) == Ypos)
            {
                return;
            }

            if (Canvas.GetTop(this) == Ypos || IsVisible == false)
            {
                Canvas.SetLeft(this, Xpos);
                Canvas.SetTop(this, Ypos);
                return;
            }



            PostionTranslation = new Animations.Transations.EaseInOut
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetPostionTrigger
            };

            if (double.IsNaN(Canvas.GetLeft(this)))
            { // this will only occures on start up
                Canvas.SetLeft(this, Xpos);
                Canvas.SetTop(this, Ypos);
                return;
            }

            InitialPos = new Vector(Canvas.GetLeft(this), Canvas.GetTop(this));
            FinalPos = new Vector(Xpos, Ypos);

            PostionTranslation.TranslateForward();
        }

        private void SetPostionTrigger(double value){
            Canvas.SetLeft(this, InitialPos.X + (FinalPos.X - InitialPos.X) * value);
            Canvas.SetTop(this, InitialPos.Y + (FinalPos.Y - InitialPos.Y) * value);
        }


        private void OnClick(object? sender, PointerReleasedEventArgs e){
            e.Handled = true;
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased){
                if (sender is Control control){
                    var pointerPosition = e.GetPosition(control);
                    if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                    if (pointerPosition.X > Width || pointerPosition.Y > Height) return;


                    SharedData.TargetedData.Episode = Episode;

                    // Transition to the next window
                    //if (PublicWidgets.UIPlayer != null){
                    //PublicWidgets.TransitionForward(PublicWidgets.UIPlayer);
                    //}

                    // simple methode to open vlc and play the video that you clicked on

                    if (SharedData.TargetedData.Series == null || SharedData.TargetedData.Series.Name == null) {
                        return;
                    }

                    if (SharedData.TargetedData.Episode == null || SharedData.TargetedData.Episode.Name == null){
                        return;
                    }




                    var series = Uri.EscapeDataString(SharedData.TargetedData.Series.Name);
                    var episode = Uri.EscapeDataString(SharedData.TargetedData.Episode.Name);

                    var psi = new ProcessStartInfo{
                        FileName = @"vlc", // Use @ to avoid escaping backslashes
                        Arguments = Network.Server.Domain + Network.Server.Watch + $"/{series}/{episode}", // Quote the whole URL argument
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    try{
                        Process.Start(psi);
                    }
                    catch (Exception ex){
                        Console.WriteLine("Failed to launch: " + ex.Message);
                    }


                    SetComplete();

                }
            }
        }

        public async void SetComplete(object? sender=null, object? e=null) {
            if (SharedData.TargetedData.Series == null) return;
            bool results = await Network.Requester.AddCompletedEpisode(SharedData.Data.Username,
                                                               SharedData.Data.Password,
                                                               SharedData.TargetedData.Series.Name,
                                                               Episode);

            if (results)
            {
                // now we can add it locally
                SharedData.Data.SetEpisodeComplete(SharedData.TargetedData.Series.Name,
                                                   Episode);

                SetStatus();

            }

        }

        public async void RemoveComplete(object? sender = null, object? e = null)
        {
            if (SharedData.TargetedData.Series == null) return;
            bool results = await Network.Requester.RemoveCompletedEpisode(SharedData.Data.Username,
                                                               SharedData.Data.Password,
                                                               SharedData.TargetedData.Series.Name,
                                                               Episode);

            if (results)
            {
                // now we can add it locally
                SharedData.Data.RemoveEpisodeComplete(SharedData.TargetedData.Series.Name,
                                                   Episode);

                SetStatus();

            }

        }

        public async void SetBookmark(object? sender = null, object? e = null){
            if (SharedData.TargetedData.Series == null) return;

            bool results = await Network.Requester.AddBookmarkEpisode(SharedData.Data.Username,
                                                       SharedData.Data.Password,
                                                       SharedData.TargetedData.Series.Name,
                                                       Episode);

            if (results)
            {
                // now we can add it locally
                SharedData.Data.SetEpisodeBookmarked(SharedData.TargetedData.Series.Name,
                                                   Episode);

                SetStatus();

            }

        }

        public async void RemoveBookmark(object? sender = null, object? e = null){
            if (SharedData.TargetedData.Series == null) return;

            bool results = await Network.Requester.RemoveBookmarkEpisode(SharedData.Data.Username,
                                                       SharedData.Data.Password,
                                                       SharedData.TargetedData.Series.Name,
                                                       Episode);

            if (results)
            {
                // now we can add it locally
                SharedData.Data.RemoveEpisodeBookmarked(SharedData.TargetedData.Series.Name,
                                                   Episode);

                SetStatus();

            }

        }


        public void SetStatus() {
            if (SharedData.TargetedData.Series == null ||
                SharedData.TargetedData.Series.Name == null || 
                Episode == null) return;

            if (EpisodeStatus != null)
                EpisodeStatus.Reset();

            if (SharedData.Data.IsEpisodeInCompleted(
            SharedData.TargetedData.Series.Name,
            Episode.Name))
            {
                if (EpisodeStatus != null)
                    EpisodeStatus.SetColor(Themes.Complete);

                MenuItemComplete.Header = "UnComplete";
                MenuItemComplete.Click -= RemoveComplete;
                MenuItemComplete.Click += RemoveComplete;
                MenuItemComplete.Click -= SetComplete;
            }
            else {
                MenuItemComplete.Header = "Complete";
                MenuItemComplete.Click -= SetComplete;
                MenuItemComplete.Click += SetComplete;
                MenuItemComplete.Click -= RemoveComplete;

            }

            if (SharedData.Data.IsEpisodeBookmarked(
                    SharedData.TargetedData.Series.Name,
                    Episode.Name
                ))
            {
                if (EpisodeStatus != null)
                    EpisodeStatus.SetColor(Themes.BookmarkedEpisode);

                MenuItemBookmarked.Header = "UnBookmark";
                MenuItemBookmarked.Click -= RemoveBookmark;
                MenuItemBookmarked.Click += RemoveBookmark;
                MenuItemBookmarked.Click -= SetBookmark;
            }
            else {
                MenuItemBookmarked.Header = "Bookmark";
                MenuItemBookmarked.Click -= SetBookmark;
                MenuItemBookmarked.Click += SetBookmark;
                MenuItemBookmarked.Click -= RemoveBookmark;
            }
        }






        public void Dispose()
        {

            if (HoverTranslationPlayIcone != null)
            {
                PointerEntered -= HoverTranslationPlayIcone.TranslateForward;
                PointerExited -= HoverTranslationPlayIcone.TranslateBackward;
            }

            if (HoverTranslationCover != null)
            {
                PointerEntered -= HoverTranslationCover.TranslateForward;
                PointerExited -= HoverTranslationCover.TranslateBackward;
            }

            HoverTranslationPlayIcone = null;
            HoverTranslationCover = null;
            ShowHideTransition = null;
            PostionTranslation = null;

            // Detach events

            PointerReleased -= OnClick;
            //Loaded -= OnLoaded;

            if (Master != null)
            {
                Master.SizeChanged -= OnSizeChanged;
            }

            // Clear children


            if (MainCanvas != null)
            {
                MainCanvas.Children.Clear();
            }


            if (Cover != null && CoverBitmap != null)
            {
                CoverBitmap.Dispose();
                Cover.Source = null;
            }


            // Null out references
            Cover = null!;
            MainCanvas = null!;
            Episode = null!;
            Master = null!;
            HoverTranslationCover = null!;
            ShowHideTransition = null!;
            Child = null!;
        }






        public static string ExtractLastNumberString(string? input){
            if (string.IsNullOrWhiteSpace(input))
                return "None";

            // Remove file extension like ".mp4"
            string withoutExtension = Path.GetFileNameWithoutExtension(input);

            var matches = Regex.Matches(withoutExtension, @"\d+(\.\d+)?");

            return matches.Count > 0 ? matches[^1].Value : "None";
        }


        public static string SecondsToHMS(double? totalSeconds){
            if (totalSeconds == null || totalSeconds < 0)
                totalSeconds = 0;

            int roundedSeconds = (int)Math.Round(totalSeconds.Value);

            int hours = roundedSeconds / 3600;
            int minutes = (roundedSeconds % 3600) / 60;
            int seconds = roundedSeconds % 60;

            if (hours > 0)
                return $"{hours}:{minutes:D2}:{seconds:D2}";
            else
                return $"{minutes}:{seconds:D2}";
        }






        public static MemoryStream SKImageToDarkenBitmap(SKBitmap original){
            // this darkens the image at the bottom so the writing on it is more visiable

            // Clone original into a new bitmap to draw on
            var resultBitmap = new SKBitmap(original.Width, original.Height);
            using (var canvas = new SKCanvas(resultBitmap)){
                // Draw the original image
                canvas.DrawBitmap(original, 0, 0);

                // Prepare a gradient from transparent to black (bottom 40%)
                using var paint = new SKPaint{
                    Shader = SKShader.CreateLinearGradient(
                        new SKPoint(0, original.Height * 0.6f),
                        new SKPoint(0, original.Height),
                        new SKColor[] {
                    new SKColor(0, 0, 0, 0),     // transparent
                    new SKColor(0, 0, 0, 180)    // semi-transparent black
                        },
                        null,
                        SKShaderTileMode.Clamp
                    )
                };

                // Draw gradient over image
                canvas.DrawRect(new SKRect(0, original.Height * 0.6f, original.Width, original.Height), paint);
            }

            // Encode the modified image to PNG
            using var image = SKImage.FromBitmap(resultBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            var stream = new MemoryStream(data.ToArray());
            stream.Position = 0; // reset position to beginning

            return stream;
        }








    }
}
