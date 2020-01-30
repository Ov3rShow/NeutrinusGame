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
    public partial class GamePage : ContentPage
    {
        List<Image> whitePawns;
        Image lastImageTouched;
        Point p;
        // ciao
        public GamePage()
        {
            InitializeComponent();

            PrepareWhitePawns();
        }

        void PrepareWhitePawns()
        {
            whitePawns = new List<Image>();

            for(int n = 0; n < 5; n++)
            {
                Image img = new Image();
                img.Source = ImageSource.FromFile("pedina_bianca.png");
                img.Aspect = Aspect.AspectFit;
                img.Margin = new Thickness(5);

                AbsoluteLayout.SetLayoutBounds(img, new Rectangle(0, 0, 75, 75));
                AbsoluteLayout.SetLayoutFlags(img, AbsoluteLayoutFlags.None);

                img.GestureRecognizers.Add(new TapGestureRecognizer(onTap));

                GameAbsLayout.Children.Add(img);

                switch(n)
                {
                    case 0:
                        {
                            moveImage(Cella00, img);
                            break;
                        }
                    case 1:
                        {
                            moveImage(Cella10, img);
                            break;
                        }
                    case 2:
                        {
                            moveImage(Cella20, img);
                            break;
                        }
                    case 3:
                        {
                            moveImage(Cella30, img);
                            break;
                        }
                    case 4:
                        {
                            moveImage(Cella40, img);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                whitePawns.Add(img);
            }
        }

        private void onTap(View arg1, object arg2)
        {
            DisplayAlert("Mona", "Pedina selezionata", "Okay");

            p = new Point();
            p.X = arg1.X;
            p.Y = arg1.Y;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if(lastImageTouched != null)
                moveImage(sender, lastImageTouched);
        }

        void moveImage(object sender, Image image)
        {
            Point puntoFinale = calcNewPosition(sender as BoxView, image);

            image.TranslateTo(puntoFinale.X, puntoFinale.Y, 750, Easing.SinOut);
        }

        Point calcNewPosition(BoxView sender, Image pedina)
        {
            int centroXcella = Convert.ToInt32(sender.X + (sender.Width / 2));
            int centroYcella = Convert.ToInt32(sender.Y + (sender.Height / 2));

            int coordinataXFinale = Convert.ToInt32(centroXcella - (pedina.Width / 2));
            int coordinataYFinale = Convert.ToInt32(centroYcella - (pedina.Height / 2));

            return new Point(coordinataXFinale, coordinataYFinale);
        }
    }
}