using System;
using System.Collections.Generic;
using System.Text;

namespace NeutrinusGame
{
    class GameEngine
    {
        public enum Pedina
        {
            Bianca, Nera, Neutrinus, Vuoto
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
                        //prima riga, pedine nere
                        if (x == 0)
                            matrice[y, x] = Pedina.Nera;
                        //seconda riga, pedine bianche
                        else if (x == 4)
                            matrice[y, x] = Pedina.Bianca;
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
                if ((suppX >= 0 && suppY >= 0) && matrice[suppY, suppX] == Pedina.Vuoto)
                    movimenti.Add(Movimento.SinistraSu);

                //controllo su
                suppX = x;
                suppY = y - 1;
                if ((suppX >= 0 && suppY >= 0) && matrice[suppY, suppX] == Pedina.Vuoto)
                    movimenti.Add(Movimento.Su);

                //controllo su destra
                suppX = x + 1;
                suppY = y - 1;
                if ((suppX < 5 && suppY >= 0) && matrice[suppY, suppX] == Pedina.Vuoto)
                    movimenti.Add(Movimento.DestraSu);

                //controllo destra
                suppX = x + 1;
                suppY = y;
                if ((suppX < 5 && suppY >= 0) && matrice[suppY, suppX] == Pedina.Vuoto)
                    movimenti.Add(Movimento.Destra);

                //controllo giu destra
                suppX = x + 1;
                suppY = y + 1;
                if ((suppX < 5 && suppY < 5) && matrice[suppY, suppX] == Pedina.Vuoto)
                    movimenti.Add(Movimento.DestraGiu);

                //controllo giu
                suppX = x;
                suppY = y + 1;
                if ((suppX >= 0 && suppY < 5) && matrice[suppY, suppX] == Pedina.Vuoto)
                    movimenti.Add(Movimento.Giu);

                //controllo giu sinistra
                suppX = x - 1;
                suppY = y + 1;
                if ((suppX >= 0 && suppY < 5) && matrice[suppY, suppX] == Pedina.Vuoto)
                    movimenti.Add(Movimento.SinistraGiu);

                //controllo sinistra
                suppX = x - 1;
                suppY = y;
                if ((suppX >= 0 && suppY >= 0) && matrice[suppY, suppX] == Pedina.Vuoto)
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

