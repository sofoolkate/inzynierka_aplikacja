using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Room : MonoBehaviour
{
    //Lista obiektow do wybrania
    public List<SelectableObject> Selectables
    {
        get;
        private set;
    }

    void Start()
    {
        //Ustawienie tej listy poprzez znalezienie wszystkich takich komponentow u dzieci (dlatego ten skrypt musi byc nadrzedny dla pokoju, np. obiektu Room1)
        Selectables = new List<SelectableObject>(GetComponentsInChildren<SelectableObject>());
    }
}
