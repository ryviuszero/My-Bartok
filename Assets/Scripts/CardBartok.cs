using System.Collections.Generic;
using UnityEngine;

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
    static public string MOVE_EASING = Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;

    [Header("Set Dynamically: CardBartok")]
    public eCardState state = eCardState.drawpile;
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierPtsRots;
    public float timeStart;
    public float timeDuration;
    public GameObject reportFinishTo = null;
    public int eventualSortOrder;
    public string eventualSortLayer;

    [System.NonSerialized]
    public Player callbackPlayer = null;

    private void Update()
    {
        switch (state)
        {
            case eCardState.toHand:
            case eCardState.toTarget:
            case eCardState.toDrawpile:
            case eCardState.to:
                float u = (Time.time - timeStart) / timeDuration;
                float uC = Easing.Ease(u, MOVE_EASING);

                if (u < 0)
                {
                    transform.localPosition = bezierPts[0];
                    transform.localRotation = bezierPtsRots[0];
                    return;
                }
                else if (u >= 1)
                {
                    
                }
                break;
            
        }
        
    }

    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);
        bezierPts.Add(ePos);

        bezierPtsRots = new List<Quaternion>();
        bezierPtsRots.Add(transform.localRotation);
        bezierPtsRots.Add(eRot);

        if (timeStart == 0)
        {
            timeStart = Time.time;
        }
        timeDuration = MOVE_DURATION;

        state = eCardState.to;
    }

    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }
}
