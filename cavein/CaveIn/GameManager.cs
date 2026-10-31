using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// CAVE-IN: dodge falling rocks for 60 seconds. Gold gives points but makes the rocks fall faster.
// This script controls the whole game: it builds the scene, keeps the score, lives and time,
// and draws the text on the screen. It starts by itself when you press Play.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;   // lets other scripts reach the GameManager easily

    public float gameLength = 60f;        // seconds you must survive
    public int lives = 3;
    public int gold = 0;
    public float timeLeft;
    public bool playing = false;          // true while the game is running
    public bool gameOver = false;
    public bool won = false;

    Player player;
    float dangerPulse;                    // makes the Danger text grow for a moment when you take gold

    // GREED MECHANIC: rocks get faster over time AND for every gold bar you collect.
    public float Speed
    {
        get { return 1f + (gameLength - timeLeft) / 40f + gold * 0.06f; }
    }

    // Unity runs this by itself after the scene loads, so nothing needs to be set up in the editor.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void StartAutomatically()
    {
        new GameObject("Game Manager").AddComponent<GameManager>();
    }

    void Awake()
    {
        Instance = this;
        timeLeft = gameLength;

        // Camera: above and behind the floor, looking down at it
        Camera cam = Camera.main;
        if (cam == null) cam = new GameObject("Camera").AddComponent<Camera>();
        cam.transform.position = new Vector3(0f, 10f, -9f);
        cam.transform.LookAt(Vector3.zero);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.07f, 0.05f);   // dark cave

        // The floor of the mine
        MakeShape(PrimitiveType.Cube, new Vector3(0f, -0.5f, 0f), new Vector3(12f, 1f, 8f), new Color(0.45f, 0.33f, 0.22f));

        // The player (a blue capsule)
        GameObject playerShape = MakeShape(PrimitiveType.Capsule, new Vector3(0f, 1f, 0f), Vector3.one, new Color(0.2f, 0.45f, 0.9f));
        playerShape.name = "Player";
        player = playerShape.AddComponent<Player>();

        // The spawner that drops rocks and gold
        gameObject.AddComponent<Spawner>();
    }

    void Update()
    {
        // Shrink the Danger text back to normal size
        dangerPulse = Mathf.Max(0f, dangerPulse - Time.deltaTime * 3f);

        // Title screen or game over: wait for SPACE
        if (!playing)
        {
            if (SpacePressed()) StartGame();
            return;
        }

        // Count down. Surviving until 0 means you win.
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame(true);
        }
    }

    void StartGame()
    {
        // Remove rocks and gold from the last round (they are children of this object)
        foreach (Transform child in transform) Destroy(child.gameObject);

        gold = 0;
        lives = 3;
        timeLeft = gameLength;
        gameOver = false;
        playing = true;
        player.ResetPlayer();
    }

    void EndGame(bool didWin)
    {
        playing = false;
        gameOver = true;
        won = didWin;
    }

    // Called by the Player script
    public void CollectGold()
    {
        gold++;
        dangerPulse = 1f;   // feedback: the Danger text pops so the player sees greed raising the danger
    }

    public void HitByRock()
    {
        lives--;
        if (lives <= 0) EndGame(false);
    }

    // Draws all the text on the screen
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 30;
        style.normal.textColor = Color.white;

        // Top-left information
        GUI.Label(new Rect(20, 20, 800, 50), "Gold: " + gold + "     Lives: " + lives + "     Time: " + Mathf.CeilToInt(timeLeft), style);

        // Danger text grows and turns yellow for a moment when you take gold
        GUIStyle dangerStyle = new GUIStyle(style);
        dangerStyle.fontSize = 30 + Mathf.RoundToInt(dangerPulse * 16f);
        dangerStyle.normal.textColor = dangerPulse > 0f ? Color.yellow : Color.white;
        GUI.Label(new Rect(20, 60, 800, 70), "Danger: x" + Speed.ToString("0.0"), dangerStyle);

        // Big message in the middle of the screen
        style.fontSize = 38;
        style.alignment = TextAnchor.MiddleCenter;
        Rect middle = new Rect(0, 0, Screen.width, Screen.height);

        if (!playing && !gameOver)
        {
            GUI.Label(middle, "CAVE-IN\n\nDodge the falling rocks for 60 seconds.\n" +
                              "Red circles show where rocks will land.\n" +
                              "Gold gives points but makes the rocks fall FASTER.\n" +
                              "Move: WASD or Arrow keys\n\nPress SPACE to start", style);
        }
        else if (gameOver)
        {
            string result = won ? "YOU ESCAPED with " + gold + " gold!" : "BURIED! Your gold is lost.";
            GUI.Label(middle, result + "\n\nPress SPACE to play again", style);
        }
    }

    // ---------------- Helpers ----------------

    // Creates a cube/sphere/capsule/cylinder with a position, size and colour
    public static GameObject MakeShape(PrimitiveType type, Vector3 position, Vector3 size, Color color)
    {
        GameObject shape = GameObject.CreatePrimitive(type);
        shape.transform.position = position;
        shape.transform.localScale = size;
        shape.GetComponent<Renderer>().material.color = color;
        return shape;
    }

    // Reads WASD / arrow keys (works with both of Unity's input systems)
    public static Vector3 GetMoveInput()
    {
        float x = 0f, z = 0f;
#if ENABLE_INPUT_SYSTEM
        Keyboard k = Keyboard.current;
        if (k != null)
        {
            if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
            if (k.sKey.isPressed || k.downArrowKey.isPressed) z -= 1f;
            if (k.wKey.isPressed || k.upArrowKey.isPressed) z += 1f;
        }
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
#endif
        return new Vector3(x, 0f, z).normalized;
    }

    static bool SpacePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }
}
