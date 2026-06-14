using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAIssueManager : MonoBehaviour
{
    [Header("Toolkit")]
    [SerializeField] private QAToolkit qaToolkit;

    [Header("Issue List")]
    [SerializeField] private GameObject issueListWindow;
    [SerializeField] private Transform issueListContent;
    [SerializeField] private Button issueButtonTemplate;

    [Header("Issue Filter")]
    [SerializeField] private TMP_InputField issueSearchInputField;
    [SerializeField] private TMP_Dropdown statusFilterDropdown;
    [SerializeField] private TMP_Dropdown severityFilterDropdown;

    [Header("Issue Window")]
    [SerializeField] private GameObject issueWindow;
    [SerializeField] private TextMeshProUGUI issueWindowTitleText;

    [Header("Issue Input")]
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField descriptionInput;
    [SerializeField] private TMP_Dropdown statusDropdown;
    [SerializeField] private TMP_Dropdown severityDropdown;
    [SerializeField] private QAToolkitMessageView messageView;

    [Header("Delete Confirm")]
    [SerializeField] private GameObject deleteConfirmWindow;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Button cancelDeleteButton;

    [Header("Default Issues")]
    [SerializeField] private List<string> defaultIssueTitles = new List<string>();

    private QAIssueDataModule dataModule;
    private QAIssueViewModule viewModule;
    private QAIssueStorageModule storageModule;

    private int selectedIssueIndex = -1;
    private bool isNewIssueMode;
    private bool isInitialized;

    private void Awake()
    {
        dataModule = new QAIssueDataModule();
        viewModule = new QAIssueViewModule();
        storageModule = new QAIssueStorageModule();

        dataModule.Setup(defaultIssueTitles);
        storageModule.Setup(qaToolkit);

        viewModule.Setup(issueListWindow, issueWindow, issueWindowTitleText,
            titleInput, descriptionInput, statusDropdown, severityDropdown, messageView,
            issueListContent, issueButtonTemplate, issueSearchInputField,
            statusFilterDropdown, severityFilterDropdown,
            deleteConfirmWindow, confirmDeleteButton, cancelDeleteButton,
            SelectIssue, StartNewIssue, ConfirmDeleteIssue, CancelDeleteIssue);

        QAIssueSaveData saveData = storageModule.LoadIssues();

        dataModule.LoadAndMergeIssues(saveData);

        viewModule.InitializeIssueButtons(dataModule.Issues);

        SaveAllIssuesToJson();

        isInitialized = true;
    }

    private void OnDisable()
    {
        if (!isInitialized)
            return;

        SaveCurrentIssue();
        SaveAllIssuesToJson();
    }

    private void OnApplicationQuit()
    {
        if (!isInitialized)
            return;

        SaveCurrentIssue();
        SaveAllIssuesToJson();
    }

    public void OpenIssueListWindow()
    {
        qaToolkit.SetToggleBlocked(true);
        viewModule.OpenIssueListWindow();
    }

    public void CloseIssueListWindow()
    {
        if (!SaveCurrentIssue(true))
            return;

        qaToolkit.SetToggleBlocked(true);
        SaveAllIssuesToJson();

        viewModule.CloseIssueListWindow();
    }

    public void StartNewIssue()
    {
        if (!SaveCurrentIssue(true))
            return;

        selectedIssueIndex = -1;
        isNewIssueMode = true;

        viewModule.ShowNewIssueWindow();
    }

    private void SelectIssue(int index)
    {
        if (!dataModule.IsValidIndex(index))
            return;

        if (!SaveCurrentIssue(true))
            return;

        selectedIssueIndex = index;
        isNewIssueMode = false;

        QAIssueData issue = dataModule.GetIssue(index);

        viewModule.ShowEditIssueWindow(issue);
    }

    public void CloseIssueWindow()
    {
        if (!SaveCurrentIssue(true))
            return;

        SaveAllIssuesToJson();

        viewModule.CloseIssueWindow();
    }

    public void ClearIssueInput()
    {
        viewModule.ClearInput();
    }

    public void SaveData()
    {
        if (!SaveCurrentIssue(true))
            return;

        SaveAllIssuesToJson();

        viewModule.ShowMessage("Data Saved.");
    }

    public void OpenDeleteConfirmWindow()
    {
        if (isNewIssueMode || !dataModule.IsValidIndex(selectedIssueIndex))
        {
            viewModule.ShowMessage("No issue selected.");
            return;
        }

        viewModule.OpenDeleteConfirmWindow();
    }

    public void CancelDeleteIssue()
    {
        viewModule.CloseDeleteConfirmWindow();
    }

    public void ConfirmDeleteIssue()
    {
        if (!dataModule.IsValidIndex(selectedIssueIndex))
            return;

        dataModule.DeleteIssue(selectedIssueIndex);

        viewModule.RemoveIssueButtonAndRebind(selectedIssueIndex, dataModule.Issues);

        selectedIssueIndex = -1;
        isNewIssueMode = false;

        viewModule.ClearInput();
        viewModule.SetWindowTitleToNewIssue();
        viewModule.CloseDeleteConfirmWindow();
        viewModule.CloseIssueWindow();
        viewModule.ShowMessage("Issue Deleted.");

        SaveAllIssuesToJson();
    }

    public void ExportIssueReportToTxt()
    {
        if (!SaveCurrentIssue(true))
            return;

        SaveAllIssuesToJson();

        storageModule.ExportIssueReportToTxt(dataModule.Issues);
        storageModule.ExportIssueSheetToTsv(dataModule.Issues);

        viewModule.ShowMessage("Export Completed.");
    }

    private bool SaveCurrentIssue(bool showMessage = false)
    {
        string title = viewModule.GetTitleInput();
        string description = viewModule.GetDescriptionInput();
        string status = viewModule.GetStatusInput();
        string severity = viewModule.GetSeverityInput();

        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(description))
            return true;

        if (string.IsNullOrWhiteSpace(title))
            title = "Untitled Issue";

        int ignoreIndex = dataModule.IsValidIndex(selectedIssueIndex) ? selectedIssueIndex : -1;

        if (dataModule.HasSameTitle(title, ignoreIndex))
        {
            if (showMessage)
                viewModule.ShowMessage("Same title already exists.");

            return false;
        }

        if (isNewIssueMode || !dataModule.IsValidIndex(selectedIssueIndex))
        {
            int newIssueIndex = dataModule.AddIssue(title, description, status, severity);

            selectedIssueIndex = newIssueIndex;
            isNewIssueMode = false;

            QAIssueData newIssue = dataModule.GetIssue(newIssueIndex);

            viewModule.AddIssueButton(newIssueIndex, newIssue.title);
        }
        else
        {
            dataModule.UpdateIssue(selectedIssueIndex, title, description, status, severity);

            QAIssueData issue = dataModule.GetIssue(selectedIssueIndex);

            viewModule.UpdateIssueButtonTitle(selectedIssueIndex, issue.title);
        }

        return true;
    }

    private void SaveAllIssuesToJson()
    {
        storageModule.SaveIssues(dataModule.Issues);
    }
}