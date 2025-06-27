using Avalonia.Controls;
using Avalonia;
using AnimeRaider.UI.Pages;
using System.Collections.Generic;
using AnimeRaider.Setting;
using System.Threading.Tasks;
using AnimeRaider.UI.Containers;
using AnimeRaider.Structures;
using System;





namespace AnimeRaider.UI
{
    public static class PublicWidgets{

        // This will hold all the Big  Widgets togather mostly Canvases
        // This file will also contain data that can be used all around
        // this also keeps track of the history of what was on screen


        // one rule that must be kept for structure  perposes  and  must
        // must not be broken is UI  classes  can  connect  back  to the
        // PublicWidgets class but the  backend  like  ConnectionMangaer
        // should not connect back to here rather the UI  should  map  a
        // function to it instead

        // We use a singlton methode to create the Objects instance this
        // allows us to access it using its public pointer



        // <ASSETS>

        private static BackButton? _BackButton;
        public static BackButton? BackButton
        {
            get { return _BackButton; }
            set { _BackButton = value; }
        }


        private static Searcher? _Searcher;
        public static Searcher? Searcher
        {
            get { return _Searcher; }
            set { _Searcher = value; }
        }




        // <PAGES>

        private static Pages.Home? _UIHome;
        public static Pages.Home? UIHome{
            get { return _UIHome; }
            set { _UIHome = value; }
        }


        private static Pages.Series? _UISeries;
        public static Pages.Series? UISeries{
            get { return _UISeries; }
            set { _UISeries = value; }
        }


        private static Pages.Search? _UISearch;
        public static Pages.Search? UISearch{
            get { return _UISearch; }
            set { _UISearch = value; }
        }




        // <POPUPS>



        private static List<Pages.Base> Pages = new List<Pages.Base>(); // a list of all the holders this is legacy code not used for recent code
        private static List<Pages.Base> PagesHistory = new List<Pages.Base>(); // this will be used with the back button to go back from where we came from

        public static Pages.Base? DisplayedPage;









        public static void Initialize(AvaloniaObject master) {
            var Master = master as Canvas;
            if (Master == null) return; // redundency if statment


            // We create all the instances of all our objects here
            // Objects will  connect them selfs  togather the back
            // button is something different


            // <ASSETS START>

            // this creates and attach the function to the back button
            BackButton = new BackButton(Master);
            Master.Children.Add(BackButton);
            BackButton.Trigger = BackButtonFunction;


            Searcher = new Searcher(Master);
            Master.Children.Add(Searcher);




            // <ASSETS END>



            // <PAGES START>

            // this creates the Home Page
            UIHome = new Pages.Home(Master);
            Pages.Add(UIHome);

            // this creates the Series Page
            UISeries = new Pages.Series(Master);
            Pages.Add(UISeries);

            // this creates the Series Page
            UISearch = new Pages.Search(Master);
            Pages.Add(UISearch);


            // <PAGES END>


            // <IN_POPUPS START>



            // <IN_POPUPS END>



            // <SETUP START>

            BackButton.Show();
            Searcher.Show();
            TransitionForward(UIHome);
            
            // <SETUP END>
        }





        private static bool _TransitionForwardRunning = false; // for thread safty
        public static async void TransitionForward(Pages.Base Page)
        {
            if (_TransitionForwardRunning) return;
            _TransitionForwardRunning = true;
            if (DisplayedPage != null)
            {
                PagesHistory.Add(DisplayedPage);
                DisplayedPage.Hide();
                DisplayedPage = Page; // redundency
                await Task.Delay(Config.TransitionDuration / 2); // wait for at bit to give ot a smoother feel
            }
            DisplayedPage = Page;
            Page.Show();
            _TransitionForwardRunning = false;
        }

        private static bool _TransitionBackRunning = false; // for thread safty
        public static async void TransitionBack(){
            if (_TransitionBackRunning) return;
            _TransitionBackRunning = true;
            if (PagesHistory.Count > 0 &&
                DisplayedPage != null)
            {
                var lastPage = PagesHistory[PagesHistory.Count - 1];
                PagesHistory.RemoveAt(PagesHistory.Count - 1);


                DisplayedPage.Hide();
                DisplayedPage = lastPage;
                await Task.Delay(Config.TransitionDuration / 2);
                DisplayedPage.Show();
            }
            _TransitionBackRunning = false;
        }




        public static void BackButtonFunction(){
            TransitionBack(); // Transision backward
        }


    }
}
