using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAScreenshotGallery : MonoBehaviour
{
    [Header("Screenshot Attach UI")]
    [SerializeField] private Button screenshotPreviewButton;
    [SerializeField] private Image screenshotPreviewImage;
    [SerializeField] private GameObject screenshotAddIconObject;
    [SerializeField] private TextMeshProUGUI selectedScreenshotFileNameText;
    [SerializeField] private Button clearSelectedScreenshotButton;

    [Header("Gallery UI")]
    [SerializeField] private GameObject screenshotGalleryWindow;
    [SerializeField] private Transform screenshotThumbnailContent;
    [SerializeField] private Button screenshotThumbnailTemplateButton;
    [SerializeField] private Button closeGalleryButton;

    [Header("Delete Screenshot UI")]
    [SerializeField] private GameObject deleteScreenshotConfirmWindow;
    [SerializeField] private TextMeshProUGUI deleteScreenshotConfirmMessageText;
    [SerializeField] private Button confirmDeleteScreenshotButton;
    [SerializeField] private Button cancelDeleteScreenshotButton;

    [Header("Reference")]
    [SerializeField] private QAToolkit qaToolkit;

    private const string DefaultReportFolderName = "QAReports";
    private const string ScreenshotFolderName = "Screenshots";
    private const string NoScreenshotText = "No Screenshot";

    private readonly List<Sprite> thumbnailSprites = new List<Sprite>();

    private Func<string, int> getScreenshotLinkedIssueCount;
    private Action<string> onScreenshotDeleted;

    private Sprite defaultScreenshotSprite;
    private Sprite selectedPreviewSprite;
    private string selectedScreenshotPath = string.Empty;
    private string pendingDeleteScreenshotPath = string.Empty;
    private bool isAttachMode = false;

    private void Awake()
    {
        InitializeWindowState();
        CacheDefaultPreviewSprite();
        BindButtons();

        SetSelectedScreenshotPath(string.Empty);
        RefreshGallery();
    }

    private void OnDestroy()
    {
        ClearThumbnailItems();
        DestroySelectedPreviewSprite();
    }

    public string GetSelectedScreenshotPath()
    {
        return selectedScreenshotPath ?? string.Empty;
    }

    public void SetupDeleteCallbacks(Func<string, int> linkedIssueCountCallback, Action<string> screenshotDeletedCallback)
    {
        getScreenshotLinkedIssueCount = linkedIssueCountCallback;
        onScreenshotDeleted = screenshotDeletedCallback;
    }

    public void SetSelectedScreenshotPath(string screenshotPath)
    {
        bool hasScreenshot = !string.IsNullOrWhiteSpace(screenshotPath) && File.Exists(screenshotPath);

        selectedScreenshotPath = hasScreenshot ? screenshotPath : string.Empty;

        Sprite newSprite = hasScreenshot ? LoadSpriteFromFile(screenshotPath) : null;
        SetPreviewSprite(newSprite);
        UpdateSelectedScreenshotUI();
    }

    public void OpenGalleryForAttach()
    {
        isAttachMode = true;
        ShowGalleryWindow();
    }

    public void OpenGalleryOnly()
    {
        isAttachMode = false;
        ShowGalleryWindow();
    }

    public void CloseGallery()
    {
        if (screenshotGalleryWindow != null)
            screenshotGalleryWindow.SetActive(false);
    }

    private void InitializeWindowState()
    {
        if (screenshotGalleryWindow != null)
            screenshotGalleryWindow.SetActive(false);

        if (screenshotThumbnailTemplateButton != null)
            screenshotThumbnailTemplateButton.gameObject.SetActive(false);

        if (deleteScreenshotConfirmWindow != null)
            deleteScreenshotConfirmWindow.SetActive(false);
    }

    private void CacheDefaultPreviewSprite()
    {
        if (screenshotPreviewImage != null)
            defaultScreenshotSprite = screenshotPreviewImage.sprite;
    }

    private void BindButtons()
    {
        if (screenshotPreviewButton != null)
        {
            screenshotPreviewButton.onClick.RemoveAllListeners();
            screenshotPreviewButton.onClick.AddListener(OpenGalleryForAttach);
        }

        if (closeGalleryButton != null)
        {
            closeGalleryButton.onClick.RemoveAllListeners();
            closeGalleryButton.onClick.AddListener(CloseGallery);
        }

        if (confirmDeleteScreenshotButton != null)
        {
            confirmDeleteScreenshotButton.onClick.RemoveAllListeners();
            confirmDeleteScreenshotButton.onClick.AddListener(ConfirmDeleteScreenshot);
        }

        if (cancelDeleteScreenshotButton != null)
        {
            cancelDeleteScreenshotButton.onClick.RemoveAllListeners();
            cancelDeleteScreenshotButton.onClick.AddListener(CloseDeleteConfirmWindow);
        }

        if (clearSelectedScreenshotButton != null)
        {
            clearSelectedScreenshotButton.onClick.RemoveAllListeners();
            clearSelectedScreenshotButton.onClick.AddListener(ClearSelectedScreenshot);
        }
    }

    private void ShowGalleryWindow()
    {
        RefreshGallery();

        if (screenshotGalleryWindow != null)
            screenshotGalleryWindow.SetActive(true);
    }

    private void RefreshGallery()
    {
        ClearThumbnailItems();

        if (qaToolkit == null)
            return;

        if (screenshotThumbnailContent == null)
            return;

        if (screenshotThumbnailTemplateButton == null)
            return;

        List<string> screenshotFolderPaths = GetScreenshotFolderPaths();

        if (screenshotFolderPaths.Count <= 0)
            return;

        List<string> screenshotPaths = GetScreenshotPaths(screenshotFolderPaths);

        for (int i = 0; i < screenshotPaths.Count; i++)
        {
            CreateThumbnailItem(screenshotPaths[i]);
        }
    }

    private List<string> GetScreenshotFolderPaths()
    {
        List<string> folderPaths = new List<string>();

        string currentScreenshotFolderPath = qaToolkit.GetScreenshotFolderPath();
        string unityDefaultScreenshotFolderPath = GetUnityDefaultScreenshotFolderPath();

        AddFolderPath(folderPaths, currentScreenshotFolderPath);

        if (!string.Equals(currentScreenshotFolderPath, unityDefaultScreenshotFolderPath, StringComparison.OrdinalIgnoreCase))
            AddFolderPath(folderPaths, unityDefaultScreenshotFolderPath);

        return folderPaths;
    }

    private string GetUnityDefaultScreenshotFolderPath()
    {
        return Path.Combine(Application.persistentDataPath, DefaultReportFolderName, ScreenshotFolderName);
    }

    private void AddFolderPath(List<string> folderPaths, string folderPath)
    {
        if (folderPaths == null)
            return;

        if (string.IsNullOrWhiteSpace(folderPath))
            return;

        folderPaths.Add(folderPath);
    }

    private List<string> GetScreenshotPaths(List<string> screenshotFolderPaths)
    {
        List<string> screenshotPaths = new List<string>();

        for (int i = 0; i < screenshotFolderPaths.Count; i++)
        {
            string folderPath = screenshotFolderPaths[i];

            if (string.IsNullOrWhiteSpace(folderPath))
                continue;

            if (!Directory.Exists(folderPath))
                continue;

            string[] pngFiles = Directory.GetFiles(folderPath, "*.png", SearchOption.TopDirectoryOnly);

            for (int j = 0; j < pngFiles.Length; j++)
            {
                AddScreenshotPath(screenshotPaths, pngFiles[j]);
            }
        }

        screenshotPaths.Sort((a, b) =>
        {
            DateTime aTime = File.GetLastWriteTime(a);
            DateTime bTime = File.GetLastWriteTime(b);

            return bTime.CompareTo(aTime);
        });

        return screenshotPaths;
    }

    private void AddScreenshotPath(List<string> screenshotPaths, string screenshotPath)
    {
        if (screenshotPaths == null)
            return;

        if (string.IsNullOrWhiteSpace(screenshotPath))
            return;

        string normalizedPath = Path.GetFullPath(screenshotPath);

        for (int i = 0; i < screenshotPaths.Count; i++)
        {
            if (string.Equals(Path.GetFullPath(screenshotPaths[i]), normalizedPath, StringComparison.OrdinalIgnoreCase))
                return;
        }

        screenshotPaths.Add(normalizedPath);
    }

    private void ClearThumbnailItems()
    {
        if (screenshotThumbnailContent == null)
            return;

        for (int i = screenshotThumbnailContent.childCount - 1; i >= 0; i--)
        {
            Transform child = screenshotThumbnailContent.GetChild(i);

            if (screenshotThumbnailTemplateButton != null && child == screenshotThumbnailTemplateButton.transform)
                continue;

            Destroy(child.gameObject);
        }

        for (int i = 0; i < thumbnailSprites.Count; i++)
        {
            DestroySpriteWithTexture(thumbnailSprites[i]);
        }

        thumbnailSprites.Clear();
    }

    private void CreateThumbnailItem(string screenshotPath)
    {
        Sprite thumbnailSprite = LoadSpriteFromFile(screenshotPath);

        if (thumbnailSprite == null)
            return;

        thumbnailSprites.Add(thumbnailSprite);

        Button thumbnailButton = Instantiate(screenshotThumbnailTemplateButton, screenshotThumbnailContent);
        thumbnailButton.gameObject.SetActive(true);

        Image thumbnailImage = thumbnailButton.GetComponent<Image>();

        if (thumbnailImage == null)
            thumbnailImage = thumbnailButton.GetComponentInChildren<Image>(true);

        if (thumbnailImage != null)
        {
            thumbnailImage.sprite = thumbnailSprite;
            thumbnailImage.preserveAspect = true;
        }

        thumbnailButton.onClick.RemoveAllListeners();
        thumbnailButton.onClick.AddListener(() => { SelectScreenshot(screenshotPath); });

        Button deleteButton = FindThumbnailDeleteButton(thumbnailButton);

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() => { OpenDeleteConfirmWindow(screenshotPath); });
        }
    }


    private Button FindThumbnailDeleteButton(Button thumbnailButton)
    {
        if (thumbnailButton == null)
            return null;

        Button[] buttons = thumbnailButton.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            if (button == null)
                continue;

            if (button == thumbnailButton)
                continue;

            string buttonName = button.gameObject.name;

            if (string.Equals(buttonName, "DeleteButton", StringComparison.OrdinalIgnoreCase))
                return button;
        }

        return null;
    }

    private void OpenDeleteConfirmWindow(string screenshotPath)
    {
        if (string.IsNullOrWhiteSpace(screenshotPath))
            return;

        pendingDeleteScreenshotPath = screenshotPath;
        UpdateDeleteConfirmMessage(screenshotPath);

        if (deleteScreenshotConfirmWindow != null)
            deleteScreenshotConfirmWindow.SetActive(true);
    }

    private void CloseDeleteConfirmWindow()
    {
        pendingDeleteScreenshotPath = string.Empty;

        if (deleteScreenshotConfirmWindow != null)
            deleteScreenshotConfirmWindow.SetActive(false);
    }

    private void UpdateDeleteConfirmMessage(string screenshotPath)
    {
        if (deleteScreenshotConfirmMessageText == null)
            return;

        int linkedIssueCount = 0;

        if (getScreenshotLinkedIssueCount != null)
            linkedIssueCount = getScreenshotLinkedIssueCount.Invoke(screenshotPath);

        string fileName = Path.GetFileName(screenshotPath);

        if (linkedIssueCount > 0)
        {
            deleteScreenshotConfirmMessageText.text = fileName + " is linked to " + linkedIssueCount +
                " issue(s). Delete this PNG file and clear the linked screenshot path?";
        }
        else
        {
            deleteScreenshotConfirmMessageText.text = "Delete this screenshot PNG file?\n" + fileName;
        }
    }

    private void ConfirmDeleteScreenshot()
    {
        string screenshotPath = pendingDeleteScreenshotPath;

        if (string.IsNullOrWhiteSpace(screenshotPath))
        {
            CloseDeleteConfirmWindow();
            return;
        }

        bool deleteSucceeded = DeleteScreenshotFile(screenshotPath);

        if (deleteSucceeded)
        {
            if (IsSameScreenshotPath(selectedScreenshotPath, screenshotPath))
                SetSelectedScreenshotPath(string.Empty);

            onScreenshotDeleted?.Invoke(screenshotPath);
            RefreshGallery();
        }

        CloseDeleteConfirmWindow();
    }

    private bool DeleteScreenshotFile(string screenshotPath)
    {
        try
        {
            if (File.Exists(screenshotPath))
            {
                File.Delete(screenshotPath);
                return true;
            }

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to delete screenshot file: " + exception.Message);
            return false;
        }
    }

    private bool IsSameScreenshotPath(string firstPath, string secondPath)
    {
        if (string.IsNullOrWhiteSpace(firstPath))
            return false;

        if (string.IsNullOrWhiteSpace(secondPath))
            return false;

        return string.Equals(firstPath.Trim(), secondPath.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private void SelectScreenshot(string screenshotPath)
    {
        if (!isAttachMode)
            return;

        SetSelectedScreenshotPath(screenshotPath);
        CloseGallery();
    }

    private void ClearSelectedScreenshot()
    {
        SetSelectedScreenshotPath(string.Empty);
    }

    private void SetPreviewSprite(Sprite sprite)
    {
        DestroySelectedPreviewSprite();
        selectedPreviewSprite = sprite;

        bool hasScreenshot = selectedPreviewSprite != null;

        if (screenshotPreviewImage != null)
        {
            screenshotPreviewImage.sprite = hasScreenshot ? selectedPreviewSprite : defaultScreenshotSprite;
            screenshotPreviewImage.preserveAspect = hasScreenshot;
        }
    }

    private void UpdateSelectedScreenshotUI()
    {
        bool hasScreenshot = !string.IsNullOrWhiteSpace(selectedScreenshotPath);

        if (screenshotAddIconObject != null)
            screenshotAddIconObject.SetActive(!hasScreenshot);

        if (selectedScreenshotFileNameText != null)
            selectedScreenshotFileNameText.text = hasScreenshot ? Path.GetFileName(selectedScreenshotPath) : NoScreenshotText;

        if (clearSelectedScreenshotButton != null)
            clearSelectedScreenshotButton.interactable = hasScreenshot;
    }

    private void DestroySelectedPreviewSprite()
    {
        DestroySpriteWithTexture(selectedPreviewSprite);
        selectedPreviewSprite = null;
    }

    private void DestroySpriteWithTexture(Sprite sprite)
    {
        if (sprite == null)
            return;

        Texture2D texture = sprite.texture;

        if (texture != null)
            Destroy(texture);

        Destroy(sprite);
    }

    private Sprite LoadSpriteFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return null;

        if (!File.Exists(filePath))
            return null;

        byte[] imageBytes = File.ReadAllBytes(filePath);

        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        if (!texture.LoadImage(imageBytes))
        {
            Destroy(texture);
            return null;
        }

        texture.name = Path.GetFileNameWithoutExtension(filePath);

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        sprite.name = Path.GetFileNameWithoutExtension(filePath);

        return sprite;
    }
}
