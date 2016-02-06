using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollViewItem : MonoBehaviour
{
    //Referencja do widoku modelu
    public ModelView ModelView;
    //Referencja do nazwy
    public Text Name;
    //Do miniaturki
    public RawImage Thumbnail;
    //No i do obiektu na ktory wskazuje, ale to ukrywamy w inspektorze (przypisujemy w ModelView.cs)
    [HideInInspector]
    public SelectableObject SelectableObject;

    public void OnSelectObject()
    {
        //Wywolywane gdy nacisniemy ten obiekt na liscie, przelaczamy model na ten "klikniety"
        ModelView.SetCurrentObject(SelectableObject);
    }
}
