using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] Texture2D cursorIcon;
    CursorMode cursorMode = CursorMode.ForceSoftware;
    [SerializeField] Vector2 cursorPosition;

    void Start()
    {
        Cursor.SetCursor(cursorIcon,cursorPosition, cursorMode);
        

    }

}
