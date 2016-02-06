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
}