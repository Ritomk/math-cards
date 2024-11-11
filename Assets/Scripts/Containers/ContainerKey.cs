using System;
using System.Collections.Generic;
using UnityEngine;

public enum OwnerType
{
    Player,
    Enemy
}

public enum CardContainerType
{
    Hand,
    Deck,
    AttackTable,
    DefenceTable,
    Merger
}

[Serializable]
public struct ContainerKey : IEquatable<ContainerKey>
{
    public OwnerType OwnerType;
    public CardContainerType ContainerType;

    public ContainerKey(OwnerType ownerType, CardContainerType containerType)
    {
        OwnerType = ownerType;
        ContainerType = containerType;
        
    }

    public bool Equals(ContainerKey other)
    {
        return OwnerType == other.OwnerType && ContainerType == other.ContainerType;
    }

    public override bool Equals(object obj)
    {
        return obj is ContainerKey other && Equals(other);
    }
}