using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBack : MonoBehaviour
{
    public GameObject cardBack;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void Update()
    {
        if (DisplayCard.staticCardBack == true) {
            cardBack.SetActive(true);
        } else {
            cardBack.SetActive(false);
        }
    }    
}
