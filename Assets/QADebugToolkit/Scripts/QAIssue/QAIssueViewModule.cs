using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAIssueViewModule
{
    private GameObject issueListWindow;

    private GameObject issueWindow;
    private TextMeshProUGUI issueWindowTitleText;
    private TMP_InputField titleInput;
    private TMP_InputField descriptionInput;
    private QAToolkitMessageView messageView;

    private Transform issueListContent;
    private Button issueButtonTemplate;
    private TMP_InputField issueSearchInputField;

    private GameObject deleteConfirmWindow;
    private Button confirmDeleteButton;
    private Button cancelDeleteButton;

    private readonly List<Button> createdIssueButtons = new List<Button>();

    private Action<int> onSelectIssue;
    private Action onStartNewIssue;
    private Action onConfirmDeleteIssue;
    private Action onCancelDeleteIssue;

    public void Setup(GameObject issueListWindow, GameObject issueWindow, TextMeshProUGUI issueWindowTitleText,
        TMP_InputField titleInput, TMP_InputField descriptionInput, QAToolkitMessageView messageView, Transform issueListContent,
        Button issueButtonTemplate, TMP_InputField issueSearchInputField, GameObject deleteConfirmWindow, Button confirmDeleteButton, Button cancelDeleteButton,
        Action<int> selectIssueCallback, Action startNewIssueCallback, Action confirmDeleteCallback, Action cancelDeleteCallback)
    {
        this.issueListWindow = issueListWindow;

        this.issueWindow = issueWindow;
        this.issueWindowTitleText = issueWindowTitleText;
        this.titleInput = titleInput;
        this.descriptionInput = descriptionInput;
        this.messageView = messageView;

        this.issueListContent = issueListContent;
        this.issueButtonTemplate = issueButtonTemplate;
        this.issueSearchInputField = issueSearchInputField;

        this.deleteConfirmWindow = deleteConfirmWindow;
        this.confirmDeleteButton = confirmDeleteButton;
        this.cancelDeleteButton = cancelDeleteButton;

        onSelectIssue = selectIssueCallback;
        onStartNewIssue = startNewIssueCallback;
        onConfirmDeleteIssue = confirmDeleteCallback;
        onCancelDeleteIssue = cancelDeleteCallback;

        SetupDeleteConfirmButtons();
        SetupSearchInputField();

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

    private void SetupSearchInputField()
    {
        if (issueSearchInputField == null)
            return;

        issueSearchInputField.onValueChanged.RemoveListener(FilterIssueButtons);
        issueSearchInputField.onValueChanged.AddListener(FilterIssueButtons);
    }

    private void FilterIssueButtons(string keyword)
    {
        string searchKeyword = keyword.Trim();

        int newIssueButtonIndex = createdIssueButtons.Count - 1;

        for (int i = 0; i < createdIssueButtons.Count; i++)
        {
            Button button = createdIssueButtons[i];

            if (button == null)
                continue;

            // + New Issue 버튼은 검색어와 상관없이 항상 표시
            if (i == newIssueButtonIndex)
            {
                button.gameObject.SetActive(true);
                continue;
            }

            if (string.IsNullOrEmpty(searchKeyword))
            {
                button.gameObject.SetActive(true);
                continue;
            }

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);
            string title = buttonText != null ? buttonText.text : string.Empty;

            bool isMatched = title.IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0;

            button.gameObject.SetActive(isMatched);
        }

        RefreshIssueListLayout();
    }

    private void ApplyCurrentSearchFilter()
    {
        if (issueSearchInputField == null)
            return;

        FilterIssueButtons(issueSearchInputField.text);
    }

    private void RefreshIssueListLayout()
    {
        RectTransform contentRect = issueListContent as RectTransform;

        if (contentRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    public void InitializeIssueButtons(IReadOnlyList<QAIssueData> issues)
    {
        ClearCreatedButtons();

        if (issues == null)
            return;

        for (int i = 0; i < issues.Count; i++)
        {
            CreateIssueButton(i, issues[i].title);
        }

        CreateNewIssueButton();
        ApplyCurrentSearchFilter();
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

    public void AddIssueButton(int issueIndex, string title)
    {
        if (issueIndex < 0)
            return;

        if (createdIssueButtons.Count > 0 && issueIndex < createdIssueButtons.Count)
        {
            SetButtonAsIssue(createdIssueButtons[issueIndex], issueIndex, title);
            CreateNewIssueButton();
            ApplyCurrentSearchFilter();
            return;
        }

        CreateIssueButton(issueIndex, title);
        CreateNewIssueButton();
        ApplyCurrentSearchFilter();
    }

    public void UpdateIssueButtonTitle(int issueIndex, string title)
    {
        if (issueIndex < 0 || issueIndex >= createdIssueButtons.Count)
            return;

        SetButtonAsIssue(createdIssueButtons[issueIndex], issueIndex, title);
        ApplyCurrentSearchFilter();
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
        ApplyCurrentSearchFilter();
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

        ApplyCurrentSearchFilter();
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