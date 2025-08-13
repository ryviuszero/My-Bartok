using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ePlayerType
{
    human,
    ai
}

[System.Serializable]
public class Player
{
    public ePlayerType type = ePlayerType.ai;
    public int playerNum;
    public SlotDef handSlotDef;

    public List<CardBartok> hand;

    public CardBartok AddCard(CardBartok card)
    {
        if (hand == null)
        {
            hand = new List<CardBartok>();
        }

        hand.Add(card);

        if (type == ePlayerType.human)
        {
            CardBartok[] cards = hand.ToArray();
            cards = cards.OrderBy(cd => cd.rank).ToArray();

            hand = new List<CardBartok>(cards);
        }

        card.SetSortingLayerName("10");
        card.eventualSortLayer = handSlotDef.layerName;

        FanHand();

        return card;
    }

    public void FanHand()
    {
        float staratRot = 0;
        staratRot = handSlotDef.rot;
        if (hand.Count > 1)
        {
            // staratRot += Bartok.S.
        }
    }

}
