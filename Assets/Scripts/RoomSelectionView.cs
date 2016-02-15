using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RoomSelectionView : MonoBehaviour
{
    [SerializeField]
    private RoomSelectionUIItem _itemTemplate;
    [SerializeField]
    private RectTransform _content;
    [SerializeField]
    private Texture2D _commingSoonTexture;

    void Start()
    {
        //Pobieramy komponent RectTransform
        RectTransform templateRect = _itemTemplate.GetComponent<RectTransform>();
        //I liste pokoi(jow?)
        List<Room> roomList = WorldManager.Instance.GetRoomList();
        _content.sizeDelta = new Vector2((roomList.Count + 1) * (templateRect.rect.width + 40), _content.rect.height);
        //Wyznaczamy poczatkowa pozycje kafelkow do wyboru
        Vector3 initPos = new Vector3(-_content.rect.width / 2.0f + templateRect.rect.width / 2.0f + 10.0f, 0.0f, 0.0f);
        //I koncowa
        Vector3 endPos = new Vector3(_content.rect.width / 2.0f + templateRect.rect.width / 2.0f + 10.0f, 0.0f, 0.0f);
        //I roznice pozycji miedzy dwoma kafelkami
        Vector3 deltaPos = endPos - initPos;
        deltaPos /= roomList.Count;
        for (int i = 0; i < roomList.Count; ++i)
        {
            //Tworzymy nowy kafelek pokoju
            GameObject newRoom = Instantiate(_itemTemplate.gameObject);
            //Ustawiamy rodzica
            newRoom.transform.parent = _itemTemplate.transform.parent;
            //I skale, bo to Unity
            newRoom.transform.localScale = Vector3.one;
            //I pozycje
            RectTransform newRoomRect = newRoom.GetComponent<RectTransform>();
            newRoomRect.anchoredPosition3D = initPos + deltaPos * i;
            //I wymiary
            newRoomRect.sizeDelta = new Vector2(templateRect.rect.width, templateRect.rect.height);
            //I pokoj
            RoomSelectionUIItem rsuii = newRoom.GetComponent<RoomSelectionUIItem>();
            rsuii.Room = roomList[i];
            //I nazwe
            rsuii.NameText.text = rsuii.Room.Name;
            //I miniaturke
            rsuii.Thumbnail.texture = rsuii.Room.Thumbnail;
            //I wlaczamy go
            newRoom.SetActive(true);
        }
        //Tworzymy dodatkowy sobie do wyswietlenia - comming soon, na tej samej zasadzie co wyzej
        /*
        GameObject commingSoonRoom = Instantiate(_itemTemplate.gameObject);
        commingSoonRoom.transform.parent = _itemTemplate.transform.parent;
        commingSoonRoom.transform.localScale = Vector3.one;
        RectTransform commingSoonRoomRect = commingSoonRoom.GetComponent<RectTransform>();
        commingSoonRoomRect.anchoredPosition3D = initPos + deltaPos * roomList.Count;
        commingSoonRoomRect.sizeDelta = new Vector2(templateRect.rect.width, templateRect.rect.height);
        RoomSelectionUIItem commingSoonRSUUI = commingSoonRoom.GetComponent<RoomSelectionUIItem>();
        commingSoonRSUUI.Room = null;
        commingSoonRSUUI.NameText.text = string.Empty;
        commingSoonRSUUI.Thumbnail.texture = _commingSoonTexture;
        //Usuwamy event od klikniecia ;) nie chcemy przeciez klikac w CommingSoon, prawda? :)
        commingSoonRSUUI.GetComponent<EventTrigger>().triggers.RemoveAll(
            (trigger) => {return trigger.eventID == EventTriggerType.PointerUp;}    //Predykaty, predykaty, usuwamy wtedy, gdy typ evenetu jest PointerUp, coby nie kliknac :)
            );
        commingSoonRoom.SetActive(true);*/
    }
}