            public RisultatoTurno EffettuaTurno(ref Giocatore giocatore, int pedinaX, int pedinaY, Movimento movimento)
            {

                /*Effettua lo spostamento della pedina all'interno della matrice. La pedina viene spostata finchè non incontra
                 un'altra pedina o finchè non viene raggiunto il bordo della pedina. Il risultato restituito indica se il gioco prosegue
                 con un altro turno o se la partita è terminata.*/

                bool cicloInterrotto = false;
                int ultimoXvalido = -1, ultimoYvalido = -1;
                Pedina pedinaSelezionata = matrice[pedinaX, pedinaY];

                switch (movimento)
                {
                    case Movimento.SinistraSu:
                        {
                            for (int x = pedinaX; x >= 0; x--)
                            {
                                if (cicloInterrotto)
                                    break;

                                for (int y = pedinaY; y >= 0; y--)
                                {
                                    if (matrice[y, x] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                    {
                                        cicloInterrotto = true;
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    case Movimento.Su:
                        {
                            for (int y = pedinaY; y >= 0; y--)
                            {
                                if (matrice[y, pedinaX] == Pedina.Vuoto)
                                {
                                    ultimoXvalido = pedinaX;
                                    ultimoYvalido = y;
                                }
                                else
                                    break;
                            }

                            break;
                        }
                    case Movimento.DestraSu:
                        {
                            for (int x = pedinaX; x < 5; x++)
                            {
                                if (cicloInterrotto)
                                    break;

                                for (int y = pedinaY; y >= 0; y--)
                                {
                                    if (matrice[y, x] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                    {
                                        cicloInterrotto = true;
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    case Movimento.Destra:
                        {
                            for (int x = pedinaX; x < 5; x++)
                            {
                                if (matrice[pedinaY, x] == Pedina.Vuoto)
                                {
                                    ultimoXvalido = x;
                                    ultimoYvalido = pedinaY;
                                }
                                else
                                    break;
                            }

                            break;
                        }
                    case Movimento.DestraGiu:
                        {
                            for (int x = pedinaX; x < 5; x++)
                            {
                                if (cicloInterrotto)
                                    break;

                                for (int y = pedinaY; y < 5; y++)
                                {
                                    if (matrice[y, x] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                    {
                                        cicloInterrotto = true;
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    case Movimento.Giu:
                        {
                            for (int y = pedinaY; y < 5; y++)
                            {
                                if (matrice[y, pedinaX] == Pedina.Vuoto)
                                {
                                    ultimoXvalido = pedinaX;
                                    ultimoYvalido = y;
                                }
                                else
                                    break;
                            }

                            break;
                        }
                    case Movimento.SinistraGiu:
                        {
                            for (int x = pedinaX; x >= 0; x--)
                            {
                                if (cicloInterrotto)
                                    break;

                                for (int y = pedinaY; y < 5; y++)
                                {
                                    if (matrice[y, x] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = y;
                                    }
                                    else
                                    {
                                        cicloInterrotto = true;
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    case Movimento.Sinistra:
                        {
                            for (int x = pedinaX; x >= 0; x--)
                            {
                                if (matrice[pedinaY, x] == Pedina.Vuoto)
                                {
                                    ultimoXvalido = x;
                                    ultimoYvalido = pedinaY;
                                }
                                else
                                    break;
                            }

                            break;
                        }
                }

                turniGiocati++;

                //Se viene trovata una coordinata valida per spostare la pedina, effettuo lo spostamento
                if (ultimoXvalido != -1 && ultimoYvalido != -1)
                {
                    ultimoGiocatore = giocatore;

                    //Spostamento della pedina
                    Pedina tempPedina = matrice[pedinaX, pedinaY];
                    matrice[pedinaX, pedinaY] = Pedina.Vuoto;
                    matrice[ultimoXvalido, ultimoYvalido] = tempPedina;

                    //Trovo le coordinate del neutrinus per identificare se la partita è stata vinta con l'ultima mossa eseguita
                    //TODO: gestire anche gli altri casi di vittoria
                    int neutrinusX, neutrinusY;
                    FindNeutrinusCoordinates(out neutrinusX, out neutrinusY);
                    if (neutrinusX != -1 && neutrinusY != -1)
                    {
                        if (neutrinusX == 0)
                            return RisultatoTurno.FineGiocoVinceNero;
                        else if (neutrinusY == 4)
                            return RisultatoTurno.FineGiocoVinceBianco;
                        else
                        {
                            if ((pedinaSelezionata == Pedina.Nera || pedinaSelezionata == Pedina.Bianca) || turniGiocati <= 2)
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
                                bool cicloInterrotto = false;

                                for (int x = pedinaX; x >= 0; x--)
                                {
                                    if (cicloInterrotto)
                                        break;

                                    for (int y = pedinaY; y >= 0; y--)
                                    {
                                        if (matrice[y, x] == Pedina.Vuoto)
                                        {
                                            ultimoXvalido = x;
                                            ultimoYvalido = y;
                                        }
                                        else
                                        {
                                            cicloInterrotto = true;
                                            break;
                                        }
                                    }
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                        case Movimento.Su:
                            {
                                bool cicloInterrotto = false;

                                for (int y = pedinaY; y >= 0; y--)
                                {
                                    if (matrice[y, pedinaX] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = pedinaX;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                        case Movimento.DestraSu:
                            {
                                bool cicloInterrotto = false;

                                for (int x = pedinaX; x < 5; x++)
                                {
                                    if (cicloInterrotto)
                                        break;

                                    for (int y = pedinaY; y >= 0; y--)
                                    {
                                        if (matrice[y, x] == Pedina.Vuoto)
                                        {
                                            ultimoXvalido = x;
                                            ultimoYvalido = y;
                                        }
                                        else
                                        {
                                            cicloInterrotto = true;
                                            break;
                                        }
                                    }
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                        case Movimento.Destra:
                            {
                                bool cicloInterrotto = false;

                                for (int x = pedinaX; x < 5; x++)
                                {
                                    if (matrice[pedinaY, x] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = pedinaY;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                        case Movimento.DestraGiu:
                            {
                                bool cicloInterrotto = false;

                                for (int x = pedinaX; x < 5; x++)
                                {
                                    if (cicloInterrotto)
                                        break;

                                    for (int y = pedinaY; y < 5; y++)
                                    {
                                        if (matrice[y, x] == Pedina.Vuoto)
                                        {
                                            ultimoXvalido = x;
                                            ultimoYvalido = y;
                                        }
                                        else
                                        {
                                            cicloInterrotto = true;
                                            break;
                                        }
                                    }
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                        case Movimento.Giu:
                            {
                                bool cicloInterrotto = false;

                                for (int y = pedinaY; y < 5; y++)
                                {
                                    if (matrice[y, pedinaX] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = pedinaX;
                                        ultimoYvalido = y;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                        case Movimento.SinistraGiu:
                            {
                                bool cicloInterrotto = false;

                                for (int x = pedinaX; x >= 0; x--)
                                {
                                    if (cicloInterrotto)
                                        break;

                                    for (int y = pedinaY; y < 5; y++)
                                    {
                                        if (matrice[y, x] == Pedina.Vuoto)
                                        {
                                            ultimoXvalido = x;
                                            ultimoYvalido = y;
                                        }
                                        else
                                        {
                                            cicloInterrotto = true;
                                            break;
                                        }
                                    }
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                        case Movimento.Sinistra:
                            {
                                bool cicloInterrotto = false;

                                for (int x = pedinaX; x >= 0; x--)
                                {
                                    if (matrice[pedinaY, x] == Pedina.Vuoto)
                                    {
                                        ultimoXvalido = x;
                                        ultimoYvalido = pedinaY;
                                    }
                                    else
                                        break;
                                }

                                coordinatePossibili.Add(new Coordinata(ultimoXvalido * 2, ultimoYvalido * 2));

                                break;
                            }
                    }
                }

                return coordinatePossibili;
            }
        }
    }
}
