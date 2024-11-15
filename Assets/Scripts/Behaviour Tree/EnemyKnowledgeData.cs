using System.Collections.Generic;
using UnityEngine.Rendering;

public class EnemyKnowledgeData
{
    public Dictionary<int, Card> selfHandCardsDictionary;
    public int selfDeckCardsCount;
    public List<string> selfMergerList;
    public List<string> selfAttackTableList;
    public List<string> selfDefenceTableList;
    public int playerHandCardsCount;
    public int playerDeckCardsCount;
    public int playerMergerCardsCount;
    public List<string> playerAttackTableList;
    public List<string> playerDefenceTableList;
}