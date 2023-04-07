using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    
    public static int [] DeckAsInts = new int[52];
    /*
    public static List<int> Player1Deck = new List<int>();
    public static List<int> Player2Deck = new List<int>();
    */

    public Card card;
    // Start is called before the first frame update
    void Start()
    {
        for (int i =0; i < 52; i++) {
            DeckAsInts[i] = i;
        }
        Shuffle(DeckAsInts);

        for (int i=0; i < 52; i++) { //distribute cards to player decks
            if (i % 2 == 0) {
                PlayerDeck.Player1Deck.Add(Database.GetItemByID(DeckAsInts[i]));
            } else {
                PlayerDeck.Player2Deck.Add(Database.GetItemByID(DeckAsInts[i]));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Shuffle<T>(T[] array) {
        for (int i = array.Length; i > 1; i--) {
            // Pick random element to swap.
            int j = UnityEngine.Random.Range(0,i);
            // Swap.
            T tmp = array[j];
            array[j] = array[i - 1];
            array[i - 1] = tmp;
        }
    }
}
