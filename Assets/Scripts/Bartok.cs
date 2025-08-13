using System.Collections.Generic;
using UnityEngine;

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
    public eTurnState turnState = eTurnState.idle;
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
