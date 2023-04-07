
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [CreateAssetMenu(fileName="New Card Database", menuName = "Assets/Databases/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public List<Card> allCards; //stores all cards in card database asset
}

