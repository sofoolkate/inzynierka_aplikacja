using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomSelectionUIItem : MonoBehaviour
{
    public Text NameText;
    public RawImage Thumbnail;

    [HideInInspector]
    public Room Room;

    public void OnSelect()
    {
        //W momencie klikniecia idz do pokoju
        WorldManager.Instance.GoToRoom(Room);
    }
}
