using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //This SINGLETON has a static variable, that will be a reference to it's own instance.
    public static GameManager instance;

    public Color[] teamColors;

    //We store the outposts array in the GameManager, so we only have to search for them once!
    public Outpost[] outposts;

    // Start is called before the first frame update
    // Awake is called before Start
    void Awake()
    {
        //At the very beginning of the game, we make the instance variable reference THIS instance
        instance = this;

        outposts = FindObjectsOfType<Outpost>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
