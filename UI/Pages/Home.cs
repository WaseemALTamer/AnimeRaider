using AnimeRaider.UI.Containers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using System;




namespace AnimeRaider.UI.Pages
{
    public class Home : Base
    {

        public List<Poster> Posters = new List<Poster>();


        public double PadY = 20;
        public double PadX = 20;


        public Home(Canvas? master) : base(master){
            if (MainCanvas == null) return;

            Loaded += OnLoaded;

            if (Master != null) {
                Master.SizeChanged += OnSizeChanged;
            }

            if (ScrollViewer != null)
                ScrollViewer.PropertyChanged += OnPropertyChanged;

        }

        private async void OnLoaded(object? sender=null, object? e=null)
        {
            SharedData.Data.RandomSeries = await Network.Requester.GetRandomSeries(12);
            if (SharedData.Data.RandomSeries != null){

                for (int i = 0; i < SharedData.Data.RandomSeries.Count ; i++) {
                    Poster poster = new Poster(MainCanvas, SharedData.Data.RandomSeries[i]);
                    Posters.Add(poster);

                    if (MainCanvas != null){
                        MainCanvas.Children.Add(poster);
                    }

                    SetPosterPos();

                    await Task.Delay(50);

                }
            }
        }


        private void OnSizeChanged(object? sender = null, object? e = null) {
            SetPosterPos();
        }

        private void OnPropertyChanged(object? sender = null, object? e = null) {
            ShowPostersVisaibleOnly();
        }


        private void SetPosterPos() {

            if (MainCanvas == null) return;

            MainCanvas.Height = 0;
            for (int i = 0; i < Posters.Count ; i++){
                
                var poster = Posters[i];
                if (poster != null){
                    double _SpacePerCover = MainCanvas.Width / (poster.Width + PadX);
                    double _ColumnsNum = Math.Floor(_SpacePerCover);
                    double _LeftSpace = (_SpacePerCover - _ColumnsNum) * (poster.Width + PadX);
                    double _RowNum = Math.Floor(i / _ColumnsNum);

                    if (_ColumnsNum == 0){
                        _RowNum = i;
                    }

                    double _extraspace = (_LeftSpace - PadX) / 2;

                    double _PosX = (PadX * (i % _ColumnsNum)) + (poster.Width * (i % _ColumnsNum)) + PadX + _extraspace;
                    double _PosY = (PadY * ((_RowNum) + 1)) + (poster.Height * (_RowNum));


                    poster.SetPostionTranslate(_PosX, _PosY);

                    MainCanvas.Height = _PosY + poster.Height + PadY;
                }
            }


            ShowPostersVisaibleOnly();
        }

        private void ShowPostersVisaibleOnly() {

            if (ScrollViewer == null || MainCanvas == null) return;

            double ScrollViwerYOffset = ScrollViewer.Offset.Y;

            if (Posters == null) return;

            for (int i = 0; i < Posters.Count; i++){

                var Cover = Posters[i];
                if (Cover == null) continue;

                double _LowerBounds = ScrollViwerYOffset - (Cover.Height + PadY);
                double _UpperBounds = ScrollViwerYOffset + Height;

                double _ColumnsNum = Math.Floor(MainCanvas.Width / (Cover.Width + PadX));
                if (_ColumnsNum <= 0) _ColumnsNum = 1;
                double _RowNum = Math.Floor(i / _ColumnsNum);



                double _PosY = (PadY * ((_RowNum) + 1)) + (Cover.Height * (_RowNum));



                if (_PosY >= _LowerBounds && _PosY <= _UpperBounds){
                    Posters[i].IsVisible = true;
                    Cover.Show();
                }
                else{
                    Posters[i].IsVisible = false;
                }
            }

        }



    }
}
