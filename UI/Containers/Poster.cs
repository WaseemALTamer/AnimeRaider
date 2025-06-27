using AnimeRaider.Structures;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using AnimeRaider.Setting;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using SkiaSharp;
using Avalonia;
using System.IO;







namespace AnimeRaider.UI.Containers
{



    public class Poster : Border
    {


        private Canvas? Master;


        private Animations.Transations.EaseInOut? PostionTranslation;
        private Animations.Transations.EaseInOut? HoverTranslation;
        private Animations.Transations.Uniform? ShowHideTransition;





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
        public Bitmap? CoverBitmap
        {
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
                Trigger = OnHoverSetMainCanvasSize
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


        public void OnHoverSetMainCanvasSize(double Value){
            if (MainCanvas != null){
                MainCanvas.Width = Width * Value;
                MainCanvas.Height = Height * Value;

                if (Cover != null) { 
                    Cover.Width = Width * Value;
                    Cover.Height = Height * Value;
                }


            }

        }


        private Vector InitialPos;
        private Vector FinalPos;
        public void SetPostionTranslate(double Xpos, double Ypos)
        {

            if (PostionTranslation != null && PostionTranslation.FunctionRunning) {
                FinalPos = new Vector(Xpos, Ypos);
                return;
            };

            if (Canvas.GetLeft(this) == Xpos && Canvas.GetTop(this) == Ypos) {
                return;
            }

            if (Canvas.GetTop(this) == Ypos || IsVisible == false) {
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
