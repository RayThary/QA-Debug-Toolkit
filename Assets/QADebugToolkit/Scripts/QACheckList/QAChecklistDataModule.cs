using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class QAChecklistData
{
    public string checklistId;
    public string title;
    public string note;
    public string status;
    public string sceneName;
}

[Serializable]
public class QAChecklistSaveData
{
    public List<QAChecklistData> checklists = new List<QAChecklistData>();
}

public class QAChecklistDataModule
{
    public const string DefaultStatus = "Not Tested";

    private readonly List<QAChecklistData> checklistList = new List<QAChecklistData>();
    public IReadOnlyList<QAChecklistData> Checklists => checklistList;

    private List<string> defaultChecklistTitles = new List<string>();

    public void Setup(List<string> defaultTitles)
    {
        defaultChecklistTitles = defaultTitles ?? new List<string>();
    }

    public void LoadAndMergeChecklists(QAChecklistSaveData saveData)
    {
        checklistList.Clear();

        List<QAChecklistData> savedChecklists = new List<QAChecklistData>();

        if (saveData != null && saveData.checklists != null)
            savedChecklists.AddRange(saveData.checklists);        

        for (int i = 0; i < defaultChecklistTitles.Count; i++)
        {
            string defaultTitle = defaultChecklistTitles[i];

            int savedIndex = FindSavedChecklistIndexByTitle(savedChecklists, defaultTitle);

            if (savedIndex >= 0)
            {
                QAChecklistData savedChecklist = savedChecklists[savedIndex];

                EnsureChecklistData(savedChecklist, i);

                checklistList.Add(savedChecklist);
                savedChecklists.RemoveAt(savedIndex);
            }
            else
            {
                QAChecklistData defaultChecklist = CreateChecklistData(defaultTitle, string.Empty, DefaultStatus);
                checklistList.Add(defaultChecklist);
            }
        }

        for (int i = 0; i < savedChecklists.Count; i++)
        {
            EnsureChecklistData(savedChecklists[i], i);
            checklistList.Add(savedChecklists[i]);
        }
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < checklistList.Count;
    }

    public QAChecklistData GetChecklist(int index)
    {
        if (!IsValidIndex(index))
            return null;

        return checklistList[index];
    }

    public int AddChecklist(string title, string note, string status)
    {
        QAChecklistData newChecklist = CreateChecklistData(title, note, status);
        checklistList.Add(newChecklist);

        return checklistList.Count - 1;
    }

    public void UpdateChecklist(int index, string title, string note, string status)
    {
        if (!IsValidIndex(index))
            return;

        QAChecklistData checklist = checklistList[index];

        checklist.title = title;
        checklist.note = note;
        checklist.status = GetValidStatus(status, DefaultStatus);
        checklist.sceneName = SceneManager.GetActiveScene().name;
    }

    public void DeleteChecklist(int index)
    {
        if (!IsValidIndex(index))
            return;

        checklistList.RemoveAt(index);
    }

    public bool HasSameTitle(string title, int ignoreIndex)
    {
        if (string.IsNullOrWhiteSpace(title))
            return false;

        string compareTitle = title.Trim();

        for (int i = 0; i < checklistList.Count; i++)
        {
            if (i == ignoreIndex)
                continue;

            if (checklistList[i] == null)
                continue;

            string checklistTitle = checklistList[i].title;

            if (string.IsNullOrWhiteSpace(checklistTitle))
                continue;

            if (string.Equals(checklistTitle.Trim(), compareTitle, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private QAChecklistData CreateChecklistData(string title, string note, string status)
    {
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        QAChecklistData checklist = new QAChecklistData();

        checklist.checklistId = "CHECK_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        checklist.title = title ?? string.Empty;
        checklist.note = note ?? string.Empty;
        checklist.status = GetValidStatus(status, DefaultStatus);
        checklist.sceneName = SceneManager.GetActiveScene().name;

        return checklist;
    }

    private int FindSavedChecklistIndexByTitle(List<QAChecklistData> savedChecklists, string title)
    {
        if (savedChecklists == null)
            return -1;

        for (int i = 0; i < savedChecklists.Count; i++)
        {
            if (savedChecklists[i] == null)
                continue;

            if (savedChecklists[i].title == title)
                return i;
        }

        return -1;
    }

    private void EnsureChecklistData(QAChecklistData checklist, int index)
    {
        if (checklist == null)
            return;

        if (string.IsNullOrWhiteSpace(checklist.checklistId))
            checklist.checklistId = "CHECK_LOADED_" + index + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

        if (string.IsNullOrWhiteSpace(checklist.status))
            checklist.status = DefaultStatus;
    }

    private string GetValidStatus(string value, string defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        return value.Trim();
    }

}