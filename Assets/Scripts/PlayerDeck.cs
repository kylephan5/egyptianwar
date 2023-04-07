using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public static List<Card> Player1Deck = new List<Card>(); // stores cards datatype in list
    public static List<Card> Player2Deck = new List<Card>();

    public GameObject CardToHand;
    public GameObject[] Clones;
    public GameObject Hand;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator StartGame() {
        for (int i=0; i < 26; i++) {
            yield return new WaitForSeconds(0);
            Instantiate(CardToHand,transform.position, transform.rotation);
        }
    }
}
