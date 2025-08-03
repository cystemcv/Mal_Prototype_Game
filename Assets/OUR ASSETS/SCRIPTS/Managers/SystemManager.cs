using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour, IDataPersistence
{
    public static SystemManager Instance;

    //enums
    public enum GameMode { MainMode, DuoMode, AnyMode }
    public GameMode gameMode = GameMode.MainMode;

    public enum SystemModes { MAINMENU, PAUSED, GAMEPLAY, COMBAT, DUNGEON }
    public SystemModes systemMode = SystemModes.MAINMENU;

    public enum UIScreens { MainMenu, ModeSelectionMenu, CharacterSelectionMenu, SaveMenu, LoadMenu, LibraryMenu, OptionsMenu }
    public UIScreens currentUIScreen = UIScreens.MainMenu;

    public enum SaveLoadModes { SAVE, LOAD }
    public SaveLoadModes saveLoadMode = SaveLoadModes.SAVE;

    public enum AbilityModes { NONE, TARGET, CHOICE, SHIELDCHOICE, MOVEHERO }
    public AbilityModes abilityMode = AbilityModes.NONE;

    public enum CombatTurns { playerStartTurn, playerTurn, playerEndTurn, enemyStartTurn, enemyTurn, enemyEndTurn }
    public CombatTurns combatTurn;

    public enum AdjustNumberModes { ATTACK, HEAL, SHIELD, COUNTER, ARMOR }
    public AdjustNumberModes adjustNumberModes;

    public enum AddCardTo { Hand, discardPile, combatDeck, mainDeck }
    public AddCardTo addCardTo;

    public enum CardType { Attack, Magic, Skill, Focus, Status, Curse, Summon, None}
    public CardType cardType;

    public enum Rarity { Common, Rare, Epic, Legendary,Curse };
    public Rarity rarity;


    public enum EntityAnimation { MeleeAttack, ProjectileAttack, SpellCast }; //Actual classes to be determined
    public EntityAnimation entityAnimation;

    public enum EntitySound { Generic, Fire, MeleeHit, SwordSlice, Buff, Debuff }; //Actual classes to be determined
    public EntitySound entitySound;

    public enum MainClass { MONSTER, SUPPORT, COMMON, CURSE, UNKOWN, SUMMON, ANGEL, SUMMONER }; //Actual classes to be determined
    public MainClass mainClass;

    public enum AbilityType { ATTACK, OTHER }

    public enum AITypeOfAttack { SINGLETARGET, AOE }

    public enum AIWhoToTarget { ENEMY, PLAYER }

    public enum AIIntend { ATTACK, MAGIC, BUFF, DEBUFF, CARDDECK }

    public enum AICommandType { MANUAL, RANDOM }

    public enum CardThrow { DISCARD, BANISH, DECK }

    public enum StatModifiedType { NORMAL,PERCENTAGE, TARGETNORMAL, TARGETPERCENTAGE, TARGETFIXEDAMOUNT }
    public enum StatModifiedAttribute { ATTACK, DEFENCE, ARMOR, MAXHEALTH}

    public enum EntityTag { Player, Enemy, PlayerSummon, EnemySummon, CompanionPos, PlayerPos, EnemyPos }

    public enum EntityMode { NORMAL, FROZEN, PARALYZED, BURNED, DEAD }


    public enum ControlBy { PLAYER, AI }

    public enum PlanetTypes { BATTLE, EVENT, SHOP, RESOURCES, REWARD, BOSS, REST, START, ELITEBATTLE, HIDDEN }

    public enum SelectionScreenPrefabType { CHARACTER, COMPANION }; //Actual classes to be determined

    public enum ItemIn { INVENTORY, LOOT, ARTIFACTS, COMPANION }

    public enum ItemCategory { RESOURCE, CONSUMABLE, CARD, ARTIFACT, COMPANIONITEM, RANDOMCOMPANIONITEM, RANDOMARTIFACTITEM,RANDOMRECIPEITEM }

    public enum BattleGroundType { WATER, MAGMA, DARK, ELECTRIC, LUSH, SAND };

    public enum NotificationOperation { WARNING,ERROR,SUCCESS }

    public enum ActivationType
    {
        None, OnDraw, OnLoot, OnPlayCard, OnCombatStart, OnPlayerTurnStart, OnPlayerTurnEnd, OnEnemyDeath, OnPlayerDeath, OnCombatEnd,
        OnNonCombatRoom, OnEntityGetHit, OnAdventureSceneLoaded, OnNextGalaxyGeneration
    }

    //end of enums

    //public bool thereIsActivatedCard = false;

    public Camera mainCamera;
    //public Camera uiCamera;

    //common colors
    public string colorGolden = "FFAD04";
    public string colorDarkGrey = "252525";
    public string colorLightGrey = "727272";

    public string colorWhite = "FFFFFF";
    public string colorRed = "FF0000";
    public string colorBlue = "003FFF";
    public string colorLightBlue = "0079FF";
    public string colorVeryLightBlue = "5DC7FF";
    public string colorYellow = "F9FF00";
    public string colorGreen = "25FF00";
    public string colorGrey = "363636";
    public string colorOrange = "e07b00";
    public string colorDiscordRed = "FF5555";
    public string colorSkyBlue = "00CCFF";

    //completely transparent
    public string colorTransparent = "FFFFFF00";

    //specific
    public string colorActivationFail = "FF000010";
    public string colorActivationSuccess = "25FF0010";

    public GameObject uiManager;
    public GameObject audioManager;
    public GameObject dataPersistenceManager;
    public GameObject deckManager;
    public GameObject handManager;
    public GameObject combatManager;
    public GameObject dungeonGeneratorManager;
    public GameObject combatScene;

    //system variables
    public float totalTimePlayed = 0f;

    public Sprite intend_Attack;
    public Sprite intend_Magic;
    public Sprite intend_Buff;
    public Sprite intend_Debuff;
    public Sprite intend_CardDeck;

    public GameObject intendObject;

    [Header("PREFABS FOR ENTITIES")]
    public GameObject entity_buffDebuffObject;
    public GameObject entity_healthBarObject;
    public GameObject entity_intendObject;
    public GameObject entity_summonTurnsObject;

    [Header("OBJECTS ON SCENE THAT CAN BE USED")]
    public GameObject object_HighlightButton;

    [Header("LOADING")]
    public GameObject LoadingScreen;

    [Header("MATERIALS")]
    public Material materialDefaultEntity;
    public Material materialDamagedEntity;
    public Material materialTargetEntity;
    public Material materialMouseOverEntity;

    private void Awake()
    {
        if (Instance == null)
        {
            DOTween.Init();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

       StaticData.InitializeJsons();
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
        if (this.systemMode == SystemModes.GAMEPLAY)
        {
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

    public IEnumerator DestroyAllChildrenIE(GameObject parent)
    {
        // Create a list to hold references to children
        List<Transform> children = new List<Transform>();

        // Collect references to all children first
        foreach (Transform child in parent.transform)
        {
            children.Add(child);
        }

        // Now destroy each collected child using a for loop
        for (int i = 0; i < children.Count; i++)
        {
            Transform child = children[i];
            // Destroy each child GameObject
            Destroy(child.gameObject);
            string objName = child.gameObject.name;
            yield return null; // Wait for a frame to ensure destruction

        }
    }

    public IEnumerator DestroyObjectIE(GameObject objectToDestroy, float timer)
    {

        Destroy(objectToDestroy, timer);

        yield return null; // Wait for a frame to ensure destruction

    }

    public GameObject SpawnPrefab(GameObject spawnPrefab, GameObject target, string name, Vector3 spawnVector3)
    {


        //spawn prefab
        GameObject spawnPrefabTrue = Instantiate(spawnPrefab, target.transform.position, Quaternion.identity);
        spawnPrefabTrue.transform.SetParent(target.transform);
        spawnPrefabTrue.name = name;
        spawnPrefabTrue.transform.position = new Vector3(
            spawnPrefabTrue.transform.position.x + spawnVector3.x,
            spawnPrefabTrue.transform.position.y + spawnVector3.y,
            spawnPrefabTrue.transform.position.z + spawnVector3.z
            );


         return spawnPrefabTrue; 
    }

    public IEnumerator SpawnPrefabIE(GameObject spawnPrefab, GameObject target, float spawnTimer, string name, Vector3 spawnVector3)
    {
        //missing prefab vfx
        if (spawnPrefab == null || target == null)
        {
            yield return null; // Wait for a frame to ensure destruction
        }

        //spawn prefab
        GameObject spawnPrefabTrue = Instantiate(spawnPrefab, target.transform.position, Quaternion.identity);
        spawnPrefabTrue.transform.SetParent(target.transform);
        spawnPrefabTrue.name = name;
        spawnPrefabTrue.transform.position = new Vector3(
            spawnPrefabTrue.transform.position.x + spawnVector3.x,
            spawnPrefabTrue.transform.position.y + spawnVector3.y,
            spawnPrefabTrue.transform.position.z + spawnVector3.z
            );

        //check if there is cnvas then assign camera

        Canvas canvas = spawnPrefabTrue.GetComponent<Canvas>();
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        if (canvas != null)
        {
            canvas.worldCamera = mainCamera;
        }

        ////if the target is player then we wanna rotate 
        //if (target.tag == "Player")
        //{
        //    spawnPrefabTrue.transform.Rotate(new Vector3(0, 180, 0));
        //}

        yield return null; // Wait for a frame to ensure destruction
    }

    public void DestroyAllChildren(GameObject parent)
    {
        // Create a list to hold references to children
        List<Transform> children = new List<Transform>();

        // Collect references to all children first
        foreach (Transform child in parent.transform)
        {
            children.Add(child);
        }

        // Now destroy each collected child using a for loop
        for (int i = 0; i < children.Count; i++)
        {
            Transform child = children[i];
            // Destroy each child GameObject
            Destroy(child.gameObject);
            string objName = child.gameObject.name;


        }
    }

    public List<GameObject> GetAllChildren(GameObject parent)
    {
        List<GameObject> listOfChildGameobjects = new List<GameObject>();

        foreach (Transform child in parent.transform)
        {

            // Destroy each child GameObject
            listOfChildGameobjects.Add(child.gameObject);
        }

        return listOfChildGameobjects;

    }

    //loading screen
    public void LoadScene(string sceneName, float manualLoadingTime, bool triggerLoadingStartAnimation, bool triggerLoadingEndAnimation)
    {

        StartCoroutine(LoadSceneAsync(sceneName, manualLoadingTime, triggerLoadingStartAnimation, triggerLoadingEndAnimation));
    }

    public IEnumerator LoadSceneAsync(string sceneName, float manualLoadingTime, bool triggerLoadingStartAnimation, bool triggerLoadingEndAnimation)
    {

        //wait for the transition
        if (triggerLoadingStartAnimation)
        {
            Animator loadingAnimator = LoadingScreen.GetComponent<Animator>();
            loadingAnimator.SetTrigger("LoadingStart");
        }

        yield return new WaitForSeconds(manualLoadingTime);

        //do the async operation
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);


        //check operation if is done
        while (!asyncOperation.isDone)
        {

            yield return null;

        }

        //now the scene is loaded
        if (triggerLoadingEndAnimation)
        {
            Animator loadingAnimator = LoadingScreen.GetComponent<Animator>();
            loadingAnimator.SetTrigger("LoadingEnd");
        }
        //loadingAnimator.SetTrigger("LoadingEnd");
        yield return new WaitForSeconds(manualLoadingTime);

    }

    public IEnumerator TriggerLoadStartAnimation()
    {
        Animator loadingAnimator = LoadingScreen.GetComponent<Animator>();
        loadingAnimator.SetTrigger("LoadingStart");

        yield return null;
    }

    public IEnumerator TriggerLoadEndAnimation()
    {
        Animator loadingAnimator = LoadingScreen.GetComponent<Animator>();
        loadingAnimator.SetTrigger("LoadingEnd");

        yield return null;
    }

    public void ChangeTargetMaterial(Material customMaterial, GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (target.GetComponent<EntityClass>() != null)
        {

            //change material
            target.transform.Find("model").Find("Sprite").GetComponent<SpriteRenderer>().material = customMaterial;

        }
        else if (target.transform.Find("Icon") != null)
        {
            //change material
            target.transform.Find("Icon").GetComponent<SpriteRenderer>().material = customMaterial;

        }
        else
        {

            //ui or not ui
            if (target.GetComponent<SpriteRenderer>() != null)
            {
                target.GetComponent<SpriteRenderer>().material = customMaterial;
            }
            else if (target.GetComponent<Image>() != null)
            {
                target.GetComponent<Image>().material = customMaterial;
            }


        }

    }

    public void ChangeTargetEntityTransparency(float alpha, GameObject target)
    {


        // Find the SpriteRenderer component
        SpriteRenderer spriteRenderer = target.transform.Find("model").Find("Sprite").GetComponent<SpriteRenderer>();

        // Get the current color of the sprite
        Color color = spriteRenderer.color;

        // Modify the alpha value
        color.a = alpha;

        // Apply the modified color back to the SpriteRenderer
        spriteRenderer.color = color;

    }

    public void ChangeTargetEntityColor(string hex, GameObject target)
    {
        SpriteRenderer spriteRenderer;
        if (target.GetComponent<EntityClass>() != null && target.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD)
        {
            // Find the SpriteRenderer component
            spriteRenderer = target.transform.Find("model").Find("Sprite").GetComponent<SpriteRenderer>();
        }
        else if (target.transform.Find("Icon") != null)
        {
            //change material
            spriteRenderer = target.transform.Find("Icon").GetComponent<SpriteRenderer>();

        }
        else
        {
            spriteRenderer = target.GetComponent<SpriteRenderer>();
        }

        // Apply the modified color back to the SpriteRenderer
        spriteRenderer.color = GetColorFromHex(hex);

    }


    // Function to get objects that have all specified tags
    public List<GameObject> FindGameObjectsWithTags(List<string> tags)
    {
        // If no tags are provided, return an empty list
        if (tags == null || tags.Count == 0)
        {
            return new List<GameObject>();
        }

        List<GameObject> listOfGameobjects = new List<GameObject>();

        foreach (string tag in tags)
        {

            //if tag is then we need to enable them

            //find 
            GameObject[] gameobjectsFound = GameObject.FindGameObjectsWithTag(tag);

            //add array to list
            listOfGameobjects.AddRange(gameobjectsFound);

        }


        return listOfGameobjects;
    }




    public List<GameObject> GetObjectsWithTagsFromGameobjectOppossite(GameObject gameObjectParam)
    {
        List<GameObject> gameobjectsFound = new List<GameObject>();
        List<string> tags = new List<string>();

        if (gameObjectParam.tag == "Player" || gameObjectParam.tag == "PlayerSummon")
        {
            tags = GetEnemyTagsList();
        }
        else
        {
            tags = GetPlayerTagsList();
        }

        gameobjectsFound = SystemManager.Instance.FindGameObjectsWithTags(tags);

        return gameobjectsFound;
    }

    public List<GameObject> GetObjectsWithTagsFromGameobjectSameSide(GameObject gameObjectParam)
    {
        List<GameObject> gameobjectsFound = new List<GameObject>();
        List<string> tags = new List<string>();

        if (gameObjectParam.tag == "Player" || gameObjectParam.tag == "PlayerSummon")
        {
            tags = GetPlayerTagsList();

        }
        else
        {
            tags = GetEnemyTagsList();
        }

        gameobjectsFound = SystemManager.Instance.FindGameObjectsWithTags(tags);

        return gameobjectsFound;
    }

    public List<string> GetAllTagsList()
    {
        List<string> tags = new List<string>();

        tags.Add("Enemy");
        tags.Add("EnemySummon");
        tags.Add("Player");
        tags.Add("PlayerSummon");

        return tags;
    }

    public List<string> GetEnemyTagsList()
    {
        List<string> tags = new List<string>();

        tags.Add("Enemy");
        tags.Add("EnemySummon");

        return tags;
    }

    public List<string> GetPlayerTagsList()
    {
        List<string> tags = new List<string>();

        tags.Add("Player");
        tags.Add("PlayerSummon");

        return tags;
    }

    public void SpawnEffectPrefab(GameObject spawnPrefab, GameObject target, float spawnTimer)
    {
        //missing prefab vfx
        if (spawnPrefab == null || target == null)
        {
            return;
        }

        //spawn prefab
        GameObject spawnPrefabTrue = Instantiate(spawnPrefab, target.transform.position, Quaternion.identity);
        spawnPrefabTrue.transform.SetParent(target.transform);

        //if the target is player then we wanna rotate 
        if (target.tag == "Player")
        {
            spawnPrefabTrue.transform.Rotate(new Vector3(0, 180, 0));
        }

        Destroy(spawnPrefabTrue, spawnTimer);
    }

}

