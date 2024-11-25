using System.Collections.Generic;
using UnityEngine.Rendering;

public class EnemyKnowledgeData
{
    public Dictionary<int, Card> selfHandCardsDictionary;
    public int selfDeckCardsCount;
    public List<int> selfMergerList;
    public List<int> selfAttackTableList;
    public List<int> selfDefenceTableList;
    public int playerHandCardsCount;
    public int playerDeckCardsCount;
    public int playerMergerCardsCount;
    public List<int> playerAttackTableList;
    public List<int> playerDefenceTableList;
}