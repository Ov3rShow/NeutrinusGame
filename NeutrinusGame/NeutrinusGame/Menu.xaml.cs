using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeutrinusGame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu : ContentPage
    {
        public Menu()
        {
            InitializeComponent();
        }

        private async void btn_newGame(object sender, EventArgs e)
        {
            GamePage gamePage = new GamePage();

            await Navigation.PushModalAsync(gamePage);

            gamePage.CreateWhitePawns();
            gamePage.PrepareWhitePawns();
            gamePage.CreateBlackPawns();
            gamePage.PrepareBlackPawns();
            gamePage.CreateNeutrinus();
        }
    }
}