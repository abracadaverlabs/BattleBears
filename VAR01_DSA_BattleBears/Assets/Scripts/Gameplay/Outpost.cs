using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outpost : MonoBehaviour
{
    public Renderer flagRenderer;
    public float captureRate;

    //internal is like public, but doesn't show in the inspector
    internal int currentTeam;
    internal float captureValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color teamColor = GameManager.instance.teamColors[currentTeam];
        //We set the flag to change color based on the team, but also the captureValue
        //If the flag is uncaptured, it's white, otherwise the teamColor
        flagRenderer.material.color = Color.Lerp(Color.white, teamColor, captureValue);
    }

    private void OnTriggerStay(Collider other)
    {
        BaseTeddy teddy = other.GetComponent<BaseTeddy>();
        
        //We check if the object in our trigger has a BaseTeddy component...
        if (teddy != null)
        {
            if (teddy.team == currentTeam)
            {
                captureValue += captureRate;
                //We clamp our value at 1, so if someone stays at the outpost for a long time, the captureValue isn't crazy high
                if (captureValue >= 1)
                {
                    captureValue = 1;
                }
            } else
            {
                captureValue -= captureRate;
                if (captureValue < 0)
                {
                    //Switch captureValue from negative -0.02 to positive 0.02
                    captureValue = -captureValue;
                    //Change the team to this teddy
                    currentTeam = teddy.team;
                }
            }
        }
    }
}
