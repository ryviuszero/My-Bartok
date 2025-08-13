using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private Sprite _tSp = null;
    private GameObject _tGo = null;
    private SpriteRenderer _tSR = null;

    [Header("Set in Inspector")]
    public Sprite suitClub;
    public Sprite suitDiamond;
    public Sprite suitHeart;
    public Sprite suitSpade;
    public Sprite[] faceSprites;
    public Sprite[] rankSprites;

    public Sprite cardBack;
    public Sprite cardBackGold;
    public Sprite cardFront;
    public Sprite cardFrontGold;

    public GameObject prefabCard;
    public GameObject prefabSprite;
    public bool startFaceUp = false;

    [Header("Set Dynamically")]
    public PT_XMLReader xmlr;
    public List<string> cardNames;
    public List<Card> cards;
    public List<Decorator> decorators;
    public List<CardDefinition> cardDefinitions;
    public Transform deckAnchor;
    public Dictionary<string, Sprite> dictSuits;

    public void InitDeck(string deckXMLText)
    {
        if (GameObject.Find("_Deck") == null)
        {
            GameObject anchorGo = new GameObject("_Deck");
            deckAnchor = anchorGo.transform;
        }

        dictSuits = new Dictionary<string, Sprite>()
        {
            { "C", suitClub },
            { "D", suitDiamond },
            { "H", suitHeart },
            { "S", suitSpade }
        };

        ReadDeck(deckXMLText);

        MakeCards();
    }

    public void ReadDeck(string deckXMLText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(deckXMLText);

        decorators = new();

        PT_XMLHashList xDecos = xmlr.xml["xml"][0]["decorators"];
        Decorator deco;
        for (int i = 0; i < xDecos.Count; i++)
        {
            deco = new Decorator();

            deco.type = xDecos[i].att("type");
            deco.flip = (xDecos[i].att("flip") == "1");
            deco.scale = float.Parse(xDecos[i].att("scale"));
            deco.loc.x = float.Parse(xDecos[i].att("x"));
            deco.loc.y = float.Parse(xDecos[i].att("y"));
            deco.loc.z = float.Parse(xDecos[i].att("z"));

            decorators.Add(deco);
        }

        cardDefinitions = new();

        PT_XMLHashList xCardDefs = xmlr.xml["xml"][0]["card"];
        for (int i = 0; i < xCardDefs.Count; i++)
        {
            CardDefinition cDef = new();
            cDef.rank = int.Parse(xCardDefs[i].att("rank"));

            PT_XMLHashList xPips = xCardDefs[i]["pip"];
            if (xPips != null)
            {
                for (int j = 0; j < xPips.Count; j++)
                {
                    deco = new Decorator();

                    deco.type = "pip";
                    deco.flip = (xPips[j].att("flip") == "1");
                    deco.loc.x = float.Parse(xPips[j].att("x"));
                    deco.loc.y = float.Parse(xPips[j].att("y"));
                    deco.loc.z = float.Parse(xPips[j].att("z"));

                    if (xPips[j].HasAtt("scale"))
                        deco.scale = float.Parse(xPips[j].att("scale"));


                    cDef.pips.Add(deco);
                }
            }

            if (xCardDefs[i].HasAtt("face"))
                cDef.face = xCardDefs[i].att("face");

            cardDefinitions.Add(cDef);
        }

    }

    public CardDefinition GetCardDefinitionByRank(int rank)
    {
        foreach (CardDefinition cDef in cardDefinitions)
            if (cDef.rank == rank) return cDef;

        return null;
    }

    public void MakeCards()
    {
        cardNames = new List<string>();
        string[] letters = new string[] { "C", "D", "H", "S" };
        foreach (string letter in letters)
        {
            for (int i = 0; i < 13; i++)
            {
                cardNames.Add(letter + (i + 1));
            }
        }

        cards = new List<Card>();



    }

    public Card MakeCard(int cNum)
    {
        GameObject cgo = Instantiate<GameObject>(prefabCard);

        cgo.transform.parent = deckAnchor;
        Card card = cgo.GetComponent<Card>();

        cgo.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        card.name = cardNames[cNum];
        card.suit = card.name[0].ToString();
        card.rank = int.Parse(card.name.Substring(1));

        if (card.suit == "D" || card.suit == "H")
        {
            card.colS = "Red";
            card.color = Color.red;
        }

        card.def = GetCardDefinitionByRank(card.rank);



        return card;
    }

    private void AddDecorators(Card card)
    {

    }

    private void AddPips(Card card)
    {

    }

    private void AddFace(Card card)
    {

    }

    private void AddBack(Card card)
    {
        
    }



    
}
