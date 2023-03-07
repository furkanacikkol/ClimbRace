using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public static string prefLevel = "prefLevel"; // that is player prefs key for level storage.

    [System.Serializable]
    public enum GameState // all game states controll with this enum
    {
        idle, // first game launched (main menu) state (waiting that state means player is waiting for click play button)
        play, //Game is plaiyng. that state means game is playing on this state. You can let all controllers to work by checking game state is play.
        win,  // game won. That state means player won the level. all ui works and others needed ones can execute by checking this state
        fail,// game failed. That state means player fail the level. all ui works and others needed ones can execute by checking this state
 
    }

    [System.Serializable]
    public enum TutorialState // all game states controll with this enum
    {
         hold,
        swipe,
        end,
        tap,
        nullObj
    }

}
