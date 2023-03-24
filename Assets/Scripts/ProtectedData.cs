using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EgyptianWar
{
    /// <summary>
    /// Stores the important data of the game
    /// We will encypt the fields in a multiplayer game.
    /// </summary>
    [Serializable]
    public class ProtectedData
    {
        [SerializeField]
        List<byte> poolOfCards = new List<byte>();
        [SerializeField]
        List<byte> player1Cards = new List<byte>();
        [SerializeField]
        List<byte> player2Cards = new List<byte>();
        [SerializeField]
        string player1Id;
        [SerializeField]
        string player2Id;
        [SerializeField]
        string currentTurnPlayerId;
        [SerializeField]
        int currentGameState;
        [SerializeField]
        int selectedRank;

        byte[] encryptionKey;
        byte[] safeData;

        Card card;

        public ProtectedData(string p1Id, string p2Id, string roomId)
        {
            player1Id = p1Id;
            player2Id = p2Id;
            currentTurnPlayerId = "";
            selectedRank = (int)Ranks.NoRanks;
        }

        public void SetPoolOfCards(List<byte> cardValues)
        {
            poolOfCards = cardValues;
        }

        public List<byte> GetPoolOfCards()
        {
            List<byte> result;
            result = poolOfCards;
            return result;
        }

        public List<byte> PlayerCards(Player player)
        {
            List<byte> result;
            if (player.PlayerId.Equals(player1Id))
            {
                result = player1Cards;
            }
            else
            {
                result = player2Cards;
            }
            return result;
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.AddRange(cardValues);
            }
            else
            {
                player2Cards.AddRange(cardValues);
            }

        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.Add(cardValue);
            }
            else
            {
                player2Cards.Add(cardValue);
            }
        }

        public byte AddTopCardToPool(Player player, Pool pool)
        {
            byte cardToAdd;
            if (player.PlayerId.Equals(player1Id)) {
                cardToAdd = player1Cards.ElementAt(0);
                poolOfCards.Add(cardToAdd);
            } else {
                cardToAdd = player2Cards.ElementAt(0);
                poolOfCards.Add(cardToAdd);
            }
            return cardToAdd;
            
        }

        public void RemoveTopCardFromPlayer(Player player)
        {
            if (player.PlayerId.Equals(player1Id)) {
                player1Cards.RemoveAt(0);
            } else {
                player2Cards.RemoveAt(0);
            }
        }

        public byte RemoveCardFromPool(Pool pool)
        {
            byte cardToReturn;
            cardToReturn = poolOfCards.ElementAt(0);
            poolOfCards.RemoveAt(0);
            return cardToReturn;
        }

        public bool GameFinished()
        {
            bool result = false;

            if (player1Cards.Count == 0)
            {
                result = true;
            }

            if (player2Cards.Count == 0)
            {
                result = true;
            }

            return result;
        }

        public string WinnerPlayerId()
        {
            if (player2Cards.Count == 0)
            {
                string result = player1Id;
                return result;
            }
            else
            {
                string result = player2Id;
                return result;
            }

        }

        public void SetCurrentTurnPlayerId(string playerId)
        {
            currentTurnPlayerId = playerId;
        }

        public string GetCurrentTurnPlayerId()
        {
            string result;
            result = currentTurnPlayerId;
            return result;
        }

        public void SetGameState(int gameState)
        {
            currentGameState = gameState;
        }
        public int GetGameState()
        {
            int result;
            result = currentGameState;
            return result;
        }

        public void SetSelectedRank(int rank)
        {
            selectedRank = rank;
        }

        public int GetSelectedRank()
        {
            int result;
            result = selectedRank;
            return result;
        }

        public Byte[] ToArray()
        {
            return safeData;
        }

        public void ApplyByteArray(Byte[] byteArray)
        {
            safeData = byteArray;
        }
    }
}