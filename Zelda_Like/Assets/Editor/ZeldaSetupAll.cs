using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ZeldaSetupAll
{
    private const string RoomParentPrefabPath = "Assets/Rooms/RoomParent.prefab";
    private const string RoomBossEndFolderPath = "Assets/Rooms/Rooms/RoomBossEnd";
    private const string RoomBossEndPrefabPath = RoomBossEndFolderPath + "/RoomBossEnd.prefab";

    private readonly struct RebindRowRefs
    {
        public RebindRowRefs(string actionName, TextMeshProUGUI displayText, Button rebindButton)
        {
            ActionName = actionName;
            DisplayText = displayText;
            RebindButton = rebindButton;
        }

        public string ActionName { get; }
        public TextMeshProUGUI DisplayText { get; }
        public Button RebindButton { get; }
    }

    [MenuItem("Zelda/Setup/0 - Run ALL Setup")]
    public static void RunAllSetup()
    {
        bool pauseOk = CreatePausePanelInternal();
        bool prefabOk = CreateRoomBossEndPrefabInternal(false) != null;
        bool endPanelOk = SetupEndPanelInternal(false);

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Zelda Setup",
            $"Pause Panel: {(pauseOk ? "OK" : "Check Console")}\nRoomBossEnd Prefab: {(prefabOk ? "OK" : "Check Console")}\nEnd Panel: {(endPanelOk ? "OK" : "Check Console")}",
            "OK");
    }

    [MenuItem("Zelda/Setup/1 - Create Pause Panel")]
    public static void CreatePausePanel()
    {
        CreatePausePanelInternal();
    }

    [MenuItem("Zelda/Setup/2 - Create RoomBossEnd Prefab")]
    public static void CreateRoomBossEndPrefab()
    {
        CreateRoomBossEndPrefabInternal(true);
    }

    [MenuItem("Zelda/Setup/3 - Setup End Panel")]
    public static void SetupEndPanel()
    {
        SetupEndPanelInternal(true);
    }

    private static bool CreatePausePanelInternal()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning("[ZeldaSetupAll] Canvas introuvable dans la scène active.");
            return false;
        }

        Transform existingPausePanel = canvas.transform.Find("PausePanel");
        if (existingPausePanel != null)
        {
            Object.DestroyImmediate(existingPausePanel.gameObject);
        }

        GameObject pausePanel = CreateUIObject("PausePanel", canvas.transform, typeof(Image));
        StretchRect(pausePanel.GetComponent<RectTransform>());
        pausePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
        pausePanel.SetActive(false);

        GameObject container = CreateUIObject("PauseContainer", pausePanel.transform, typeof(Image), typeof(VerticalLayoutGroup));
        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;
        containerRect.sizeDelta = new Vector2(400f, 500f);

        Image containerImage = container.GetComponent<Image>();
        containerImage.color = new Color(0.12f, 0.12f, 0.12f, 0.96f);

        VerticalLayoutGroup layout = container.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 15f;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childControlHeight = true;
        layout.childControlWidth = true;

        CreateTextBlock(container.transform, "Title", "⏸ PAUSE", 32, FontStyles.Bold, TextAlignmentOptions.Center, Color.white, -1f, 40f);
        CreateSeparator(container.transform);
        CreateTextBlock(container.transform, "MusicLabel", "🎵 Musique", 16, FontStyles.Normal, TextAlignmentOptions.Left, Color.white, -1f, 24f);
        Slider musicSlider = CreateSlider(container.transform, "MusicSlider", 1f);
        CreateTextBlock(container.transform, "SfxLabel", "🔊 Effets sonores", 16, FontStyles.Normal, TextAlignmentOptions.Left, Color.white, -1f, 24f);
        Slider sfxSlider = CreateSlider(container.transform, "SFXSlider", 1f);
        CreateSeparator(container.transform);
        CreateTextBlock(container.transform, "ControlsLabel", "🎮 CONTROLES", 20, FontStyles.Bold, TextAlignmentOptions.Center, Color.white, -1f, 30f);

        List<RebindRowRefs> rebindRows = new List<RebindRowRefs>
        {
            CreateRebindRow(container.transform, "Move", "Se déplacer"),
            CreateRebindRow(container.transform, "Attack", "Attaque"),
            CreateRebindRow(container.transform, "Weapon1", "Arme 1"),
            CreateRebindRow(container.transform, "Weapon2", "Arme 2")
        };

        CreateSeparator(container.transform);
        Button resumeButton = CreateButton(container.transform, "ResumeButton", "▶ Reprendre", new Color32(52, 152, 219, 255), new Vector2(300f, 50f));
        Button menuButton = CreateButton(container.transform, "MainMenuButton", "🏠 Menu Principal", new Color32(41, 75, 160, 255), new Vector2(300f, 50f));
        Button quitButton = CreateButton(container.transform, "QuitButton", "✕ Quitter", new Color32(139, 0, 0, 255), new Vector2(300f, 50f));

        GameObject managerObject = FindSceneObject("PauseManager");
        if (managerObject == null)
        {
            managerObject = new GameObject("PauseManager");
        }

        PauseManager pauseManager = managerObject.GetComponent<PauseManager>();
        if (pauseManager == null)
        {
            pauseManager = managerObject.AddComponent<PauseManager>();
        }

        SerializedObject so = new SerializedObject(pauseManager);
        SetObjectReference(so, "pausePanel", pausePanel);
        SetObjectReference(so, "musicVolumeSlider", musicSlider);
        SetObjectReference(so, "sfxVolumeSlider", sfxSlider);
        SetObjectReference(so, "resumeButton", resumeButton);
        SetObjectReference(so, "mainMenuButton", menuButton);
        SetObjectReference(so, "quitButton", quitButton);

        PlayerInput playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogWarning("[ZeldaSetupAll] PlayerInput introuvable dans la scène.");
        }

        SetObjectReference(so, "playerInput", playerInput);

        SerializedProperty rebindListProp = so.FindProperty("rebindButtons");
        if (rebindListProp != null)
        {
            rebindListProp.arraySize = rebindRows.Count;
            for (int i = 0; i < rebindRows.Count; i++)
            {
                SerializedProperty element = rebindListProp.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("actionName").stringValue = rebindRows[i].ActionName;
                element.FindPropertyRelative("displayText").objectReferenceValue = rebindRows[i].DisplayText;
                element.FindPropertyRelative("rebindButton").objectReferenceValue = rebindRows[i].RebindButton;
            }
        }
        else
        {
            Debug.LogWarning("[ZeldaSetupAll] Propriété 'rebindButtons' introuvable sur PauseManager.");
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(pauseManager);
        EditorUtility.SetDirty(managerObject);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        Debug.Log("[ZeldaSetupAll] Pause panel créé et PauseManager configuré.");
        return playerInput != null;
    }

    private static GameObject CreateRoomBossEndPrefabInternal(bool showDialog)
    {
        GameObject roomParentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RoomParentPrefabPath);
        if (roomParentPrefab == null)
        {
            Debug.LogWarning($"[ZeldaSetupAll] Prefab introuvable: {RoomParentPrefabPath}");
            return null;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(roomParentPrefab) as GameObject;
        if (instance == null)
        {
            Debug.LogWarning("[ZeldaSetupAll] Impossible d'instancier RoomParent.");
            return null;
        }

        instance.name = "RoomBossEnd";

        RoomBossEnd roomBossEnd = instance.GetComponent<RoomBossEnd>();
        if (roomBossEnd == null)
        {
            roomBossEnd = instance.AddComponent<RoomBossEnd>();
        }

        BoxCollider trigger = instance.GetComponent<BoxCollider>();
        if (trigger == null)
        {
            trigger = instance.AddComponent<BoxCollider>();
        }

        trigger.isTrigger = true;
        trigger.center = new Vector3(0f, 1.5f, 0f);
        trigger.size = new Vector3(14f, 3f, 14f);

        Transform spawnPoint = instance.transform.Find("BossSpawnPoint");
        if (spawnPoint == null)
        {
            GameObject spawnPointObject = new GameObject("BossSpawnPoint");
            spawnPointObject.transform.SetParent(instance.transform, false);
            spawnPoint = spawnPointObject.transform;
        }

        spawnPoint.localPosition = new Vector3(0f, 0.5f, 5f);
        spawnPoint.localRotation = Quaternion.identity;
        spawnPoint.localScale = Vector3.one;

        SerializedObject bossSo = new SerializedObject(roomBossEnd);
        SetObjectReference(bossSo, "bossSpawnPoint", spawnPoint);
        bossSo.ApplyModifiedProperties();

        EnsureFolderExists(RoomBossEndFolderPath);
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(instance, RoomBossEndPrefabPath);
        Object.DestroyImmediate(instance);

        if (savedPrefab == null)
        {
            Debug.LogWarning("[ZeldaSetupAll] Échec de sauvegarde du prefab RoomBossEnd.");
            return null;
        }

        RoomManager roomManager = Object.FindFirstObjectByType<RoomManager>();
        if (roomManager == null)
        {
            Debug.LogWarning("[ZeldaSetupAll] RoomManager introuvable dans la scène active.");
        }
        else
        {
            SerializedObject roomManagerSo = new SerializedObject(roomManager);
            SetObjectReference(roomManagerSo, "bossRoomPrefab", savedPrefab);
            SetBool(roomManagerSo, "spawnBossRoom", true);
            roomManagerSo.ApplyModifiedProperties();
            EditorUtility.SetDirty(roomManager);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        EditorUtility.SetDirty(savedPrefab);
        AssetDatabase.SaveAssets();

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Zelda Setup", "RoomBossEnd prefab créé et RoomManager configuré.", "OK");
        }

        Debug.Log($"[ZeldaSetupAll] Prefab créé: {RoomBossEndPrefabPath}");
        return savedPrefab;
    }

    private static bool SetupEndPanelInternal(bool showDialog)
    {
        GameObject endPanel = GameObject.Find("End_Panel");
        if (endPanel == null)
        {
            endPanel = FindSceneObject("End_Panel");
        }

        if (endPanel == null)
        {
            Debug.LogWarning("[ZeldaSetupAll] End_Panel introuvable dans la scène active.");
            return false;
        }

        GameObject content = GetOrCreateChild(endPanel, "Content", typeof(VerticalLayoutGroup));
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(380f, 220f);

        VerticalLayoutGroup contentLayout = content.GetComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 16f;
        contentLayout.padding = new RectOffset(20, 20, 20, 20);
        contentLayout.childAlignment = TextAnchor.MiddleCenter;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;
        contentLayout.childControlHeight = true;
        contentLayout.childControlWidth = true;

        TextMeshProUGUI victoryText = GetOrCreateText(content.transform, "VictoryText");
        ConfigureText(victoryText, "VICTOIRE !", 36, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
        SetLayout(victoryText.gameObject, -1f, 40f, -1f, -1f, 1f, -1f);

        Button returnToMenuButton = GetOrCreateButton(content.transform, "ReturnToMenuButton", "🏠 Retour Menu", new Color32(41, 75, 160, 255), new Vector2(260f, 50f));
        Button quitButton = GetOrCreateButton(content.transform, "QuitButton", "✕ Quitter", new Color32(139, 0, 0, 255), new Vector2(260f, 50f));

        endPanel.SetActive(false);

        bool prefabFieldAssigned = TryAssignEndPanelReferencesToPrefab(endPanel, returnToMenuButton, quitButton, victoryText);
        AssignEndPanelReferencesToSceneInstances(endPanel, returnToMenuButton, quitButton, victoryText);

        EditorUtility.SetDirty(endPanel);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();

        if (!prefabFieldAssigned)
        {
            Debug.LogWarning("[ZeldaSetupAll] Les références de scène ne peuvent pas être persistées dans RoomBossEnd.prefab. Le panneau a été préparé dans la scène, mais les références prefab devront être résolues à l'exécution ou sur une instance de scène.");
        }

        if (showDialog)
        {
            EditorUtility.DisplayDialog("Zelda Setup", "End_Panel configuré. Vérifiez la console pour les limitations de référence prefab/scène.", "OK");
        }

        Debug.Log("[ZeldaSetupAll] End_Panel configuré.");
        return true;
    }

    private static bool TryAssignEndPanelReferencesToPrefab(GameObject endPanel, Button returnToMenuButton, Button quitButton, TextMeshProUGUI victoryText)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RoomBossEndPrefabPath);
        if (prefab == null)
        {
            Debug.LogWarning($"[ZeldaSetupAll] Prefab introuvable: {RoomBossEndPrefabPath}");
            return false;
        }

        RoomBossEnd roomBossEnd = prefab.GetComponent<RoomBossEnd>();
        if (roomBossEnd == null)
        {
            Debug.LogWarning("[ZeldaSetupAll] Composant RoomBossEnd introuvable sur le prefab.");
            return false;
        }

        if (!EditorUtility.IsPersistent(endPanel) || !EditorUtility.IsPersistent(returnToMenuButton) || !EditorUtility.IsPersistent(quitButton) || !EditorUtility.IsPersistent(victoryText))
        {
            return false;
        }

        SerializedObject so = new SerializedObject(roomBossEnd);
        SetObjectReference(so, "endPanel", endPanel);
        SetObjectReference(so, "returnToMenuButton", returnToMenuButton);
        SetObjectReference(so, "quitButton", quitButton);
        SetObjectReference(so, "victoryText", victoryText);
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(roomBossEnd);
        AssetDatabase.SaveAssets();
        return true;
    }

    private static void AssignEndPanelReferencesToSceneInstances(GameObject endPanel, Button returnToMenuButton, Button quitButton, TextMeshProUGUI victoryText)
    {
        RoomBossEnd[] roomBossEnds = Object.FindObjectsByType<RoomBossEnd>(FindObjectsSortMode.None);
        foreach (RoomBossEnd roomBossEnd in roomBossEnds)
        {
            if (EditorUtility.IsPersistent(roomBossEnd))
            {
                continue;
            }

            SerializedObject so = new SerializedObject(roomBossEnd);
            SetObjectReference(so, "endPanel", endPanel);
            SetObjectReference(so, "returnToMenuButton", returnToMenuButton);
            SetObjectReference(so, "quitButton", quitButton);
            SetObjectReference(so, "victoryText", victoryText);
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(roomBossEnd);
        }
    }

    private static RebindRowRefs CreateRebindRow(Transform parent, string actionName, string labelText)
    {
        GameObject row = CreateUIObject(actionName + "Row", parent, typeof(HorizontalLayoutGroup));
        SetLayout(row, -1f, 36f, -1f, -1f, 1f, -1f);

        HorizontalLayoutGroup horizontal = row.GetComponent<HorizontalLayoutGroup>();
        horizontal.spacing = 10f;
        horizontal.childAlignment = TextAnchor.MiddleCenter;
        horizontal.childForceExpandWidth = false;
        horizontal.childForceExpandHeight = false;
        horizontal.childControlWidth = true;
        horizontal.childControlHeight = true;

        TextMeshProUGUI actionLabel = CreateTextBlock(row.transform, actionName + "Label", labelText, 15, FontStyles.Normal, TextAlignmentOptions.Left, Color.white, 150f, 30f);
        SetLayout(actionLabel.gameObject, 150f, 30f, 120f, -1f, 1f, -1f);

        TextMeshProUGUI keyLabel = CreateTextBlock(row.transform, actionName + "Key", "?", 15, FontStyles.Bold, TextAlignmentOptions.Center, Color.white, 80f, 30f);
        SetLayout(keyLabel.gameObject, 80f, 30f, 70f, -1f, 0f, -1f);

        Button rebindButton = CreateButton(row.transform, actionName + "Button", "Changer", new Color32(90, 90, 90, 255), new Vector2(60f, 30f));
        SetLayout(rebindButton.gameObject, 60f, 30f, 60f, 30f, 0f, 0f);

        return new RebindRowRefs(actionName, keyLabel, rebindButton);
    }

    private static GameObject CreateSeparator(Transform parent)
    {
        GameObject separator = CreateUIObject("Separator", parent, typeof(Image));
        separator.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
        SetLayout(separator, -1f, 2f, -1f, 2f, 1f, 0f);
        return separator;
    }

    private static TextMeshProUGUI CreateTextBlock(Transform parent, string name, string content, float fontSize, FontStyles fontStyles, TextAlignmentOptions alignment, Color color, float preferredWidth, float preferredHeight)
    {
        TextMeshProUGUI text = GetOrCreateText(parent, name);
        ConfigureText(text, content, fontSize, fontStyles, alignment, color);
        SetLayout(text.gameObject, preferredWidth, preferredHeight, preferredWidth > 0f ? preferredWidth : -1f, -1f, preferredWidth > 0f ? 0f : 1f, 0f);
        return text;
    }

    private static void ConfigureText(TextMeshProUGUI text, string content, float fontSize, FontStyles fontStyles, TextAlignmentOptions alignment, Color color)
    {
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyles;
        text.alignment = alignment;
        text.color = color;
        text.enableWordWrapping = false;

        if (text.font == null && TMP_Settings.defaultFontAsset != null)
        {
            text.font = TMP_Settings.defaultFontAsset;
        }
    }

    private static Slider CreateSlider(Transform parent, string name, float defaultValue)
    {
        GameObject sliderObject = CreateUIObject(name, parent, typeof(Slider));
        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(0f, 30f);
        SetLayout(sliderObject, -1f, 30f, -1f, 30f, 1f, 0f);

        GameObject background = CreateUIObject("Background", sliderObject.transform, typeof(Image));
        StretchRect(background.GetComponent<RectTransform>(), new Vector2(0f, 0.2f), new Vector2(1f, 0.8f), Vector2.zero, Vector2.zero);
        background.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);

        GameObject fillArea = CreateUIObject("Fill Area", sliderObject.transform);
        StretchRect(fillArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(10f, 0f), new Vector2(-10f, 0f));

        GameObject fill = CreateUIObject("Fill", fillArea.transform, typeof(Image));
        StretchRect(fill.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image fillImage = fill.GetComponent<Image>();
        fillImage.color = new Color(0.27f, 0.74f, 0.43f, 1f);

        GameObject handleSlideArea = CreateUIObject("Handle Slide Area", sliderObject.transform);
        StretchRect(handleSlideArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(10f, 0f), new Vector2(-10f, 0f));

        GameObject handle = CreateUIObject("Handle", handleSlideArea.transform, typeof(Image));
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(20f, 20f);
        Image handleImage = handle.GetComponent<Image>();
        handleImage.color = Color.white;

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = defaultValue;
        slider.wholeNumbers = false;
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;

        return slider;
    }

    private static Button CreateButton(Transform parent, string name, string label, Color backgroundColor, Vector2 size)
    {
        Button button = GetOrCreateButton(parent, name, label, backgroundColor, size);
        SetLayout(button.gameObject, size.x, size.y, size.x, size.y, 0f, 0f);
        return button;
    }

    private static Button GetOrCreateButton(Transform parent, string name, string label, Color backgroundColor, Vector2 size)
    {
        GameObject buttonObject = GetOrCreateChild(parent.gameObject, name, typeof(Image), typeof(Button));
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.sizeDelta = size;

        Image image = buttonObject.GetComponent<Image>();
        image.color = backgroundColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        TextMeshProUGUI text = GetOrCreateText(buttonObject.transform, "Text");
        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10f, 0f);
        textRect.offsetMax = new Vector2(-10f, 0f);
        ConfigureText(text, label, 18, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);

        return button;
    }

    private static TextMeshProUGUI GetOrCreateText(Transform parent, string name)
    {
        GameObject textObject = GetOrCreateChild(parent.gameObject, name, typeof(TextMeshProUGUI));
        return textObject.GetComponent<TextMeshProUGUI>();
    }

    private static GameObject GetOrCreateChild(GameObject parent, string name, params System.Type[] components)
    {
        Transform child = parent.transform.Find(name);
        GameObject childObject = child != null ? child.gameObject : CreateUIObject(name, parent.transform, components);

        foreach (System.Type componentType in components)
        {
            if (componentType != null && childObject.GetComponent(componentType) == null)
            {
                childObject.AddComponent(componentType);
            }
        }

        return childObject;
    }

    private static GameObject CreateUIObject(string name, Transform parent, params System.Type[] additionalComponents)
    {
        List<System.Type> types = new List<System.Type> { typeof(RectTransform) };
        if (additionalComponents != null)
        {
            types.AddRange(additionalComponents);
        }

        GameObject gameObject = new GameObject(name, types.ToArray());
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static void StretchRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static void StretchRect(RectTransform rect)
    {
        StretchRect(rect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
    }

    private static void SetLayout(GameObject gameObject, float preferredWidth, float preferredHeight, float minWidth, float minHeight, float flexibleWidth, float flexibleHeight)
    {
        LayoutElement layout = gameObject.GetComponent<LayoutElement>();
        if (layout == null)
        {
            layout = gameObject.AddComponent<LayoutElement>();
        }

        if (preferredWidth >= 0f)
        {
            layout.preferredWidth = preferredWidth;
        }

        if (preferredHeight >= 0f)
        {
            layout.preferredHeight = preferredHeight;
        }

        if (minWidth >= 0f)
        {
            layout.minWidth = minWidth;
        }

        if (minHeight >= 0f)
        {
            layout.minHeight = minHeight;
        }

        if (flexibleWidth >= 0f)
        {
            layout.flexibleWidth = flexibleWidth;
        }

        if (flexibleHeight >= 0f)
        {
            layout.flexibleHeight = flexibleHeight;
        }
    }

    private static void SetObjectReference(SerializedObject so, string propertyName, Object value)
    {
        SerializedProperty property = so.FindProperty(propertyName);
        if (property == null)
        {
            Debug.LogWarning($"[ZeldaSetupAll] Propriété '{propertyName}' introuvable sur {so.targetObject.name}.");
            return;
        }

        property.objectReferenceValue = value;
    }

    private static void SetBool(SerializedObject so, string propertyName, bool value)
    {
        SerializedProperty property = so.FindProperty(propertyName);
        if (property == null)
        {
            Debug.LogWarning($"[ZeldaSetupAll] Propriété '{propertyName}' introuvable sur {so.targetObject.name}.");
            return;
        }

        property.boolValue = value;
    }

    private static void EnsureFolderExists(string assetFolderPath)
    {
        string[] parts = assetFolderPath.Split('/');
        if (parts.Length == 0)
        {
            return;
        }

        string currentPath = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string nextPath = currentPath + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, parts[i]);
            }

            currentPath = nextPath;
        }
    }

    private static GameObject FindSceneObject(string name)
    {
        GameObject direct = GameObject.Find(name);
        if (direct != null)
        {
            return direct;
        }

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject gameObject in allObjects)
        {
            if (gameObject.name == name && gameObject.scene.IsValid() && !EditorUtility.IsPersistent(gameObject))
            {
                return gameObject;
            }
        }

        return null;
    }
}
