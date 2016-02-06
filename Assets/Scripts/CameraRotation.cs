using UnityEngine;
using System.Collections;

public class CameraRotation : MonoBehaviour
{
    //Wystawiamy sobie zmienne w inspektorze
    [SerializeField]
    private float _mouseXSensivity;
    [SerializeField]
    private float _mouseYSensivity;
    [SerializeField]
    private Transform _cameraMinRotationRestriction;
    [SerializeField]
    private Transform _cameraMaxRotationRestriction;
    [SerializeField]
    private Color _highlightColor;

    //Katy eulera do ograniczania ruchu kamery
    private Vector3 _cameraMinRotationRestrictionEulerAngles;
    private Vector3 _cameraMaxRotationRestrictionEulerAngles;
    //Poprzednia pozycja myszki do wyliczania przesuniecia
    private Vector3 _previousMousePosition;

    //Obecnie wskazywany mesh
    private MeshRenderer _currentPointedObject;

    //Komponent kamery
    private Camera _camera;

    void Start()
    {
        _cameraMinRotationRestrictionEulerAngles = _cameraMinRotationRestriction.rotation.eulerAngles;
        _cameraMaxRotationRestrictionEulerAngles = _cameraMaxRotationRestriction.rotation.eulerAngles;
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_currentPointedObject != null)
            {
                GoToModelView();
                return;
            }
        }

        //Tworzymy promien od naszej pozycji do kursora myszki
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Sprawdzamy, czy nasz kochany promien w cos uderzyl
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider != null)
            {
                //Jak uderzyl, to probujemy wyciagnac mesha z tego ;)
                MeshRenderer mr = hit.collider.gameObject.GetComponent<MeshRenderer>();
                if(mr == null)
                {
                    //Jak sie nie udalo, to znaczy, ze nie pokazujemy juz na zaden obiekt i mozemy odznaczyc poprzednio podswietlony (o ile byl)
                    SetCurrentPointedObject(null);
                }
                else
                {
                    //Sprawdzamy, czy obiekt na ktory pokazujemy jest rozny od pokazywanego do tej pory
                    if(mr != _currentPointedObject)
                    {
                        SetCurrentPointedObject(mr);
                    }
                }
            }
        }
        else
        {
            //A jak nam sie nie udalo trafic w nic to odznaczamy obiekt podswietlany do tej pory (o ile byl)
            SetCurrentPointedObject(null);
        }
    }

    void LateUpdate()
    {
        //Pozycja myszki obecna, wyliczanie przesuniecia
        Vector3 mousePosition = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            //Pobieramy katy Eulera obecne
            Vector3 currentEulerAngles = transform.rotation.eulerAngles;
            Vector3 mouseDeltaPosition = mousePosition - _previousMousePosition;
            //Dodajemy przesuniecie pomnozone przez czas, ktory uplynal i czulosc myszki
            currentEulerAngles.x += mouseDeltaPosition.y * _mouseYSensivity * Time.deltaTime;
            currentEulerAngles.y += mouseDeltaPosition.x * _mouseXSensivity * Time.deltaTime;
            //Ograniczamy katy Eulera do min i max
            currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, _cameraMinRotationRestrictionEulerAngles.x, _cameraMaxRotationRestrictionEulerAngles.x);
            currentEulerAngles.y = Mathf.Clamp(currentEulerAngles.y, _cameraMinRotationRestrictionEulerAngles.y, _cameraMaxRotationRestrictionEulerAngles.y);
            //Ustawiamy rotacje (rotacja jest w kwaternionach wiec tworzymy taki z katow Eulera)
            transform.rotation = Quaternion.Euler(currentEulerAngles);
        }
        _previousMousePosition = mousePosition;
    }

    void SetCurrentPointedObject(MeshRenderer newCPO)
    {
        Transform parent = null;
        //Sprawdzamy, czy mamy zaznaczony obiekt, jesli tak to go odznaczamy
        if (_currentPointedObject != null)
        {
            _currentPointedObject.material.color = Color.white;
            //Sprawdzamy, czy obiekt ma rodzica (ktorego dzieci tez mogly zostac zaznaczone, np. zegar, tarcza itp)
            //Sprawdzamy rowniez, czy rodzicem nie jest pokoj caly, bo wtedy odznaczylibysmy caly pokoj :(
            parent = _currentPointedObject.transform.parent;
            if (parent != null && !parent.name.Contains("Room"))
            {
                //Odznaczamy te meshe tez
                MeshRenderer[] mrs = parent.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in mrs)
                {
                    mr.material.color = Color.white;
                }
            }
        }
        //Nowy zaznaczony obiekt
        _currentPointedObject = newCPO;
        if (_currentPointedObject != null)
        {
            //Zaznaczamy go wybranym kolorem
            _currentPointedObject.material.color = _highlightColor;
            //Sprawdzamy, czy ma rodzica
            //Sprawdzamy rowniez, czy rodzicem nie jest pokoj caly, bo wtedy zaznaczylibysmy caly pokoj :(
            parent = _currentPointedObject.transform.parent;
            if (parent != null && !parent.name.Contains("Room"))
            {
                //Pobieramy meshe i je zaznaczamy
                MeshRenderer[] mrs = parent.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in mrs)
                {
                    mr.material.color = _highlightColor;
                }
            }
        }
    }

    void GoToModelView()
    {
        //Sprawdzamy, czy zaznaczony obiekt ma komponent SelectableObject (potrzebny do uzyskania informacji o nazwie, opisie, miniaturce)
        SelectableObject so = _currentPointedObject.GetComponent<SelectableObject>();
        if(so == null)
        {
            //Jesli nie ma to szukamy u rodzica i u dzieci ewentualnie
            so = _currentPointedObject.GetComponentInParent<SelectableObject>();
            if(so == null)
            {
                so = _currentPointedObject.GetComponentInChildren<SelectableObject>();
            }
        }
        if(so != null)
        {
            //Jesli jest ten komponent znaleziony to przelaczamy widok
            WorldManager.Instance.GoToModelView(so);
        }
    }
}
