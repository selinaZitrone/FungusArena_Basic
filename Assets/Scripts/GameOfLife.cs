using System;

public class GameOfLife
{
  // This function contains the algorith for game of life
  // it takes a first matrix status as input for the current game status at timestep 1
  // then it calculates the new status for all cells and saves it in the second matrix toRender
  // the second matrix can then be used to render the output to the screen
  // this algorithm now only updates the gol integer variable from the ColCel struct
  public static void UpdateColCellStatus(BattleManager.ColCel[,] status, BattleManager.ColCel[,] toRender)
    {
        int arraySizeX = status.GetLength(0);
        int arraySizeY = status.GetLength(1);

        //loop through status array and check neighbors of each cell
        for (int j = 0; j < arraySizeX; j++) //columns
        {
            for (int i = 0; i < arraySizeY; i++) // rows
            {
                // only do this if the cell status is not 2 (outside the petridish)
                if (status[i, j].gol != 2)
                {
                    int sumOfNeighbors = 0;
                    //calculate number of alive neighbors
                    for (int x = Math.Max(j - 1, 0); x < Math.Min(j + 2, arraySizeX); x++) //columns of neighbors
                    {
                        for (int y = Math.Max(i - 1, 0); y < Math.Min(i + 2, arraySizeY); y++)
                        {
                            if (status[x, y].gol != 2)
                            {
                                sumOfNeighbors += status[y, x].gol;
                            }

                        }
                    }
                    //find out the new status of the respective cell:
                    int cellStatus = status[i, j].gol;
                    sumOfNeighbors -= cellStatus; //subtract cell status because it was part of the sum of neighbors

                    toRender[i, j].gol = cellStatus;

                    switch (cellStatus)
                    {
                        case 1:
                            switch (sumOfNeighbors)
                            {
                                case 2:
                                    break;
                                case 3:
                                    break;
                                default:
                                    toRender[i, j].gol = 0;
                                    break;
                            }
                            break;
                        case 0:
                            switch (sumOfNeighbors)
                            {
                                case 3:
                                    toRender[i, j].gol = 1;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

}
