using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class QAChecklistStorageModule
{
    private QAToolkit qaToolkit;

    public void Setup(QAToolkit qaToolkit)
    {
        this.qaToolkit = qaToolkit;
    }

    public QAChecklistSaveData LoadChecklists()
    {
        string jsonPath = GetChecklistJsonPath();

        if (string.IsNullOrWhiteSpace(jsonPath))
            return null;

        if (!File.Exists(jsonPath))
            return null;

        string json = File.ReadAllText(jsonPath);

        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonUtility.FromJson<QAChecklistSaveData>(json);
    }

    public void SaveChecklists(IReadOnlyList<QAChecklistData> checklists)
    {
        if (checklists == null)
            return;

        string jsonPath = GetChecklistJsonPath();

        if (string.IsNullOrWhiteSpace(jsonPath))
            return;

        QAChecklistSaveData saveData = new QAChecklistSaveData();

        for (int i = 0; i < checklists.Count; i++)
        {
            saveData.checklists.Add(checklists[i]);
        }

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(jsonPath, json, Encoding.UTF8);
    }

    public void ExportChecklistReportToTxt(IReadOnlyList<QAChecklistData> checklists)
    {
        if (checklists == null || checklists.Count <= 0)
            return;

        string checklistFolderPath = GetChecklistFolderPath();

        if (string.IsNullOrWhiteSpace(checklistFolderPath))
            return;

        for (int i = 0; i < checklists.Count; i++)
        {
            QAChecklistData checklist = checklists[i];

            if (checklist == null)
                continue;

            string safeTitle = GetSafeFileName(checklist.title);
            string fileName = "checklist_" + safeTitle + ".txt";
            string reportPath = Path.Combine(checklistFolderPath, fileName);

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Checklist Item " + (i + 1));
            builder.AppendLine("Title : " + checklist.title);
            builder.AppendLine("Status : " + checklist.status);            
            builder.AppendLine("Scene : " + checklist.sceneName);            
            builder.AppendLine();
            builder.AppendLine("Note");
            builder.AppendLine(checklist.note);

            File.WriteAllText(reportPath, builder.ToString(), Encoding.UTF8);

            Debug.Log("QA Checklist Report Exported : " + reportPath);
        }
    }

    public void ExportChecklistSheetToTsv(IReadOnlyList<QAChecklistData> checklists)
    {
        if (checklists == null || checklists.Count <= 0)
            return;

        string checklistFolderPath = GetChecklistFolderPath();

        if (string.IsNullOrWhiteSpace(checklistFolderPath))
            return;

        string sheetPath = Path.Combine(checklistFolderPath, "checklist_00_sheet_export.txt");

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("No\tChecklist Id\tTitle\tStatus\tCreated Time\tUpdated Time\tScene\tScene Time\tNote");

        for (int i = 0; i < checklists.Count; i++)
        {
            QAChecklistData checklist = checklists[i];

            if (checklist == null)
                continue;

            builder.Append(i + 1);
            builder.Append('\t');
            builder.Append(CleanSheetCell(checklist.checklistId));
            builder.Append('\t');
            builder.Append(CleanSheetCell(checklist.title));
            builder.Append('\t');
            builder.Append(CleanSheetCell(checklist.status));
            builder.Append('\t');
            builder.Append(CleanSheetCell(checklist.sceneName));
            builder.Append('\t');
            builder.Append(CleanSheetCell(checklist.note));
            builder.AppendLine();
        }

        File.WriteAllText(sheetPath, builder.ToString(), Encoding.UTF8);

        Debug.Log("QA Checklist Sheet Exported : " + sheetPath);
    }

    private string GetChecklistJsonPath()
    {
        string checklistFolderPath = GetChecklistFolderPath();

        if (string.IsNullOrWhiteSpace(checklistFolderPath))
            return string.Empty;

        return Path.Combine(checklistFolderPath, "checklists.json");
    }

    private string GetChecklistFolderPath()
    {
        if (qaToolkit == null)
            return string.Empty;

        return qaToolkit.GetChecklistFolderPath();
    }

    private string GetSafeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Untitled_Check_Item";

        string fileName = value.Trim();

        char[] invalidChars = Path.GetInvalidFileNameChars();

        for (int i = 0; i < invalidChars.Length; i++)
        {
            fileName = fileName.Replace(invalidChars[i], '_');
        }

        if (string.IsNullOrWhiteSpace(fileName))
            return "Untitled_Check_Item";

        return fileName;
    }

    private string CleanSheetCell(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Replace("\t", " ").Replace("\r\n", " / ").Replace("\n", " / ").Replace("\r", " / ").Trim();
    }
}