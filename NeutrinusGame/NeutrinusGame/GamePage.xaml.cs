using System;
using System.Collections.Generic;
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

        Giocatore giocatore = Giocatore.Nessuno;

        public GamePage()
        {
            InitializeComponent();

            FindFirstPlayer();
        }

        void FindFirstPlayer()
        {
            giocatore = Engine.GetInstance().GetPrimoGiocatore();

            ShowPlayerTurn();
        }

        void ShowPlayerTurn()
        {
            if (giocatore.Equals(Giocatore.GiocatoreBianco))
            {
                LabelGiocatoreBianco.ScaleTo(1.5, 250, Easing.SinOut);
                LabelGiocatoreNero.ScaleTo(1, 250, Easing.SinOut);
            }
                
            else if (giocatore.Equals(Giocatore.GiocatoreNero))
            {
                LabelGiocatoreNero.ScaleTo(1.5, 250, Easing.SinOut);
                LabelGiocatoreBianco.ScaleTo(1, 250, Easing.SinOut);
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
            if(arg1 == neutrinus)
            {
                giocatore = Engine.GetInstance().GetPrimoGiocatore();
                ShowPlayerTurn();
            }
            else
            {
                int x = ((CustomImage)arg1).TableColumn;
                int y = ((CustomImage)arg1).TableRow;
                List<Movimento> movimenti = Engine.GetInstance().MovimentiPossibili(x, y);
                List<Coordinata> coordinate = Engine.GetInstance().TrovaDestinazioni(movimenti, x, y);

                ClearAllCellsTint();
                TintCells(coordinate);
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            
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
                    boxView.BackgroundColor = Color.Chocolate;
                }
            }
        }

        void ClearAllCellsTint()
        {
            foreach (BoxView view in GameGrid.Children)
            {
                view.BackgroundColor = Color.Transparent;
            }
        }

        void MoveImage(object sender, CustomImage image, uint length)
        {
            Point puntoFinale = CalcNewPosition(sender as BoxView, image);

            int row = (int)((BoxView)sender).GetValue(Grid.RowProperty);
            int column = (int)((BoxView)sender).GetValue(Grid.ColumnProperty);

            image.TranslateTo(puntoFinale.X, puntoFinale.Y, length, Easing.SinOut);

            image.TableRow = row;
            image.TableColumn = column;
        }

        Point CalcNewPosition(BoxView sender, Image pedina)
        {
            int centroXcella = Convert.ToInt32(sender.X + (sender.Width / 2));
            int centroYcella = Convert.ToInt32(sender.Y + (sender.Height / 2));

            int coordinataXFinale = Convert.ToInt32(centroXcella - (pedina.Width / 2));
            int coordinataYFinale = Convert.ToInt32(centroYcella - (pedina.Height / 2));

            return new Point(coordinataXFinale, coordinataYFinale);
        }
    }
}