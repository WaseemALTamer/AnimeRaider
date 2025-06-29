using AnimeRaider.Setting;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using System;






namespace AnimeRaider.UI.Containers.Common
{
    internal class Status : Border
    {

        private Animations.Transations.Uniform? ColorTransation;
        

        public Status() {
            CornerRadius = new Avalonia.CornerRadius(1000);
            Width = 25;
            Height = 25;
            Opacity = 0.7;

            Background = new SolidColorBrush(Color.FromUInt32(0x00000000)); // we start it with the invisable color

        }


        private IBrush? _startingColor;
        private IBrush? _targetColor;
        public void SetColor(IBrush newColor) {

            _startingColor = Background;
            _targetColor = newColor;

            if (ColorTransation != null && 
                ColorTransation.FunctionRunning == true) 
            {
                ColorTransation.Reset();
            }

            ColorTransation = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = ColorTransitionTrigger
            };

            ColorTransation.TranslateForward();

        }


        private void ColorTransitionTrigger(double value){

            var startBrush = _startingColor as SolidColorBrush;
            var targetBrush = _targetColor as SolidColorBrush;

            byte Lerp(byte start, byte end) => (byte)(start + (end - start) * value);

            if (startBrush == null || targetBrush == null)
                return;

            var newColor = Color.FromArgb(
                Lerp(startBrush.Color.A, targetBrush.Color.A),
                Lerp(startBrush.Color.R, targetBrush.Color.R),
                Lerp(startBrush.Color.G, targetBrush.Color.G),
                Lerp(startBrush.Color.B, targetBrush.Color.B)
            );

            Background = new SolidColorBrush(newColor);
        }


        public void Reset() {
            SetColor(new SolidColorBrush(Color.FromUInt32(0x00000000)));
        }


    }
}
