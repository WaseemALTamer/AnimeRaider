using AnimeRaider.Setting;
using Avalonia.Controls;
using Avalonia;

namespace AnimeRaider.UI.Containers
{
    public class DropDownLocalSort : Border
    {
        private Canvas? Master;


        private Animations.Transations.Uniform? ShowHideTransation;


        private ComboBox? MainMenu;

        private ComboBoxItem CompletedItem = new ComboBoxItem { Content = "Completed", FontSize = 18 };
        private ComboBoxItem WatchingItem = new ComboBoxItem { Content = "Watching", FontSize = 18 };
        private ComboBoxItem PlanToWatchItem = new ComboBoxItem { Content = "PlanToWatch", FontSize = 18 };
        private ComboBoxItem BookmarkedItem = new ComboBoxItem { Content = "Bookmarked", FontSize = 18 };
        private ComboBoxItem WatchAgainItem = new ComboBoxItem { Content = "WatchAgain", FontSize = 18 };

        public DropDownLocalSort(Canvas master) {
            Master = master;
            IsVisible = false;
            Opacity = 0;
            Width = 200; Height = 50;

            CornerRadius = new CornerRadius(Config.CornerRadius);

            MainMenu = new ComboBox
            {
                Width = 250,
                Height = 40,
                PlaceholderText = "User Category",
                Background = Themes.DropDown,
                PlaceholderForeground = Themes.WaterMark,
                Items = {
                    CompletedItem,
                    WatchingItem, 
                    PlanToWatchItem,
                    BookmarkedItem,
                    WatchAgainItem 
                },
                CornerRadius = new CornerRadius(Config.CornerRadius),
                FontSize = 18
            };
            MainMenu.PointerPressed += (s, e) => { MainMenu.SelectedIndex = -1; };

            MainMenu.SelectionChanged += OnSelectionChanged;


            ShowHideTransation = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetOpacity
            };

            if (Master != null){
                OnSizeChanged();
                Master.SizeChanged += OnSizeChanged;
            }


            Child = MainMenu;
        }


        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){
                Canvas.SetLeft(this, Master.Width - this.Width - 200);
                Canvas.SetTop(this, 25);
            }
        }


        bool IsShowing = false;
        public void Show(){

            if (IsShowing == true) return;

            if (ShowHideTransation == null) return;
            IsShowing = true;
            ShowHideTransation.TranslateForward();
        }

        public void Hide(){

            if (IsShowing == false) return;

            if (ShowHideTransation == null) return;
            IsShowing = false;
            ShowHideTransation.TranslateBackward();
        }


        public void SetOpacity(double Value){
            Opacity = Value;
            IsVisible = Value != 0;
        }



        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (MainMenu == null) return;

            if (MainMenu.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem)
                {
                    case var _ when selectedItem == WatchingItem:
                        SetWatching();
                        break;
                    case var _ when selectedItem == CompletedItem:
                        SetCompleted();
                        break;
                    case var _ when selectedItem == PlanToWatchItem:
                        SetPlanToWatch();
                        break;
                    case var _ when selectedItem == BookmarkedItem:
                        SetBookmarked();
                        break;
                    case var _ when selectedItem == WatchAgainItem:
                        SetWatchAgain();
                        break;
                }
            }
        }


        private void SetCompleted()
        {
            if (SharedData.Data.UserData == null) return;

            if (PublicWidgets.UISorter != null && 
                SharedData.Data.UserData.Completed != null) 
            {
                PublicWidgets.UISorter.Update(SharedData.Data.UserData.Completed);
                if (PublicWidgets.DisplayedPage != PublicWidgets.UISorter)
                    PublicWidgets.TransitionForward(PublicWidgets.UISorter);
            }
        }

        private void SetWatching() {

            if (SharedData.Data.UserData == null) return;

            if (PublicWidgets.UISorter != null &&
                SharedData.Data.UserData.Watching != null)
            {
                PublicWidgets.UISorter.Update(SharedData.Data.UserData.Watching);
                if (PublicWidgets.DisplayedPage != PublicWidgets.UISorter)
                    PublicWidgets.TransitionForward(PublicWidgets.UISorter);
            }

        }


        private void SetPlanToWatch()
        {
            if (SharedData.Data.UserData == null) return;

            if (PublicWidgets.UISorter != null &&
                SharedData.Data.UserData.PlanToWatch != null)
            {
                PublicWidgets.UISorter.Update(SharedData.Data.UserData.PlanToWatch);
                if (PublicWidgets.DisplayedPage != PublicWidgets.UISorter)
                    PublicWidgets.TransitionForward(PublicWidgets.UISorter);
            }


        }

        private void SetBookmarked()
        {

            if (SharedData.Data.UserData == null) return;

            if (PublicWidgets.UISorter != null &&
                SharedData.Data.UserData.Bookmarked != null)
            {
                PublicWidgets.UISorter.Update(SharedData.Data.UserData.Bookmarked);
                if (PublicWidgets.DisplayedPage != PublicWidgets.UISorter)
                    PublicWidgets.TransitionForward(PublicWidgets.UISorter);
            }

        }

        private void SetWatchAgain()
        {
            if (SharedData.Data.UserData == null) return;

            if (PublicWidgets.UISorter != null &&
                SharedData.Data.UserData.WatchAgain != null)
            {
                PublicWidgets.UISorter.Update(SharedData.Data.UserData.WatchAgain);
                if (PublicWidgets.DisplayedPage != PublicWidgets.UISorter)
                    PublicWidgets.TransitionForward(PublicWidgets.UISorter);
            }


        }


    }
}
