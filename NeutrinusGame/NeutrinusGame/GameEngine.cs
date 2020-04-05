using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NeutrinusGame
{
    class GameEngine
    {
        public enum Pedina
        {
            /*l'elemento NeraOBianca serve solo a livello di codice per capire se il giocatore dovrà spostare una pedina normale o 
             il neutrinus, non deve essere mai instanziata nella griglia di gioco del codice qui sotto*/
            Bianca, Nera, Neutrinus, Vuoto, NeraOBianca
        }

        public enum Movimento
        {
            SinistraSu, Su, DestraSu, Destra, DestraGiu, Giu, SinistraGiu, Sinistra
        }

        public struct Coordinata
        {
            public Coordinata(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public enum Giocatore
        {
            //Per convenzione il giocatore 1 è quello con il telefono correttamente rivolto verso di lui
            GiocatoreNero, GiocatoreBianco, Nessuno
        }

        public enum RisultatoTurno
        {
            ProssimoTurnoNeutrinus, ProssimoTurnoPedina, FineGiocoVinceBianco, FineGiocoVinceNero, Errore
        }

        public class Engine
        {
            private readonly int NUM_ROWS = 5;
            private readonly int NUM_COLUMNS = 5;

            private static Engine gameEngine;
            private Pedina[,] matrice;

            private Giocatore ultimoGiocatore = Giocatore.Nessuno;

            private int turniGiocati = 0;

            public static Engine GetInstance()
            {
                if (gameEngine is null)
                {
                    gameEngine = new Engine();
                    gameEngine.NuovaPartita();
                }

                return gameEngine;
            }

            private void NuovaPartita()
            {
                /*Metodo chiamato quando viene giocata una nuova partita, prepara la griglia e i dati*/
                gameEngine.PreparaGriglia();
                ultimoGiocatore = Giocatore.Nessuno;
                turniGiocati = 0;
            }

            private void PreparaGriglia()
            {
                /*La griglia viene preparata con i valori iniziali del gioco: pedine nere nella prima riga, 
                 * pedine bianche nell'ultima riga e al centro il neutrinus*/

                matrice = new Pedina[NUM_ROWS, NUM_COLUMNS];

                for (int x = 0; x < NUM_ROWS; x++)
                {
                    for (int y = 0; y < NUM_COLUMNS; y++)
                    {
                        //prima riga, pedine bianche
                        if (x == 0)
                            matrice[y, x] = Pedina.Bianca;
                        //seconda riga, pedine nere
                        else if (x == 4)
                            matrice[y, x] = Pedina.Nera;
                        //pedina centrale, neutrinus
                        else if (x == 2 && y == 2)
                            matrice[y, x] = Pedina.Neutrinus;
                        //parte vuota del tabellone
                        else
                            matrice[y, x] = Pedina.Vuoto;
                    }
                }
            }

            public List<Movimento> MovimentiPossibili(int x, int y)
            {
                /*Restituisce una lista contenente tutti i movimenti possibili di una pedina specifica.
                 Il controllo dei movimenti che è possibile eseguire viene effettuato verificando la presenza di altre pedine
                 o dei bordi della tabella*/

                List<Movimento> movimenti = new List<Movimento>();
                int suppX, suppY;

                //controllo su sinistra
                suppX = x - 1;
                suppY = y - 1;
                if ((suppX >= 0 && suppY >= 0) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.SinistraSu);

                //controllo su
                suppX = x;
                suppY = y - 1;
                if ((suppX >= 0 && suppY >= 0) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.Su);

                //controllo su destra
                suppX = x + 1;
                suppY = y - 1;
                if ((suppX < 5 && suppY >= 0) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.DestraSu);

                //controllo destra
                suppX = x + 1;
                suppY = y;
                if ((suppX < 5 && suppY >= 0) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.Destra);

                //controllo giu destra
                suppX = x + 1;
                suppY = y + 1;
                if ((suppX < 5 && suppY < 5) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.DestraGiu);

                //controllo giu
                suppX = x;
                suppY = y + 1;
                if ((suppX >= 0 && suppY < 5) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.Giu);

                //controllo giu sinistra
                suppX = x - 1;
                suppY = y + 1;
                if ((suppX >= 0 && suppY < 5) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.SinistraGiu);

                //controllo sinistra
                suppX = x - 1;
                suppY = y;
                if ((suppX >= 0 && suppY >= 0) && matrice[suppX, suppY] == Pedina.Vuoto)
                    movimenti.Add(Movimento.Sinistra);

                return movimenti;
            }

            private Giocatore ProssimoGiocatore()
            {
                /*Restituisce il prossimo giocatore che deve effettuare il turno. Se deve essere selezionato il primo giocatore
                 esso viene scelto casualmente. Altrimenti, viene scelto il giocatore diverso da quello che ha giocato l'ultimo turno*/

                Giocatore prossimoGiocatore;

                if (gameEngine.ultimoGiocatore == Giocatore.Nessuno)
                {
                    Random rand = new Random();
                    if (rand.Next(2) == 0)
                        prossimoGiocatore = Giocatore.GiocatoreNero;
                    else
                        prossimoGiocatore = Giocatore.GiocatoreBianco;
                }
                else if (gameEngine.ultimoGiocatore == Giocatore.GiocatoreNero)
                    prossimoGiocatore = Giocatore.GiocatoreBianco;
                else
                    prossimoGiocatore = Giocatore.GiocatoreNero;

                return prossimoGiocatore;
            }

            public RisultatoTurno EffettuaTurno(ref Giocatore giocatore, int pedinaX, int pedinaY, int pedinaEndX, int pedinaEndY)
            {

                /*Effettua lo spostamento della pedina all'interno della matrice. La pedina viene spostata finchè non incontra
                 un'altra pedina o finchè non viene raggiunto il bordo della pedina. Il risultato restituito indica se il gioco prosegue
                 con un altro turno o se la partita è terminata.*/

                Pedina pedinaSelezionata = matrice[pedinaX, pedinaY];

                //int convertedPedinaX = (int)(pedinaX / 2);
                //int convertedPedinaY = (int)(pedinaY / 2);
                int convertedEndPedinaX = (int)(pedinaEndX / 2);
                int convertedEndPedinaY = (int)(pedinaEndY / 2);

                matrice[convertedEndPedinaX, convertedEndPedinaY] = matrice[pedinaX, pedinaY];
                matrice[pedinaX, pedinaY] = Pedina.Vuoto;
                
                turniGiocati++;

                ultimoGiocatore = giocatore;
                int neutrinusX, neutrinusY;

                FindNeutrinusCoordinates(out neutrinusX, out neutrinusY);

                if (neutrinusX != -1 && neutrinusY != -1)
                {
                    if (neutrinusY == 0)
                        return RisultatoTurno.FineGiocoVinceBianco;
                    else if (neutrinusY == 4)
                        return RisultatoTurno.FineGiocoVinceNero;
                    else if(IsNeutrinusCaged())
                    {
                        if (giocatore == Giocatore.GiocatoreBianco)
                            return RisultatoTurno.FineGiocoVinceBianco;
                        else
                            return RisultatoTurno.FineGiocoVinceNero;
                    }
                    else
                    {
                        if ((pedinaSelezionata == Pedina.Nera || pedinaSelezionata == Pedina.Bianca) || turniGiocati <= 1)
                        {
                            /*Se la partita non è terminata e la pedina mossa è bianca o nera
                             * viene valorizzato il giocatore con il giocatore che dovrà fare la prossima mossa e 
                             * viene specificato che dovrà muovere il neutrinus*/
                            giocatore = ProssimoGiocatore();
                            return RisultatoTurno.ProssimoTurnoNeutrinus;
                        }
                        else
                            /*Se la partita non è terminata e la pedina mossa è il neutrinus, il giocatore rimane lo stesso e 
                             * viene specificato che come prossima mossa deve muvoere una pedina normale*/
                            return RisultatoTurno.ProssimoTurnoPedina;
                       
                    }
                }
                else
                    return RisultatoTurno.Errore;
            }

            public Giocatore GetPrimoGiocatore()
            {
                return ProssimoGiocatore();
            }

            private void FindNeutrinusCoordinates(out int coordX, out int coordY)
            {
                /*Identifica le coordinate del Neutrinus cercando la pedina nella matrice*/

                for (int x = 0; x < NUM_COLUMNS; x++)
                {
                    for (int y = 0; y < NUM_ROWS; y++)
                    {
                        if (matrice[x, y] == Pedina.Neutrinus)
                        {
                            coordX = x;
                            coordY = y;

                            return;
                        }
                    }
                }

                coordX = -1;
                coordY = -1;
            }

            public List<Coordinata> TrovaDestinazioni(List<Movimento> movimenti, int pedinaX, int pedinaY)
            {
                int ultimoXvalido = -1, ultimoYvalido = -1;
                Pedina pedinaSelezionata = matrice[pedinaX, pedinaY];
                List<Coordinata> coordinatePossibili = new List<Coordinata>();

                foreach (Movimento movimento in movimenti)
                {
                    switch (movimento)
                    {
                        case Movimento.SinistraSu:
                            {

                                int y = pedinaY - 1;
                                for (int x = pedinaX - 1; x >= 0; x--)
                                {

                                    if ((x >= 0 && y >= 0) && matrice[x, y] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;

                                    y--;

                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                        case Movimento.Su:
                            {


                                for (int y = pedinaY - 1; y >= 0; y--)
                                {
                                    if (matrice[pedinaX, y] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = pedinaX;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                        case Movimento.DestraSu:
                            {


                                int y = pedinaY - 1;
                                for (int x = pedinaX + 1; x < 5; x++)
                                {
                                    if ((x < 5 && y >= 0) && matrice[x, y] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;

                                    y--;

                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                        case Movimento.Destra:
                            {


                                for (int x = pedinaX + 1; x < 5; x++)
                                {
                                    if (matrice[x, pedinaY] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = pedinaY;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                        case Movimento.DestraGiu:
                            {

                                int y = pedinaY + 1;
                                for (int x = pedinaX + 1; x < 5; x++)
                                {


                                    if ((x < 5 && y < 5) && matrice[x, y] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;

                                    y++;


                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                        case Movimento.Giu:
                            {


                                for (int y = pedinaY + 1; y < 5; y++)
                                {
                                    if (matrice[pedinaX, y] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = pedinaX;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                        case Movimento.SinistraGiu:
                            {


                                int y = pedinaY + 1;
                                for (int x = pedinaX - 1; x >= 0; x--)
                                {

                                    if ((x >= 0 && y < 5) && matrice[x, y] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;
                                    y++;



                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                        case Movimento.Sinistra:
                            {

                                for (int x = pedinaX - 1; x >= 0; x--)
                                {
                                    if (matrice[x, pedinaY] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = pedinaY;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2 + 2, ultimoYvalido * 2 + 2));

                                break;
                            }
                    }
                }

                return coordinatePossibili;
            }

            public bool IsNeutrinusCaged()
            {
                bool caged = false;

                int x, y;

                FindNeutrinusCoordinates(out x, out y);

                if (MovimentiPossibili(x, y).Count == 0)
                    caged = true;

                return caged;
            }

            public void resetGameEngine()
            {
                gameEngine = null;
            }

            public void DebugPrintGrid()
            {
                for (int x = 0; x < NUM_COLUMNS; x++)
                {
                    String row = "";

                    for (int y = 0; y < NUM_ROWS; y++)
                    {
                        if (matrice[y, x] == Pedina.Bianca)
                            row += "B";
                        else if (matrice[y, x] == Pedina.Nera)
                            row += "N";
                        if (matrice[y, x] == Pedina.Vuoto)
                            row += "V";
                        if (matrice[y, x] == Pedina.Neutrinus)
                            row += "X";
                    }

                    Debug.Print(row);
                }
            }

        }
    }
}
