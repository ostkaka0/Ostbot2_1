using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OstBot_2_1
{
    public struct Position
    {
        public int x;
        public int y;
        public int cost;

        public Position(int x, int y, int cost)
        {
            this.x = x;
            this.y = y;
            this.cost = cost;
        }
    }

    class Square
    {
        public int x;
        public int y;
        public int G;
        public int H;
        public int cost;
        public Square parent;
        public int F
        {
            get
            {
                return G + H;
            }
        }
        public Square(int x, int y, int G, int H, int cost, Square parent)
        {
            this.x = x;
            this.y = y;
            this.G = H;
            this.H = H;
            this.cost = cost;
            this.parent = parent;
        }

        public override bool Equals(object obj)
        {
            Square square = obj as Square;
            return square.x == x && square.y == y;
        }

        public bool Equals(Square square)
        {
            return square.x == x && square.y == y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + 15 + x.GetHashCode() + y.GetHashCode();
        }

        public static bool operator !=(Square a, Square b)
        {
            if ((object)b == null || (object)a == null)
            {
                return (object)b != (object)a;
            }
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator ==(Square a, Square b)
        {
            if ((object)b == null || (object)a == null)
            {
                return (object)b == (object)a;
            }
            return a.x == b.x && a.y == b.y;
        }
    }

    class PathFinding
    {
        public Stack<Square> closedSquares = new Stack<Square>();
        public List<Square> openSquares = new List<Square>();
        Stopwatch lagMeter = new Stopwatch();

        private  int maxBlocks = 200 * 400;

        private Position[] adjacentSquares = null;/* = new Position[8] { 
            new Position(1, 1, 14), 
            new Position(-1, 1, 14), 
            new Position(-1, -1, 14),
            new Position(1, -1, 14), 
            new Position(1, 0, 10), 
            new Position(-1, 0, 10),
            new Position(0, 1, 10),
            new Position(0, -1, 10)};*/


        public Stack<Square> Begin(int startX, int startY, int targetX, int targetY, Position[] adjacentSquares)
        {
            //return null;

            if (adjacentSquares == null)
            {
                this.adjacentSquares = new Position[8] { 
                new Position(1, 1, 14), 
                new Position(-1, 1, 14), 
                new Position(-1, -1, 14),
                new Position(1, -1, 14), 
                new Position(1, 0, 10), 
                new Position(-1, 0, 10),
                new Position(0, 1, 10),
                new Position(0, -1, 10)};
            }
            else
            {
                this.adjacentSquares = adjacentSquares;
            }

            lagMeter.Start();
            closedSquares.Clear();
            openSquares.Clear();
            Square startingSquare = new Square(startX, startY, 0, CalculateH(startX, startY, targetX, targetY), 0, null);
            openSquares.Add(startingSquare);
            for (int i = 0; i < maxBlocks; i++)
            {
                if (!GetCloseSquareData(targetX, targetY))
                    break;
            }
            Console.WriteLine("one pathfinding took " + lagMeter.ElapsedMilliseconds + "MS");
            if (openSquares.Count == 0 && closedSquares.Count != 0)
            {
                Square parent = null;
                foreach (Square s in closedSquares)
                {
                    if (parent == null || s.H < parent.H)
                    {
                        parent = s;
                    }
                }
                if (parent != null)
                {
                    //Console.WriteLine("Lowest H pos X:" + parent.x + " Y:" + parent.y);
                    //Stack<Square> temp = new PathFinding().Begin(startX, startY, parent.x, parent.y, adjacentSquares);
                    return null;//temp;
                }

            }
            Stack<Square> finalPath = new Stack<Square>();
            if (closedSquares.Count > 0)
            {
                Square currentSquare = closedSquares.Pop();
                while (currentSquare != null && currentSquare.parent != null)
                {
                    Square parentOfCurrent = currentSquare.parent;
                    finalPath.Push(parentOfCurrent);
                    //Console.WriteLine("Added square to path X:" + parentOfCurrent.x + " Y:" + parentOfCurrent.y);
                    currentSquare = parentOfCurrent;
                }
            }
            return finalPath;
        }



        public bool GetCloseSquareData(int targetX, int targetY)
        {
            Square parent = null;
            foreach (Square s in openSquares)
            {
                if (parent == null || s.F < parent.F)
                {
                    parent = s;
                }
            }

            if (parent != null)
            {
                closedSquares.Push(parent);
            }

            if (parent == null || parent.x == targetX && parent.y == targetY || openSquares.Count == 0)
                return false;
            openSquares.Remove(parent);

            for (int a = 0; a < 8; a++)
            {
                Square adjacentSquare = new Square(
                    parent.x + adjacentSquares[a].x, parent.y + adjacentSquares[a].y,
                    parent.G + 10 + adjacentSquares[a].cost,
                    CalculateH(
                        parent.x + adjacentSquares[a].y, parent.y + adjacentSquares[a].y,
                        targetX, targetY),
                    adjacentSquares[a].cost, parent);

                if (!closedSquares.Contains(adjacentSquare) && OstBot.room.getBotMapBlock(0, adjacentSquare.x, adjacentSquare.y).blockId == 4)

                    if (!openSquares.Contains(adjacentSquare))
                    {
                        openSquares.Add(adjacentSquare);
                    }
                    else
                    {
                        int oldG = adjacentSquare.G;
                        int newG = parent.G + 10 + adjacentSquares[a].cost;
                        if (newG < oldG)
                        {
                            adjacentSquare.parent = parent;
                            adjacentSquare.G = newG;
                        }
                    }
            }
            return true;
        }

        public int CalculateH(int x1, int y1, int x2, int y2)
        {
            return (Math.Abs(x1 - x2) + Math.Abs(y1 - y2));
        }
    }

}
