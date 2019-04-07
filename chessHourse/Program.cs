using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chessHourse
{
    class Program
    {
        class Board //решил делать по правилу Варнсдорфа, без всяких там страшных рекурсий
        {
            int[][] board; //здесь будет лежать матрица доступности
            int[][] way; //тут будет хранится путь
            uint size = 8;
            public Board(uint size)
            {
                this.size = size;
                // выделяю место под доску
                board = new int[size][];
                way = new int[size][];
                for (int i = 0; i < size; i++)
                {
                    board[i] = new int[size];
                    way[i] = new int[size]; 
                    for (int j = 0; j < size; j++)
                    {
                        board[i][j] = countAccessibility(j, i); //просчитываю уровень доступности(количество клеток, с которых можно попасть в эту клетку) для каждой клетки поля
                    }
                }
                //decreaseAccessibility(3, 3);
            }
            int countAccessibility(int x, int y) //подсчет доступных ходов в эту позицию из других позиций. нужен для заполнения матрицы доступности
            {
                int accessibility = 0;
                //буквы Г по y
                if (y + 2 < size && x + 1 < size) accessibility++;
                if (y + 2 < size && x - 1 >= 0) accessibility++;
                if (y - 2 >= 0 && x - 1 >= 0) accessibility++;
                if (y - 2 >= 0 && x + 1 < size) accessibility++;

                //буквы Г по x
                if (x + 2 < size && y + 1 < size) accessibility++;
                if (x + 2 < size && y - 1 >= 0) accessibility++;
                if (x - 2 >= 0 && y - 1 >= 0) accessibility++;
                if (x - 2 >= 0 && y + 1 < size) accessibility++;

                return accessibility;
            }
            void decreaseAccessibility(int x, int y) //уменьшение доступности для всех соседних клеток(вызывается после хода, т.к. одна клетка выбывает и теперь количество позиций, с которых можно попасть к этим клеткам на 1 меньше)
            {
                //буквы Г по y
                if (y + 2 < size && x + 1 < size) board[y + 2][x + 1]--;
                if (y + 2 < size && x - 1 >= 0) board[y + 2][x - 1]--;
                if (y - 2 >= 0 && x - 1 >= 0) board[y - 2][x - 1]--;
                if (y - 2 >= 0 && x + 1 < size) board[y - 2][x + 1]--;

                //буквы Г по x
                if (x + 2 < size && y + 1 < size) board[y + 1][x + 2]--;
                if (x + 2 < size && y - 1 >= 0) board[y - 1][x + 2]--;
                if (x - 2 >= 0 && y - 1 >= 0) board[y - 1][x - 2]--;
                if (x - 2 >= 0 && y + 1 < size) board[y + 1][x - 2]--;
            }
            public void printAccessibility()
            {
                foreach(int[] i in board)
                {
                    foreach(int j in i)
                    {
                        Console.Write(j + " ");
                    }
                    Console.WriteLine();
                }
            }
            struct NextPos
            {
                public int x, y, access;
            }
            void isPossibleToMove(int x, int y, ref NextPos np) //чтоб не писать одно и тоже 100500 раз сделал функцию
            {
                //если доступность данной клетки меньше той, что лежит в np и в эту клетку ранее не ходили, то np стает этой клеткой
                if (board[y][x] < np.access && board[y][x] > 0)
                {
                    np.x = x;
                    np.y = y;
                    np.access = board[y][x];
                }
            }
            void move(ref int x, ref int y) //цель функции найти клетку с наименьшей доступностью и походить туда
            {
                NextPos np = new NextPos(); //чтобы было удобнее передавать в параметры пихнул все в структурку 
                np.access = 100500;
                np.x = -1;
                np.y = -1;
                //буквы Г по y
                if (y + 2 < size && x + 1 < size) isPossibleToMove(x + 1, y + 2, ref np);
                if (y + 2 < size && x - 1 >= 0) isPossibleToMove(x - 1, y + 2, ref np);
                if (y - 2 >= 0 && x - 1 >= 0) isPossibleToMove(x - 1, y - 2, ref np);
                if (y - 2 >= 0 && x + 1 < size) isPossibleToMove(x + 1, y - 2, ref np);

                //буквы Г по x
                if (x + 2 < size && y + 1 < size) isPossibleToMove(x + 2, y + 1, ref np);
                if (x + 2 < size && y - 1 >= 0) isPossibleToMove(x + 2, y - 1, ref np);
                if (x - 2 >= 0 && y - 1 >= 0) isPossibleToMove(x - 2, y - 1, ref np);
                if (x - 2 >= 0 && y + 1 < size) isPossibleToMove(x - 2, y + 1, ref np);

                x = np.x;
                y = np.y;
            }
            public void findHourseWay(int x, int y)
            {
                if(x < 0 || y < 0 || x > size || y > size)
                {
                    Console.WriteLine("Позиция невозможна :с");
                    return;
                }
                Console.WriteLine("(" + x + ", " + y + ")");
                int step = 1;
                int prevx; 
                int prevy;
                do
                {
                    way[y][x] = step; //заполняю текущую позицию номером шага в пути(по сути путь нужен только для вывода)
                    board[y][x] = 0; //теперь в эту позицию нельзя будет больше походить
                    prevx = x; //запоминаю позиции перед ходом, чтобы потом пересчитать матрицу доступности(одна клетка пропала, с нее уже нельзя будет походить в соседние)
                    prevy = y;
                    move(ref x, ref y); //хожу в клетку с наименьшей доступностью
                    decreaseAccessibility(prevx, prevy); //пересчитываю доступность для прошлой позиции
                    step++; 

                } while (step < size * size); //до тех пор, пока количество шагов не будет равным количеству клеток поля
                way[y][x] = step;//так получается, что цикл перестает работать раньше, чем последняя позиция успевает заполниться номером шага, по этому я делаю это
            }
            public void printWay()
            {
                foreach (int[] i in way)
                {
                    foreach (int j in i)
                    {
                        Console.Write(j + " ");
                    }
                    Console.WriteLine();
                }
            }
            public void clear() //возврат матриц в начальное состояние
            {
                for(int i = 0; i < size; i++)
                {
                    for(int j = 0; j < size; j++)
                    {
                        way[i][j] = 0;
                        board[i][j] = countAccessibility(j, i);
                    }
                }
            }
        }
        static void Main(string[] args)
        {
            Board board = new Board(8);
            Console.WriteLine("Так выглядит матрица доступности: ");
            board.printAccessibility();

            board.findHourseWay(5, 7);
            board.printWay();
            board.clear();

            board.findHourseWay(3, 4);
            board.printWay();
            board.clear();

            board.findHourseWay(0, 0);
            board.printWay();
            
        }
    }
}
