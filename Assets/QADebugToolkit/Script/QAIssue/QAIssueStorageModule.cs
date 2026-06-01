using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class QAIssueStorageModule
{
    private QAToolkit qaToolkit;

    public void Setup(QAToolkit qaToolkit)
    {
        this.qaToolkit = qaToolkit;
    }

    public QAIssueSaveData LoadIssues()
    {
        string jsonPath = GetIssueJsonPath();

        if (string.IsNullOrWhiteSpace(jsonPath))
            return null;

        if (!File.Exists(jsonPath))
            return null;

        string json = File.ReadAllText(jsonPath);

        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonUtility.FromJson<QAIssueSaveData>(json);
    }

    public void SaveIssues(IReadOnlyList<QAIssueData> issues)
    {
        if (issues == null)
            return;

        string jsonPath = GetIssueJsonPath();

        if (string.IsNullOrWhiteSpace(jsonPath))
            return;

        QAIssueSaveData saveData = new QAIssueSaveData();

        for (int i = 0; i < issues.Count; i++)
        {
            saveData.issues.Add(issues[i]);
        }

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(jsonPath, json, Encoding.UTF8);
    }

    public void ExportIssueReportToTxt(IReadOnlyList<QAIssueData> issues)
    {
        if (issues == null || issues.Count <= 0)
            return;

        string issueFolderPath = GetIssueFolderPath();

        if (string.IsNullOrWhiteSpace(issueFolderPath))
            return;

        for (int i = 0; i < issues.Count; i++)
        {
            QAIssueData issue = issues[i];

            if (issue == null)
                continue;

            string safeTitle = GetSafeFileName(issue.title);
            string fileName = "issues_" + safeTitle + ".txt";
            string reportPath = Path.Combine(issueFolderPath, fileName);

            string reportText = string.Empty;

            reportText += "Issue " + (i + 1) + System.Environment.NewLine;
            reportText += "Title : " + issue.title + System.Environment.NewLine;
            reportText += "Created Time : " + issue.createdTime + System.Environment.NewLine;
            reportText += "Updated Time : " + issue.updatedTime + System.Environment.NewLine;
            reportText += "Scene : " + issue.sceneName + System.Environment.NewLine;
            reportText += "Scene Time : " + issue.sceneTime + System.Environment.NewLine;
            reportText += System.Environment.NewLine;
            reportText += "Description" + System.Environment.NewLine;
            reportText += issue.description + System.Environment.NewLine;

            File.WriteAllText(reportPath, reportText, Encoding.UTF8);

            Debug.Log("QA Issue Report Exported : " + reportPath);
        }
    }

    public void ExportIssueSheetToTsv(IReadOnlyList<QAIssueData> issues)
    {
        if (issues == null || issues.Count <= 0)
            return;

        string issueFolderPath = GetIssueFolderPath();

        if (string.IsNullOrWhiteSpace(issueFolderPath))
            return;

        string sheetPath = Path.Combine(issueFolderPath, "issues_00_sheet_export.txt");

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("No\tIssue Id\tTitle\tCreated Time\tUpdated Time\tScene\tScene Time\tDescription");

        for (int i = 0; i < issues.Count; i++)
        {
            QAIssueData issue = issues[i];

            if (issue == null)
                continue;

            builder.Append(i + 1);
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.issueId));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.title));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.createdTime));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.updatedTime));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.sceneName));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.sceneTime));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.description));
            builder.AppendLine();
        }

        File.WriteAllText(sheetPath, builder.ToString(), Encoding.UTF8);

        Debug.Log("QA Issue Sheet Exported : " + sheetPath);
    }

    private string GetIssueJsonPath()
    {
        string issueFolderPath = GetIssueFolderPath();

        if (string.IsNullOrWhiteSpace(issueFolderPath))
            return string.Empty;

        return Path.Combine(issueFolderPath, "issues.json");
    }

    private string GetIssueFolderPath()
    {
        if (qaToolkit == null)
            return string.Empty;

        return qaToolkit.GetIssueFolderPath();
    }

    private string GetSafeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Untitled_Issue";

        string fileName = value.Trim();

        char[] invalidChars = Path.GetInvalidFileNameChars();

        for (int i = 0; i < invalidChars.Length; i++)
        {
            fileName = fileName.Replace(invalidChars[i], '_');
        }

        if (string.IsNullOrWhiteSpace(fileName))
            return "Untitled_Issue";

        return fileName;
    }

    private string CleanSheetCell(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Replace("\t", " ").Replace("\r\n", " / ").Replace("\n", " / ").Replace("\r", " / ").Trim();
    }
}