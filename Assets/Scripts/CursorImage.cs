using UnityEngine;
using System.Collections;

public class CursorImage : MonoBehaviour
{
    [SerializeField]
    private Texture2D _cursorTexture;

    void Start()
    {
        //Ustawianie tekstury kursora (zakladam cos ciekawszego niz Windowsowy, w razie czego zakomentowac), przesuniecie go o polowe wspolrzednych tekstury (zeby srodek tekstury byl pointerem) 
        Cursor.SetCursor(_cursorTexture, new Vector2(_cursorTexture.width, _cursorTexture.height) * 0.5f, CursorMode.Auto);
    }
}
