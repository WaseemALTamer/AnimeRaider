using Avalonia.Interactivity;
using AnimeRaider.Setting;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using System;








namespace AnimeRaider.UI.Containers{


    public class Connector : Border
    {


        private Canvas? Master;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }



        private Image? CredentialsImage;

        private Button? ConnectButton;
        private TextBox? HostAdressEntry;
        private TextBox? UsernameEntry;
        private TextBox? PasswordEntry;


        private Animations.Transations.Uniform? WrongHostnameTranstion;

        public Connector(Canvas? master) {

            Master = master;

            Opacity = 1;
            ClipToBounds = true;
            Background = Themes.Holder;
            CornerRadius = new CornerRadius(Config.CornerRadius);


            MainCanvas = new Canvas{
                ClipToBounds=true,
            };
            Child = MainCanvas;

            ConnectButton = new Button{
                Content = "Connect",
                Width = 150,
                Height = 50,
                Background = Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius)
            };
            MainCanvas.Children.Add(ConnectButton);
            ConnectButton.Click += OnClickConnectButton;


            HostAdressEntry = new TextBox{
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


            WrongHostnameTranstion = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongHostname,
            };

            CredentialsImage = new Image{
                Width = 200, Height = 200,
            };

            MainCanvas.Children.Add(CredentialsImage);

            Assets.AddAwaitedActions(() => {
                CredentialsImage.Source = Assets.CredentialsBitmap;
            });

            OnSizeChanged();

            if (Master != null){
                //Master.SizeChanged += OnSizeChanged;
            }

        }

        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null){

            if (Master == null) return;

            Width = 400;
            Height = 550;


            if (MainCanvas != null){
                MainCanvas.Width = Width;
                MainCanvas.Height = Height;

                if (CredentialsImage != null) {
                    Canvas.SetLeft(CredentialsImage, (MainCanvas.Width - CredentialsImage.Width) / 2);
                    Canvas.SetTop(CredentialsImage,  25);
                }

                if (ConnectButton != null){
                    Canvas.SetLeft(ConnectButton, (MainCanvas.Width - ConnectButton.Width) / 2);
                    Canvas.SetTop(ConnectButton, (MainCanvas.Height - ConnectButton.Height) - 10);
                }


                if (HostAdressEntry != null){
                    Canvas.SetLeft(HostAdressEntry, (MainCanvas.Width - HostAdressEntry.Width) / 2);
                    Canvas.SetTop(HostAdressEntry, 250);
                }

                if (UsernameEntry != null){
                    Canvas.SetLeft(UsernameEntry, (MainCanvas.Width - UsernameEntry.Width) / 2);
                    Canvas.SetTop(UsernameEntry, 320);
                }

                if (PasswordEntry != null){
                    Canvas.SetLeft(PasswordEntry, (MainCanvas.Width - PasswordEntry.Width) / 2);
                    Canvas.SetTop(PasswordEntry, 370);
                }

            }
        }


        private void TriggerWrongHostname(double value){
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


        private void OnEnetryKeyDown(object? sender, KeyEventArgs? e){
            // this will  detect when  a key  is pressed  for the entery
            // this will be used to detect when the enter key is pressed
            // to trigger the Accept function

            if (e == null) return;

            if (e.Key == Key.Enter){
                OnClickConnectButton(null, null); // simulate clicking the accept button
            }
        }


        private async void OnClickConnectButton(object? sender = null, RoutedEventArgs? e = null){
            if (WrongHostnameTranstion == null) return;
            if (HostAdressEntry == null || HostAdressEntry.Text == null) {
                WrongHostnameTranstion.TranslateForward();
                return;
            }

            if (HostAdressEntry.Text[^1] != '/'){
                HostAdressEntry.Text += "/";
            }

            Network.Server.Domain = HostAdressEntry.Text;



            bool result = await Network.Requester.GetPing();
            if (result){
                if(PublicWidgets.Searcher != null)
                    PublicWidgets.Searcher.Show();

                if (PublicWidgets.UIHome != null)
                    PublicWidgets.TransitionForward(PublicWidgets.UIHome);
            }
            else {
                WrongHostnameTranstion.TranslateForward();
            }





        }

    }
}
