using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class QAIssueData
{
    public string issueId;
    public string title;
    public string description;
    public string status;
    public string severity;
    public string createdTime;
    public string updatedTime;
    public string sceneName;
    public string sceneTime;
}

[Serializable]
public class QAIssueSaveData
{
    public List<QAIssueData> issues = new List<QAIssueData>();
}

public class QAIssueDataModule
{
    public const string DefaultStatus = "Open";
    public const string DefaultSeverity = "Medium";

    private readonly List<QAIssueData> issueList = new List<QAIssueData>();
    public IReadOnlyList<QAIssueData> Issues => issueList;

    private List<string> defaultIssueTitles = new List<string>();

    public void Setup(List<string> defaultTitles)
    {
        defaultIssueTitles = defaultTitles ?? new List<string>();
    }

    public void LoadAndMergeIssues(QAIssueSaveData saveData)
    {
        issueList.Clear();

        List<QAIssueData> savedIssues = new List<QAIssueData>();

        if (saveData != null && saveData.issues != null)
            savedIssues.AddRange(saveData.issues);

        for (int i = 0; i < defaultIssueTitles.Count; i++)
        {
            string defaultTitle = defaultIssueTitles[i];

            int savedIndex = FindSavedIssueIndexByTitle(savedIssues, defaultTitle);

            if (savedIndex >= 0)
            {
                QAIssueData savedIssue = savedIssues[savedIndex];

                EnsureIssueData(savedIssue, i);

                issueList.Add(savedIssue);
                savedIssues.RemoveAt(savedIndex);
            }
            else
            {
                QAIssueData defaultIssue = CreateIssueData(defaultTitle, string.Empty, DefaultStatus, DefaultSeverity);
                issueList.Add(defaultIssue);
            }
        }

        for (int i = 0; i < savedIssues.Count; i++)
        {
            EnsureIssueData(savedIssues[i], i);
            issueList.Add(savedIssues[i]);
        }
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < issueList.Count;
    }

    public QAIssueData GetIssue(int index)
    {
        if (!IsValidIndex(index))
            return null;

        return issueList[index];
    }

    public int AddIssue(string title, string description, string status, string severity)
    {
        QAIssueData newIssue = CreateIssueData(title, description, status, severity);
        issueList.Add(newIssue);

        return issueList.Count - 1;
    }

    public void UpdateIssue(int index, string title, string description, string status, string severity)
    {
        if (!IsValidIndex(index))
            return;

        QAIssueData issue = issueList[index];

        issue.title = title;
        issue.description = description;
        issue.status = GetValidValue(status, DefaultStatus);
        issue.severity = GetValidValue(severity, DefaultSeverity);
        issue.updatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        issue.sceneName = SceneManager.GetActiveScene().name;
        issue.sceneTime = FormatTime(Time.timeSinceLevelLoad);
    }

    public void DeleteIssue(int index)
    {
        if (!IsValidIndex(index))
            return;

        issueList.RemoveAt(index);
    }

    public bool HasSameTitle(string title, int ignoreIndex)
    {
        if (string.IsNullOrWhiteSpace(title))
            return false;

        string compareTitle = title.Trim();

        for (int i = 0; i < issueList.Count; i++)
        {
            if (i == ignoreIndex)
                continue;

            if (issueList[i] == null)
                continue;

            string issueTitle = issueList[i].title;

            if (string.IsNullOrWhiteSpace(issueTitle))
                continue;

            if (string.Equals(issueTitle.Trim(), compareTitle, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private QAIssueData CreateIssueData(string title, string description, string status, string severity)
    {
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        QAIssueData issue = new QAIssueData();

        issue.issueId = "ISSUE_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        issue.title = title ?? string.Empty;
        issue.description = description ?? string.Empty;
        issue.status = GetValidValue(status, DefaultStatus);
        issue.severity = GetValidValue(severity, DefaultSeverity);
        issue.createdTime = nowTime;
        issue.updatedTime = nowTime;
        issue.sceneName = SceneManager.GetActiveScene().name;
        issue.sceneTime = FormatTime(Time.timeSinceLevelLoad);

        return issue;
    }

    private int FindSavedIssueIndexByTitle(List<QAIssueData> savedIssues, string title)
    {
        if (savedIssues == null)
            return -1;

        for (int i = 0; i < savedIssues.Count; i++)
        {
            if (savedIssues[i] == null)
                continue;

            if (savedIssues[i].title == title)
                return i;
        }

        return -1;
    }

    private void EnsureIssueData(QAIssueData issue, int index)
    {
        if (issue == null)
            return;

        if (string.IsNullOrWhiteSpace(issue.issueId))
            issue.issueId = "ISSUE_LOADED_" + index + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

        if (string.IsNullOrWhiteSpace(issue.status))
            issue.status = DefaultStatus;

        if (string.IsNullOrWhiteSpace(issue.severity))
            issue.severity = DefaultSeverity;
    }

    private string GetValidValue(string value, string defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        return value.Trim();
    }

    private string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
