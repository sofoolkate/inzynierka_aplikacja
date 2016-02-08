using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//Pomocniczy enum
public enum RotateDirection
{
    Left,
    Right
}

public class ModelView : MonoBehaviour
{
    [SerializeField]
    private GameObject _modelObject;
    [SerializeField]
    private Text _description;
    [SerializeField]
    private GameObject _listObjectTemplate;
    [SerializeField]
    private RectTransform _content;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _movingSpeed;

    private GameObject _currentObject;
    private GameObject _currentAdditionalObject;
    private List<GameObject> _listObjects;

    private Vector2 _prevMousePosition;
    private RotateDirection _currentDirection;
    private bool _rotating;

    public void SetList(List<SelectableObject> selectables)
    {
        //Tworzymy liste obiektow
        _listObjects = new List<GameObject>();
        //Wyciagamy informacje o transformie szablonu elementu listy (tej po prawo)
        RectTransform templateRect = _listObjectTemplate.GetComponent<RectTransform>();
        for(int i = 0; i < selectables.Count; ++i)
        {
            //Tworzymy nowy obiekt na podstawie szablonu
            GameObject newSelectable = Instantiate(_listObjectTemplate);
            //Przypisujemy tego samego rodzica
            newSelectable.transform.parent = _listObjectTemplate.transform.parent;
            //Naprawiamy skale, Unity cos psuje
            newSelectable.transform.localScale = Vector3.one;
            //Ustawiamy pozycje
            newSelectable.GetComponent<RectTransform>().anchoredPosition3D = Vector3.down * (i * (templateRect.rect.height + 10) + 50);
            //I wymiary
            newSelectable.GetComponent<RectTransform>().sizeDelta = new Vector2(templateRect.rect.width, templateRect.rect.height);
            //Oraz uzupelniamy informacje na podstawie tego co juz wiemy o tym obiekcie (zeby UI wiedzialo co wyswietlic)
            ScrollViewItem svi = newSelectable.GetComponent<ScrollViewItem>();
            svi.Name.text = selectables[i].Name;
            svi.Thumbnail.texture = selectables[i].Thumbnail;
            svi.SelectableObject = selectables[i];
            //Wlaczamy obiekt
            newSelectable.SetActive(true);
            //Dodajemy do listy (WAZNE, dzieki temu mozna potem posprzatac ladnie po sobie :P )
            _listObjects.Add(newSelectable);
        }
        //Ustalamy rozmiar okienka sluzacego dla scroll view do przycinania widoku dobrego :)
        _content.sizeDelta = new Vector2(_content.rect.width, selectables.Count * (templateRect.rect.height + 10) + 50);
    }

    public void SetCurrentObject(SelectableObject so)
    {
        //Jesli juz jakis obiekt byl pokazywany to go usunmy
        if(_currentObject != null)
        {
            Destroy(_currentObject);
        }
        if(_currentAdditionalObject != null)
        {
            Destroy(_currentAdditionalObject);
        }
        if(so == null)
        {
            return;
        }
        //Stworzmy obiekt w oparciu o przeslane informacje (obiekt, mesh, rotacja), szablon (pozycje)
        _currentObject = (GameObject)Instantiate(so.gameObject, _modelObject.transform.position, so.transform.rotation);
        ChangeLayerRecursively(_currentObject, _modelObject.layer);
        //Ustawmy rodzica, pozwala zachowac porzadek w hierarchii
        _currentObject.transform.parent = _modelObject.transform.parent;
        //Odkolorujmy meshe
        MeshRenderer mr = _currentObject.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.material.color = Color.white;
        }
        else
        {
            //Jak potrzeba to dzieci tez odkolorujmy
            MeshRenderer[] mrs = _currentObject.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer tmp in mrs)
            {
                tmp.material.color = Color.white;
            }
        }
        //Ustawmy opis
        _description.text = so.Description;

        if(so.AdditionalObject != null)
        {
            //Obliczamy offset pozycji dodatkowego gameobjectu
            Vector3 offset = so.AdditionalObject.gameObject.transform.position - so.gameObject.transform.position;
            _currentAdditionalObject = (GameObject)Instantiate(so.AdditionalObject, _modelObject.transform.position + offset, so.AdditionalObject.transform.rotation);
            ChangeLayerRecursively(_currentAdditionalObject, _modelObject.layer);
            //Ustawmy rodzica, pozwala zachowac porzadek w hierarchii oraz zachowac wszelkie transformacje typu obiekt etc
            _currentAdditionalObject.transform.parent = _currentObject.transform;
            //Odkolorujmy meshe
            mr = _currentAdditionalObject.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.material.color = Color.white;
            }
            else
            {
                //Jak potrzeba to dzieci tez odkolorujmy
                MeshRenderer[] mrs = _currentAdditionalObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer tmp in mrs)
                {
                    tmp.material.color = Color.white;
                }
            }

            //Jesli ten drugi obiekt tez jest wybieralny to ma opis :)
            //Wiec doklejamy opis tego drugiego obiektu do pierwszego :)
            SelectableObject additionalSO = so.AdditionalObject.GetComponent<SelectableObject>();
            if(additionalSO != null)
            {
                _description.text += "\n\n";
                _description.text += additionalSO.Description;
            }
        }
    }

    void ChangeLayerRecursively(GameObject go, int layer)
    {
        //Jak jest nullem to co mam zmieniac :D
        if(go == null)
        {
            return;
        }
        //Zmiana warstwy (zeby kamera odpowiednia widziala obiekt)
        go.layer = layer;

        //Zrobmy to dla wszystkich dzieci
        foreach(Transform t in go.transform)
        {
            ChangeLayerRecursively(t.gameObject, layer);
        }
    }

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            Vector2 deltaPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _prevMousePosition;
            RotateObject(_rotationSpeed * -deltaPos.x * 0.5f);
        }

        float y = Input.GetAxis("Vertical");
        int dir = y > 0.0f ? 1 : (y < 0.0f ? - 1 : 0);
        MoveObject(dir);

        float scrollY = Input.mouseScrollDelta.y;
        Vector3 scale = Vector3.one;
        scale = _currentObject.transform.localScale;
        scale *= (1.0f + scrollY / 100.0f);
        _currentObject.transform.localScale = scale;

        _prevMousePosition = Input.mousePosition;
    }

    void RotateObject(float x)
    {
        //Obrot
        _currentDirection = x < 0.0f ? RotateDirection.Right : RotateDirection.Left;
        if (_currentObject != null)
        {
            //Space.World - obrot o os (0,1,0) w przestrzeni swiata, nie obiektu (wazne, ladnie sie teraz obraca)
            _currentObject.transform.Rotate(Vector3.up * x * Time.deltaTime, Space.World);
        }
    }

    void MoveObject(int dir)
    {
        if (_currentObject != null)
        {
            //Rusz obiektem w gore :)
            _currentObject.transform.Translate(Vector3.up * dir * _movingSpeed * Time.deltaTime, Space.World);
        }
    }

    public void OnExitClick()
    {
        //Wychodzimy, niszczymy obiekt pokazywany
        if (_currentObject != null)
        {
            Destroy(_currentObject);
        }
        //Niszczymy dodatkowy obiekt
        if (_currentAdditionalObject != null)
        {
            Destroy(_currentAdditionalObject);
        }
        //No i wszystkie z listy po prawo (mowilem, ze bedziemy sprzatac po sobie? :P )
        foreach(GameObject go in _listObjects)
        {
            Destroy(go);
        }
        //Wracamy do widoku pokoju
        WorldManager.Instance.BackToRoom();
    }
}
