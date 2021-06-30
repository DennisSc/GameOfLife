using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    public class Rule30
    {

        private int WidthX;
     
        private int WidthY;

        public Rule30()
        {
            
        }

        private int ToInt(bool value)
        {
            return value ? 1 : 0;
        }

      

        /// <summary>
        /// calculate next generation of game of life cells based on existing board of cells
        /// </summary>
        /// <param name="board">existing board to base calculations on</param>
        /// <returns>bool[,] array with next generation board of cells</returns>
        public bool[,] calculateNextBoardScanline(bool[,] board)
        {
            WidthX = board.GetLength(0);
            WidthY = board.GetLength(1);

            bool[,] Tempboard = new bool[WidthX, WidthY];

            for (int j = 0; j < WidthY - 1; j++)
            {
                for (int i = 0; i < WidthX - 3; i++)
                {
               
                    if (board[i, j] == true)
                        if (board[i + 1, j] == true)
                            Tempboard[i + 1, j + 1] = false;
                        else
                            if (board[i + 2, j] == true)
                                Tempboard[i + 1, j + 1] = false;
                        else
                            Tempboard[i + 1, j + 1] = true;
                    else if (board[i, j] == false)
                        if (board[i + 1, j] == true)
                            Tempboard[i + 1, j + 1] = true;
                        else
                            if (board[i + 2, j] == true)
                                Tempboard[i + 1, j + 1] = true;
                        else
                            Tempboard[i + 1, j + 1] = false;



                }
            }

            
            return Tempboard;
        }


        /// <summary>
        /// calculate next generation of game of life cells based on existing board of cells
        /// </summary>
        /// <param name="board">existing board to base calculations on</param>
        /// <returns>bool[,] array with next generation board of cells</returns>
        public bool[,] calculateNextBoard(bool[,] board)
        {
            WidthX = board.GetLength(0);
            WidthY = board.GetLength(1);

            bool[,] Tempboard = new bool[WidthX, WidthY];

            for (int j = 0; j < WidthY-1; j++)
            {
                for (int i = 0; i < WidthX - 3; i++)
                {
                    if (board[i, j] == true)
                        Tempboard[i, j] = true;

                    if (board[i, j] == true)
                    {
                        if (board[i + 1, j] == true)
                            Tempboard[i + 1, j + 1] = false;
                        else
                        {
                            if (board[i + 2, j] == true)
                                Tempboard[i + 1, j + 1] = false;
                            else
                                Tempboard[i + 1, j + 1] = true;
                        }
                    }
                    else if (board[i, j] == false)
                    {
                        if (board[i + 1, j] == true)
                            Tempboard[i + 1, j + 1] = true;
                        else if (board[i + 2, j] == true)
                            Tempboard[i + 1, j + 1] = true;
                        else
                            Tempboard[i + 1, j + 1] = false;
                    }


                }
            }


            return Tempboard;
        }


        /// <summary>
        /// calculate next generation of game of life cells based on existing board of cells
        /// </summary>
        /// <param name="board">existing board to base calculations on</param>
        /// <returns>bool[,] array with next generation board of cells</returns>
        public bool[,] calculateNextBoardBackwards(bool[,] board)
        {
            WidthX = board.GetLength(0);
            WidthY = board.GetLength(1);

            bool[,] Tempboard = new bool[WidthX, WidthY];

            for (int j = 1; j < WidthY; j++)
            {
                for (int i = 0; i < WidthX - 3; i++)
                {
                    if (board[i,j])
                        Tempboard[i, j-1] = true;

                    if (board[i, j] == true)
                        if (board[i + 1, j] == true)
                            Tempboard[i + 1, j] = false;
                        else
                            if (board[i + 2, j] == true)
                            Tempboard[i + 1, j] = false;
                        else
                            Tempboard[i + 1, j] = true;
                    else if (board[i, j] == false)
                        if (board[i + 1, j] == true)
                            Tempboard[i + 1, j] = true;
                        else
                            if (board[i + 2, j] == true)
                            Tempboard[i + 1, j] = true;
                        else
                            Tempboard[i + 1, j] = false;



                }
            }


            return Tempboard;
        }
    }
}
