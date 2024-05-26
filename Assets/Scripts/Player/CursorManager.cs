using UnityEngine;

namespace Player {
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] public Texture2D cursorTexture;
        public Vector2 cursorHotSpot;
        [SerializeField] public Texture2D crosshairTexture;
        public Vector2 crosshairHotSpot;
        // Start is called before the first frame update
        public void SetCursor(Texture2D newCursorTexture, Vector2 hotSpot)
        {
            cursorHotSpot = hotSpot;
            Cursor.SetCursor(newCursorTexture, cursorHotSpot, CursorMode.Auto);
        }
        void Start() {
            cursorTexture = Resources.Load<Texture2D>("Art/Cursors/Cursor Pointer");
            crosshairTexture = Resources.Load<Texture2D>("Art/Cursors/47");
            cursorHotSpot = new Vector2(0, 0);
            crosshairHotSpot = new Vector2(crosshairTexture.width / 2, crosshairTexture.height / 2);
            Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
        }
    }
}
