using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EgyptianWar
{
    /// <summary>
    /// Manages the positions of the player's cards
    /// </summary>
    [Serializable]
    public class Player : IEquatable<Player>
    {
        public string PlayerId;
        public string PlayerName;
        public bool IsAI;
        public Vector2 Position;

        int numberOfDisplayingCards;

        public List<Card> DisplayingCards = new List<Card>();

        public Vector2 NextCardPosition()
        {
            Vector2 nextPos = Position + Vector2.right * Constants.PLAYER_CARD_POSITION_OFFSET * numberOfDisplayingCards;
            return nextPos;
        }

        public void SetCardValues(List<byte> values)
        {
            if (DisplayingCards.Count != values.Count)
            {
                Debug.LogError($"Displaying cards count {DisplayingCards.Count} is not equal to card values count {values.Count} for {PlayerId}");
                return;
            }

            for (int index = 0; index < values.Count; index++)
            {
                Card card = DisplayingCards[index];
                card.SetCardValue(values[index]);
            }
        }

        public void HideCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(false);
            }
        }

        public void ShowCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(true);
            }
        }
        
        public void ShowCardValue(Card card)
        {
            card.SetFaceUp(true);
        }
        
        public void HideCardValue(Card card)
        {
            card.SetFaceUp(false);
        }

        public void ReceiveDisplayingCard(Card card)
        {
            DisplayingCards.Add(card);
            card.OwnerId = PlayerId;
            numberOfDisplayingCards++;
        }

        public Card GiveTopCard()
        {
            Card cardToReturn = DisplayingCards[0];
            ShowCardValue(cardToReturn);
            DisplayingCards[0].OwnerId = null;
            DisplayingCards.RemoveAt(0);
            numberOfDisplayingCards--;

            return cardToReturn;
        }

        public void RepositionDisplayingCards(CardAnimator cardAnimator)
        {
            numberOfDisplayingCards = 0;
            foreach (Card card in DisplayingCards)
            {
                numberOfDisplayingCards++;
                cardAnimator.AddCardAnimation(card, NextCardPosition());
            }
        }

        public void SendDisplayingCardToPlayer(Player receivingPlayer, CardAnimator cardAnimator, List<byte> cardValues, bool isLocalPlayer)
        {
            int playerDisplayingCardsCount = DisplayingCards.Count;

            if (playerDisplayingCardsCount < cardValues.Count)
            {
                Debug.LogError("Not enough displaying cards");
                return;
            }

            for (int index = 0; index < cardValues.Count; index++)
            {

                Card card = null;
                byte cardValue = cardValues[index];

                if (isLocalPlayer)
                {
                    foreach (Card c in DisplayingCards)
                    {
                        if (c.Rank == Card.GetRank(cardValue) && c.Suit == Card.GetSuit(cardValue))
                        {
                            card = c;
                            break;
                        }
                    }
                }
                else
                {
                    card = DisplayingCards[playerDisplayingCardsCount - 1 - index];
                    card.SetCardValue(cardValue);
                    card.SetFaceUp(true);
                }

                if(card != null)
                {
                    DisplayingCards.Remove(card);
                    receivingPlayer.ReceiveDisplayingCard(card);
                    cardAnimator.AddCardAnimation(card, receivingPlayer.NextCardPosition());
                    numberOfDisplayingCards--;
                }
                else
                {
                    Debug.LogError("Unable to find displaying card.");
                }
            }

            RepositionDisplayingCards(cardAnimator);
        }

        public bool Equals(Player other)
        {
            if (PlayerId.Equals(other.PlayerId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
