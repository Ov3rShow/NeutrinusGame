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
            string g1 = "", g2 = "";
            if (NomeG1.Text == null || NomeG1.Text.Equals(""))
            {
                g1 = "Giocatore 1";
            }
            else
            {
                g1 = NomeG1.Text;
            }
            if (NomeG2.Text == null || NomeG2.Text.Equals(""))
            {
                g2 = "Giocatore 2";
            }
            else
            {
                g2 = NomeG2.Text;
            }
            GamePage gamePage = new GamePage(g1,g2);

            NomeG1.Text = "";
            NomeG2.Text = "";
            popupLoginView.IsVisible = false;

            await Navigation.PushModalAsync(gamePage);

            gamePage.CreateWhitePawns();
            gamePage.PrepareWhitePawns();
            gamePage.CreateBlackPawns();
            gamePage.PrepareBlackPawns();
            gamePage.CreateNeutrinus();


        }

        private void btn_CancelPopup(object sender, EventArgs e)
        {
            NomeG1.Text = "";
            NomeG2.Text = "";
            popupLoginView.IsVisible = false;
        }

        
    }
}