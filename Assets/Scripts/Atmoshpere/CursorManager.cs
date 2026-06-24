using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;

    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        SetCursor();
    }

    public void SetCursor()
    {
        Cursor.SetCursor(
            cursorTexture,
            Vector2.zero,
            cursorMode
        );
    }
}