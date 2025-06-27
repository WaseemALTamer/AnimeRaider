using AnimeRaider.UI.Containers;
using Avalonia.Controls;


namespace AnimeRaider.UI.Pages
{
    public class Credentials : Base
    {


        public Connector? Connector;


        public Credentials(Canvas? master) : base(master){

            if (MainCanvas == null) return;

            Connector = new Connector(MainCanvas);
            MainCanvas.Children.Add(Connector);

            if (Master != null){
                Master.SizeChanged += OnSizeChanged;
                OnSizeChanged();
            }

        }


        private void OnSizeChanged(object? sender = null, object? e = null){

            if (MainCanvas != null) {
                if (Connector != null) {
                    Canvas.SetLeft(Connector, (MainCanvas.Width - Connector.Width) /2);
                    Canvas.SetTop(Connector, (MainCanvas.Height - Connector.Height) / 2);
                }
            
            }

            
        }





    }
}
