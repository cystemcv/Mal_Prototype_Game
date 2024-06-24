using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour, IDataPersistence
{
    public static SystemManager Instance;

    //enums
    public enum GameMode { MainMode, DuoMode, AnyMode }
    public GameMode gameMode = GameMode.MainMode;

    public enum SystemModes { MAINMENU, PAUSED, GAMEPLAY, COMBAT}
    public SystemModes systemMode = SystemModes.MAINMENU;

    public enum UIScreens { MainMenu, ModeSelectionMenu, CharacterSelectionMenu, SaveMenu, LoadMenu, LibraryMenu, OptionsMenu }
    public UIScreens currentUIScreen = UIScreens.MainMenu;

    public enum SaveLoadModes { SAVE, LOAD}
    public SaveLoadModes saveLoadMode = SaveLoadModes.SAVE;

    public enum AbilityModes { NONE, TARGET, CHOICE }
    public AbilityModes abilityMode = AbilityModes.NONE;

    public enum CombatTurns { playerStartTurn, playerTurn, playerEndTurn, enemyStartTurn, enemyTurn, enemyEndTurn }
    public CombatTurns combatTurn;

    public enum AdjustNumberModes { ATTACK, HEAL, SHIELD }
    public AdjustNumberModes adjustNumberModes;

    public enum AddCardTo { Hand, discardPile, combatDeck, mainDeck }
    public AddCardTo addCardTo;

    public enum CardType { Attack, Magic, Skill, Focus, Status, Curse, }
    public CardType cardType;

    public enum EntityAnimation { MeleeAttack, ProjectileAttack, SpellCast }; //Actual classes to be determined
    public EntityAnimation entityAnimation;

    public enum EntitySound { Generic, Fire, MeleeHit, SwordSlice, Buff, Debuff }; //Actual classes to be determined
    public EntitySound entitySound;

    public enum MainClass { Enemy,Knight, Rogue, Hierophant, Chaos_Mage, Ranger, Sniper }; //Actual classes to be determined
    public MainClass mainClass;

    public enum TypeOfAttack { SIMPLE, MELLEE, PROJECTILE }

    public enum AITypeOfAttack { SINGLETARGET,AOE }

    public enum AIWhoToTarget { ENEMY, PLAYER }

    public enum AIIntend { ATTACK, MAGIC, BUFF, DEBUFF }

    //end of enums

    public bool thereIsActivatedCard = false;

    public Camera mainCamera;
    public Camera uiCamera;

    //common colors
    public string colorGolden = "FFAD04";
    public string colorDarkGrey = "252525";
    public string colorLightGrey = "727272";

    public string colorWhite = "FFFFFF";
    public string colorRed = "FF0000";
    public string colorBlue = "003FFF";
    public string colorLightBlue = "0079FF";
    public string colorYellow = "F9FF00";
    public string colorGreen = "25FF00";

    //completely transparent
    public string colorTransparent = "FFFFFF00";

    //specific
    public string colorActivationFail = "FF000060";
    public string colorActivationSuccess = "25FF0060";

    public GameObject uiManager;
    public GameObject audioManager;
    public GameObject dataPersistenceManager;

    //system variables
    public float totalTimePlayed = 0f;

    public Sprite intend_Attack;
    public Sprite intend_Magic;
    public Sprite intend_Buff;
    public Sprite intend_Debuff;

    public GameObject intendObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // called first
    void OnEnable()
    {
       
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
 
        //check if the scene was from a load button
        if (DataPersistenceManager.Instance.isLoadScene)
        {
            DataPersistenceManager.Instance.LoadGame(DataPersistenceManager.Instance.savefileID);
        }

        //check if the scene is main menu
        if (scene.name == "scene_MainMenu")
        {
            systemMode = SystemModes.MAINMENU;
        }
        else
        {
            systemMode = SystemModes.GAMEPLAY;
        }

    }

    // called when the game is terminated
    void OnDisable()
    {
     
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EnableDisable_UIManager(bool enable)
    {

        if (enable)
        {
            //make the default Main menu
            UIManager.Instance.NavigateMenu("MAIN MENU");
            uiManager.SetActive(true);
        }
        else
        {
            uiManager.SetActive(false);
        }

    }
    public void EnableDisable_AudioManager(bool enable)
    {
        if (enable)
        {
            audioManager.SetActive(true);
        }
        else
        {
            audioManager.SetActive(false);
        }
    }

    public float GetTotalPlayTime()
    {
        return totalTimePlayed;
    }

    public void Update()
    {
        if (this.systemMode == SystemModes.GAMEPLAY) {
            this.totalTimePlayed += Time.deltaTime;
        }
    }

    public void LoadData(GameData data)
    {
        this.totalTimePlayed = data.totalTimePlayed; 
    }

    public void SaveData(ref GameData data)
    {
    
        data.totalTimePlayed = this.totalTimePlayed;
    }

    public string ConvertTimeToReadable(float totalTime)
    {
        // Convert totalTime to hours, minutes, and seconds
        int hours = (int)(totalTime / 3600);
        int minutes = (int)((totalTime % 3600) / 60);
        int seconds = (int)(totalTime % 60);

        string readableTime = hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");

        return readableTime;
    }

    public void RuntimeInitializeOnLoadMethod()
    {
   
    }

    public Color GetColorFromHex(string hex)
    {
     
        Color colorFromHex = Color.white;
        bool boolColor = ColorUtility.TryParseHtmlString("#" + hex, out colorFromHex);

        return colorFromHex;
    }

    public void DestroyAllChildren(GameObject parent)
    {

        foreach (Transform child in parent.transform)
        {
            // Destroy each child GameObject
            Destroy(child.gameObject);
        }

    }

    public List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> listOfChildGameobjects = new List<GameObject>(); 

        foreach (Transform child in parent.transform)
        {
            Debug.Log(child.name);
            // Destroy each child GameObject
            listOfChildGameobjects.Add(child.gameObject);
        }

        return listOfChildGameobjects;

    }

}
