using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace EgyptianWar
{
    public class Game : MonoBehaviour
    {
        public Text MessageText;

        protected CardAnimator cardAnimator;

        public Button PlayerDrop;
        public Button PlayerSlap;
        public Button ToLobby;

        [SerializeField]
        protected GameDataManager gameDataManager;

        public List<Transform> PlayerPositions = new List<Transform>();
        public List<Transform> PoolPositions = new List<Transform>();
        //public List<Transform> BookPositions = new List<Transform>();

        [SerializeField]
        protected Player localPlayer;
        [SerializeField]
        protected Player remotePlayer;
        [SerializeField]
        protected Pool Pool;

        [SerializeField]
        protected Player currentTurnPlayer;
        [SerializeField]
        protected Player currentTurnTargetPlayer;

        [SerializeField]
        protected Card selectedCard;
        [SerializeField]
        protected Ranks selectedRank;

        public enum GameState
        {
            Idle,
            GameStarted,
            TurnStarted,
            WaitingForDrop,
            TurnDrop,
            PlacedRoyal,
            TurnSelectingNumber,
            TurnConfirmedSelectedNumber,
            TurnWaitingForOpponentConfirmation,
            TurnOpponentConfirmed,
            TurnGoFish,
            GameFinished
        };

        [SerializeField]
        protected GameState gameState = GameState.Idle;

        protected byte cardValue;
        protected int tries;
        protected Card card;
        protected bool slapPossible;
        protected bool botSlapped;

        protected void Awake()
        {
            Debug.Log("base awake");
            localPlayer = new Player();
            localPlayer.PlayerId = "offline-player";
            localPlayer.PlayerName = "Player";
            localPlayer.Position = PlayerPositions[0].position;
            //localPlayer.BookPosition = BookPositions[0].position;

            remotePlayer = new Player();
            remotePlayer.PlayerId = "offline-bot";
            remotePlayer.PlayerName = "Bot";
            remotePlayer.Position = PlayerPositions[1].position;
            //remotePlayer.BookPosition = BookPositions[1].position;
            remotePlayer.IsAI = true;

            Pool.Position = PoolPositions[0].position;

            cardAnimator = FindObjectOfType<CardAnimator>();
            PlayerDrop = GameObject.Find("Canvas/PlayerDrop").GetComponent<Button>();
            PlayerSlap = GameObject.Find("Canvas/PlayerSlap").GetComponent<Button>();
            ToLobby = GameObject.Find("Canvas/Panel/Restart").GetComponent<Button>();
        }

        protected void Start()
        {
            gameState = GameState.GameStarted;
            GameFlow();
        }

        protected void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.Escape)) {
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
            }
            #endif
        }

        //****************** Game Flow *********************//
        public virtual void GameFlow()
        {
            if (gameState > GameState.GameStarted)
            {
                CheckPlayersCards();
                ShowAndHidePlayersDisplayingCards();
                botSlapped = false;

                if (gameDataManager.GameFinished())
                {
                    gameState = GameState.GameFinished;
                }
            }

            switch (gameState)
            {
                case GameState.Idle:
                    {
                        Debug.Log("IDLE");
                        break;
                    }
                case GameState.GameStarted:
                    {
                        Debug.Log("GameStarted");
                        OnGameStarted();
                        break;
                    }
                case GameState.TurnStarted:
                    {
                        Debug.Log("TurnStarted");
                        OnTurnStarted();
                        break;
                    }
                case GameState.WaitingForDrop:
                    {
                        Debug.Log("WaitingForDrop");
                        OnWaitingForDrop();
                        break;
                    }
                case GameState.TurnDrop:
                    {
                        Debug.Log("TurnDrop");
                        OnTurnDrop();
                        break;
                    }
                case GameState.PlacedRoyal:
                    {
                        Debug.Log("PlacedRoyal");
                        OnPlacedRoyal();
                        break;
                    }
                case GameState.GameFinished:
                    {
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
                    }
            }
        }

        protected virtual void OnGameStarted()
        {
            gameDataManager = new GameDataManager(localPlayer, remotePlayer);
            gameDataManager.Shuffle();
            gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer, Constants.PLAYER_INITIAL_CARDS);

            cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer, Constants.PLAYER_INITIAL_CARDS);

            gameState = GameState.TurnStarted;
        }

        protected virtual void OnTurnStarted()
        {
            SwitchTurn();
            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Your turn. Drop a card in the middle.");
                PlayerDrop.interactable = true;
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName}'s turn");
                PlayerDrop.interactable = false;
            }
            if (currentTurnPlayer.IsAI)
            {
                StartCoroutine(Wait3Seconds());
            }
            gameState = GameState.WaitingForDrop;
        }

        protected virtual void OnWaitingForDrop()
        {
            Debug.Log(currentTurnPlayer.PlayerName);
        }

        protected virtual void OnTurnDrop() //player needs to select top card and drop into middle pile, card dropped
        {   
            byte value = gameDataManager.AddTopCardToPool(currentTurnPlayer, Pool); //working
            cardValue = value;
            gameDataManager.RemoveTopCardFromPlayer(currentTurnPlayer); //working, gives 25 cards

            cardAnimator.AddCardToPool(currentTurnPlayer, Pool, value);

            slapPossible = TestSlapPossible(value);

            List<byte> playerCards = gameDataManager.PlayerCards(currentTurnPlayer);
            if ((playerCards.Count == 0) && (!slapPossible)) {
                gameState = GameState.GameFinished;
                GameFlow();
            }
            
            if (slapPossible) {
                StartCoroutine(BotSlap()); //slap for AI
                if (botSlapped == true) {
                    botSlapped = false;
                    GameFlow();
                }
            }
    
            //see if dropclicked came from a placed royal
            if (gameState == GameState.PlacedRoyal) { 
                if (currentTurnPlayer.IsAI) {
                }else { //local player turn
                    tries--;
                    if ((tries == 0) && (value > 3) && (value < 40) && (!slapPossible)) {
                        List<byte> poolOfCards = gameDataManager.PoolOfCards();
                        int iter = poolOfCards.Count;
                        while (iter != 0) {
                            byte card = poolOfCards.ElementAt(0);
                            byte val = gameDataManager.RemoveCardFromPool(Pool);
                            gameDataManager.AddCardValueToPlayer(currentTurnTargetPlayer, val);

                            cardAnimator.AddCardToPlayer(currentTurnTargetPlayer, Pool, val);
                            iter--;
                        }
                        gameState = GameState.TurnStarted;
                        GameFlow();
                    }
                    SetMessage($"{currentTurnPlayer.PlayerName} has {tries} tries left to play a royal card");
                }
            }
            
            //royal is placed
            if ((value >= 0 && value <= 3) || (value >= 40 && value <= 51)) { //doesnt matter what gamestate is
                gameState = GameState.PlacedRoyal;
                GameFlow();
            } else if (gameState == GameState.TurnDrop) {
                gameState = GameState.TurnStarted;
                GameFlow();
            }
        }

        protected virtual void OnPlacedRoyal()
        {
            SwitchTurn();
            if (currentTurnPlayer == localPlayer)
            {
                PlayerDrop.interactable = true;
            }
            else
            {
                PlayerDrop.interactable = false;
            }
            if ((cardValue >= 0) && (cardValue <= 3)) { // Ace, 4 tries
                tries = 4;
                SetMessage($"{currentTurnPlayer.PlayerName} has {tries} tries left to play a royal card");
            } else if ((cardValue >= 40) && (cardValue <= 43)) {
                tries = 1;
                SetMessage($"{currentTurnPlayer.PlayerName} has {tries} tries left to play a royal card");
            } else if ((cardValue >= 44) && (cardValue <= 47)) {
                tries = 2;
                SetMessage($"{currentTurnPlayer.PlayerName} has {tries} tries left to play a royal card");
            } else if ((cardValue >= 48) && (cardValue <= 51)) {
                tries = 3;
                SetMessage($"{currentTurnPlayer.PlayerName} has {tries} tries left to play a royal card");
            } 

            if (currentTurnPlayer.IsAI) {
                StartCoroutine(BotPlaceCards(tries));
            }
        }

        public void OnGameFinished()
        {
            if (gameDataManager.Winner() == localPlayer)
            {
                SetMessage($"You WON!");
            }
            else
            {
                SetMessage($"You LOST!");
            }
        }

        //******************* UI Methods *******************//
        protected virtual void DropClicked()
        {
            if (gameState == GameState.WaitingForDrop) {
                gameState = GameState.TurnDrop;
                GameFlow();
            } else if (gameState == GameState.PlacedRoyal) {
                OnTurnDrop();
            }
        }

        protected virtual void SlapClicked()
        {
            if (slapPossible) {
                //if AI, then dont switch
                if (currentTurnPlayer == localPlayer) {
                    SwitchTurn();
                }
                //gives cards to 
                List<byte> poolOfCards = gameDataManager.PoolOfCards();
                int iter = poolOfCards.Count;
                 while (iter != 0) {
                    byte card = poolOfCards.ElementAt(0);
                    byte value = gameDataManager.RemoveCardFromPool(Pool);
                    gameDataManager.AddCardValueToPlayer(localPlayer, value);

                    cardAnimator.AddCardToPlayer(localPlayer, Pool, value);
                    iter--;
                }
                gameState = GameState.TurnStarted;
                //GameFlow();
            }
        }

        public void RestartClicked()
        {
            SceneManager.LoadScene("LobbyScene");
        }

        //****************** AI Methods *************************//
        IEnumerator Wait3Seconds()
        {
            yield return new WaitForSeconds(3);
            if ((currentTurnPlayer.IsAI) && (botSlapped == false)) {
                DropClicked();
            }
        }

        IEnumerator BotPlaceCards(int tries)
        {
            bool royalPlaced = false;
            for (int i=tries; i>0; i--) {
                SetMessage($"{currentTurnPlayer.PlayerName} has {i} tries left to play a royal card");
                yield return new WaitForSeconds(3);
                List<byte> poolOfCards = gameDataManager.PoolOfCards();
                if (poolOfCards.Count == 0) { //just recently slapped
                    gameState = GameState.TurnStarted;
                    GameFlow();
                    break;
                }
                DropClicked();

                if ((cardValue >= 0 && cardValue <= 3) || (cardValue >= 40 && cardValue <= 51)) { // place royal
                    royalPlaced = true;
                    SwitchTurn();
                    gameState = GameState.PlacedRoyal;
                    GameFlow();
                    break;
                }
                /*
                if (slapPossible) {
                    GameFlow();
                    break;
                } 
                */
            }

            if (!(royalPlaced) && !(slapPossible)) { //at the end, if no slap is possible and no royal has been placed
                gameState = GameState.TurnStarted;
                SetMessage($"Giving cards to {currentTurnTargetPlayer.PlayerName}");
                List<byte> poolOfCards = gameDataManager.PoolOfCards();
                int iter = poolOfCards.Count;
                while (iter != 0) {
                    byte card = poolOfCards.ElementAt(0);
                    byte value = gameDataManager.RemoveCardFromPool(Pool);
                    gameDataManager.AddCardValueToPlayer(currentTurnTargetPlayer, value);

                    cardAnimator.AddCardToPlayer(currentTurnTargetPlayer, Pool, value);
                    iter--;
                }
                GameFlow();
            }
        }

        IEnumerator BotSlap()
        {
            yield return new WaitForSeconds(2);
            botSlapped = true;
            List<byte> poolOfCards = gameDataManager.PoolOfCards();
            if (poolOfCards.Count == 0) { // player already slapped
                Debug.Log("Darn");
                if (currentTurnPlayer == localPlayer) {
                    SwitchTurn();
                }
                gameState = GameState.TurnStarted;
                GameFlow();
            } else { // give cards to bot, they slapped
                Debug.Log("slapped");
                int iter = poolOfCards.Count;
                while (iter != 0) {
                    byte card = poolOfCards.ElementAt(0);
                    byte value = gameDataManager.RemoveCardFromPool(Pool);
                    gameDataManager.AddCardValueToPlayer(remotePlayer, value);

                    cardAnimator.AddCardToPlayer(remotePlayer, Pool, value);
                    iter--;
                }
                if (currentTurnPlayer.IsAI) {
                    SwitchTurn();
                }
                gameState = GameState.TurnStarted;
            }
        }
        //****************** Helper Methods *********************//
        public bool TestSlapPossible(byte value) {
            List<byte> poolOfCards = gameDataManager.PoolOfCards();
            int numberOfCards = poolOfCards.Count;
            if (numberOfCards == 1) {
                return false;
            }
            byte upper = 0; //upper #
            byte lower = 0; //lower #

            if (value % 4 == 0) {
                upper = (byte)((int)value + 3);
                lower = (byte)((int)value);
            } else if (value % 4 == 1) {
                upper = (byte)((int)value + 2);
                lower = (byte)((int)value - 1);
            } else if (value % 4 == 2) {
                upper = (byte)((int)value + 1);
                lower = (byte)((int)value - 2);
            } else if (value % 4 == 3) {
                upper = (byte)((int)value);
                lower = (byte)((int)value - 3);
            }

            if ((poolOfCards[numberOfCards - 2] >= lower) && (poolOfCards[numberOfCards - 2] <= upper)) {
                return true;
            } else if(numberOfCards == 2) {
                return false;
            }else if ((poolOfCards[numberOfCards - 3] >= lower) && (poolOfCards[numberOfCards - 3] <= upper)) {
                return true;
            }
            return false;
        }

        public void ResetSelectedCard()
        {
            if (selectedCard != null)
            {
                selectedCard.OnSelected(false);
                selectedCard = null;
                selectedRank = 0;
            }
        }

        protected void SetMessage(string message)
        {
            MessageText.text = message;
        }

        public void SwitchTurn()
        {
            if (currentTurnPlayer == null)
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
                return;
            }

            if (currentTurnPlayer == localPlayer)
            {
                currentTurnPlayer = remotePlayer;
                currentTurnTargetPlayer = localPlayer;
            }
            else
            {
                currentTurnPlayer = localPlayer;
                currentTurnTargetPlayer = remotePlayer;
            }
        }

        public void CheckPlayersCards()
        {
            List<byte> playerCardValues = gameDataManager.PlayerCards(localPlayer);
            localPlayer.SetCardValues(playerCardValues);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer);
            remotePlayer.SetCardValues(playerCardValues);
        }

        public void ShowAndHidePlayersDisplayingCards()
        {
            localPlayer.HideCardValues();
            remotePlayer.HideCardValues();
        }

        //****************** Animator Event *********************//
        public virtual void AllAnimationsFinished()
        {
            if ((gameState != GameState.PlacedRoyal) || (tries == 0)) {
                GameFlow();
            }
        }
    }
} 