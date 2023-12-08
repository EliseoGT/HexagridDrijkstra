using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Hexagrid
{

    public partial class MainForm : Form
    {
        public class Celda
        {
            public int fila;     //Filas, comienza en 0, de arriba hacia abajo
            public int columna;     // Columnas, comienza en 0 de izquierda a derecha
            public double distancia; //Distancia de la propia celda hacia la celda de inicio, usado para el algoritmo Drijkstra
            public int level;   // Usado como segundo parametro en las listas
            public Celda prev;   // Cada estado apunta a la celda previa hasta llegar al punto de inicio

            public Celda(int fila, int columna)
            {
                this.fila = fila;
                this.columna = columna;
            }

        } 

        //Valores enumerados de las celdas para graficar
        const int VALMAXIMO = Int32.MaxValue; 
        const int VACIA = 0;      
        const int OBSTACULO = 1;       
        const int PUNTOINICIO = 2;      
        const int PUNTOLLEGADA = 3;     
        const int FRONTERA = 4;   
        const int SINPASO = 5;     
        const int RUTA = 6;      
        
        int filas, columnas, tamañoCelda;
        double radio, altura, borde;        
 
        List<Celda> conjAbierto = new List<Celda>(); // Celdas que se pueden reanudar la busqueda
        List<Celda> conjCerrado = new List<Celda>(); // Celdas que ya no se puede reanudar la busqueda (llego a un tope)
        List<Celda> cuadricula = new List<Celda>(); // Celdas que seran explorados por el algoritmo

        Celda PuntoA;
        Celda PuntoB;

        int[,] tablero;
        Point[,] centros;
        double distancia;
        bool enEjecucion, encontrado, buscando, finBusqueda;
        int level;

        bool mouse_down = false;
        int fila_actual, columna_actual, valor_actual;
        
        public MainForm()
        {
            InitializeComponent();
            RbtCuadrado.CheckedChanged += new EventHandler(RadioButtons_CheckedChanged);
            RbtHexagon.CheckedChanged += new EventHandler(RadioButtons_CheckedChanged);
            RbtHexagon.Checked = true;
            IniciarTablero(false);
        }

        #region Codigo Formulario
        //Cuando el usuario selecciona una figura
        private void RadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            enEjecucion = false;
            BtnResolver.Enabled = true;
            BtnResolver.ForeColor = Color.Black;
            IniciarTablero(false);
        }

        //Cuando el usuario da clic en reiniciar
        private void ClearButton_Click(object sender, EventArgs e)
        {
            enEjecucion = false;
            BtnResolver.Enabled = true;
            BtnResolver.ForeColor = Color.Black;
            RellenarCeldas();
            Invalidate();
        }

        //Cuando el usuario da clic en resolver
        private void RealTimeButton_Click(object sender, EventArgs e)
        {
            if (enEjecucion)
                return;
            enEjecucion = true;
            buscando = true;
            // La iniciacion de dijkstra debe ser antes de buscar
            // para tomar en cuenta los obstaculos
            IniciarDijkstra();
            BtnResolver.ForeColor = Color.Red; 
            RealTime_action();
        } 
        private void RealTime_action()
        {
            do
                RevisarSiTermino();
            while (!finBusqueda);
        } 
        private void Animation_action()
        {
        } 
        private void Timer_Tick(object sender, EventArgs e)
        {
            Animation_action();
        } 

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush brocha;
            brocha = new SolidBrush(Color.DarkGray);
            // Rellena el fondo
            if (RbtTriangulo.Checked)
                g.FillRectangle(brocha, new Rectangle(10, 10, (int)((columnas / 2.0 + 0.5) * borde + 1), (int)(filas * altura + 1)));
            if (RbtCuadrado.Checked)
                g.FillRectangle(brocha, new Rectangle(10, 10, columnas * tamañoCelda + 1, filas * tamañoCelda + 1));
            if (RbtHexagon.Checked)
                g.FillRectangle(brocha, new Rectangle(10, 10, (int)((columnas - 1) / 2 * 3 * radio + 2 * radio), (int)(filas * 2 * altura + 1)));
            brocha.Dispose();

            //Pinta las celdas individualmente
            for (int r = 0; r < filas; r++)
                for (int c = 0; c < columnas; c++)
                {
                    if (tablero[r, c] == VACIA)
                        brocha = new SolidBrush(Color.White);
                    else if (tablero[r, c] == PUNTOINICIO)
                        brocha = new SolidBrush(Color.Red);
                    else if (tablero[r, c] == PUNTOLLEGADA)
                        brocha = new SolidBrush(Color.Cyan);
                    else if (tablero[r, c] == OBSTACULO)
                        brocha = new SolidBrush(Color.Black);
                    else if (tablero[r, c] == FRONTERA)
                        brocha = new SolidBrush(Color.White);
                    else if (tablero[r, c] == SINPASO)
                        brocha = new SolidBrush(Color.White);
                    else if (tablero[r, c] == RUTA)
                        brocha = new SolidBrush(Color.Lime);

                    //Calcula las cuadriculas de las distintas formas
                    Figuras figuras = new Figuras(borde, altura, tamañoCelda, radio);

                    if (RbtTriangulo.Checked)

                        g.FillPolygon(brocha, figuras.CalcularTriangulo(r, c));
                    if (RbtCuadrado.Checked)
                        g.FillPolygon(brocha, figuras.CalcularCuadrado(r, c));
                    if (RbtHexagon.Checked)
                        if ((c % 2 == 0) || (!(c % 2 == 0) && (r < filas - 1)))
                            g.FillPolygon(brocha, figuras.CalcularHexagono(r, c));
                    brocha.Dispose();
                }
        }

        // Maneja la colocacion de obstaculos y su destruccion
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_down = true;
            distancia = VALMAXIMO;
            Celda cur_cell = BuscarCeldaMouse(e.Y, e.X);
            int fila = cur_cell.fila;
            int columna = cur_cell.columna;

            if (RbtCuadrado.Checked ? fila >= 0 && fila < filas && columna >= 0 && columna < columnas : distancia < VALMAXIMO)
            {
                if (enEjecucion ? true : !encontrado && !buscando)
                {
                    if (enEjecucion)
                        RellenarCeldas();
                    fila_actual = fila;
                    columna_actual = columna;
                    valor_actual = tablero[fila, columna];
                    if (valor_actual == VACIA)
                        tablero[fila, columna] = OBSTACULO;
                    if (valor_actual == OBSTACULO)
                        tablero[fila, columna] = VACIA;

                    IniciarDijkstra();
                }
                if (enEjecucion)
                    RealTime_action();
                else
                    Invalidate(); //Fuerza el dibujo en el tablero
            }
        }

        // Permite mover los puntos de origen y salida, ademas de permitir dibujar trazos
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouse_down)
                return;
            distancia = VALMAXIMO;
            Celda cur_cell = BuscarCeldaMouse(e.Y, e.X);
            int fila = cur_cell.fila;
            int columna = cur_cell.columna;

            if (RbtCuadrado.Checked ? fila >= 0 && fila < filas && columna >= 0 && columna < columnas : distancia < VALMAXIMO)
            {
                if (enEjecucion ? true : !encontrado && !buscando)
                {
                    if (enEjecucion)
                        RellenarCeldas();
                    if (!(fila == fila_actual && columna == columna_actual) && (valor_actual == PUNTOINICIO || valor_actual == PUNTOLLEGADA))
                    {
                        int new_val = tablero[fila, columna];
                        if (new_val == VACIA)
                        {
                            tablero[fila, columna] = valor_actual;
                            if (valor_actual == PUNTOINICIO)
                            {
                                PuntoA.fila = fila;
                                PuntoA.columna = columna;
                            }
                            else
                            {
                                PuntoB.fila = fila;
                                PuntoB.columna = columna;
                            }
                            tablero[fila_actual, columna_actual] = new_val;
                            fila_actual = fila;
                            columna_actual = columna;
                            valor_actual = tablero[fila, columna];
                        }
                    }
                    else if (tablero[fila, columna] != PUNTOINICIO && tablero[fila, columna] != PUNTOLLEGADA)
                        tablero[fila, columna] = OBSTACULO;
                    IniciarDijkstra();
                }
                if (enEjecucion)
                    RealTime_action();
                else
                    Invalidate();
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            mouse_down = false;
        }

        #endregion

        #region Codigo principal

        // Busca a que celda se apunta con el mouse
        private Celda BuscarCeldaMouse(int cur_Y, int cur_X)
        {
            int row = 0, col = 0;
            double dx, dy, cur_dist;
            if (RbtCuadrado.Checked)
            {
                row = (cur_Y - 10) / tamañoCelda;
                col = (cur_X - 10) / tamañoCelda;
            }
            else
            {
                for (int r = 0; r < filas; r++)
                    for (int c = 0; c < columnas; c++)
                    { 
                        dx = cur_X - centros[r, c].X;
                        dy = cur_Y - centros[r, c].Y;
                        cur_dist = Math.Sqrt(dx * dx + dy * dy);
                        if (cur_dist < Math.Min(distancia , radio))
                        {
                            distancia = cur_dist;
                            row = r;
                            col = c;
                        }
                    }
            }
            return new Celda(row, col);
        } 

        private void IniciarTablero(bool makeMaze)
        {
            level = 0;
            filas = 20;
            columnas = 19;
           
            // La cuadricula hexagonal debe tener un numero impar de columnas
            if (RbtHexagon.Checked && columnas % 2 != 1)
            {
                columnas -= 1;
            }
            
            tablero = new int[filas, columnas];
            centros = new Point[filas, columnas];
            if (RbtCuadrado.Checked)
            {
                PuntoA = new Celda(filas - 2, 1);
                PuntoB = new Celda(1, columnas - 2);
            }
            else
            {
                PuntoA = new Celda(filas - 1, 0);
                PuntoB = new Celda(0, columnas - 1);
            }

            // Cálculo del borde y la altura de la celda triangular
            if (RbtTriangulo.Checked)
            {
                borde = Math.Min(500 / (columnas / 2.0 + 1), 1000 / (filas * Math.Sqrt(3)));
                altura = borde * Math.Sqrt(3) / 2;
                radio = altura * 2 / 3;
            }

            // Cálculo del tamaño y la altura de la celda cuadrada
            if (RbtCuadrado.Checked)
            {
                tamañoCelda = (int)(500 / (filas > columnas ? filas : columnas));
            }

            // Cálculo del radio y la mitad de la altura de la celda hexagonal
            if (RbtHexagon.Checked)
            {
                radio = Math.Min(1000 / (3.0 * columnas + 1), 500 / (filas * Math.Sqrt(3)));
                altura = radio * Math.Sqrt(3) / 2;
            }

            // Cálculo de las coordenadas de los centros de las celdas

            int y = 0;
            for (int r = 0; r < filas; r++)
                for (int c = 0; c < columnas; c++)
                {
                    if (RbtTriangulo.Checked)
                    {
                        if ((c % 2 == 0 && r % 2 == 0) || (c % 2 != 0 && r % 2 != 0))
                            y = (int)Math.Round(r * altura + altura / 3);
                        if ((c % 2 == 0 && r % 2 != 0) || (c % 2 != 0 && r % 2 == 0))
                            y = (int)Math.Round(r * altura + altura * 2 / 3);
                        centros[r, c] = new Point(11 + (int)Math.Round(((c + 1) / 2.0) * borde),
                                                  11 + y);
                    }
                    if (RbtCuadrado.Checked)
                        centros[r, c] = new Point(11 + c * tamañoCelda + tamañoCelda / 2,
                                                  11 + r * tamañoCelda + tamañoCelda / 2);
                    if (RbtHexagon.Checked)
                        if ((c % 2) == 0)
                            centros[r, c] = new Point((int)Math.Round(11 + (c / 2 * 3 + 1) * radio),
                                                      (int)Math.Round(11 + (r * 2 + 1) * altura));
                        else
                            centros[r, c] = new Point((int)Math.Round(11 + radio / 2 + (c / 2 * 3 + 2) * radio),
                                                      (int)Math.Round(11 + 2 * (r + 1) * altura));
                }
            RellenarCeldas();
            if (RbtHexagon.Checked)
                for (int c = 0; c < columnas; c++)
                    if (c % 2 != 0)
                        tablero[filas - 1,c] = OBSTACULO;

            Invalidate(); //Forzar coloreaoo
        }
        #endregion

        #region Algoritmo Dijkstra

        private void IniciarDijkstra()
        {
            // Limpiar el tablero/cuadricula
            cuadricula.Clear();
            // Conectar las celdas al punto de inicio
            EncontrarAdyacentes(PuntoA);
            // Por cada celda en la cuadricula
            foreach (Celda celda in cuadricula)
            {
                // Se hace la distancia inalcanzable si es que no esta colindante
                celda.distancia = VALMAXIMO;
                // Se desreferencia el anterior
                celda.prev = null;
            }
            // Regresa el index del punto de inicio
            cuadricula[EstaEnLaLista(cuadricula, PuntoA)].distancia = 0;
            // Busca en la lista de nodos que cumplan la distancia
            cuadricula.Sort((r1, r2) => r1.distancia.CompareTo(r2.distancia));
            // Inicia la lista de celdas que ya no se podran reanudar
            conjCerrado.Clear();
        } 

        private void ExpandirCelda()
        {
            // Mientras que la lista de celdas no este vacia
            if (cuadricula.Count == 0)
                return;
            // Se quita la celda que tenga la menor distancia, por ahi se armara el camino
            Celda celdaMinDist = (Celda)cuadricula[0];
            cuadricula.RemoveAt(0);
            // La misma celda se añade a la lista de celdas que no se pueden reanudar
            conjCerrado.Add(celdaMinDist);
            // Si se encuentra el punto al que se quiere llegar
            if (celdaMinDist.fila == PuntoB.fila && celdaMinDist.columna == PuntoB.columna)
            {
                encontrado = true;
                return;
            }
            // Actualizar la celda que ya no tiene caminos disponibles
            tablero[celdaMinDist.fila, celdaMinDist.columna] = SINPASO;
            // Si no se encuentra el destino (tiene distancia "infinita")
            if (celdaMinDist.distancia == VALMAXIMO)
            {
                // No hay solucion, y no se dibuja el camino
                return;
            } 
              // Crear los vecinos de la celda previa
            List<Celda> celdasAdyacentes = obtenerVecinos(celdaMinDist, false);
            // Por cada celda vecina en celdaMinDist
            foreach (Celda celdaVecina in celdasAdyacentes)
            {
                // Se usa un alternador para calcular la menor distancia
                double alt = celdaMinDist.distancia + DistanciaEntreCeldas(celdaMinDist, celdaVecina);
                // Si la distancia es menor comparada a la celdaVecina
                if (alt < celdaVecina.distancia)
                {
                    // Se cambia la distancia por la menor
                    celdaVecina.distancia = alt;
                    // Se coloca el sucesor para despues marcar el camino
                    celdaVecina.prev = celdaMinDist;
                    // Se marca la celda que se puede reanudar de no haber encontrado el camino por donde iba
                    tablero[celdaVecina.fila, celdaVecina.columna] = FRONTERA;
                    // Determina el orden relativo de los elementos en la lista después de la ordenación.
                    cuadricula.Sort((x, y) =>
                    {
                        int result = x.distancia.CompareTo(y.distancia);
                        if (result == 0)
                            result = x.level.CompareTo(y.level);
                        return result;
                    });
                }
            }
        }

        private List<Celda> obtenerVecinos(Celda actual, bool crearConexiones)
        {
            int r = actual.fila;
            int c = actual.columna;
            List<Celda> vecinos = new List<Celda>();

            Action<int, int> agregarVecino = (row, col) =>
            {
                if (row >= 0 && row < filas && col >= 0 && col < columnas && tablero[row, col] != OBSTACULO &&
                    (RbtHexagon.Checked || RbtCuadrado.Checked ||
                     (RbtTriangulo.Checked && ((row % 2 == 0 && col % 2 == 0) || (row % 2 != 0 && col % 2 != 0)))) &&
                    EstaEnLaLista(conjAbierto, new Celda(row, col)) == -1 &&
                    EstaEnLaLista(conjCerrado, new Celda(row, col)) == -1)
                {
                    Celda vecino = new Celda(row, col);
                    if (crearConexiones)
                    {
                        vecinos.Add(vecino);
                    }
                    else
                    {
                        int graphIndex = EstaEnLaLista(cuadricula, vecino);
                        if (graphIndex > -1)
                        {
                            cuadricula[graphIndex].level = ++level;
                            vecinos.Add(cuadricula[graphIndex]);
                        }
                    }
                }
            };

            agregarVecino(r - 1, c); // Arriba
            agregarVecino(r - 1, c + (RbtHexagon.Checked && c % 2 == 0 ? 1 : 0)); // Arriba-derecha
            agregarVecino(r, c + 1); // Derecha
            agregarVecino(r + 1, c + (RbtHexagon.Checked && c % 2 != 0 ? 1 : 0)); // Abajo-derecha
            agregarVecino(r + 1, c); // Abajo
            agregarVecino(r + 1, c - (RbtHexagon.Checked && c % 2 != 0 ? 1 : 0)); // Abajo-izquierda
            agregarVecino(r, c - 1); // Izquierda
            agregarVecino(r - 1, c - (RbtHexagon.Checked && c % 2 == 0 ? 1 : 0)); // Arriba-izquierda

            return vecinos;
        }

        private double DistanciaEntreCeldas(Celda celda1, Celda celda2)
        {
            double dist;
            double dx = centros[celda1.fila, celda1.columna].X - centros[celda2.fila, celda2.columna].X;
            double dy = centros[celda1.fila, celda1.columna].Y - centros[celda2.fila, celda2.columna].Y;
            if (RbtTriangulo.Checked || RbtHexagon.Checked)
                // Calcula la distancia euclidiana para triangulos y hexagonos
                dist = Math.Sqrt(dx * dx + dy * dy);
            else
                // Calcula la distancia segun lo alto y largo
                dist = Math.Abs(dx) + Math.Abs(dy);
            return dist;
        } 

        private void EncontrarAdyacentes(Celda celdaInicio)
        {
            Stack<Celda> stack = new Stack<Celda>();
            List<Celda> succesors;
            stack.Push(celdaInicio);
            cuadricula.Add(celdaInicio);
            while (!(stack.Count == 0))
            {
                celdaInicio = stack.Pop();
                succesors = obtenerVecinos(celdaInicio, true);
                foreach (Celda c in succesors)
                    if (EstaEnLaLista(cuadricula, c) == -1)
                    {
                        stack.Push(c);
                        cuadricula.Add(c);
                    }
            }
        }

        private int EstaEnLaLista(List<Celda> lista, Celda actual)
        {
            int index = -1;
            for (int i = 0; i < lista.Count; i++)
            {
                Celda listItem = (Celda)lista[i];
                if (actual.fila == listItem.fila && actual.columna == listItem.columna)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        #endregion

        private void RevisarSiTermino()
        {
            if ((cuadricula.Count == 0))
            {
                finBusqueda = true;
                tablero[PuntoA.fila, PuntoA.columna] = PUNTOINICIO;
                Invalidate();
            }
            else
            {
                ExpandirCelda();
                if (encontrado)
                {
                    finBusqueda = true;
                    MarcarRuta();
                    Invalidate();
                }
            }

        }

        //Calcula el camino y lo traza 
        private void MarcarRuta()
        {
            int pasos = 0;
            double distance = 0;
            int index = EstaEnLaLista(conjCerrado, PuntoB);
            Celda actual = (Celda)conjCerrado[index];
            tablero[actual.fila, actual.columna] = PUNTOLLEGADA;
            do
            {
                pasos++;
                double dx = centros[actual.fila, actual.columna].X - centros[actual.prev.fila, actual.prev.columna].X;
                double dy = centros[actual.fila, actual.columna].Y - centros[actual.prev.fila, actual.prev.columna].Y;
                distance += Math.Sqrt(dx * dx + dy * dy);
                actual = actual.prev;
                tablero[actual.fila, actual.columna] = RUTA;
            } while (!(actual.fila == PuntoA.fila && actual.columna == PuntoA.columna));
            tablero[PuntoA.fila, PuntoA.columna] = PUNTOINICIO;

        }

        private void RellenarCeldas()
        {   
            //Mantiene las posiciones de las celdas si se esta buscando actualmente
            if (buscando || finBusqueda || enEjecucion)
            {
                for (int r = 0; r < filas; r++)
                    for (int c = 0; c < columnas; c++)
                    {
                        if (tablero[r, c] == FRONTERA || tablero[r, c] == SINPASO || tablero[r, c] == RUTA)
                            tablero[r, c] = VACIA;
                        if (tablero[r, c] == PUNTOINICIO)
                            PuntoA = new Celda(r, c);
                        if (tablero[r, c] == PUNTOLLEGADA)
                            PuntoB = new Celda(r, c);
                    }
            } 
            //Si no se esta buscando, pinta todo de blanco y manda los puntos A y B a su lugar de origen.
            else
            {
                for (int r = 0; r < filas; r++)
                    for (int c = 0; c < columnas; c++)
                        tablero[r, c] = VACIA;
                if (RbtCuadrado.Checked)
                {
                    PuntoA = new Celda(filas - 2, 1);
                    PuntoB = new Celda(1, columnas - 2);
                }
                else
                {
                    PuntoA = new Celda(filas - 1, 0);
                    PuntoB = new Celda(0, columnas - 1);
                }
            }

            encontrado = false;
            buscando = enEjecucion;
            finBusqueda = false;


            if (!enEjecucion)
            {
                tablero[PuntoB.fila, PuntoB.columna] = PUNTOLLEGADA;
                tablero[PuntoA.fila, PuntoA.columna] = PUNTOINICIO;
            }
        }
    } 
} 