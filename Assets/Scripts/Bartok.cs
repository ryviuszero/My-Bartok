using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eTurnState
{
    idle,
    pre,
    waiting,
    post,
    gameOver
}

public class Bartok : MonoBehaviour
{
    static public Bartok S;
    static public Player CURRENT_PLAYER;

    [Header("Set in Inspector")]
    public TextAsset DeckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    public float handFanDegress = 10f;
    public float drawTimeStagger = 0.25f;
    public int numStartingCards = 7;

    [Header("Set Dynamically")]
    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;
    public List<Player> players;
    public CardBartok targetCard;
    public eTurnState phase = eTurnState.idle;
    private Layout layout;
    private Transform layoutAnchor;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(DeckXML.text);
        Deck.Shuffle(ref deck.cards);

        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);

        drawPile = UpgradeCardsList(deck.cards);
        LayoutGame();

    }

    public void LayoutGame()
    {
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        ArrangeDrawPile();

        Player pl;
        players = new List<Player>();
        foreach (SlotDef tSD in layout.slotDefs)
        {
            pl = new Player();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = tSD.player;
        }

        players[0].type = ePlayerType.human;

        CardBartok card;
        for (int i = 0; i < numStartingCards; i++)
        {
            // For each player
            for (int j = 0; j < 4; j++)
            {
                card = Draw();
                card.timeStart = Time.time + drawTimeStagger * (i * 4 + j);

                players[(j + 1) % 4].AddCard(card);

            }


        }

        Invoke("DrawFirstTarget", drawTimeStagger * numStartingCards * 4 + 4);


    }

    public void DrawFirstTarget()
    {
        CardBartok card = MoveToTarget(Draw());

        card.reportFinishTo = gameObject;
    }

    public void CBCallback(CardBartok card)
    {
        Utils.tr("Bartok:CBCallback():", card.name);
        StartGame();
    }

    public void StartGame()
    {
        PassTurn(1);
    }

    public void PassTurn(int num = -1)
    {
        if (num == -1)
        {
            int ndx = players.IndexOf(CURRENT_PLAYER);
            num = (ndx + 1) % 4;
        }

        int lastPlayerNum = -1;
        if (CURRENT_PLAYER != null)
        {
            lastPlayerNum = CURRENT_PLAYER.playerNum;

            if (CheckGameOver()) return;
        }

        CURRENT_PLAYER = players[num];
        phase = eTurnState.pre;

        CURRENT_PLAYER.TakeTurn();

        Utils.tr("Bartok:PassTurn():", "Old: " + lastPlayerNum, "New " + CURRENT_PLAYER.playerNum);
    }

    public bool ValidPlay(CardBartok card)
    {
        if (card.rank == targetCard.rank) return true;
        if (card.suit == targetCard.suit) return true;

        return false;
    }

    public bool CheckGameOver()
    {
        if (drawPile.Count == 0)
        {
            List<Card> cards = new List<Card>();
            foreach (CardBartok card in discardPile)
            {
                cards.Add(card);
            }

            discardPile.Clear();
            Deck.Shuffle(ref cards);
            drawPile = UpgradeCardsList(cards);
            ArrangeDrawPile();
        }

        if (CURRENT_PLAYER.hand.Count == 0)
        {
            phase = eTurnState.gameOver;
            Invoke("RestartGame", 1);
            return true;
        }

        return false;
    }


    public CardBartok MoveToTarget(CardBartok card)
    {
        card.timeStart = 0;
        card.MoveTo(layout.discardPile.pos + Vector3.back);
        card.state = eCardState.toTarget;
        card.faceUp = true;

        card.SetSortingLayerName("10");
        card.eventualSortLayer = layout.target.layerName;

        if (targetCard != null)
        {
            MoveToDiscard(targetCard);
        }

        targetCard = card;
        return card;
    }

    public void MoveToDiscard(CardBartok card)
    {
        card.state = eCardState.discard;

        discardPile.Add(card);

        card.SetSortingLayerName(layout.discardPile.layerName);
        card.SetSortOrder(discardPile.Count * 4);
        card.transform.localPosition = layout.discardPile.pos + Vector3.back / 2;
    }

    public CardBartok Draw()
    {
        CardBartok newCard = drawPile[0];

        if (drawPile.Count == 0)
        {
            int ndx;
            while (discardPile.Count > 0)
            {
                ndx = Random.Range(0, discardPile.Count);
                drawPile.Add(discardPile[ndx]);
                discardPile.RemoveAt(ndx);
            }

            ArrangeDrawPile();

            float t = Time.time;
            foreach (CardBartok card in drawPile)
            {
                card.transform.localPosition = layout.discardPile.pos;
                card.callbackPlayer = null;
                card.MoveTo(layout.drawPile.pos);
                card.timeStart = t;
                t += 0.02f;
                card.state = eCardState.toDrawpile;
                card.eventualSortLayer = "10";
            }
        }

        drawPile.RemoveAt(0);
        return newCard;
    }


    public void ArrangeDrawPile()
    {
        CardBartok tCB;

        for (int i = 0; i < drawPile.Count; i++)
        {
            tCB = drawPile[i];
            tCB.transform.SetParent(layoutAnchor);
            tCB.transform.localPosition = layout.drawPile.pos;
            tCB.faceUp = false;
            tCB.SetSortingLayerName(layout.drawPile.layerName);
            tCB.SetSortOrder(-i * 4);
            tCB.state = eCardState.drawpile;
        }
    }

    public List<CardBartok> UpgradeCardsList(List<Card> lCards)
    {
        List<CardBartok> lCardBartok = new();

        foreach (Card card in lCards)
        {
            lCardBartok.Add(card as CardBartok);
        }

        return lCardBartok;
    }
    public void RestartGame()
    {
        CURRENT_PLAYER = null;
        SceneManager.LoadScene("Bartok_Scene");
    }

    public void CardClicked(CardBartok card)
    {
        if (CURRENT_PLAYER.type != ePlayerType.human) return;
        if (phase == eTurnState.waiting) return;

        switch (card.state)
        {
            case eCardState.drawpile:
                CardBartok newCard = CURRENT_PLAYER.AddCard(Draw());
                newCard.callbackPlayer = CURRENT_PLAYER;

                Utils.tr("Bartok:CardClicked()", "Draw", newCard.name);
                phase = eTurnState.waiting;
                break;
            case eCardState.hand:
                if (ValidPlay(card))
                {
                    CURRENT_PLAYER.RemoveCard(card);
                    MoveToTarget(card);
                    card.callbackPlayer = CURRENT_PLAYER;

                    Utils.tr("Bartok:CardClicked()", "Play", card.name);
                    phase = eTurnState.waiting;
                }
                else
                {
                    Utils.tr("Bartok:CardClicked()", "Attempted to Play", card.name);
                }
                break;
        }
    }
}

