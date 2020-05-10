using UnityEngine;
using UnityEngine.EventSystems;

public class VitrusClicker : MonoBehaviour, IPointerDownHandler
{
    BattleManager battleManager;

    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }
    /// <summary>
    /// this function is triggered every time the player clicks somewhere.
    /// it detects where the player has clicked and if they clicked within the rectangle,
    /// the function SetAPointInMatirix from the battleManager is called and it changes the respective cell status of the clicked cell
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
       

        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;
        Debug.Log("LocalCursor:" + localCursor);

        // if pivot 0.0
        battleManager.SetAPointInMatrix(
            (int)(localCursor.y / (GetComponent<RectTransform>().rect.height / battleManager.matrixSize)),
            (int)(localCursor.x / (GetComponent<RectTransform>().rect.width / battleManager.matrixSize))
            );
    }

    
}
