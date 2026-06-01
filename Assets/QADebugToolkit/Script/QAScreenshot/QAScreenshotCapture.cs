using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class QAScreenshotCapture : MonoBehaviour
{
    [Header("Toolkit")]
    [SerializeField] private QAToolkit qaToolkit;

    [Header("UI")]
    [SerializeField] private QAToolkitMessageView savePathText;

    public void CaptureScreenshot()
    {
        if (qaToolkit == null)
            return;

        string screenshotFolderPath = qaToolkit.GetScreenshotFolderPath();

        string fileName = "Screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(screenshotFolderPath, fileName);

        ScreenCapture.CaptureScreenshot(filePath);

        if (savePathText != null)
            savePathText.ShowMessage("Saved : " + fileName);

        UnityEngine.Debug.Log("QA Screenshot Saved : " + filePath);
    }

    [ContextMenu("Open Screenshot Folder")]
    private void OpenScreenshotFolderFromInspector()
    {
        if (qaToolkit == null)
            return;

        string screenshotFolderPath = qaToolkit.GetScreenshotFolderPath();

        if (!Directory.Exists(screenshotFolderPath))
            return;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Process.Start("explorer.exe", screenshotFolderPath.Replace("/", "\\"));
#else
        Application.OpenURL("file://" + screenshotFolderPath);
#endif
    }
}