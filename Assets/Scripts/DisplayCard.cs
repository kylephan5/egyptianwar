using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour
{
    // Start is called before the first frame update
    public Card card;

    public bool cardBack;
    public static bool staticCardBack;
    public Image imageFront;

    public GameObject Player1Hand;
    public GameObject Player1Card;
    public static int numberOfCardsInPlayer1Deck; // sets numberOfCards to be 26

    void Start()
    {
        numberOfCardsInPlayer1Deck = PlayerDeck.Player1Deck.Count;
        cardBack = true;
    }
 
    void Update() 
    {

        Player1Hand = GameObject.Find("Player1Panel");
        if (this.transform.parent == Player1Hand.transform.parent) {
            cardBack = true; // do not want to see cards in hand, keep them facedown
        }
        staticCardBack = cardBack;

        if (this.tag == "Clone") {
            int x = PlayerDeck.Player1Deck[numberOfCardsInPlayer1Deck - 1].id;
            GetItem(x);
            this.tag = "Untagged";
        }
        
    }

    public void GetItem(int Id)
    { 
        SetUI(Database.GetItemByID(Id));
    }

    private void SetUI(Card card) { //pass in a specific card object instance
        imageFront.sprite = card.imageFront; 
    }

    int j = 0;
    public void Player1DeckOnClick() {
        cardBack = false;
        if (j < PlayerDeck.Player1Deck.Count) {
            int x = PlayerDeck.Player1Deck[j].id;
            if ((staticCardBack) && (cardBack)) {
                Debug.Log("Not getting any cards.");
                return;
            }
            GetItem(x);
            PlayerDeck.Player1Deck.RemoveAt(j);
        } else {
            Debug.Log("No more cards");
        }
    }

    int i = 0;
    public void Player2DeckOnClick() {
        cardBack = false;
        if (i < PlayerDeck.Player2Deck.Count) {
            int x = PlayerDeck.Player2Deck[i].id;
            if ((staticCardBack) && (cardBack)) {
                Debug.Log("Not getting any cards.");
                return;
            }
            GetItem(x);
            PlayerDeck.Player2Deck.RemoveAt(i);
        } else {
            Debug.Log("No more cards");
        }
    }
}
