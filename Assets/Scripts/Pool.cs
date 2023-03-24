using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EgyptianWar
{
    [Serializable]
    public class Pool
    {
        public Vector2 Position;

        public List<Card> DisplayingCards = new List<Card>();

        int numberOfCardsInPool;

        public void ReceivePlayerCard(Card card)
        {
            DisplayingCards.Add(card);
            card.OwnerId = null;
            numberOfCardsInPool++;
        }

        public Card GiveTopCard(Player player)
        {
            if(numberOfCardsInPool == 0) {
                return null;
            }
            Card cardToReturn = DisplayingCards[0];
            HideCardValue(cardToReturn);
            DisplayingCards[0].OwnerId = player.PlayerId;
            DisplayingCards.RemoveAt(0);
            numberOfCardsInPool--;

            return cardToReturn;
        }

        public void HideCardValue(Card card)
        {
            card.SetFaceUp(false);
        }
    }
}
