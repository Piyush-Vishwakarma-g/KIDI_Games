using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TraversePathGame : MonoBehaviour
{
    [Header("Optional Art")]
    public Sprite playerSprite;
    public Sprite fingerSprite;

    [Header("Maze")]
    public int totalLevels = 10;
    public int seed = 260922;
    public float cellSize = 0.72f;
    public float wallWidth = 0.055f;

    [Header("Colors")]
    public Color wallColor = new Color(0.45f, 0.95f, 0.25f, 1f);
    public Color green = new Color(0.20f, 1f, 0.35f, 1f);
    public Color red = new Color(1f, 0.12f, 0.16f, 1f);
    public Color backgroundColor = new Color(0.035f, 0.025f, 0.13f, 1f);

    public bool IsGameOver { get; private set; }

    private Camera cam;
    private GameObject levelRoot;
    private PathPlayer player;
    private PathVisual pathVisual;
    private FingerGuide fingerGuide;
    private List<Vector3> solutionWorld = new();
    private Vector3 startWorld;
    private Vector3 finishWorld;
    private int currentLevel;
    private float progress;

    // UI
    private TMP_Text levelText;
    private TMP_Text progressText;
    private GameObject retryPanel;
    private GameObject completePanel;
    private Slider progressSlider;
    private Button retryButton;
    private Button nextButton;

    private MazeGenerator maze;

    private void Awake()
    {
        SetupCamera();
        SetupBackground();
        SetupUI();
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Application.targetFrameRate = 60;
        EnsureEventSystem();
        currentLevel = Mathf.Clamp(PlayerPrefs.GetInt("TraversePathLevel", 0), 0, totalLevels - 1);
        LoadLevel(currentLevel);
    }

    private void SetupCamera()
    {
        cam = Camera.main;
        if (cam == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cam = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        cam.orthographic = true;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.backgroundColor = backgroundColor;
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        GameObject go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    private void SetupBackground()
    {
        GameObject bg = new GameObject("Background");
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = backgroundColor;
        sr.sortingOrder = -20;
        bg.transform.position = new Vector3(0, 0, 2);
        bg.transform.localScale = new Vector3(20f, 30f, 1f);
    }

    private void SetupUI()
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        levelText = CreateText(canvasObject.transform, "LevelText", "LEVEL 1", 48, new Vector2(0, -75), new Vector2(700, 80));
        progressText = CreateText(canvasObject.transform, "ProgressText", "0%", 32, new Vector2(0, -145), new Vector2(500, 60));

        progressSlider = CreateSlider(canvasObject.transform);

        retryPanel = CreatePanel(canvasObject.transform, "TryAgainPanel", new Vector2(0, 0));
        CreateText(retryPanel.transform, "RetryTitle", "TRY AGAIN", 58, new Vector2(0, 100), new Vector2(700, 100));
        retryButton = CreateButton(retryPanel.transform, "Try Again", new Vector2(0, -20));
        retryButton.onClick.AddListener(RestartLevel);
        retryPanel.SetActive(false);

        completePanel = CreatePanel(canvasObject.transform, "CompletePanel", new Vector2(0, 0));
        CreateText(completePanel.transform, "CompleteTitle", "GREAT JOB!", 64, new Vector2(0, 110), new Vector2(800, 120));
        nextButton = CreateButton(completePanel.transform, "NEXT LEVEL", new Vector2(0, -30));
        nextButton.onClick.AddListener(NextLevel);
        completePanel.SetActive(false);
    }

    public void LoadLevel(int levelIndex)
    {
        IsGameOver = false;
        progress = 0f;
        retryPanel.SetActive(false);
        completePanel.SetActive(false);

        if (levelRoot != null) Destroy(levelRoot);
        levelRoot = new GameObject("Level_" + (levelIndex + 1));

        int width = 7 + Mathf.Min(levelIndex / 3, 2);       // 7,8,9
        int height = 9 + levelIndex;                         // 9..18
        int minPath = Mathf.Max(12, (width + height) / 2 + levelIndex * 2);

        Vector2Int start = new Vector2Int(width / 2, 0);
        Vector2Int finish = new Vector2Int(width / 2, height - 1);

        maze = new MazeGenerator();
        maze.Generate(width, height, seed + levelIndex * 7919, start, finish, minPath);

        float boardWidth = (width - 1) * cellSize;
        float boardHeight = (height - 1) * cellSize;
        Vector3 origin = new Vector3(-boardWidth * 0.5f, -boardHeight * 0.5f - 0.25f, 0f);

        BuildWalls(origin);
        BuildSolution(origin, start, finish);
        BuildPlayer(origin, start);
        BuildFinish(origin, finish);
        FitCamera(boardHeight);

        levelText.text = "LEVEL " + (levelIndex + 1) + " / " + totalLevels;
        progressText.text = "0%";
        progressSlider.value = 0f;
    }

    private void BuildWalls(Vector3 origin)
    {
        GameObject wallsRoot = new GameObject("MazeWalls");
        wallsRoot.transform.SetParent(levelRoot.transform);
        Material material = CreateMaterial();

        for (int x = 0; x < maze.Width; x++)
        {
            for (int y = 0; y < maze.Height; y++)
            {
                MazeGenerator.Cell c = maze.Cells[x, y];
                Vector3 center = CellWorld(origin, x, y);

                if (c.top) CreateWall(wallsRoot.transform, center + Vector3.up * (cellSize * 0.5f), new Vector3(cellSize, wallWidth, 1), material);
                if (c.left) CreateWall(wallsRoot.transform, center + Vector3.left * (cellSize * 0.5f), new Vector3(wallWidth, cellSize, 1), material);

                if (x == maze.Width - 1 && c.right)
                    CreateWall(wallsRoot.transform, center + Vector3.right * (cellSize * 0.5f), new Vector3(wallWidth, cellSize, 1), material);

                if (y == 0 && c.bottom)
                    CreateWall(wallsRoot.transform, center + Vector3.down * (cellSize * 0.5f), new Vector3(cellSize, wallWidth, 1), material);
            }
        }
    }

    private void BuildSolution(Vector3 origin, Vector2Int start, Vector2Int finish)
    {
        solutionWorld.Clear();
        foreach (Vector2Int cell in maze.Solution)
            solutionWorld.Add(CellWorld(origin, cell.x, cell.y));

        startWorld = solutionWorld[0];
        finishWorld = solutionWorld[solutionWorld.Count - 1];

        GameObject visualObject = new GameObject("PathVisual");
        visualObject.transform.SetParent(levelRoot.transform);
        pathVisual = visualObject.AddComponent<PathVisual>();
        pathVisual.Build(solutionWorld, green);
        pathVisual.SetProgress(0f, green);
    }

    private void BuildPlayer(Vector3 origin, Vector2Int start)
    {
        GameObject playerObject = new GameObject("Player");
        playerObject.transform.SetParent(levelRoot.transform);
        playerObject.transform.position = startWorld;
        playerObject.transform.localScale = Vector3.one * (cellSize * 0.62f);

        SpriteRenderer sr = playerObject.AddComponent<SpriteRenderer>();
        sr.sprite = playerSprite != null ? playerSprite : CreateCircleSprite(96);
        sr.color = Color.white;
        sr.sortingOrder = 20;

        player = playerObject.AddComponent<PathPlayer>();
        player.validRadius = cellSize * 0.42f;
        player.maxJumpDistance = cellSize * 0.85f;
        player.Initialize(cam, this, solutionWorld);

        GameObject fingerObject = new GameObject("FingerGuide");
        fingerObject.transform.SetParent(levelRoot.transform);
        fingerGuide = fingerObject.AddComponent<FingerGuide>();
        fingerGuide.SetTarget(playerObject.transform);

        GameObject fingerVisual = new GameObject("FingerVisual");
        fingerVisual.transform.SetParent(fingerObject.transform);
        SpriteRenderer fingerSR = fingerVisual.AddComponent<SpriteRenderer>();
        fingerSR.sprite = fingerSprite != null ? fingerSprite : CreateRingSprite(96);
        fingerSR.color = new Color(1f, 1f, 1f, 0.9f);
        fingerSR.sortingOrder = 25;
        fingerVisual.transform.localScale = Vector3.one * (cellSize * 0.30f);
        fingerGuide.fingerVisual = fingerVisual.transform;
    }

    private void BuildFinish(Vector3 origin, Vector2Int finish)
    {
        GameObject finishObject = new GameObject("Finish");
        finishObject.transform.SetParent(levelRoot.transform);
        finishObject.transform.position = finishWorld;

        SpriteRenderer sr = finishObject.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(96);
        sr.color = green;
        sr.sortingOrder = 12;
        finishObject.transform.localScale = Vector3.one * (cellSize * 0.50f);

        GameObject ring = new GameObject("FinishRing");
        ring.transform.SetParent(finishObject.transform);
        SpriteRenderer ringSR = ring.AddComponent<SpriteRenderer>();
        ringSR.sprite = CreateRingSprite(128);
        ringSR.color = new Color(green.r, green.g, green.b, 0.55f);
        ringSR.sortingOrder = 11;
        ring.transform.localScale = Vector3.one * 1.4f;
    }

    public void SetProgress(float value)
    {
        progress = Mathf.Clamp01(value);
        progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
        progressSlider.value = progress;
        if (pathVisual != null) pathVisual.SetProgress(progress, green);
    }

    public void WrongPath(Vector3 attemptedPosition)
    {
        if (IsGameOver) return;
        IsGameOver = true;
        pathVisual.FlashWrong(red);
        Handheld.Vibrate();
        retryPanel.SetActive(true);
    }

    public void CompleteLevel()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        SetProgress(1f);
        completePanel.SetActive(true);

        int next = Mathf.Clamp(currentLevel + 1, 0, totalLevels - 1);
        if (currentLevel >= PlayerPrefs.GetInt("TraversePathLevel", 0))
        {
            PlayerPrefs.SetInt("TraversePathLevel", next);
            PlayerPrefs.Save();
        }

        nextButton.GetComponentInChildren<TMP_Text>().text = currentLevel >= totalLevels - 1 ? "PLAY AGAIN" : "NEXT LEVEL";
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevel);
    }

    public void NextLevel()
    {
        if (currentLevel >= totalLevels - 1)
            currentLevel = 0;
        else
            currentLevel++;

        LoadLevel(currentLevel);
    }

    private Vector3 CellWorld(Vector3 origin, int x, int y)
    {
        return origin + new Vector3(x * cellSize, y * cellSize, 0f);
    }

    private void FitCamera(float boardHeight)
    {
        float screenAspect = Mathf.Max(0.1f, (float)Screen.width / Screen.height);
        float boardWidth = maze.Width * cellSize + 0.7f;
        float verticalNeed = boardHeight + 1.9f;
        float horizontalNeed = boardWidth / screenAspect;
        cam.orthographicSize = Mathf.Max(verticalNeed, horizontalNeed) * 0.5f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
    }

    private void CreateWall(Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.SetParent(parent);
        wall.transform.position = position;
        wall.transform.localScale = scale;

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = wallColor;
        sr.material = material;
        sr.sortingOrder = 10;
    }

    private Material CreateMaterial()
    {
        Shader shader = Shader.Find("Sprites/Default");
        return new Material(shader);
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 2);
    }

    private Sprite CreateCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] p = new Color[size * size];
        float c = (size - 1) * 0.5f;
        float r = c - 1f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - c;
                float dy = y - c;
                p[y * size + x] = dx * dx + dy * dy <= r * r ? Color.white : Color.clear;
            }
        tex.SetPixels(p);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private Sprite CreateRingSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] p = new Color[size * size];
        float c = (size - 1) * 0.5f;
        float outer = c - 1f;
        float inner = c * 0.62f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - c;
                float dy = y - c;
                float d2 = dx * dx + dy * dy;
                p[y * size + x] = d2 <= outer * outer && d2 >= inner * inner ? Color.white : Color.clear;
            }
        tex.SetPixels(p);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private TMP_Text CreateText(Transform parent, string name, string text, int fontSize, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPosition;
        TMP_Text t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = fontSize;
        t.alignment = TextAlignmentOptions.Center;
        t.color = Color.white;
        return t;
    }

    private Slider CreateSlider(Transform parent)
    {
        GameObject go = new GameObject("ProgressSlider");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(650, 28);
        rt.anchoredPosition = new Vector2(0, -205);

        Slider slider = go.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0;
        slider.interactable = false;

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(1, 1, 1, 0.18f);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(go.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = green;
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        slider.targetGraphic = fillImage;
        slider.fillRect = fillRT;
        return slider;
    }

    private GameObject CreatePanel(Transform parent, string name, Vector2 position)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.1f, 0.3f);
        rt.anchorMax = new Vector2(0.9f, 0.7f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.05f, 0.02f, 0.18f, 0.96f);
        return panel;
    }

    private Button CreateButton(Transform parent, string label, Vector2 position)
    {
        GameObject go = new GameObject(label + "Button");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(500, 130);
        rt.anchoredPosition = position;

        Image image = go.AddComponent<Image>();
        image.color = green;
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;

        TMP_Text text = CreateText(go.transform, "Label", label, 40, Vector2.zero, new Vector2(500, 130));
        text.color = Color.black;
        return button;
    }
}
