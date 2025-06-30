using AnimeRaider.Structures;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using AnimeRaider.Setting;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using SkiaSharp;
using System.IO;
using Avalonia;
using Avalonia.Interactivity;
using AnimeRaider.UI.Containers.Common;
using System;








namespace AnimeRaider.UI.Containers
{



    public class Poster : Border
    {


        private Canvas? Master;


        private Animations.Transations.Uniform? PostionTranslation;
        private Animations.Transations.EaseInOut? HoverTranslation;
        private Animations.Transations.Uniform? ShowHideTransition;


        private ContextMenu? RightClickMenu;

        private MenuItem CopyNameItem = new MenuItem { Header = "Copy Name" };

        private MenuItem CompletedItem = new MenuItem { Header = "Completed"};
        private MenuItem WatchingItem = new MenuItem { Header = "Watching" };
        private MenuItem PlanToWatchItem = new MenuItem { Header = "Plan To Watch"};
        private MenuItem BookmarkedItem = new MenuItem { Header = "Bookmarked"};
        private MenuItem WatchAgainItem = new MenuItem { Header = "Watch Again"};


        private Status? SeriesStatus;





        private Canvas? _MainCanvas;
        public Canvas? MainCanvas
        {
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        private Series? _Series;
        public Series? Series{
            get { return _Series; }
            set { _Series = value; }
        }


        private Image? _Cover;
        public Image? Cover{
            get { return _Cover; }
            set { _Cover = value; }
        }

        private Bitmap? _CoverBitmap;
        public Bitmap? CoverBitmap{
            get { return _CoverBitmap; }
            set { _CoverBitmap = value; }
        }


        public Poster(Canvas? master, Series? series){
            Master = master;
            Series = series;

            Background = Themes.Poster;
            ClipToBounds = true;
            IsVisible = false;
            Opacity = 0;

            CornerRadius = new CornerRadius(Config.CornerRadius);


            MainCanvas = new Canvas();


            ShowHideTransition = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration / 2,
                Trigger = ShowHideSetOpeicity,
            };



            HoverTranslation = new Animations.Transations.EaseInOut{
                StartingValue = 1,
                EndingValue = 1.1,
                CurrentValue = 1,
                Duration = Config.TransitionHover * 3,
                Trigger = OnHoverSetCoverSize
            };

            PointerEntered += HoverTranslation.TranslateForward;
            PointerExited += HoverTranslation.TranslateBackward;
            PointerReleased += OnClick;

            Cover = new Image {
                Stretch = Stretch.Uniform,
            };

            MainCanvas.Children.Add(Cover);
            //Loaded += OnLoaded; // instead  of loading  the  content when we create
            // the poseter we load the content when we show the
            // content 



            SeriesStatus = new Status();
            MainCanvas.Children.Add(SeriesStatus);
            


            RightClickMenu = new ContextMenu {
                ItemsSource = new[] {
                        CopyNameItem,
                        CompletedItem,
                        WatchingItem,
                        PlanToWatchItem,
                        BookmarkedItem,
                        WatchAgainItem
                }
            };

            CopyNameItem.Click += OnClickCopyName;

            SetStatus();

            ContextMenu = RightClickMenu;


            if (Master != null){
                Master.SizeChanged += OnSizeChanged;
                OnSizeChanged(); // trigger the function so we can set the dimentions
            }

            Child = MainCanvas;

        }


