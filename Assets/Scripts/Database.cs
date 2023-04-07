using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour //holds all of the internal cards
{
    public CardDatabase cards;
    private static Database instance;

    private void Awake() //create database
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public static Card GetItemByID(int Id) { //given a specific ID, find that card in cardDatabase and return it
        foreach(Card card in instance.cards.allCards) {
            if (card.id == Id) {
                return card;
            }
        }
        return null;
    }
}
