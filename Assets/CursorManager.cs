    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

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
    void Start()
    {
        cursorHotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        crosshairHotSpot = new Vector2(crosshairTexture.width / 2, crosshairTexture.height / 2);
        Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
    }




        // Update is called once per frame
    }
