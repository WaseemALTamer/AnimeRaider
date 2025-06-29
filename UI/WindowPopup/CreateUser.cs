using AnimeRaider.Setting;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;


namespace AnimeRaider.UI.WindowPopup
{
    public class CreateUser: Base
    {

        private Animations.Transations.Uniform? WrongHostnameTranstion;
        private Animations.Transations.Uniform? WrongUsernameTranstion;
        private Animations.Transations.Uniform? WrongPasswordTranstion;


        private TextBox? HostAdressEntry;
        private TextBox? UsernameEntry;
        private TextBox? PasswordEntry;

        private Button? CreateButton;

        public CreateUser(Canvas master) : base(master)
        {

            if (MainCanvas == null) return;

            HostAdressEntry = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 40,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius),
                Watermark = "HostAdress",
                Background = Themes.Entry,
            };
            MainCanvas.Children.Add(HostAdressEntry);
            HostAdressEntry.KeyDown += OnEnetryKeyDown;


            UsernameEntry = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 40,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius),
                Watermark = "Username",
                Background = Themes.Entry,
            };
            MainCanvas.Children.Add(UsernameEntry);
            UsernameEntry.KeyDown += OnEnetryKeyDown;

            PasswordEntry = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 40,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius),
                Watermark = "Password",
                PasswordChar = char.Parse("*"),
                Background = Themes.Entry,
            };
            MainCanvas.Children.Add(PasswordEntry);
            PasswordEntry.KeyDown += OnEnetryKeyDown;



            CreateButton = new Button
            {
                Content = "Create Account",
                Width = 300,
                Height = 50,
                Background = Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius)
            };
            MainCanvas.Children.Add(CreateButton);
            CreateButton.Click += OnClickCreateButton;


            WrongHostnameTranstion = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongHostname,
            };


            WrongUsernameTranstion = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongUsername,
            };

            WrongPasswordTranstion = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongPassword,
            };


            MinHeight = 270;
            

            OnSizeChanged();
            MainCanvas.SizeChanged += OnSizeChanged;

        }


        private void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null) {
            if (MainCanvas == null) return;

            if (MainCanvas != null)
            {
                MainCanvas.Width = Width;
                MainCanvas.Height = Height;






                if (HostAdressEntry != null)
                {
                    Canvas.SetLeft(HostAdressEntry, (MainCanvas.Width - HostAdressEntry.Width) / 2);
                    Canvas.SetTop(HostAdressEntry, 25);
                }

                if (UsernameEntry != null)
                {
                    Canvas.SetLeft(UsernameEntry, (MainCanvas.Width - UsernameEntry.Width) / 2);
                    Canvas.SetTop(UsernameEntry, 95);
                }

                if (PasswordEntry != null)
                {
                    Canvas.SetLeft(PasswordEntry, (MainCanvas.Width - PasswordEntry.Width) / 2);
                    Canvas.SetTop(PasswordEntry, 145);
                }

                if (CreateButton != null){
                    Canvas.SetLeft(CreateButton, (MainCanvas.Width - CreateButton.Width) / 2);
                    Canvas.SetTop(CreateButton, MainCanvas.Height  - CreateButton.Height - 10);
                }

            }

        }


        private void TriggerWrongHostname(double value)
        {
            if (Background is SolidColorBrush solidBrush)
            {
                var originalColorEntry = Themes.Entry;
                var targetColor = Themes.Wrong;

                byte Lerp(byte start, byte end) => (byte)(start + (end - start) * value);

                //var newColorEntry = Color.FromArgb(
                //    Lerp(originalColorEntry.Color.A, targetColor.Color.A),
                //    Lerp(originalColorEntry.Color.R, targetColor.Color.R),
                //    Lerp(originalColorEntry.Color.G, targetColor.Color.G),
                //    Lerp(originalColorEntry.Color.B, targetColor.Color.B)
                //);

                var originalColorText = Themes.Text;

                var newColorText = Color.FromArgb(
                    Lerp(originalColorText.Color.A, targetColor.Color.A),
                    Lerp(originalColorText.Color.R, targetColor.Color.R),
                    Lerp(originalColorText.Color.G, targetColor.Color.G),
                    Lerp(originalColorText.Color.B, targetColor.Color.B)
                );

                if (HostAdressEntry != null)
                {
                    // changing the color of the background is better but Avilonia doesnt document the c#
                    // part of there code look into making your own text entry at  this point, prefarably
                    // in the near future switch to the orginal sloution

                    //Entry.Background = new SolidColorBrush(newColorEntry);
                    HostAdressEntry.Foreground = new SolidColorBrush(newColorText);
                }

                if (WrongHostnameTranstion != null &&
                    WrongHostnameTranstion.FunctionRunning == false &&
                    value == 1)
                { // loop around and go back to normal
                    WrongHostnameTranstion.TranslateBackward();
                }
            }
        }


        private void TriggerWrongUsername(double value)
        {
            if (Background is SolidColorBrush solidBrush)
            {
                var originalColorEntry = Themes.Entry;
                var targetColor = Themes.Wrong;

                byte Lerp(byte start, byte end) => (byte)(start + (end - start) * value);

                //var newColorEntry = Color.FromArgb(
                //    Lerp(originalColorEntry.Color.A, targetColor.Color.A),
                //    Lerp(originalColorEntry.Color.R, targetColor.Color.R),
                //    Lerp(originalColorEntry.Color.G, targetColor.Color.G),
                //    Lerp(originalColorEntry.Color.B, targetColor.Color.B)
                //);

                var originalColorText = Themes.Text;

                var newColorText = Color.FromArgb(
                    Lerp(originalColorText.Color.A, targetColor.Color.A),
                    Lerp(originalColorText.Color.R, targetColor.Color.R),
                    Lerp(originalColorText.Color.G, targetColor.Color.G),
                    Lerp(originalColorText.Color.B, targetColor.Color.B)
                );

                if (UsernameEntry != null)
                {
                    // changing the color of the background is better but Avilonia doesnt document the c#
                    // part of there code look into making your own text entry at  this point, prefarably
                    // in the near future switch to the orginal sloution

                    //Entry.Background = new SolidColorBrush(newColorEntry);
                    UsernameEntry.Foreground = new SolidColorBrush(newColorText);
                }

                if (WrongUsernameTranstion != null &&
                    WrongUsernameTranstion.FunctionRunning == false &&
                    value == 1)
                { // loop around and go back to normal
                    WrongUsernameTranstion.TranslateBackward();
                }
            }
        }


        private void TriggerWrongPassword(double value)
        {
            if (Background is SolidColorBrush solidBrush)
            {
                var originalColorEntry = Themes.Entry;
                var targetColor = Themes.Wrong;

                byte Lerp(byte start, byte end) => (byte)(start + (end - start) * value);

                //var newColorEntry = Color.FromArgb(
                //    Lerp(originalColorEntry.Color.A, targetColor.Color.A),
                //    Lerp(originalColorEntry.Color.R, targetColor.Color.R),
                //    Lerp(originalColorEntry.Color.G, targetColor.Color.G),
                //    Lerp(originalColorEntry.Color.B, targetColor.Color.B)
                //);

                var originalColorText = Themes.Text;

                var newColorText = Color.FromArgb(
                    Lerp(originalColorText.Color.A, targetColor.Color.A),
                    Lerp(originalColorText.Color.R, targetColor.Color.R),
                    Lerp(originalColorText.Color.G, targetColor.Color.G),
                    Lerp(originalColorText.Color.B, targetColor.Color.B)
                );

                if (PasswordEntry != null)
                {
                    // changing the color of the background is better but Avilonia doesnt document the c#
                    // part of there code look into making your own text entry at  this point, prefarably
                    // in the near future switch to the orginal sloution

                    //Entry.Background = new SolidColorBrush(newColorEntry);
                    PasswordEntry.Foreground = new SolidColorBrush(newColorText);
                }

                if (WrongPasswordTranstion != null &&
                    WrongPasswordTranstion.FunctionRunning == false &&
                    value == 1)
                { // loop around and go back to normal
                    WrongPasswordTranstion.TranslateBackward();
                }
            }
        }


        private void OnEnetryKeyDown(object? sender, KeyEventArgs? e)
        {
            // this will  detect when  a key  is pressed  for the entery
            // this will be used to detect when the enter key is pressed
            // to trigger the Accept function

            if (e == null) return;

            if (e.Key == Key.Enter){
                OnClickCreateButton(null, null);
            }
        }


        public async void OnClickCreateButton(object? sender = null, RoutedEventArgs? e = null){

            if (HostAdressEntry == null || HostAdressEntry.Text == null || HostAdressEntry.Text == "") {
                if (WrongHostnameTranstion != null)
                    WrongHostnameTranstion.TranslateForward();
                return;
            }
            


            Network.Server.Domain = HostAdressEntry.Text;
            if (HostAdressEntry.Text[^1] != '/'){
                HostAdressEntry.Text += "/";
            }
            bool result;

            // check the server is online
            result = await Network.Requester.GetPing();
            if (!result){
                if (WrongHostnameTranstion != null)
                    WrongHostnameTranstion.TranslateForward();
                return;
            }

            if (UsernameEntry == null ||  PasswordEntry == null) return;

            if (UsernameEntry.Text == null || UsernameEntry.Text == "") {
                if (WrongUsernameTranstion != null)
                    WrongUsernameTranstion.TranslateForward();
                return;
            }

            if (PasswordEntry.Text == null || PasswordEntry.Text == ""){
                if (WrongPasswordTranstion != null)
                    WrongPasswordTranstion.TranslateForward();
                return;
            }

            result = await Network.Requester.CreateUser(UsernameEntry.Text, PasswordEntry.Text);
            if (result)
            {
                HideRight();
            }
            else {
                if (WrongHostnameTranstion != null)
                    WrongHostnameTranstion.TranslateForward();
                if (WrongUsernameTranstion != null)
                    WrongUsernameTranstion.TranslateForward();
                if (WrongPasswordTranstion != null)
                    WrongPasswordTranstion.TranslateForward();
            }

        }


    }
}
