﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EgyptianWar
{
    [Serializable]
    public class EncryptedData
    {
        public byte[] data;
    }

    [Serializable]
    public class GameDataManager
    {
        Player localPlayer;
        Player remotePlayer;

        [SerializeField]
        ProtectedData protectedData;

        public GameDataManager(Player local, Player remote, string roomId = "1234567890123456")
        {
            localPlayer = local;
            remotePlayer = remote;
            protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer.PlayerId, roomId);
        }

        public void Shuffle()
        {
            List<byte> cardValues = new List<byte>();

            for (byte value = 0; value < 52; value++)
            {
                cardValues.Add(value);
            }

            List<byte> poolOfCards = new List<byte>();

            for (int index = 0; index < 52; index++)
            {
                int valueIndexToAdd = UnityEngine.Random.Range(0, cardValues.Count);

                byte valueToAdd = cardValues[valueIndexToAdd];
                poolOfCards.Add(valueToAdd);
                cardValues.Remove(valueToAdd);
            }

            protectedData.SetPoolOfCards(poolOfCards);
        }

        public void DealCardValuesToPlayer(Player player, int numberOfCards)
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;
            int start = numberOfCardsInThePool - numberOfCards;

            List<byte> cardValues = poolOfCards.GetRange(start, numberOfCards);
            poolOfCards.RemoveRange(start, numberOfCards);

            protectedData.AddCardValuesToPlayer(player, cardValues);
            protectedData.SetPoolOfCards(poolOfCards);
        }

        public byte DrawCardValue()
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;

            if (numberOfCardsInThePool > 0)
            {
                byte cardValue = poolOfCards[numberOfCardsInThePool - 1];
                poolOfCards.Remove(cardValue);
                protectedData.SetPoolOfCards(poolOfCards);
                return cardValue;
            }

            return Constants.POOL_IS_EMPTY;
        }

        public List<byte> PlayerCards(Player player)
        {
            return protectedData.PlayerCards(player);
        }

        public List<byte> PoolOfCards()
        {
            return protectedData.GetPoolOfCards();
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            protectedData.AddCardValuesToPlayer(player, cardValues);
        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
            protectedData.AddCardValueToPlayer(player, cardValue);
        }

        public byte AddTopCardToPool(Player player, Pool pool) //for onturndrop
        {
            return protectedData.AddTopCardToPool(player, pool);
        }

        public void RemoveTopCardFromPlayer(Player player)
        {
            protectedData.RemoveTopCardFromPlayer(player);
        }

        public byte RemoveCardFromPool(Pool pool)
        {
            return protectedData.RemoveCardFromPool(pool);
        }

        public Player Winner()
        {
            string winnerPlayerId = protectedData.WinnerPlayerId();
            if (winnerPlayerId.Equals(localPlayer.PlayerId))
            {
                return localPlayer;
            }
            else
            {
                return remotePlayer;
            }
        }

        public bool GameFinished()
        {
            return protectedData.GameFinished();
        }

        public void SetCurrentTurnPlayer(Player player)
        {
            protectedData.SetCurrentTurnPlayerId(player.PlayerId);
        }

        public Player GetCurrentTurnPlayer()
        {
            string playerId = protectedData.GetCurrentTurnPlayerId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else
            {
                return remotePlayer;
            }
        }

        public Player GetCurrentTurnTargetPlayer()
        {
            string playerId = protectedData.GetCurrentTurnPlayerId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return remotePlayer;
            }
            else
            {
                return localPlayer;
            }
        }

        public void SetGameState(Game.GameState gameState)
        {
            protectedData.SetGameState((int)gameState);
        }

        public Game.GameState GetGameState()
        {
            return (Game.GameState)protectedData.GetGameState();
        }

        public void SetSelectedRank(Ranks rank)
        {
            protectedData.SetSelectedRank((int)rank);
        }

        public Ranks GetSelectedRank()
        {
            return (Ranks)protectedData.GetSelectedRank();
        }

        public EncryptedData EncryptedData()
        {
            Byte[] data = protectedData.ToArray();

            EncryptedData encryptedData = new EncryptedData();
            encryptedData.data = data;

            return encryptedData;
        }

        public void ApplyEncrptedData(EncryptedData encryptedData)
        {
            if(encryptedData == null)
            {
                return;
            }

            protectedData.ApplyByteArray(encryptedData.data);
        }
    }
}
