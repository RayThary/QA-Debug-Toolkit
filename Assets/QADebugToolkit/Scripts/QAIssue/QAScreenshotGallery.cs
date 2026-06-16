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

    [Header("Reference")]
    [SerializeField] private QAToolkit qaToolkit;

    private const string DefaultReportFolderName = "QAReports";
    private const string ScreenshotFolderName = "Screenshots";
    private const string NoScreenshotText = "No Screenshot";

    private readonly List<Sprite> thumbnailSprites = new List<Sprite>();

    private Sprite defaultScreenshotSprite;
    private Sprite selectedPreviewSprite;
    private string selectedScreenshotPath = string.Empty;

    private void Awake()
    {
        if (screenshotGalleryWindow != null)
            screenshotGalleryWindow.SetActive(false);

        if (screenshotThumbnailTemplateButton != null)
            screenshotThumbnailTemplateButton.gameObject.SetActive(false);

        if (screenshotPreviewImage != null)
            defaultScreenshotSprite = screenshotPreviewImage.sprite;

        if (screenshotPreviewButton != null)
        {
            screenshotPreviewButton.onClick.RemoveAllListeners();
            screenshotPreviewButton.onClick.AddListener(OpenGallery);
        }

        if (closeGalleryButton != null)
        {
            closeGalleryButton.onClick.RemoveAllListeners();
            closeGalleryButton.onClick.AddListener(CloseGallery);
        }

        if (clearSelectedScreenshotButton != null)
        {
            clearSelectedScreenshotButton.onClick.RemoveAllListeners();
            clearSelectedScreenshotButton.onClick.AddListener(ClearSelectedScreenshot);
        }

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

    public void SetSelectedScreenshotPath(string screenshotPath)
    {
        bool hasScreenshot = !string.IsNullOrWhiteSpace(screenshotPath) && File.Exists(screenshotPath);

        selectedScreenshotPath = hasScreenshot ? screenshotPath : string.Empty;

        Sprite newSprite = hasScreenshot ? LoadSpriteFromFile(screenshotPath) : null;
        SetPreviewSprite(newSprite);
        UpdateSelectedScreenshotUI();
    }

    public void OpenGallery()
    {
        RefreshGallery();

        if (screenshotGalleryWindow != null)
            screenshotGalleryWindow.SetActive(true);
    }

    public void CloseGallery()
    {
        if (screenshotGalleryWindow != null)
            screenshotGalleryWindow.SetActive(false);
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
    }

    private void SelectScreenshot(string screenshotPath)
    {
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

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        sprite.name = Path.GetFileNameWithoutExtension(filePath);

        return sprite;
    }
}
