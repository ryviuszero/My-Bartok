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

    public void TakeTurn()
    {
        Utils.tr("Player.TakeTurn");

        if (type == ePlayerType.human) return;
        Bartok.S.phase = eTurnState.waiting;

        List<CardBartok> validCards = new List<CardBartok>();
        foreach (CardBartok cb in hand)
        {
            if (Bartok.S.ValidPlay(cb))
            {
                validCards.Add(cb);
            }
        }

        CardBartok card;
        if (validCards.Count == 0)
        {
            card = AddCard(Bartok.S.Draw());
            card.callbackPlayer = this;
            return;
        }

        card = validCards[Random.Range(0, validCards.Count)];
        RemoveCard(card);
        Bartok.S.MoveToTarget(card);
        card.callbackPlayer = this;
    }

    public void CBCallback(CardBartok card)
    {
        Utils.tr("Player.CBCallback()", card.name, "Player " + playerNum);
        Bartok.S.PassTurn();
    }

    public CardBartok RemoveCard(CardBartok card)
    {
        if (hand == null || !hand.Contains(card))
        {
            return null;
        }

        hand.Remove(card);
        FanHand();

        return card;
    }

    public void FanHand()
    {
        float staratRot = 0;
        staratRot = handSlotDef.rot;
        if (hand.Count > 1)
        {
            staratRot += Bartok.S.handFanDegress * (hand.Count - 1) / 2;
        }

        Vector3 pos;
        float rot;
        Quaternion rotQ;

        for (int i = 0; i < hand.Count; i++)
        {
            rot = staratRot - Bartok.S.handFanDegress * i;
            rotQ = Quaternion.Euler(0, 0, rot);

            pos = Vector3.up * CardBartok.CARD_HEIGHT / 2f;
            pos = rotQ * pos;
            pos += handSlotDef.pos;
            pos.z = -0.5f * i;

            if (Bartok.S.phase != eTurnState.idle)
            {
                hand[i].timeStart = 0;
            }

            hand[i].MoveTo(pos, rotQ);
            hand[i].state = eCardState.toHand;
            hand[i].faceUp = (type == ePlayerType.human);
            hand[i].eventualSortOrder = i * 4;
        }
    }

}