        bool _loaded = false;
        private async void OnLoaded(object? sender = null, object? e = null){

            if (_loaded) {
                return;
            }

            _loaded = true;

            if (Series == null ||
                Series.Cover == null ||
                Cover == null) return;


            int targetWidth = (int)(Width * 1.4);  // capture UI property on UI thread
            // Run the image download and resizing off the UI thread
            MemoryStream? imageStream = await Task.Run(async () =>{
                SKBitmap? loaded = await Network.Requester.GetImage(Series.Cover);
                if (loaded == null) return null;

                MemoryStream? resizedStream = ResizeBitmap(loaded, targetWidth);
                return resizedStream;
            });

            if (imageStream == null) return;

            // Set the Avalonia image source on the UI thread
            using (imageStream) {

                CoverBitmap = new Bitmap(imageStream);
                Cover.Source = CoverBitmap;

                if (MainCanvas != null) {

                    MainCanvas.Width = Width;
                    MainCanvas.Height = Height;

                    Cover.Width = MainCanvas.Width;
                    Cover.Height = MainCanvas.Height;
                }
            }

            // just in case the using doesnt dispose of the stream
            imageStream.Dispose();
        }


        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){
                Width = 270;
                Height = 405;


                if (MainCanvas != null){
                    MainCanvas.Width = Width;
                    MainCanvas.Height = Height;

                    if (SeriesStatus != null){
                        Canvas.SetLeft(SeriesStatus, MainCanvas.Width - SeriesStatus.Width - 25);
                        Canvas.SetTop(SeriesStatus, 25);
                    }


                }


            }
        }



        public void Show(){
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



        private async void OnClickCopyName(object? sender, RoutedEventArgs args){
            if (Series == null) return;
            if (Series.Name == null) return;

            
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard == null) return;

            var dataObject = new DataObject();
            dataObject.Set(DataFormats.Text, Series.Name);
            await clipboard.SetDataObjectAsync(dataObject);
        }


        // COMPLETEED
        public async void SetCompleted(object? sender = null, object? e = null){
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.AddSeriesToCategory(SharedData.Data.Username,
                                                               SharedData.Data.Password,
                                                               Network.Server.Completed,
                                                               Series);

            if (results){
                // now we can add it locally
                SharedData.Data.AddSeriesToList(Series, SharedData.Data.UserData.Completed);
                SetStatus();

            }

        }

        public async void RemoveCompleted(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.RemoveSeriesToCategory(SharedData.Data.Username,
                                                               SharedData.Data.Password,
                                                               Network.Server.Completed,
                                                               Series);

            if (results){
                // now we can add it locally
                SharedData.Data.RemoveSeriesFromList(Series, SharedData.Data.UserData.Completed);
                SetStatus();
            }

        }

        // WATCHING
        public async void SetWatching(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.AddSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.Watching,
                Series);

            if (results)
            {
                SharedData.Data.AddSeriesToList(Series, SharedData.Data.UserData.Watching);
                SetStatus();
            }
        }

        public async void RemoveWatching(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.RemoveSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.Watching,
                Series);

            if (results)
            {
                SharedData.Data.RemoveSeriesFromList(Series, SharedData.Data.UserData.Watching);
                SetStatus();
            }
        }

        // PLAN TO WATCH
        public async void SetPlanToWatch(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.AddSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.PlanToWatch,
                Series);

            if (results)
            {
                SharedData.Data.AddSeriesToList(Series, SharedData.Data.UserData.PlanToWatch);
                SetStatus();
            }
        }

        public async void RemovePlanToWatch(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.RemoveSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.PlanToWatch,
                Series);

            if (results)
            {
                SharedData.Data.RemoveSeriesFromList(Series, SharedData.Data.UserData.PlanToWatch);
                SetStatus();
            }
        }

        // BOOKMARKED
        public async void SetBookmarked(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.AddSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.Bookmarked,
                Series);

            if (results)
            {
                SharedData.Data.AddSeriesToList(Series, SharedData.Data.UserData.Bookmarked);
                SetStatus();
            }
        }

        public async void RemoveBookmarked(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.RemoveSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.Bookmarked,
                Series);

            if (results)
            {
                SharedData.Data.RemoveSeriesFromList(Series, SharedData.Data.UserData.Bookmarked);
                SetStatus();
            }
        }

        // WATCH AGAIN
        public async void SetWatchAgain(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.AddSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.WatchAgain,
                Series);

            if (results)
            {
                SharedData.Data.AddSeriesToList(Series, SharedData.Data.UserData.WatchAgain);
                SetStatus();
            }
        }

        public async void RemoveWatchAgain(object? sender = null, object? e = null)
        {
            if (Series == null || SharedData.Data.UserData == null) return;

            bool results = await Network.Requester.RemoveSeriesToCategory(
                SharedData.Data.Username,
                SharedData.Data.Password,
                Network.Server.WatchAgain,
                Series);

            if (results)
            {
                SharedData.Data.RemoveSeriesFromList(Series, SharedData.Data.UserData.WatchAgain);
                SetStatus();
            }
        }






        public void SetStatus()
        {
            if (Series == null || SharedData.Data.UserData == null) return;
            
            if(SeriesStatus != null)
                SeriesStatus.Reset();


            // WatchAgain
            if (SharedData.Data.IsSeriesInList(Series, SharedData.Data.UserData.WatchAgain))
            {
                WatchAgainItem.Header = "UnWatchAgain";
                WatchAgainItem.Click -= RemoveWatchAgain;
                WatchAgainItem.Click += RemoveWatchAgain;
                WatchAgainItem.Click -= SetWatchAgain;
            }
            else
            {
                WatchAgainItem.Header = "WatchAgain";
                WatchAgainItem.Click -= SetWatchAgain;
                WatchAgainItem.Click += SetWatchAgain;
                WatchAgainItem.Click -= RemoveWatchAgain;
            }

            // PlanToWatch
            if (SharedData.Data.IsSeriesInList(Series, SharedData.Data.UserData.PlanToWatch))
            {
                PlanToWatchItem.Header = "UnPlanToWatch";
                PlanToWatchItem.Click -= RemovePlanToWatch;
                PlanToWatchItem.Click += RemovePlanToWatch;
                PlanToWatchItem.Click -= SetPlanToWatch;
                if (SeriesStatus != null)
                    SeriesStatus.SetColor(Themes.PlanToWatch);
            }
            else
            {
                PlanToWatchItem.Header = "PlanToWatch";
                PlanToWatchItem.Click -= SetPlanToWatch;
                PlanToWatchItem.Click += SetPlanToWatch;
                PlanToWatchItem.Click -= RemovePlanToWatch;
            }

            // Watching
            if (SharedData.Data.IsSeriesInList(Series, SharedData.Data.UserData.Watching))
            {
                WatchingItem.Header = "UnWatching";
                WatchingItem.Click -= RemoveWatching;
                WatchingItem.Click += RemoveWatching;
                WatchingItem.Click -= SetWatching;
                if (SeriesStatus != null)
                    SeriesStatus.SetColor(Themes.Watching);
            }
            else
            {
                WatchingItem.Header = "Watching";
                WatchingItem.Click -= SetWatching;
                WatchingItem.Click += SetWatching;
                WatchingItem.Click -= RemoveWatching;
            }

            // Completed
            if (SharedData.Data.IsSeriesInList(Series, SharedData.Data.UserData.Completed))
            {
                CompletedItem.Header = "UnCompleted";
                CompletedItem.Click -= RemoveCompleted;
                CompletedItem.Click += RemoveCompleted;
                CompletedItem.Click -= SetCompleted;
                if (SeriesStatus != null)
                    SeriesStatus.SetColor(Themes.Complete);
            }
            else
            {
                CompletedItem.Header = "Completed";
                CompletedItem.Click -= SetCompleted;
                CompletedItem.Click += SetCompleted;
                CompletedItem.Click -= RemoveCompleted;
            }

            // Bookmarked
            if (SharedData.Data.IsSeriesInList(Series, SharedData.Data.UserData.Bookmarked))
            {
                BookmarkedItem.Header = "UnBookmarked";
                BookmarkedItem.Click -= RemoveBookmarked;
                BookmarkedItem.Click += RemoveBookmarked;
                BookmarkedItem.Click -= SetBookmarked;
                if (SeriesStatus != null)
                    SeriesStatus.SetColor(Themes.Bookmarked);
            }
            else
            {
                BookmarkedItem.Header = "Bookmarked";
                BookmarkedItem.Click -= SetBookmarked;
                BookmarkedItem.Click += SetBookmarked;
                BookmarkedItem.Click -= RemoveBookmarked;
            }


        }

















        public void Dispose(){

            if (HoverTranslation != null) {
                PointerEntered -= HoverTranslation.TranslateForward;
                PointerExited -= HoverTranslation.TranslateBackward;
            }

            HoverTranslation = null;
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
            

            if (MainCanvas != null) {
                MainCanvas.Children.Clear();
            }


            if (Cover != null && CoverBitmap != null) {
                CoverBitmap.Dispose();
                Cover.Source = null;
            }


            // Null out references
            Cover = null!;
            MainCanvas = null!;
            Series = null!;
            Master = null!;
            HoverTranslation = null!;
            ShowHideTransition = null!;
            Child = null!;
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

            if (Cover != null) {
                Cover.Opacity = Value;
                return;
            }
            if (HoverTranslation != null) HoverTranslation.Reset();
        }


        public void OnHoverSetCoverSize(double Value){

            if (Cover != null) { 
                Cover.Width = Width * Value;
                Cover.Height = Height * Value;

                if (MainCanvas != null){
                    Canvas.SetLeft(Cover, (MainCanvas.Width - Cover.Width) / 2);
                    Canvas.SetTop(Cover, (MainCanvas.Height - Cover.Height) / 2);
                }
            }


        }


        private Vector InitialPos;
        private Vector FinalPos;

        private double LastVlaue; // this will be used for clamping the last frame for smother transitions

        public void SetPostionTranslate(double Xpos, double Ypos){
            if (PostionTranslation != null && PostionTranslation.FunctionRunning){
                PostionTranslation.Pause();
            };



            if (double.IsNaN(Canvas.GetLeft(this))){ 
                // this will only occures on start up
                Canvas.SetLeft(this, Xpos);
                Canvas.SetTop(this, Ypos);
                return;
            }

            Vector newFinal = new Vector(Xpos, Ypos);

            if (FinalPos == newFinal) return;


            InitialPos = new Vector(Canvas.GetLeft(this), Canvas.GetTop(this));
            FinalPos = newFinal;
            Vector delta = FinalPos - InitialPos;



            if (delta.Length < 10) {
                Canvas.SetLeft(this, Xpos);
                Canvas.SetTop(this, Ypos);
                return;
            }

            LastVlaue = 0;
            PostionTranslation = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration * (delta.Length / Config.TransitionReferenceDistance),
                Trigger = SetPostionTrigger
            };

            PostionTranslation.TranslateForward();
        }

        private void SetPostionTrigger(double value){
            if (value > LastVlaue) {
                LastVlaue = value;
                Canvas.SetLeft(this, InitialPos.X + (FinalPos.X - InitialPos.X) * value);
                Canvas.SetTop(this, InitialPos.Y + (FinalPos.Y - InitialPos.Y) * value);
            }
        }


        private void OnClick(object? sender, PointerReleasedEventArgs e)
        {

            e.Handled = true;
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                if (sender is Control control)
                {
                    var pointerPosition = e.GetPosition(control);
                    if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                    if (pointerPosition.X > Width || pointerPosition.Y > Height) return;


                    SharedData.TargetedData.Series = Series;
                    SharedData.TargetedData.CoverImage = CoverBitmap;

                    // Transition to the next window
                    if (PublicWidgets.UISeries != null){
                        PublicWidgets.TransitionForward(PublicWidgets.UISeries);
                    }

                }
            }
        }









        public static MemoryStream ResizeBitmap(SKBitmap original, int targetPixelWidth){
            float aspectRatio = (float)original.Height / original.Width;
            int newHeight = (int)(targetPixelWidth * aspectRatio);

            var resized = original.Resize(new SKImageInfo(targetPixelWidth, newHeight), SKFilterQuality.Medium);

            using var image = SKImage.FromBitmap(resized);
            using var data = image.Encode(SKEncodedImageFormat.Png, 90);

            var stream = new MemoryStream(data.ToArray());
            stream.Position = 0; // reset position to beginning

            return stream;  // do NOT dispose here
        }




    }
}
