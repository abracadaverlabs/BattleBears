using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //This static reference is used to access this object from anywhere (= Singleton)
    public static UIManager instance;

    public UIScreen openingScreen;

    //We create a Dictionary that stores Types of objects, linking them to the instance of objects
    private Dictionary<Type, UIScreen> typeToScreen;

    private UIScreen currentScreen;

    private void Awake()
    {
        //We assign the singleton to this instance
        instance = this;
        typeToScreen = new Dictionary<Type, UIScreen>();

        //We disable all screens at the start
        UIScreen[] allScreens = GetComponentsInChildren<UIScreen>(true);
        foreach (UIScreen screen in allScreens)
        {
            screen.gameObject.SetActive(false);
            //This fills my Dictionary with all the screens
            //E.g. The type 'GameScreen' will link to the GameScreen object.
            typeToScreen.Add(screen.GetType(), screen);
        }

        openingScreen.gameObject.SetActive(true);
        currentScreen = openingScreen;
    }

    //This will show the instance of this type in the game
    public void Show<T>()
    {
        Type screenType = typeof(T);
        UIScreen newScreen = typeToScreen[screenType];
        //We disable the screen that was currently visible
        currentScreen.gameObject.SetActive(false);
        newScreen.gameObject.SetActive(true);
        //We store what screen is visible so we can disable it later
        currentScreen = newScreen;
    }


}
