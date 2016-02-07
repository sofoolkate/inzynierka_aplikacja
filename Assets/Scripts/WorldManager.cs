using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;

    public static WorldManager Instance
    {
        //Zrobmy sobie singletona <3
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<WorldManager>();
                if(_instance == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<WorldManager>();
                    if(_instance == null)
                    {
                        Debug.LogError("An error has occured while creating WorldManager object");
                        Debug.Break();
                    }
                }
            }
            return _instance;
        }
    }

    //Lista pokoi
    [SerializeField]
    private List<Room> _rooms;
    //Widok modelu
    [SerializeField]
    private ModelView _modelView;
    [SerializeField]
    private RoomSelectionView _roomSelectionView;

    private int _activeRoom = 0;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            //Zabezpieczamy sie przed istnieniem dwoch menedzerow
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Wroc do menu na escape
            BackToMenu();
        }
    }

    public void GoToModelView(SelectableObject so)
    {
        //Wylacz widok pokoju
        _rooms[_activeRoom].gameObject.SetActive(false);
        //Wlacz widok modelu, przekaz mu wybrany obiekt oraz wszystkie obietky w tym pokoju
        _modelView.gameObject.SetActive(true);
        _modelView.SetCurrentObject(so);
        _modelView.SetList(_rooms[_activeRoom].Selectables);
    }

    public void BackToRoom()
    {
        //Wlacz widok pokoju
        _rooms[_activeRoom].gameObject.SetActive(true);
        //Wylacz widok modelu
        _modelView.gameObject.SetActive(false);
    }

    public void GoToRoom(Room r)
    {
        //Wylacz widok menu
        _roomSelectionView.gameObject.SetActive(false);
        //Sprawdz czy masz taki pokoj (powinien byc, ale ktoz to wie)
        _activeRoom = _rooms.IndexOf(r);
        if(_activeRoom < 0)
        {
            //Jak nie masz to dodaj sobie
            _rooms.Add(r);
            _activeRoom = _rooms.IndexOf(r);
        }
        //Wlacz pokoj
        r.gameObject.SetActive(true);
    }

    public void BackToMenu()
    {
        //Jesli jestes w widoku modelu to nie wracaj do menu
        if(_modelView.gameObject.activeInHierarchy)
        {
            return;
        }
        //Wylacz pokoj
        _rooms[_activeRoom].gameObject.SetActive(false);
        //Wlacz widok wyboru pokoju (menu)
        _roomSelectionView.gameObject.SetActive(true);
    }

    public List<Room> GetRoomList()
    {
        //Zwroc liste pokojow
        return _rooms;
    }
}