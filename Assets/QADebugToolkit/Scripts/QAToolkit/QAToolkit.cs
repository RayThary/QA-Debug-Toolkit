using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QAToolkit : MonoBehaviour
{
    public enum SaveRootType
    {
        ProjectFolder,
        AppData,
        CustomPath
    }

    [Header("Toolkit UI")]
    [SerializeField] private Canvas toolkitCanvas;
    [SerializeField] private CanvasScaler toolkitCanvasScaler;

    [Header("Runtime Info Text")]
    [SerializeField] private TextMeshProUGUI sceneNameText;
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI timeScaleText;

    [Header("Font Settings")]
    [SerializeField] private TMP_FontAsset basicFont;

    [Header("Canvas Settings")]
    [SerializeField] private int canvasSortingOrder = 30000;
    [SerializeField] private Vector2 referenceResolution = new Vector2(1920f, 1080f);
    [SerializeField, Range(0f, 1f)] private float matchWidthOrHeight = 0.5f;

    [Header("Save Settings")]
    [SerializeField] private SaveRootType saveRootType = SaveRootType.ProjectFolder;
    [SerializeField] private string saveFolderName = "QAReports";
    [SerializeField] private string customSavePath;

    [Header("Toolkit Settings")]
    [SerializeField] private bool startClosed = true;
    [SerializeField] private bool applyCanvasSettingsOnAwake = true;
    [SerializeField] private bool applyFontOnAwake = true;


    private bool isOpen;
    private bool isToggleBlocked;
    private float fpsTimer;
    private int frameCount;
    private float currentFps;

    private void Awake()
    {
        FindCanvasComponents();
        CreateDefaultFolders();

        if (applyFontOnAwake)
            ApplyTMPFont();

        if (applyCanvasSettingsOnAwake)
            ApplyCanvasSettings();

        if (startClosed)
            SetToolkitActive(false);
    }

    private void Update()
    {
        CheckToggleInput();

        if (!isOpen)
            return;

        UpdateRuntimeInfo();
    }

    private void FindCanvasComponents()
    {
        if (toolkitCanvas == null)
            toolkitCanvas = GetComponentInChildren<Canvas>(true);

        if (toolkitCanvasScaler == null && toolkitCanvas != null)
            toolkitCanvasScaler = toolkitCanvas.GetComponent<CanvasScaler>();
    }

    private void CheckToggleInput()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.f1Key.wasPressedThisFrame)
            return;

        if (ShouldBlockToggleInput())
            return;

        ToggleToolkit();
    }

    private void ToggleToolkit()
    {
        SetToolkitActive(!isOpen);
    }

    public void SetToggleBlocked(bool value)
    {
        isToggleBlocked = value;
    }

    private bool ShouldBlockToggleInput()
    {
        if (isToggleBlocked)
            return true;

        return IsInputFieldFocused();
    }

    private bool IsInputFieldFocused()
    {
        if (EventSystem.current == null)
            return false;

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
            return false;

        if (selectedObject.GetComponent<TMP_InputField>() != null)
            return true;

        if (selectedObject.GetComponent<InputField>() != null)
            return true;

        return false;
    }

    private void SetToolkitActive(bool value)
    {
        isOpen = value;

        if (toolkitCanvas == null)
            return;

        toolkitCanvas.gameObject.SetActive(isOpen);

        if (isOpen)
            UpdateRuntimeInfo();
    }

    private void ApplyCanvasSettings()
    {
        if (toolkitCanvas == null)
            return;

        toolkitCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        toolkitCanvas.sortingOrder = canvasSortingOrder;

        if (toolkitCanvasScaler == null)
            return;

        toolkitCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        toolkitCanvasScaler.referenceResolution = referenceResolution;
        toolkitCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        toolkitCanvasScaler.matchWidthOrHeight = matchWidthOrHeight;
    }

    private void ApplyTMPFont()
    {
        if (basicFont == null)
            basicFont = TMP_Settings.defaultFontAsset;

        // Default TMP font asset is missing.
        if (basicFont == null)
            return;

        TextMeshProUGUI[] tmpTexts = GetComponentsInChildren<TextMeshProUGUI>(true);

        for (int i = 0; i < tmpTexts.Length; i++)
        {
            tmpTexts[i].font = basicFont;
        }
    }

    private void UpdateRuntimeInfo()
    {
        UpdateFps();

        if (sceneNameText != null)
            sceneNameText.text = "Scene : " + SceneManager.GetActiveScene().name;

        if (playTimeText != null)
            playTimeText.text = "Scene Time : " + FormatTime(Time.timeSinceLevelLoad);

        if (fpsText != null)
            fpsText.text = "FPS : " + currentFps.ToString("F1");

        if (timeScaleText != null)
            timeScaleText.text = "Time Scale : " + Time.timeScale.ToString("F2");
    }

    private void UpdateFps()
    {
        frameCount++;
        fpsTimer += Time.unscaledDeltaTime;

        if (fpsTimer < 0.25f)
            return;

        currentFps = frameCount / fpsTimer;
        frameCount = 0;
        fpsTimer = 0f;
    }

    private string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void CreateDefaultFolders()
    {
        GetScreenshotFolderPath();
        GetIssueFolderPath();
        GetChecklistFolderPath();
    }

    private string GetProjectRootPath()
    {
        DirectoryInfo projectRoot = Directory.GetParent(Application.dataPath);

        if (projectRoot == null)
            return Application.dataPath;

        return projectRoot.FullName;
    }

    private string GetRootPath()
    {
        string rootPath = string.Empty;

        switch (saveRootType)
        {
            case SaveRootType.ProjectFolder:
                rootPath = Directory.GetParent(Application.dataPath).FullName;
                break;

            case SaveRootType.AppData:
                rootPath = Application.persistentDataPath;
                break;

            case SaveRootType.CustomPath:
                rootPath = customSavePath;
                break;
        }

        if (string.IsNullOrWhiteSpace(rootPath))
            rootPath = Directory.GetParent(Application.dataPath).FullName;

        return rootPath;
    }

    private string GetQAReportFolderPath()
    {
        string path = Path.Combine(GetRootPath(), saveFolderName);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }


    public string GetScreenshotFolderPath()
    {
        string path = Path.Combine(GetQAReportFolderPath(), "Screenshots");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public string GetIssueFolderPath()
    {
        string path = Path.Combine(GetProjectRootPath(), saveFolderName, "Issues");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public string GetChecklistFolderPath()
    {
        string path = Path.Combine(GetQAReportFolderPath(), "Checklist");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

}