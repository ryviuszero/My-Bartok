using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum eCardState
{
    toDrawpile,
    drawpile,
    toHand,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}

public class CardBartok : Card
{
    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING =  Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;

    [Header("Set Dynamically: CardBartok")]
    public eCardState state = eCardState.toDrawpile;
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierPtsRots;
    public float timeStart;
    public float timeDuration;
    public GameObject reportFinishTo = null;
    public int eventualSortOrder;
    public string eventualSortLayer;

    [System.NonSerialized]
    public Player callbackPlayer = null;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
