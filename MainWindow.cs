using Avalonia.Interactivity;
using System.Threading.Tasks;
using AnimeRaider.Setting;
using Avalonia.Controls;
using AnimeRaider.UI;
using System;
using AnimeRaider.Structures;



namespace AnimeRaider
{
    class MainWindow : Window{
        
        public Canvas? MainCanvas;

        public MainWindow(){
            Icon = Assets.Icone;
            Title = Config.ApplicationName;


            MinWidth = 912;
            MinHeight = 513;


            Loaded += OnLoaded;
            Closing += OnClickExit;

        }

        async public void OnLoaded(object? sender, RoutedEventArgs? e){

            // we make sure that we got the Width and Height as they are nessasary for the next parts of the  code
            // i cant relie on the Avalonia.Net to give me the right values as soon as possible because it runs it
            // asynchronously and it might run my code before which would create problems for me as i am  reliying 
            // on there code to run first
            while (double.IsNaN(Width) || double.IsNaN(Height)) await Task.Delay(1);



            MainCanvas = new Canvas
            { // now we create the canvase after we load up for preformece
                Width = Width,
                Height = Height,
                Background = Themes.Backgruond,
                Focusable = true,
            };


            // this is the part of the code that handels all the other  Canvasue on screen
            // note that this file is for desktop only but this part can be shared acrross
            // probably based on the Avalonia.Net support for other devices in terms of UI

            PublicWidgets.Initialize(MainCanvas);


            Resized += OnResize;
            Content = MainCanvas;
        }


        public void OnResize(object? sender, WindowResizedEventArgs? e){
            if (MainCanvas != null){
                MainCanvas.Width = Width;
                MainCanvas.Height = Height;
            }
        }


        private void OnClickExit(object? sender, object? e) {

            Credentials credentials = new Credentials { 
                Domain = Network.Server.Domain,
                Username = SharedData.Data.Username,
                Password = SharedData.Data.Password,
            };

            Appdata.SaveConfig(credentials);

            Environment.Exit(0);
        }
    }
}