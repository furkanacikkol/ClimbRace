using UnityEngine;
using Framework;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isCpiLevel;
    public int maxLevelInGame = 2;

    [Header("Game Variables")] public Consts.GameState _gameState;

    public Consts.GameState gameState // its enough to set this variable any where to change the game state.


    {
        get { return _gameState; }
        set
        {
            _gameState = value;
            if (gameState == Consts.GameState.idle)
            {
                uiManager.ShowPanel(uiManager.menu);
            }

            if (gameState == Consts.GameState.play)
            {
                uiManager.ShowPanel(uiManager.play);
            }

            if (gameState == Consts.GameState.win)
            {
                level += 1;
                Invoke("win",
                    3); //its call  with 2 sec. delay player need to see what happened after win before closing scene with win ui
            }

            if (gameState == Consts.GameState.fail)
            {
                Invoke("fail",
                    2); //its call  with 2 sec. delay player need to see what happened after fail before closing scene with win ui
            }
        }
    }


    public int _level;
    public int level // stored level id just changing this value will be write on player prefs automaticly
    {
        get { return _level; }
        set
        {
            _level = value;
            PlayerPrefs.SetInt(Consts.prefLevel, level);
        }
    }

    public Level activeLevel;

    [Header("Managers")] //sound manager, vibration manager all managers can be assign here.
    public UIManager uiManager;


    private void Awake()
    {
        if (!instance) // singleton controller. All games references stored on this main manager
            instance = this;


        Application.targetFrameRate = 60;
    }

    void Start()
    {
        gameState = Consts.GameState.idle;
        SetPlayerPrefs(); // all player prefs is loading in this method. its calling only one time when initializing main scene.
        prepeareTraningLevel(); // initialize first level ready for play in idle mod. after player start game will play this level.
    }

    void SetPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(Consts.prefLevel))
        {
            level = 1;
        }
        else
        {
            level = PlayerPrefs.GetInt(Consts.prefLevel);
        }
    }


    public int getFakeLevel()
    {
        int tmplevel;
        if (level % maxLevelInGame == 0)
            tmplevel = maxLevelInGame;
        else
            tmplevel = level % maxLevelInGame;
        return tmplevel;
    }


    void
        clearScene() // All waste of object which stay in scene from previous level can be tagged with "trash" so this method will find them and will destroy.
    {
        GameObject[] trashes = GameObject.FindGameObjectsWithTag("trash");

        if (trashes != null)
        {
            foreach (GameObject trash in trashes)
            {
                Destroy(trash);
            }
        }
    }


    public void prepeareLevel()
    {
        if (activeLevel)
        {
            DestroyImmediate(activeLevel.gameObject);
        }

        clearScene(); //for destroy all unneccessary objects in scene which are stay in scene by previous level.
        var path = isCpiLevel ? "CPI/level" : "Levels/level";

        activeLevel = Instantiate(Resources.Load(path + getFakeLevel().ToString()) as GameObject)
            .GetComponent<Level>();
        uiManager.matchSliderBar.GetComponentInChildren<TextMeshProUGUI>().text = SetMeter().ToString("0") + "m";
      //  uiManager.RefreshLevelProgression();
    }

    public void prepeareTraningLevel()
    {
        if (activeLevel)
        {
            DestroyImmediate(activeLevel.gameObject);
        }

        clearScene(); //for destroy all unneccessary objects in scene which are stay in scene by previous level.
        activeLevel = Instantiate(Resources.Load("Levels/training") as GameObject)
            .GetComponent<Level>();
    }


    void win()
    {
        uiManager.ShowPanel(uiManager.levelComplete);
    }


    void fail()
    {
        uiManager.ShowPanel(uiManager.levelFail);
    }

    void Update()
    {
        if (gameState != Consts.GameState.play)
            return;


        if (Input.GetKeyDown(KeyCode.F))
        {
            gameState = Consts.GameState.fail;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            gameState = Consts.GameState.win;
        }
    }

    private float SetMeter()
    {
        float targetMeter = 0;
        var lvl = _level % 4;
        targetMeter = lvl switch
        {
            0 => 125f,
            1 => 50f,
            2 => 75f,
            3 => 100f,
            _ => 50
        };

        return targetMeter;
    }
}