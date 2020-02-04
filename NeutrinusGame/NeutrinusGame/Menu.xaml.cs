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

        private void btn_newGame(object sender, EventArgs e)
        {
            popupLoginView.IsVisible = true;
        }

        private void btn_quit(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }

        private void btn_settings(object sender, EventArgs e)
        {

        }

        private async void btn_OkPopup(object sender, EventArgs e)
        {
            popupLoginView.IsVisible = false;
            string g1= NomeG1.Text, g2= NomeG2.Text;
            // se vuoto va in eccezzione why?

            if (g1.Equals(null) || g1.Equals("")){
                g1 = "Giocatore 1";
            }
            if (g2.Equals(null) || g2.Equals(""))
            {
                g2 = "Giocatore 2";
            }

            GamePage gamePage = new GamePage();

            gamePage.setPlayersName(g1, g2);

            gamePage.CreateWhitePawns();
            gamePage.PrepareWhitePawns();
            gamePage.CreateBlackPawns();
            gamePage.PrepareBlackPawns();
            gamePage.CreateNeutrinus();

            await Navigation.PushModalAsync(gamePage);

        }
    }
}