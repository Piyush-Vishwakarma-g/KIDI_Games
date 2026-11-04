using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotPlantManager : MonoBehaviour
{
    private const int GridSize = 3;
    private const float MoveDuration = 0.2f;
    private static readonly Color ButtonPurple = new Color(0.47f, 0.04f, 0.82f);

    [SerializeField] private Sprite waterButtonSprite;

    private readonly RectTransform[] flowers = new RectTransform[GridSize * GridSize];
    private readonly Image[][] flowerParts = new Image[GridSize * GridSize][];
    private readonly Button[] directionButtons = new Button[4];

    private Canvas gameCanvas;
    private RectTransform canvasRect;
    private RectTransform boardRect;
    private RectTransform robotRect;
    private Button waterButton;
    private Sprite circleSprite;
    private Sprite roundedButtonSprite;
    private Sprite waterIconSprite;
    private Texture2D circleTexture;
    private Texture2D roundedButtonTexture;
    public Vector2Int robotCell = new Vector2Int(1, 1);
    private bool isMoving;

    private static readonly Vector2Int[] DirectionOffsets =
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };

    private static readonly string[] DirectionLabels =
    {
        "↓  Bottom",
        "↑  Top",
        "←  Left",
        "→  Right"
    };

    private static readonly Color[] FlowerColors =
    {
        new Color(1f, 0.28f, 0.52f),
        new Color(1f, 0.56f, 0.16f),
        new Color(0.32f, 0.68f, 1f),
        new Color(0.82f, 0.39f, 1f),
        new Color(1f, 0.34f, 0.3f)
    };

    private void Start()
    {
        if (!ResolveSceneReferences())
        {
            enabled = false;
            return;
        }

        Canvas.ForceUpdateCanvases();
        CreateUiSprites();
        ConfigureRobot();
        CreateControls();
        RefreshControls();
    }

    private bool ResolveSceneReferences()
    {
        gameCanvas = FindRequired<Canvas>("Canvas");
        Image boardImage = FindRequired<Image>("tile");
        Image robotImage = FindRequired<Image>("robot");

        if (gameCanvas == null || boardImage == null || robotImage == null)
        {
            return false;
        }

        if (waterButtonSprite == null)
        {
            Debug.LogError("RobotPlantManager needs the Water button sprite assigned.", this);
            return false;
        }

        boardRect = boardImage.rectTransform;
        robotRect = robotImage.rectTransform;
        canvasRect = gameCanvas.transform as RectTransform;

        if (canvasRect == null || boardRect.parent != robotRect.parent)
        {
            Debug.LogError("The robot and garden board must share a Canvas parent.", this);
            return false;
        }

        return true;
    }

    private T FindRequired<T>(string objectName) where T : Component
    {
        GameObject target = GameObject.Find(objectName);
        T component = target != null ? target.GetComponent<T>() : null;

        if (component == null)
        {
            Debug.LogError($"RobotPlantManager could not find {objectName} with a {typeof(T).Name} component.", this);
        }

        return component;
    }

    private void ConfigureRobot()
    {
        Image robotImage = robotRect.GetComponent<Image>();
        robotImage.preserveAspect = true;
        robotImage.raycastTarget = false;
        robotRect.localRotation = Quaternion.identity;
        robotRect.localScale = Vector3.one;
        robotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boardRect.rect.width * 0.35f);
        robotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boardRect.rect.height * 0.55f);
        robotRect.anchoredPosition = GetRobotPosition(robotCell);
    }

    private void CreateControls()
    {
        float scale = Mathf.Min(1f, canvasRect.rect.width / 1600f);
        float buttonWidth = Mathf.Min(260f * scale, canvasRect.rect.width * 0.22f);
        float buttonHeight = 88f * scale;
        float waterWidth = 100f * scale;
        float gap = 18f * scale;
        float totalWidth = buttonWidth * 4f + waterWidth + gap * 4f;
        float leftEdge = -totalWidth * 0.5f;
        float y = 55f * scale;

        for (int i = 0; i < DirectionLabels.Length; i++)
        {
            float x = leftEdge + buttonWidth * 0.5f + i * (buttonWidth + gap);
            Vector2Int offset = DirectionOffsets[i];
            directionButtons[i] = CreateButton(
                DirectionLabels[i],
                new Vector2(x, y),
                new Vector2(buttonWidth, buttonHeight),
                ButtonPurple,
                () => MoveRobot(offset));
        }

        float waterX = leftEdge + buttonWidth * 4f + gap * 3f + waterWidth * 0.5f + gap;
        waterButton = CreateButton(
            "Water",
            new Vector2(waterX, y),
            new Vector2(waterWidth, buttonHeight),
            Color.white,
            WaterCurrentPlot,
            waterIconSprite);
    }

    private Button CreateButton(
        string objectName,
        Vector2 position,
        Vector2 size,
        Color normalColor,
        UnityEngine.Events.UnityAction onClick,
        Sprite sprite = null)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.layer = gameCanvas.gameObject.layer;
        buttonObject.transform.SetParent(gameCanvas.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.sprite = sprite != null ? sprite : roundedButtonSprite;
        buttonImage.type = sprite != null ? Image.Type.Simple : Image.Type.Sliced;
        buttonImage.preserveAspect = sprite != null;
        buttonImage.color = normalColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        button.colors = new ColorBlock
        {
            normalColor = normalColor,
            highlightedColor = sprite != null ? new Color(0.83f, 0.93f, 1f) : new Color(0.69f, 0.37f, 0.96f),
            pressedColor = sprite != null ? new Color(0.72f, 0.87f, 1f) : new Color(0.34f, 0.03f, 0.63f),
            selectedColor = normalColor,
            disabledColor = new Color(0.45f, 0.42f, 0.5f, 0.65f),
            colorMultiplier = 1f,
            fadeDuration = 0.12f
        };
        button.onClick.AddListener(onClick);

        if (sprite == null)
        {
            Shadow shadow = buttonObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.12f, 0.02f, 0.19f, 0.48f);
            shadow.effectDistance = new Vector2(0f, -5f * Mathf.Min(1f, canvasRect.rect.width / 1600f));

            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            labelObject.layer = gameCanvas.gameObject.layer;
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
            label.text = objectName;
            label.fontSize = 34f * Mathf.Min(1f, canvasRect.rect.width / 1600f);
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.raycastTarget = false;
        }

        return button;
    }

    private void MoveRobot(Vector2Int offset)
    {
        if (isMoving)
        {
            return;
        }

        Vector2Int destination = robotCell + offset;
        if (!IsOnBoard(destination))
        {
            return;
        }

        robotCell = destination;
        isMoving = true;
        RefreshControls();
        StartCoroutine(AnimateRobot(GetRobotPosition(robotCell)));
    }

    private IEnumerator AnimateRobot(Vector2 destination)
    {
        Vector2 start = robotRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < MoveDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / MoveDuration);
            float easedProgress = 1f - (1f - progress) * (1f - progress);
            robotRect.anchoredPosition = Vector2.LerpUnclamped(start, destination, easedProgress);
            yield return null;
        }

        robotRect.anchoredPosition = destination;
        isMoving = false;
        RefreshControls();
    }

    private void WaterCurrentPlot()
    {
        if (isMoving || !IsOnBoard(robotCell))
        {
            return;
        }

        int index = GetPlotIndex(robotCell);
        if (flowers[index] == null)
        {
            CreateFlower(robotCell, index);
            StartCoroutine(GrowFlower(flowers[index], flowerParts[index]));
        }
        else
        {
            StartCoroutine(PulseFlower(flowers[index]));
        }
    }

    private void CreateFlower(Vector2Int cell, int index)
    {
        GameObject flowerObject = new GameObject($"Flower_{cell.x}_{cell.y}", typeof(RectTransform));
        flowerObject.layer = gameCanvas.gameObject.layer;
        flowerObject.transform.SetParent(boardRect.parent, false);

        RectTransform flowerRect = flowerObject.GetComponent<RectTransform>();
        flowerRect.anchorMin = new Vector2(0.5f, 0.5f);
        flowerRect.anchorMax = new Vector2(0.5f, 0.5f);
        flowerRect.pivot = new Vector2(0.5f, 0.5f);
        flowerRect.sizeDelta = new Vector2(boardRect.rect.width * 0.14f, boardRect.rect.height * 0.2f);
        flowerRect.anchoredPosition = GetPlotPosition(cell) + Vector2.up * (boardRect.rect.height * 0.04f);
        flowerRect.localScale = Vector3.one * 0.04f;
        flowerRect.SetAsLastSibling();

        Color petals = FlowerColors[index % FlowerColors.Length];
        AddFlowerPart(flowerRect, "Stem", null, new Color(0.14f, 0.58f, 0.2f), new Vector2(7f, 58f), new Vector2(0f, -23f), 0f);
        AddFlowerPart(flowerRect, "Left leaf", circleSprite, new Color(0.25f, 0.76f, 0.27f), new Vector2(27f, 13f), new Vector2(-14f, -18f), 28f);
        AddFlowerPart(flowerRect, "Right leaf", circleSprite, new Color(0.18f, 0.65f, 0.23f), new Vector2(27f, 13f), new Vector2(14f, -8f), -28f);

        const float headY = 37f;
        const float petalRadius = 17f;
        for (int petal = 0; petal < 5; petal++)
        {
            float angle = petal * Mathf.PI * 2f / 5f;
            Vector2 petalPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * petalRadius;
            AddFlowerPart(flowerRect, $"Petal_{petal}", circleSprite, petals, new Vector2(27f, 27f), petalPosition + Vector2.up * headY, 0f);
        }

        AddFlowerPart(flowerRect, "Flower center", circleSprite, new Color(1f, 0.84f, 0.2f), new Vector2(21f, 21f), Vector2.up * headY, 0f);
        flowers[index] = flowerRect;
        flowerParts[index] = flowerRect.GetComponentsInChildren<Image>();
    }

    private void AddFlowerPart(
        RectTransform parent,
        string objectName,
        Sprite sprite,
        Color color,
        Vector2 size,
        Vector2 position,
        float rotation)
    {
        GameObject part = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        part.layer = gameCanvas.gameObject.layer;
        part.transform.SetParent(parent, false);

        RectTransform partRect = part.GetComponent<RectTransform>();
        partRect.anchorMin = new Vector2(0.5f, 0.5f);
        partRect.anchorMax = new Vector2(0.5f, 0.5f);
        partRect.pivot = new Vector2(0.5f, 0.5f);
        partRect.sizeDelta = size;
        partRect.anchoredPosition = position;
        partRect.localRotation = Quaternion.Euler(0f, 0f, rotation);

        Image image = part.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
    }

    private IEnumerator GrowFlower(RectTransform flower, Image[] parts)
    {
        const float duration = 0.55f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float easedProgress = 1f + 2.70158f * Mathf.Pow(progress - 1f, 3f) + 1.70158f * Mathf.Pow(progress - 1f, 2f);
            flower.localScale = Vector3.one * easedProgress;

            for (int i = 0; i < parts.Length; i++)
            {
                Color color = parts[i].color;
                color.a = progress;
                parts[i].color = color;
            }

            yield return null;
        }

        flower.localScale = Vector3.one;
    }

    private IEnumerator PulseFlower(RectTransform flower)
    {
        const float duration = 0.28f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float pulse = Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI);
            flower.localScale = Vector3.one * (1f + pulse * 0.16f);
            yield return null;
        }

        flower.localScale = Vector3.one;
    }

    private void RefreshControls()
    {
        if (waterButton != null)
        {
            waterButton.interactable = !isMoving && IsOnBoard(robotCell);
        }

        for (int i = 0; i < directionButtons.Length; i++)
        {
            if (directionButtons[i] != null)
            {
                directionButtons[i].interactable = !isMoving && IsOnBoard(robotCell + DirectionOffsets[i]);
            }
        }
    }

    private bool IsOnBoard(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < GridSize && cell.y >= 0 && cell.y < GridSize;
    }

    private static int GetPlotIndex(Vector2Int cell)
    {
        return cell.y * GridSize + cell.x;
    }

    // Plot centers follow the two diagonal axes of the isometric garden-bed artwork.
    private Vector2 GetPlotPosition(Vector2Int cell)
    {
        float horizontal = 0.599f - 0.137f * cell.y + 0.105f * cell.x;
        float vertical = 0.207f + 0.103f * cell.y + 0.126f * cell.x;
        return boardRect.anchoredPosition + new Vector2(
            (horizontal - 0.5f) * boardRect.rect.width,
            (0.5f - vertical) * boardRect.rect.height);
    }

    private Vector2 GetRobotPosition(Vector2Int cell)
    {
        Vector2 plotPosition = GetPlotPosition(cell);
        float footOffset = robotRect.rect.height * 0.25f;
        return plotPosition + Vector2.up * footOffset;
    }

    private void CreateUiSprites()
    {
        circleTexture = CreateTexture(64, 64, true);
        circleSprite = Sprite.Create(circleTexture, new Rect(0f, 0f, 64f, 64f), new Vector2(0.5f, 0.5f), 64f);

        Rect sourceRect = waterButtonSprite.rect;
        Rect iconRect = new Rect(sourceRect.x + 52f, sourceRect.y + sourceRect.height - 114f, 82f, 94f);
        waterIconSprite = Sprite.Create(
            waterButtonSprite.texture,
            iconRect,
            new Vector2(0.5f, 0.5f),
            waterButtonSprite.pixelsPerUnit,
            0,
            SpriteMeshType.FullRect,
            Vector4.zero);

        const int width = 96;
        const int height = 48;
        roundedButtonTexture = CreateTexture(width, height, false, false);
        Color[] pixels = new Color[width * height];
        float radius = 15f;
        Vector2 halfSize = new Vector2(width * 0.5f, height * 0.5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 point = new Vector2(x + 0.5f, y + 0.5f) - halfSize;
                Vector2 distance = new Vector2(Mathf.Abs(point.x), Mathf.Abs(point.y)) - (halfSize - Vector2.one * radius);
                float signedDistance = Mathf.Min(Mathf.Max(distance.x, distance.y), 0f) + Vector2.Max(distance, Vector2.zero).magnitude - radius;
                float alpha = 1f - Mathf.SmoothStep(-0.65f, 0.65f, signedDistance);
                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        roundedButtonTexture.SetPixels(pixels);
        roundedButtonTexture.Apply(false, true);
        roundedButtonSprite = Sprite.Create(
            roundedButtonTexture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(radius, radius, radius, radius));
    }

    private static Texture2D CreateTexture(int width, int height, bool circle, bool makeNoLongerReadable = true)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        Color[] pixels = new Color[width * height];
        Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
        float radius = Mathf.Min(width, height) * 0.5f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float alpha = circle
                    ? 1f - Mathf.SmoothStep(radius - 1f, radius + 1f, Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center))
                    : 1f;
                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, makeNoLongerReadable);
        return texture;
    }

    private void OnDestroy()
    {
        Destroy(circleSprite);
        Destroy(roundedButtonSprite);
        Destroy(waterIconSprite);
        Destroy(circleTexture);
        Destroy(roundedButtonTexture);
    }
}
