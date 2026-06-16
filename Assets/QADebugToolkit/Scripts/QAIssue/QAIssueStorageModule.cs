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

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Issue " + (i + 1));
            builder.AppendLine("Title : " + issue.title);
            builder.AppendLine("Status : " + issue.status);
            builder.AppendLine("Severity : " + issue.severity);
            builder.AppendLine("Created Time : " + issue.createdTime);
            builder.AppendLine("Updated Time : " + issue.updatedTime);
            builder.AppendLine("Scene : " + issue.sceneName);
            builder.AppendLine("Scene Time : " + issue.sceneTime);
            builder.AppendLine("Screenshot Path : " + issue.screenshotPath);
            builder.AppendLine();
            builder.AppendLine("Description");
            builder.AppendLine(issue.description);

            File.WriteAllText(reportPath, builder.ToString(), Encoding.UTF8);
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

        builder.AppendLine("No\tIssue Id\tTitle\tStatus\tSeverity\tCreated Time\tUpdated Time\tScene\tScene Time\tScreenshot Path\tDescription");

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
            builder.Append(CleanSheetCell(issue.status));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.severity));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.createdTime));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.updatedTime));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.sceneName));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.sceneTime));
            builder.Append('\t');
            builder.Append(CleanSheetCell(issue.screenshotPath));
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
