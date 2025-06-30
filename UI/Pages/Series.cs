using AnimeRaider.UI.Containers;
using Avalonia.Controls;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using AnimeRaider.Setting;
using Avalonia;




namespace AnimeRaider.UI.Pages
{
    public class Series : Base
    {


        public List<Thumbnail> Thumbnails = new List<Thumbnail>();

        public Border? ThumbnailsBorder;
        public Canvas? ThumbnailsCanvas;

        public double PadY = 20;
        public double PadX = 20;

        private Border? CoverBorder;
        private Image? CoverImage;

        public Series(Canvas? master) : base(master){
            OnShow += Update;


            if (MainCanvas != null) {
                CoverBorder = new Border{
                    Width = 378, Height = 567,
                    CornerRadius = new CornerRadius(Config.CornerRadius),
                    ClipToBounds = true,
                };

                MainCanvas.Children.Add(CoverBorder);

                CoverImage = new Image{
                    Stretch = Avalonia.Media.Stretch.Uniform
                };
                CoverBorder.Child = CoverImage;

                ThumbnailsBorder = new Border{
                    CornerRadius = new CornerRadius(Config.CornerRadius),
                    Background = Themes.Holder,
                    ClipToBounds = true,
                };

                MainCanvas.Children.Add(ThumbnailsBorder);


                ThumbnailsCanvas = new Canvas{


                };

                ThumbnailsBorder.Child = ThumbnailsCanvas;



            }





            if (Master != null) {
                Master.SizeChanged += OnSizeChanged;
                OnSizeChanged();
            }

            if (ScrollViewer != null)
                ScrollViewer.PropertyChanged += OnPropertyChanged;

        }



        private async void Update(){

            if (CoverImage != null) {
                CoverImage.Source = SharedData.TargetedData.CoverImage;
            }

            RemoveAllPosters();

            if (SharedData.TargetedData.Series == null) return;
            var series = SharedData.TargetedData.Series;

            if (series.Episodes != null){
                for (int i = 0; i < series.Episodes.Count; i++){
                    Thumbnail thumbnail = new Thumbnail(ThumbnailsCanvas, series.Episodes[i]);
                    Thumbnails.Add(thumbnail);

                    if (ThumbnailsCanvas != null){
                        ThumbnailsCanvas.Children.Add(thumbnail);
                    }
                    SetThumbnailsPos();
                    await Task.Delay(50);

                }
            }
        }


        public void RemoveAllPosters(){
            if (MainCanvas == null) return;

            for (int i = 0; i < Thumbnails.Count; i++){
                var poster = Thumbnails[i];
                poster.Kill();
                MainCanvas.Children.Remove(poster);
            }

            Thumbnails.Clear();
            GC.Collect();
        }


        private void OnSizeChanged(object? sender = null, object? e = null){

            if (MainCanvas != null) {

                if (CoverBorder != null) {
                    Canvas.SetLeft(CoverBorder, (MainCanvas.Width - CoverBorder.Width) / 2 );
                    Canvas.SetTop(CoverBorder, 25);
                }

                if (ThumbnailsBorder != null) {

                    ThumbnailsBorder.Width = MainCanvas.Width - 100;

                    if (CoverBorder != null) {
                        Canvas.SetTop(ThumbnailsBorder, CoverBorder.Height + 100);
                    }

                    Canvas.SetLeft(ThumbnailsBorder, (MainCanvas.Width - ThumbnailsBorder.Width) / 2);


                    if (ThumbnailsCanvas != null){
                        ThumbnailsCanvas.Width = ThumbnailsBorder.Width;
                        ThumbnailsCanvas.Height = ThumbnailsBorder.Height;
                    }

                }



                

            }


            SetThumbnailsPos();
        }

        private void OnPropertyChanged(object? sender = null, object? e = null){
            ShowThumbnailsVisaibleOnly();
        }


        private void SetThumbnailsPos(){
            if (MainCanvas == null) return;
            if (ThumbnailsBorder == null) return;
            if (ThumbnailsCanvas == null) return;


            ThumbnailsCanvas.Height = 0;
            for (int i = 0; i < Thumbnails.Count; i++){
                var thumbnail = Thumbnails[i];
                if (thumbnail != null){

                    double _SpacePerCover = ThumbnailsCanvas.Width / (thumbnail.Width + PadX);
                    double _ColumnsNum = Math.Floor(_SpacePerCover);
                    double _LeftSpace = (_SpacePerCover - _ColumnsNum) * (thumbnail.Width + PadX);
                    double _RowNum = Math.Floor(i / _ColumnsNum);
                    double _extraspace = (_LeftSpace - PadX) / 2;

                    double _PosX = (PadX * (i % _ColumnsNum)) + (thumbnail.Width * (i % _ColumnsNum)) + PadX + _extraspace;
                    double _PosY = (PadY * ((_RowNum) + 1)) + (thumbnail.Height * (_RowNum));



                    thumbnail.SetPostionTranslate(_PosX, _PosY);

                    _PosY += thumbnail.Height + PadY;
                    ThumbnailsBorder.Height = _PosY;
                    ThumbnailsCanvas.Height = _PosY;
                }
            }

            MainCanvas.Height = Canvas.GetTop(ThumbnailsBorder) + ThumbnailsCanvas.Height + 50;


            ShowThumbnailsVisaibleOnly();
        }

        private void ShowThumbnailsVisaibleOnly(){
            if (ScrollViewer == null) return;
            if (ThumbnailsBorder == null) return;
            if (ThumbnailsCanvas  == null) return;

            double ScrollViwerYOffset = ScrollViewer.Offset.Y;

            for (int i = 0; i < Thumbnails.Count; i++){
                var thumbnail = Thumbnails[i];


                double _LowerBounds = ScrollViwerYOffset - thumbnail.Height;
                double _UpperBounds = ScrollViwerYOffset + Height;



                double _SpacePerCover = ThumbnailsCanvas.Width / (thumbnail.Width + PadX);
                double _ColumnsNum = Math.Floor(_SpacePerCover);
                double _LeftSpace = (_SpacePerCover - _ColumnsNum) * (thumbnail.Width + PadX);
                double _RowNum = Math.Floor(i / _ColumnsNum);
                double _extraspace = (_LeftSpace - PadX) / 2;

                double _PosX = (PadX * (i % _ColumnsNum)) + (thumbnail.Width * (i % _ColumnsNum)) + PadX + _extraspace;
                double _PosY = (PadY * ((_RowNum) + 1)) + (thumbnail.Height * (_RowNum));



                double Ypos = _PosY + Canvas.GetTop(ThumbnailsBorder);
                if (Ypos > _LowerBounds && Ypos < _UpperBounds) {
                    thumbnail.IsVisible = true;
                    thumbnail.Show();
                }
                else {
                    thumbnail.IsVisible = false;
                }
                
            }
        }


    }
}
