using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAIssueViewModule
{
    private const string DefaultStatus = "Open";
    private const string DefaultSeverity = "Medium";
    private const string AllFilter = "All";

    private GameObject issueListWindow;

    private GameObject issueWindow;
    private TextMeshProUGUI issueWindowTitleText;
    private TMP_InputField titleInput;
    private TMP_InputField descriptionInput;

    private TMP_Dropdown statusDropdown;
    private TMP_Dropdown severityDropdown;

    private QAToolkitMessageView messageView;

    private Transform issueListContent;
    private Button issueButtonTemplate;
    private TMP_InputField issueSearchInputField;

    private TMP_Dropdown statusFilterDropdown;
    private TMP_Dropdown severityFilterDropdown;

    private GameObject deleteConfirmWindow;
    private Button confirmDeleteButton;
    private Button cancelDeleteButton;

    private readonly List<Button> createdIssueButtons = new List<Button>();
    private IReadOnlyList<QAIssueData> currentIssues;

    private Action<int> onSelectIssue;
    private Action onStartNewIssue;
    private Action onConfirmDeleteIssue;
    private Action onCancelDeleteIssue;

    public void Setup(GameObject issueListWindow, GameObject issueWindow, TextMeshProUGUI issueWindowTitleText,
        TMP_InputField titleInput, TMP_InputField descriptionInput,
        TMP_Dropdown statusDropdown, TMP_Dropdown severityDropdown,
        QAToolkitMessageView messageView, Transform issueListContent,
        Button issueButtonTemplate, TMP_InputField issueSearchInputField,
        TMP_Dropdown statusFilterDropdown, TMP_Dropdown severityFilterDropdown,
        GameObject deleteConfirmWindow, Button confirmDeleteButton, Button cancelDeleteButton,
        Action<int> selectIssueCallback, Action startNewIssueCallback, Action confirmDeleteCallback, Action cancelDeleteCallback)
    {
        this.issueListWindow = issueListWindow;

        this.issueWindow = issueWindow;
        this.issueWindowTitleText = issueWindowTitleText;
        this.titleInput = titleInput;
        this.descriptionInput = descriptionInput;

        this.statusDropdown = statusDropdown;
        this.severityDropdown = severityDropdown;

        this.messageView = messageView;

        this.issueListContent = issueListContent;
        this.issueButtonTemplate = issueButtonTemplate;
        this.issueSearchInputField = issueSearchInputField;

        this.statusFilterDropdown = statusFilterDropdown;
        this.severityFilterDropdown = severityFilterDropdown;

        this.deleteConfirmWindow = deleteConfirmWindow;
        this.confirmDeleteButton = confirmDeleteButton;
        this.cancelDeleteButton = cancelDeleteButton;

        onSelectIssue = selectIssueCallback;
        onStartNewIssue = startNewIssueCallback;
        onConfirmDeleteIssue = confirmDeleteCallback;
        onCancelDeleteIssue = cancelDeleteCallback;

        SetupDeleteConfirmButtons();
        SetupSearchAndFilterInputs();

        SetIssueListWindow(false);
        SetIssueWindow(false);
        SetDeleteConfirmWindow(false);

        if (this.issueButtonTemplate != null)
            this.issueButtonTemplate.gameObject.SetActive(false);

        SetWindowTitleToNewIssue();
        ShowMessage(string.Empty);
    }

    private void SetupDeleteConfirmButtons()
    {
        if (confirmDeleteButton != null)
        {
            confirmDeleteButton.onClick.RemoveAllListeners();
            confirmDeleteButton.onClick.AddListener(() => onConfirmDeleteIssue?.Invoke());
        }

        if (cancelDeleteButton != null)
        {
            cancelDeleteButton.onClick.RemoveAllListeners();
            cancelDeleteButton.onClick.AddListener(() => onCancelDeleteIssue?.Invoke());
        }
    }

    private void SetupSearchAndFilterInputs()
    {
        if (issueSearchInputField != null)
        {
            issueSearchInputField.onValueChanged.RemoveListener(OnSearchInputChanged);
            issueSearchInputField.onValueChanged.AddListener(OnSearchInputChanged);
        }

        if (statusFilterDropdown != null)
        {
            statusFilterDropdown.onValueChanged.RemoveListener(OnStatusFilterChanged);
            statusFilterDropdown.onValueChanged.AddListener(OnStatusFilterChanged);
        }

        if (severityFilterDropdown != null)
        {
            severityFilterDropdown.onValueChanged.RemoveListener(OnSeverityFilterChanged);
            severityFilterDropdown.onValueChanged.AddListener(OnSeverityFilterChanged);
        }
    }
    private void OnSearchInputChanged(string _)
    {
        ApplyCurrentIssueFilter();
    }

    private void OnStatusFilterChanged(int _)
    {
        ApplyCurrentIssueFilter();
    }

    private void OnSeverityFilterChanged(int _)
    {
        ApplyCurrentIssueFilter();
    }

    private void ApplyCurrentIssueFilter()
    {
        string searchKeyword = GetSearchKeyword();
        string statusFilter = GetDropdownValue(statusFilterDropdown, AllFilter);
        string severityFilter = GetDropdownValue(severityFilterDropdown, AllFilter);

        int issueCount = currentIssues != null ? currentIssues.Count : 0;

        for (int i = 0; i < createdIssueButtons.Count; i++)
        {
            Button button = createdIssueButtons[i];

            if (button == null)
                continue;

            if (i >= issueCount)
            {
                button.gameObject.SetActive(true);
                continue;
            }

            QAIssueData issue = currentIssues[i];

            if (issue == null)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            bool isTitleMatched = IsTitleMatched(issue.title, searchKeyword);
            bool isStatusMatched = IsFilterMatched(issue.status, statusFilter, DefaultStatus);
            bool isSeverityMatched = IsFilterMatched(issue.severity, severityFilter, DefaultSeverity);

            bool isMatched = isTitleMatched && isStatusMatched && isSeverityMatched;

            button.gameObject.SetActive(isMatched);
        }

        RefreshIssueListLayout();
    }

    private string GetSearchKeyword()
    {
        if (issueSearchInputField == null)
            return string.Empty;

        return issueSearchInputField.text.Trim();
    }

    private bool IsTitleMatched(string title, string searchKeyword)
    {
        if (string.IsNullOrEmpty(searchKeyword))
            return true;

        string targetTitle = title ?? string.Empty;

        return targetTitle.IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool IsFilterMatched(string value, string filterValue, string defaultValue)
    {
        if (string.IsNullOrWhiteSpace(filterValue))
            return true;

        if (string.Equals(filterValue, AllFilter, StringComparison.OrdinalIgnoreCase))
            return true;

        string targetValue = string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();

        return string.Equals(targetValue, filterValue, StringComparison.OrdinalIgnoreCase);
    }

    private void RefreshIssueListLayout()
    {
        RectTransform contentRect = issueListContent as RectTransform;

        if (contentRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    public void InitializeIssueButtons(IReadOnlyList<QAIssueData> issues)
    {
        currentIssues = issues;

        ClearCreatedButtons();

        if (issues == null)
            return;

        for (int i = 0; i < issues.Count; i++)
        {
            CreateIssueButton(i, issues[i].title);
        }

        CreateNewIssueButton();
        ApplyCurrentIssueFilter();
    }

    public void OpenIssueListWindow()
    {
        SetIssueListWindow(true);
        SetDeleteConfirmWindow(false);
    }

    public void CloseIssueListWindow()
    {
        SetDeleteConfirmWindow(false);
        SetIssueWindow(false);
        SetIssueListWindow(false);
    }

    public void ShowNewIssueWindow()
    {
        ClearInput();
        SetWindowTitleToNewIssue();
        SetDeleteConfirmWindow(false);
        ShowMessage("New Issue Mode");

        SetIssueListWindow(true);
        SetIssueWindow(true);
    }

    public void ShowEditIssueWindow(QAIssueData issue)
    {
        if (issue == null)
            return;

        SetTitleInput(issue.title);
        SetDescriptionInput(issue.description);

        SetStatusInput(issue.status);
        SetSeverityInput(issue.severity);

        if (issueWindowTitleText != null)
            issueWindowTitleText.text = "Edit Issue";

        SetDeleteConfirmWindow(false);
        ShowMessage("Selected : " + issue.title);

        SetIssueListWindow(true);
        SetIssueWindow(true);
    }

    public void CloseIssueWindow()
    {
        SetDeleteConfirmWindow(false);
        SetIssueWindow(false);
    }

    public void ClearInput()
    {
        SetTitleInput(string.Empty);
        SetDescriptionInput(string.Empty);

        SetStatusInput(DefaultStatus);
        SetSeverityInput(DefaultSeverity);
    }

    public string GetTitleInput()
    {
        if (titleInput == null)
            return string.Empty;

        return titleInput.text.Trim();
    }

    public string GetDescriptionInput()
    {
        if (descriptionInput == null)
            return string.Empty;

        return descriptionInput.text.Trim();
    }

    public string GetStatusInput()
    {
        return GetDropdownValue(statusDropdown, DefaultStatus);
    }

    public string GetSeverityInput()
    {
        return GetDropdownValue(severityDropdown, DefaultSeverity);
    }

    public void SetTitleInput(string value)
    {
        if (titleInput != null)
            titleInput.text = value;
    }

    public void SetDescriptionInput(string value)
    {
        if (descriptionInput != null)
            descriptionInput.text = value;
    }

    public void SetStatusInput(string value)
    {
        SetDropdownValue(statusDropdown, value, DefaultStatus);
    }

    public void SetSeverityInput(string value)
    {
        SetDropdownValue(severityDropdown, value, DefaultSeverity);
    }

    private string GetDropdownValue(TMP_Dropdown dropdown, string defaultValue)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count <= 0)
            return defaultValue;

        int index = dropdown.value;

        if (index < 0 || index >= dropdown.options.Count)
            return defaultValue;

        string value = dropdown.options[index].text;

        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        return value.Trim();
    }

    private void SetDropdownValue(TMP_Dropdown dropdown, string value, string defaultValue)
    {
        if (dropdown == null || dropdown.options == null || dropdown.options.Count <= 0)
            return;

        string targetValue = string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (string.Equals(dropdown.options[i].text, targetValue, StringComparison.OrdinalIgnoreCase))
            {
                dropdown.SetValueWithoutNotify(i);
                dropdown.RefreshShownValue();
                return;
            }
        }

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (string.Equals(dropdown.options[i].text, defaultValue, StringComparison.OrdinalIgnoreCase))
            {
                dropdown.SetValueWithoutNotify(i);
                dropdown.RefreshShownValue();
                return;
            }
        }

        dropdown.SetValueWithoutNotify(0);
        dropdown.RefreshShownValue();
    }

    public void AddIssueButton(int issueIndex, string title)
    {
        if (issueIndex < 0)
            return;

        if (createdIssueButtons.Count > 0 && issueIndex < createdIssueButtons.Count)
        {
            SetButtonAsIssue(createdIssueButtons[issueIndex], issueIndex, title);
            CreateNewIssueButton();
            ApplyCurrentIssueFilter();
            return;
        }

        CreateIssueButton(issueIndex, title);
        CreateNewIssueButton();
        ApplyCurrentIssueFilter();
    }

    public void UpdateIssueButtonTitle(int issueIndex, string title)
    {
        if (issueIndex < 0 || issueIndex >= createdIssueButtons.Count)
            return;

        SetButtonAsIssue(createdIssueButtons[issueIndex], issueIndex, title);
        ApplyCurrentIssueFilter();
    }

    public void RemoveIssueButtonAndRebind(int removeIndex, IReadOnlyList<QAIssueData> issues)
    {

        if (removeIndex < 0 || removeIndex >= createdIssueButtons.Count)
            return;

        Button targetButton = createdIssueButtons[removeIndex];

        if (targetButton != null)
            UnityEngine.Object.Destroy(targetButton.gameObject);

        createdIssueButtons.RemoveAt(removeIndex);

        RebindIssueButtons(issues);
        ApplyCurrentIssueFilter();
    }

    public void OpenDeleteConfirmWindow()
    {
        SetDeleteConfirmWindow(true);
    }

    public void CloseDeleteConfirmWindow()
    {
        SetDeleteConfirmWindow(false);
    }

    public void SetWindowTitleToNewIssue()
    {
        if (issueWindowTitleText != null)
            issueWindowTitleText.text = "New Issue";
    }

    public void ShowMessage(string message)
    {
        if (messageView != null)
            messageView.ShowMessage(message);
    }

    private void CreateIssueButton(int issueIndex, string title)
    {
        Button newButton = CreateRawButton();

        if (newButton == null)
            return;

        createdIssueButtons.Add(newButton);
        SetButtonAsIssue(newButton, issueIndex, title);
    }

    private void CreateNewIssueButton()
    {
        Button newButton = CreateRawButton();

        if (newButton == null)
            return;

        createdIssueButtons.Add(newButton);
        SetButtonAsNewIssue(newButton);
    }

    private Button CreateRawButton()
    {
        if (issueButtonTemplate == null || issueListContent == null)
            return null;

        Button newButton = UnityEngine.Object.Instantiate(issueButtonTemplate, issueListContent);
        newButton.gameObject.SetActive(true);

        return newButton;
    }

    private void SetButtonAsIssue(Button button, int issueIndex, string title)
    {
        if (button == null)
            return;

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);

        if (buttonText != null)
            buttonText.text = title;

        button.onClick.RemoveAllListeners();

        int cachedIndex = issueIndex;
        button.onClick.AddListener(() => onSelectIssue?.Invoke(cachedIndex));
    }

    private void SetButtonAsNewIssue(Button button)
    {
        if (button == null)
            return;

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);

        if (buttonText != null)
            buttonText.text = "+ New Issue";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onStartNewIssue?.Invoke());
    }

    private void RebindIssueButtons(IReadOnlyList<QAIssueData> issues)
    {
        currentIssues = issues;

        if (issues == null)
            return;

        int targetButtonCount = issues.Count + 1;

        while (createdIssueButtons.Count > targetButtonCount)
        {
            int lastIndex = createdIssueButtons.Count - 1;

            if (createdIssueButtons[lastIndex] != null)
                UnityEngine.Object.Destroy(createdIssueButtons[lastIndex].gameObject);

            createdIssueButtons.RemoveAt(lastIndex);
        }

        while (createdIssueButtons.Count < targetButtonCount)
        {
            Button newButton = CreateRawButton();

            if (newButton == null)
                return;

            createdIssueButtons.Add(newButton);
        }

        for (int i = 0; i < createdIssueButtons.Count; i++)
        {
            if (i < issues.Count)
                SetButtonAsIssue(createdIssueButtons[i], i, issues[i].title);
            else
                SetButtonAsNewIssue(createdIssueButtons[i]);
        }

        ApplyCurrentIssueFilter();
    }

    private void ClearCreatedButtons()
    {
        for (int i = 0; i < createdIssueButtons.Count; i++)
        {
            if (createdIssueButtons[i] != null)
                UnityEngine.Object.Destroy(createdIssueButtons[i].gameObject);
        }

        createdIssueButtons.Clear();
    }

    private void SetIssueListWindow(bool value)
    {
        if (issueListWindow != null)
            issueListWindow.SetActive(value);
    }

    private void SetIssueWindow(bool value)
    {
        if (issueWindow != null)
            issueWindow.SetActive(value);
    }

    private void SetDeleteConfirmWindow(bool value)
    {
        if (deleteConfirmWindow != null)
            deleteConfirmWindow.SetActive(value);
    }
}