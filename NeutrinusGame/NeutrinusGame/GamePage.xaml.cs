using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static NeutrinusGame.GameEngine;

namespace NeutrinusGame
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        List<CustomImage> whitePawns, blackPawns;
        CustomImage neutrinus;
        CustomImage lastImageTouched = null;

        Giocatore player = Giocatore.Nessuno;

        Color possibleMoveColor = Color.Coral;

        string nomePedinaBianca, nomePerdinaNera;

        Pedina whatToMoveNext = Pedina.NeraOBianca;

        public GamePage(string nomeBianco, string nomeNero)
        {
            InitializeComponent();
            nomePedinaBianca = nomeBianco;
            nomePerdinaNera = nomeNero;
            FindFirstPlayer();

            GameGrid.RowSpacing = 0;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {

                bool answer = await this.DisplayAlert("Uscire?", "Sicuro di voler uscire dal gioco?", "Si", "Annulla");
                if (answer)
                {
                    await Navigation.PopModalAsync();
                    await Navigation.PushModalAsync(new Menu());
                }

            });

            return true;
           
        }

        void FindFirstPlayer()
        {
            player = Engine.GetInstance().GetPrimoGiocatore();

            ShowPlayerTurn(whatToMoveNext);
        }

        void ShowPlayerTurn(Pedina type)
        {
            String msg = "";

            if (type == Pedina.Neutrinus)
                msg = "Neutrinus";
            else if (type == Pedina.NeraOBianca)
                msg = "Pedina";

            if (player.Equals(Giocatore.GiocatoreBianco))
            {
                LabelGiocatoreBianco.ScaleTo(1.5, 250, Easing.SinOut);
                LabelGiocatoreBianco.Text= nomePedinaBianca + " tocca a te!\n" + msg;
                LabelGiocatoreNero.ScaleTo(1, 250, Easing.SinOut);
                LabelGiocatoreNero.Text = nomePerdinaNera;
            }
                
            else if (player.Equals(Giocatore.GiocatoreNero))
            {
                LabelGiocatoreNero.ScaleTo(1.5, 250, Easing.SinOut);
                LabelGiocatoreNero.Text= nomePerdinaNera + " tocca a te!\n" + msg;
                LabelGiocatoreBianco.ScaleTo(1, 250, Easing.SinOut);
                LabelGiocatoreBianco.Text = nomePedinaBianca;
            }
        }

        public void CreateWhitePawns()
        {
            whitePawns = new List<CustomImage>();

            for(int n = 0; n < 5; n++)
            {
                CustomImage img = new CustomImage();
                img.Source = ImageSource.FromFile("pedina_bianca.png");
                img.Aspect = Aspect.AspectFit;
                img.Margin = new Thickness(5);

                AbsoluteLayout.SetLayoutBounds(img, new Rectangle(0, 0, 75, 75));
                AbsoluteLayout.SetLayoutFlags(img, AbsoluteLayoutFlags.None);

                img.GestureRecognizers.Add(new TapGestureRecognizer(onTap));

                GameAbsLayout.Children.Add(img);

                whitePawns.Add(img);
            }
        }

        public void CreateBlackPawns()
        {
            blackPawns = new List<CustomImage>();

            for (int n = 0; n < 5; n++)
            {
                CustomImage img = new CustomImage();
                img.Source = ImageSource.FromFile("pedina_nera.png");
                img.Aspect = Aspect.AspectFit;
                img.Margin = new Thickness(5);

                AbsoluteLayout.SetLayoutBounds(img, new Rectangle(0, 0, 75, 75));
                AbsoluteLayout.SetLayoutFlags(img, AbsoluteLayoutFlags.None);

                img.GestureRecognizers.Add(new TapGestureRecognizer(onTap));

                GameAbsLayout.Children.Add(img);

                blackPawns.Add(img);
            }
        }

        public void PrepareWhitePawns()
        {
            MoveImage(Cella00, whitePawns[0], 0);
            MoveImage(Cella10, whitePawns[1], 0);
            MoveImage(Cella20, whitePawns[2], 0);
            MoveImage(Cella30, whitePawns[3], 0);
            MoveImage(Cella40, whitePawns[4], 0);
        }

        public void PrepareBlackPawns()
        {
            MoveImage(Cella04, blackPawns[0], 0);
            MoveImage(Cella14, blackPawns[1], 0);
            MoveImage(Cella24, blackPawns[2], 0);
            MoveImage(Cella34, blackPawns[3], 0);
            MoveImage(Cella44, blackPawns[4], 0);
        }

        public void CreateNeutrinus()
        {
            neutrinus = new CustomImage();
            neutrinus.Source = ImageSource.FromFile("neutrinus.png");
            neutrinus.Aspect = Aspect.AspectFit;
            neutrinus.Margin = new Thickness(5);

            AbsoluteLayout.SetLayoutBounds(neutrinus, new Rectangle(0, 0, 75, 75));
            AbsoluteLayout.SetLayoutFlags(neutrinus, AbsoluteLayoutFlags.None);

            neutrinus.GestureRecognizers.Add(new TapGestureRecognizer(onTap));

            GameAbsLayout.Children.Add(neutrinus);

            MoveImage(Cella22, neutrinus, 0);
        }

        private void onTap(View arg1, object arg2)
        {
            if(lastImageTouched != (CustomImage)arg1)
            {
                if (!checkIfPawnIsCorrectForTheTurn((CustomImage)arg1))
                {
                    //shakeAnimation(arg1);
                    return;
                }
                if(lastImageTouched != null)
                    lastImageTouched.ScaleTo(1, 50, Easing.SinOut);

                lastImageTouched = (CustomImage)arg1;
                lastImageTouched.ScaleTo(1.2, 50, Easing.SinOut);

                int x = ((CustomImage)arg1).TableColumn -1;
                int y = ((CustomImage)arg1).TableRow - 1;
                List<Movimento> movimenti = Engine.GetInstance().MovimentiPossibili(x, y);
                List<Coordinata> coordinate = Engine.GetInstance().TrovaDestinazioni(movimenti, x, y);

                ClearAllCellsTint();
                TintCells(coordinate);
            }
            else
            {
                lastImageTouched.ScaleTo(1, 50, Easing.SinOut);
                lastImageTouched = null;
                ClearAllCellsTint();
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            BoxView boxViewClicked = (BoxView)sender;
            if(boxViewClicked.BackgroundColor == possibleMoveColor)
            {
                ClearAllCellsTint();
                //se risolo si può togliere il commento
                /** Funziona il turno però non so come capire il movimento che fa e su "EffettuaTurno" è sbagliato il controllo della pedina, controlla
                  la posizione in cui c'è la pedina corrente e va sempre in errore (fixed su giù y+1)
                RisultatoTurno risultatoTurno = Engine.GetInstance().EffettuaTurno(ref player,lastImageTouched.TableColumn,lastImageTouched.TableRow, Movimento.Giu);
                MoveImage(sender, lastImageTouched, 1000);
                lastImageTouched.ScaleTo(1, 50, Easing.SinOut);

                if (risultatoTurno == RisultatoTurno.ProssimoTurnoPedina)
                {
                    ShowPlayerTurn("Pedina");

                }else if(risultatoTurno == RisultatoTurno.ProssimoTurnoNeutrinus)
                {
                    ShowPlayerTurn("Neutrinus");
                } **/

                if(lastImageTouched != null)
                {
                    int destinationRow = (int)boxViewClicked.GetValue(Grid.RowProperty) -1;
                    int destinationColumn = (int)boxViewClicked.GetValue(Grid.ColumnProperty)-1;

                    RisultatoTurno risultatoTurno = Engine.GetInstance().EffettuaTurno(ref player, lastImageTouched.TableColumn-1, lastImageTouched.TableRow-1, destinationColumn, destinationRow);

                    MoveImage(boxViewClicked, lastImageTouched, 300);
                    lastImageTouched.ScaleTo(1);

                    if (risultatoTurno == RisultatoTurno.ProssimoTurnoNeutrinus)
                    {
                        whatToMoveNext = Pedina.Neutrinus;
                        ShowPlayerTurn(whatToMoveNext);
                        whatToMoveNext = Pedina.Neutrinus;
                    }
                    else if(risultatoTurno == RisultatoTurno.ProssimoTurnoPedina)
                    {
                        whatToMoveNext = Pedina.NeraOBianca;
                        ShowPlayerTurn(whatToMoveNext);
                        whatToMoveNext = Pedina.NeraOBianca;
                    }
                    else if(risultatoTurno == RisultatoTurno.FineGiocoVinceBianco)
                    {
                        LabelGiocatoreBianco.Text = "Hai vinto!";
                        LabelGiocatoreBianco.ScaleTo(1.5, 250, Easing.SinOut);
                        LabelGiocatoreNero.ScaleTo(1, 250, Easing.SinOut);
                        LabelGiocatoreNero.Text = "Hai perso!";
                    }
                    else if(risultatoTurno == RisultatoTurno.FineGiocoVinceNero)
                    {
                        LabelGiocatoreNero.Text = "Hai vinto!";
                        LabelGiocatoreNero.ScaleTo(1.5, 250, Easing.SinOut);
                        LabelGiocatoreBianco.ScaleTo(1, 250, Easing.SinOut);
                        LabelGiocatoreBianco.Text = "Hai perso!";
                    }

                    lastImageTouched = null;
                }

            }
           
           
        }

        void TintCells(List<Coordinata> coordinates)
        {
            foreach(Coordinata coord in coordinates)
            {
                View boxView = (from cell in GameGrid.Children
                               where Grid.GetColumn(cell) == coord.X && Grid.GetRow(cell) == coord.Y
                               select cell).FirstOrDefault();

                if(boxView != null)
                {
                    boxView.BackgroundColor = possibleMoveColor;
                }
            }
        }

        void ClearAllCellsTint()
        {
            foreach (var view in GameGrid.Children)
            {
                if (view is BoxView)
                {

                    if (view.BackgroundColor == possibleMoveColor)
                        view.BackgroundColor = Color.Transparent;
                }

                
            }
        }

        void MoveImage(object sender, CustomImage image, uint length)
        {
            Point puntoFinale = CalcNewPosition(sender as BoxView, image);

            int row = (int)((BoxView)sender).GetValue(Grid.RowProperty);
            int column = (int)((BoxView)sender).GetValue(Grid.ColumnProperty);

            image.TranslateTo(puntoFinale.X, puntoFinale.Y, length, Easing.SinOut);

            image.TableRow = row / 2;
            image.TableColumn = column / 2;
        }

        Point CalcNewPosition(BoxView sender, Image pedina)
        {
            int centroXcella = Convert.ToInt32(sender.X + (sender.Width / 2));
            int centroYcella = Convert.ToInt32(sender.Y + (sender.Height / 2));

            int coordinataXFinale = Convert.ToInt32(centroXcella - (pedina.Width / 2));
            int coordinataYFinale = Convert.ToInt32(centroYcella - (pedina.Height / 2));

            return new Point(coordinataXFinale, coordinataYFinale);
        }

        bool checkIfPawnIsCorrectForTheTurn(CustomImage pawn)
        {
            if(whatToMoveNext == Pedina.Neutrinus)
            {
                if (pawn == neutrinus)
                    return true;
            }
            else
            {
                switch (player)
                {
                    case Giocatore.GiocatoreBianco:
                        {
                            if (whitePawns.Contains(pawn))
                                return true;

                            break;
                        }
                    case Giocatore.GiocatoreNero:
                        {
                            if (blackPawns.Contains(pawn))
                                return true;

                            break;
                        }
                    default:
                        { break; }
                }
            }
            
            return false;
        }

        async void shakeAnimation(View view)
        {
            //non so perchè ma sposta la pedina a cazzo in giro
            //https://github.com/trailheadtechnology/FancyAnimations/blob/master/FacncyAnimations/FacncyAnimations/ShakePage.xaml.cs
        }
    }
}