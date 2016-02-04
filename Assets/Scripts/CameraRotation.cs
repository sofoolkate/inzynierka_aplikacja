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
                    if (_currentPointedObject != null)
                    {
                        _currentPointedObject.material.color = Color.white;
                    }
                    _currentPointedObject = null;
                }
                else
                {
                    //Sprawdzamy, czy obiekt na ktory pokazujemy jest rozny od pokazywanego do tej pory
                    if(mr != _currentPointedObject)
                    {
                        //Jesli tak, to odznaczamy poprzedni obiekt (o ile byl)
                        if(_currentPointedObject != null)
                        {
                            _currentPointedObject.material.color = Color.white;
                        }
                        //Zaznaczamy nowy obiekt
                        _currentPointedObject = mr;
                        _currentPointedObject.material.color = _highlightColor;
                    }
                }
            }
        }
        else
        {
            //A jak nam sie nie udalo trafic w nic to odznaczamy obiekt podswietlany do tej pory (o ile byl)
            if (_currentPointedObject != null)
            {
                _currentPointedObject.material.color = Color.white;
            }
            _currentPointedObject = null;
        }
    }

    void LateUpdate()
    {
        //Pobieramy katy Eulera obecne
        Vector3 currentEulerAngles = transform.rotation.eulerAngles;
        //Pozycja myszki obecna, wyliczanie przesuniecia
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseDeltaPosition = mousePosition - _previousMousePosition;
        _previousMousePosition = mousePosition;
        //Dodajemy przesuniecie pomnozone przez czas, ktory uplynal i czulosc myszki
        currentEulerAngles.x += mouseDeltaPosition.y * _mouseYSensivity * Time.deltaTime;
        currentEulerAngles.y += mouseDeltaPosition.x * _mouseXSensivity * Time.deltaTime;
        //Ograniczamy katy Eulera do min i max
        currentEulerAngles.x = Mathf.Clamp(currentEulerAngles.x, _cameraMinRotationRestrictionEulerAngles.x, _cameraMaxRotationRestrictionEulerAngles.x);
        currentEulerAngles.y = Mathf.Clamp(currentEulerAngles.y, _cameraMinRotationRestrictionEulerAngles.y, _cameraMaxRotationRestrictionEulerAngles.y);
        //Ustawiamy rotacje (rotacja jest w kwaternionach wiec tworzymy taki z katow Eulera)
        transform.rotation = Quaternion.Euler(currentEulerAngles);
    }
}
