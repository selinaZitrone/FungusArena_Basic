using UnityEngine;
using UnityEngine.UI;

public class Vitrus_Genenator_2 : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image myImage;
    Texture2D texture;
    Sprite mySprite;
    int textureWidth = 256;
    int textureHeight = 256;

    public Color[] colors;

    [Header("Game of Live vars")]
    private int matrixSize = 32;
    private int cellSize = 4;

    BattleManager battleManager;

    // on start, get the matrix size and the cell size from the battleManager
    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        matrixSize = battleManager.matrixSize;
        cellSize = battleManager.cellSize;
    }

    void Update()
    {
       
    }

    /// <summary>
    /// creates the Texture2D and assign it to the SpriteRenderer
    /// Is called in BattleManager.cs (function PrepareEmptyDish())
    /// </summary>
    public void PrepareRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myImage = GetComponent<Image>();
        textureWidth = matrixSize * cellSize;
        textureHeight = matrixSize * cellSize;
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        Rect myRect = new Rect(0, 0, textureWidth, textureHeight);
        mySprite = Sprite.Create(texture, myRect, new Vector2(.5f, .5f));
        myImage.sprite = mySprite;
    }

    /// <summary>
    /// set the color or each cell based on the value of this cell in the integer matrix
    /// loops through the matrix cell by cell and in each iteration assigns a color
    /// to the number of pixels (cellSize * cellSize) that belong to this matrix cell.
    /// Is called in BattleManger.cs
    /// </summary>
    /// <param name="toRender">
    /// matrix of integers, each cell indicates which color the cell should get
    /// the matrix is converted from a ColCel[,] matrix to an int[,] matrix in BattleManager.ConvertColCelMatrixToIntMatrix()
    /// and contains the integer of the gol variable, values 0,1 or two
    /// </param>
    public void SetRenderOutput(int[,] toRender)
    {
        Color[] cols = texture.GetPixels();

        for (int x = 0; x < matrixSize; x++)
        {
            for (int y = 0; y < matrixSize; y++)
            {
                for (int xa = x * cellSize; xa < x * cellSize + cellSize; xa++)
                {
                    for (int ya = y * cellSize; ya < y * cellSize + cellSize; ya++)
                    {
                        cols[(xa * (matrixSize * cellSize)) + ya] = colors[toRender[x, y]];
                    }
                }
            }
        }
        // take the color matrix cols and update the pixel colors
        texture.SetPixels(cols);
        // upload the cahnged pixels to the graphics card
        texture.Apply();
    }
}
