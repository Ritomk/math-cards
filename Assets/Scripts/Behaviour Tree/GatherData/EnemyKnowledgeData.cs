using System.Collections.Generic;
using UnityEngine.Rendering;

public class EnemyKnowledgeData
{
    public Dictionary<int, Card> selfHandCardsDictionary;
    public int selfDeckCardsCount;
    public List<float> selfMergerList;
    public List<float> selfAttackTableList;
    public List<float> selfDefenceTableList;
    public int playerHandCardsCount;
    public int playerDeckCardsCount;
    public int playerMergerCardsCount;
    public List<float> playerAttackTableList;
    public List<float> playerDefenceTableList;
}