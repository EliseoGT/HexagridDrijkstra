using System.Drawing;

namespace Hexagrid { 
    class Figuras
    {
        private double borde;
        private double altura;
        private int tamañoCelda;
        private double radio;

        public Figuras(double borde, double altura, int tamañoCelda, double radio)
        {
            this.borde = borde;
            this.altura = altura;
            this.tamañoCelda = tamañoCelda;
            this.radio = radio;
        }

        public Point[] CalcularTriangulo(int fila, int columna)
        {
            if (((columna % 2 == 0) && (fila % 2 == 0)) || ((columna % 2 != 0) && (fila % 2 != 0)))
            {
                Point[] poligono = {
                    new Point((int)(11 + (columna/2+0.0)*borde + (fila % 2 != 0 ? borde/2 : 0) + 1), (int)(11 + fila*altura + 0)),
                    new Point((int)(11 + (columna/2+1.0)*borde + (fila % 2 != 0 ? borde/2 : 0) - 1), (int)(11 + fila*altura + 0)),
                    new Point((int)(11 + (columna/2+0.5)*borde + (fila % 2 != 0 ? borde/2 : 0) + 0), (int)(11 + (fila+1)*altura - 1))
                };
                return poligono;
            }
            else
            {
                Point[] poligono = {
                    new Point((int)(11 + (columna/2+1.0)*borde - (fila % 2 != 0 ? borde/2 : 0) + 0), (int)(11 + fila*altura + 1)),
                    new Point((int)(11 + (columna/2+1.5)*borde - (fila % 2 != 0 ? borde/2 : 0) - 1), (int)(11 + (fila+1)*altura - 1)),
                    new Point((int)(11 + (columna/2+0.5)*borde - (fila % 2 != 0 ? borde/2 : 0) + 1), (int)(11 + (fila+1)*altura - 1))
                };
                return poligono;
            }
        }

        public Point[] CalcularCuadrado(int fila, int columna)
        {
            Point[] poligono = {
                new Point((int)(11 + columna*tamañoCelda + 0), (int)(11 + fila*tamañoCelda + 0)),
                new Point((int)(11 + (columna+1)*tamañoCelda - 1), (int)(11 + fila*tamañoCelda + 0)),
                new Point((int)(11 + (columna+1)*tamañoCelda - 1), (int)(11 + (fila+1)*tamañoCelda - 1)),
                new Point((int)(11 + columna*tamañoCelda + 0), (int)(11 + (fila+1)*tamañoCelda - 1))
            };
            return poligono;
        }

        public Point[] CalcularHexagono(int fila, int columna)
        {
            if ((columna % 2) == 0)
            {
                Point[] poligono = {
                    new Point((int)(10 + radio/2 + columna/2*3*radio + 0), (int)(10 + fila*2*altura + 1)),
                    new Point((int)(10 + radio + radio/2 + columna/2*3*radio + 0), (int)(10 + fila*2*altura + 1)),
                    new Point((int)(10 + 2*radio + columna/2*3*radio - 1), (int)(10 + altura + fila*2*altura + 0)),
                    new Point((int)(10 + radio + radio/2 + columna/2*3*radio + 0), (int)(10 + 2*altura + fila*2*altura + 0)),
                    new Point((int)(10 + radio/2 + columna/2*3*radio + 0), (int)(10 + 2*altura + fila*2*altura + 0)),
                    new Point((int)(10 + columna/2*3*radio + 1), (int)(10 + altura + fila*2*altura + 0))
                };
                return poligono;
            }
            else
            {
                Point[] poligono = {
                    new Point((int)(10 + 2*radio + columna/2*3*radio + 0), (int)(10 + altura + fila*2*altura + 1)),
                    new Point((int)(10 + 3*radio + columna/2*3*radio + 0), (int)(10 + altura + fila*2*altura + 1)),
                    new Point((int)(10 + 3*radio + radio/2 + columna/2*3*radio - 1), (int)(10 + 2*altura + fila*2*altura + 0)),
                    new Point((int)(10 + 3*radio + columna/2*3*radio + 0), (int)(10 + 3*altura + fila*2*altura + 0)),
                    new Point((int)(10 + 2*radio + columna/2*3*radio + 0), (int)(10 + 3*altura + fila*2*altura + 0)),
                    new Point((int)(10 + radio + radio/2 + columna/2*3*radio + 1), (int)(10 + 2*altura + fila*2*altura + 0))
                };
                return poligono;
            }
        }
    }
}