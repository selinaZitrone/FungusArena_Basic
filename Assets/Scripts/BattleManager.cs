using UnityEngine;

public class BattleManager : MonoBehaviour
{
    Vitrus_Genenator_2 vitrusGen;

    // variables that can be changed in the editor to create the game matrix
    // and define the radius of the petridish
    [Header("Game of Live vars")]
    public int matrixSize = 32;
    public int cellSize = 4;
    public int radius = 20;
    
    // struct for all variables assigned to a single matrix cell
    // for now, only int gol (game of life integer that takes values 0,1,2)
    // later this can contain all the patches-own variables from the NetLogo model
    public struct ColCel
    {
       public int gol;
       //public string status;
    }

    // three matrixes that describe the game status at time t, time t-1 and the matrix that should be rendered by the 
    // Vitrus_Generator2
    ColCel[,] matrix_1;
    ColCel[,] matrix_2;
    ColCel[,] toRender;


    [Header("Loop variables")]
    public bool paused = false;

    // refresh time in seconds: how fast will the game run?
    public float refreshTime = 0.001f;
    float timer;

    // definiert welche matrix zur Updatefunktion als Referenz und welche als Output weitergegeben wird (works as a switch)
    bool actualMatrix = true;

    
    void Start()
    {
        vitrusGen = FindObjectOfType<Vitrus_Genenator_2>();
        paused = true;
        PrepareMatrixes();
    }

    // count down the timer and refresh only when the timer is below 0
    // in this way you can control the simulation speed (with the variable refreshTime that you can control in the editor)
    // play one round of the game, render the result to the screen and set the timer back to start.
    void Update()
    {
        if (!paused)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                PlayRound();
                vitrusGen.SetRenderOutput(ConvertColCelMatrixToIntMatrix(toRender));
                timer = refreshTime;
            }
        }
    }

    // is called when you click the button GOL-Random
    public void StandardStartOfGOL()
    {
        SetUpRandomGameOfLife();
        StartSimulation();
    }

    #region public functions

    /// <summary>
    /// assign a random status (0 or 1) to all cells that are within the petridish
    /// this has to be replace by setting the colonies start positions
    /// </summary>
    public void SetUpRandomGameOfLife()
    {
        for (int x = 0; x < matrixSize; x++)
        {
            for (int y = 0; y < matrixSize; y++)
            {
                // if the cell is not outside the petridish make it alive or dead
                if (matrix_1[x, y].gol != 2)
                {
                    matrix_1[x, y].gol = UnityEngine.Random.Range(0, 2);
                }
            }
        }
    }
    /// <summary>
    /// Prepare the game: set the timer, define the actualMatrix (which Matrix is the one that should be used to render --> see PlayRound())
    /// Prepare the vitrusGenerator and make empty petridishes
    /// Function is called when you click the button Set Petri Dish
    /// </summary>
    public void PrepareEmptyDish()
    {
        timer = refreshTime;
        actualMatrix = true;
        vitrusGen.PrepareRenderer();
        PrepareMatrixes();
        // color the cells
        vitrusGen.SetRenderOutput(ConvertColCelMatrixToIntMatrix(toRender));
    }

    public void StartSimulation()
    {
        vitrusGen.SetRenderOutput(ConvertColCelMatrixToIntMatrix(toRender));
        paused = false;
    }

    public void PauseSimulation()
    {
        paused = true;
    }

    public void UnpauseSimulation()
    {
        paused = false;
    }

    // this function is called when you click the button Pause switch
    public void SwitchPauseSimulation()
    {
        paused = !paused;
    }

    #endregion
    /// <summary>
    /// Function is called in VitrusClicker to give a cell the status 1 (in both matrixes) if it is clicked
    /// and then colors the cell according to its new status
    /// </summary>
    /// <param name="x"> cell click position x</param>
    /// <param name="y"> cell click position y</param>
    public void SetAPointInMatrix(int x, int y)
    {
        matrix_1[x, y].gol = 1;
        matrix_2[x, y].gol = 1;
        vitrusGen.SetRenderOutput(ConvertColCelMatrixToIntMatrix(toRender));
    }


    private void PrepareMatrixes()
    {
        matrix_1 = MakePetriDishFromMatrix(matrixSize, radius);
        matrix_2 = MakePetriDishFromMatrix(matrixSize, radius);

        toRender = matrix_1;
    }
     /// <summary>
     /// Play the actual game (calculate the new cell status of a cell resulting from its neighbors in the first matrix and assign it to the second matrix)
     /// switch actualMatrix after each round because in the next round the updated matrix is the old one
     /// </summary>
    private void PlayRound()
    {
        if (actualMatrix)
        {
            // matrix 1 is from timestep t and used to calculate what happens in timestep t + 1 which is then assigned to matrix 2
            // matrix 2 is the result from this timestep and will be rendered to the screen
            GameOfLife.UpdateColCellStatus(matrix_1, matrix_2);
            toRender = matrix_2;
        }
        else
        {
            GameOfLife.UpdateColCellStatus(matrix_2, matrix_1);
            toRender = matrix_1;
        }
        // in the next timestep matrix 1 and 2 will be swapped
        actualMatrix = !actualMatrix;
    }

    /// <summary>
    /// create an empty matrix of ColCels and calculate which cells are within
    /// the petridish and which cells are outside
    /// assign the gol variable according to this status (2 = outside petridish, 0 = dead but within petridish)
    /// </summary>
    /// <param name="size">matrix size</param>
    /// <param name="radius">radius of petri dish [unit: matrix cells]</param>
    /// <returns>Petridishmatrix</returns>
    private ColCel[,] MakePetriDishFromMatrix(int size, int radius)
    {
        ColCel[,] matrix = new ColCel[size, size];
        
        // 1. initialize matrix with status 2 (status for the petridish)
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j].gol = 2;
            }
        }

        // 2. make the matrix a petriDish
        int center = (int)size / 2;
        // everything inside the radius gets the status 0 assigned 
        for (int x = center - radius; x <= center; x++)
        {
            for (int y = center - radius; y <= center; y++)
            {
                if ((x - center) * (x - center) + (y - center) * (y - center) < radius * radius)
                {
                    matrix[x, y].gol = 0;
                    matrix[x, center - y + center - 1].gol = 0;
                    matrix[center - x + center - 1, y].gol = 0;
                    matrix[center - (x - center) - 1, center - (y - center) - 1].gol = 0;
                }
            }
        }
        return matrix;
    }

    /// <summary>
    /// Convert the ColCel[,] matrix that you want to color to an int[,] matrix that can be 
    /// colored using the SetRenderOutput() function from the vitrusGenerator
    /// Here we convert it using the gol variable from the ColCel struct (values 0,1,2)
    /// </summary>
    /// <param name="toConvert">
    /// ColCel[,] matrix of timestep t that should be rendered
    /// </param>
    /// <returns>
    /// matrix with integers indicating which cell should get which color
    /// </returns>
    private int[,] ConvertColCelMatrixToIntMatrix(ColCel[,] toConvert)
    {
        int[,] toReturn = new int[matrixSize,matrixSize];
        for(int x = 0; x < matrixSize; x++)
        {
            for(int y = 0; y < matrixSize; y++)
            {
                toReturn[x, y] = toConvert[x, y].gol;
            }
        }
        return toReturn;

    }


}
