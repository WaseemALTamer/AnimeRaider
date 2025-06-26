using AnimeRaider.Setting;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;




namespace AnimeRaider.UI.Containers
{
    public class Searcher : Border
    {
        // this will be a simple entry that will be used for searching
        // this will have animation and adapated logic


        private Canvas? Master;

        private Animations.Transations.EaseOut? ShowHideTransation;


        private TextBox? _Entry;
        public TextBox? Entry{
            get { return _Entry; }
            set { _Entry = value; }
        }


        public Searcher(Canvas? master = null) {

            Master = master;
            Width = 300; 
            Height = 40;
            Opacity = 0;

            CornerRadius = new CornerRadius(Config.CornerRadius);



            Entry = new TextBox {
                Width = 300,
                Height = 40,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius),
                Watermark = "Search",
                Background = Themes.Entry,
                Foreground = Themes.Text
            };

            Canvas.SetLeft(this, 100);
            Canvas.SetTop(this, 25);



            ShowHideTransation = new Animations.Transations.EaseOut{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration * 3,
                Trigger = SetOpacity
            };

            KeyDown += OnEnetryKeyDown;


            Child = Entry;
        }


        private void OnEnetryKeyDown(object? sender, KeyEventArgs? e){


            if (e == null) return;

            if (e.Key == Key.Enter || e.Key == Key.Space){
                if (PublicWidgets.UISearch == null) return;

                if (PublicWidgets.DisplayedPage != PublicWidgets.UISearch)
                    PublicWidgets.TransitionForward(PublicWidgets.UISearch);

                if (Entry != null && Entry.Text != null)
                    PublicWidgets.UISearch.Update(Entry.Text);
            }
        }





        bool IsShowing = false;
        public void Show()
        {
            if (ShowHideTransation == null) return;
            IsShowing = true;
            ShowHideTransation.TranslateForward();
        }

        public void Hide()
        {
            if (ShowHideTransation == null) return;
            IsShowing = false;
            ShowHideTransation.TranslateBackward();
        }


        public void SetOpacity(double Value){
            Opacity = Value;
            IsVisible = Value != 0;
        }

    }
}
