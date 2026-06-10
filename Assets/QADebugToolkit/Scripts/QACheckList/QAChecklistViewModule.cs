using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAChecklistViewModule
{
    private const string DefaultStatus = "Not Tested";
    private const string AllFilter = "All";

    private GameObject checklistWindow;

    private GameObject checklistItemWindow;
    private TextMeshProUGUI checklistItemWindowTitleText;
    private TMP_InputField titleInput;
    private TMP_InputField noteInput;
    private TMP_Dropdown statusDropdown;

    private QAToolkitMessageView messageView;

    private Transform checklistContent;
    private Button checklistButtonTemplate;

    private TMP_InputField checklistSearchInputField;
    private TMP_Dropdown statusFilterDropdown;

    private readonly List<Button> createdChecklistButtons = new List<Button>();
    private IReadOnlyList<QAChecklistData> currentChecklists;

    private Action<int> onSelectChecklist;
    private Action onStartNewChecklist;

    public void Setup(GameObject checklistWindow, GameObject checklistItemWindow,
        TextMeshProUGUI checklistItemWindowTitleText,
        TMP_InputField titleInput, TMP_InputField noteInput, TMP_Dropdown statusDropdown,
        QAToolkitMessageView messageView,
        Transform checklistContent, Button checklistButtonTemplate,
        TMP_InputField checklistSearchInputField, TMP_Dropdown statusFilterDropdown,
        Action<int> selectChecklistCallback, Action startNewChecklistCallback)
    {
        this.checklistWindow = checklistWindow;

        this.checklistItemWindow = checklistItemWindow;
        this.checklistItemWindowTitleText = checklistItemWindowTitleText;
        this.titleInput = titleInput;
        this.noteInput = noteInput;
        this.statusDropdown = statusDropdown;

        this.messageView = messageView;

        this.checklistContent = checklistContent;
        this.checklistButtonTemplate = checklistButtonTemplate;

        this.checklistSearchInputField = checklistSearchInputField;
        this.statusFilterDropdown = statusFilterDropdown;

        onSelectChecklist = selectChecklistCallback;
        onStartNewChecklist = startNewChecklistCallback;

        SetupSearchAndFilterInputs();

        SetChecklistWindow(false);
        SetChecklistItemWindow(false);

        if (this.checklistButtonTemplate != null)
            this.checklistButtonTemplate.gameObject.SetActive(false);

        SetWindowTitleToNewChecklistItem();
        ShowMessage(string.Empty);
    }

    private void SetupSearchAndFilterInputs()
    {
        if (checklistSearchInputField != null)
        {
            checklistSearchInputField.onValueChanged.RemoveAllListeners();
            checklistSearchInputField.onValueChanged.AddListener(OnSearchInputChanged);
        }

        if (statusFilterDropdown != null)
        {
            statusFilterDropdown.onValueChanged.RemoveAllListeners();
            statusFilterDropdown.onValueChanged.AddListener(OnStatusFilterChanged);
        }
    }

    private void OnSearchInputChanged(string value)
    {
        ApplyCurrentChecklistFilter();
    }

    private void OnStatusFilterChanged(int value)
    {
        ApplyCurrentChecklistFilter();
    }

    public void InitializeChecklistButtons(IReadOnlyList<QAChecklistData> checklists)
    {
        currentChecklists = checklists;

        ClearCreatedButtons();

        if (checklists == null)
            return;

        for (int i = 0; i < checklists.Count; i++)
        {
            CreateChecklistButton(i, checklists[i].title);
        }

        ApplyCurrentChecklistFilter();
    }

    public void OpenChecklistWindow()
    {
        SetChecklistWindow(true);
    }

    public void CloseChecklistWindow()
    {
        SetChecklistItemWindow(false);
        SetChecklistWindow(false);
    }

    public void ShowNewChecklistItemWindow()
    {
        ClearInput();
        SetWindowTitleToNewChecklistItem();
        ShowMessage("New Checklist Item Mode");

        SetChecklistWindow(true);
        SetChecklistItemWindow(true);
    }

    public void ShowEditChecklistItemWindow(QAChecklistData checklist)
    {
        if (checklist == null)
            return;

        SetTitleInput(checklist.title);
        SetNoteInput(checklist.note);
        SetStatusInput(checklist.status);

        if (checklistItemWindowTitleText != null)
            checklistItemWindowTitleText.text = "Edit Check Item";

        ShowMessage("Selected : " + checklist.title);

        SetChecklistWindow(true);
        SetChecklistItemWindow(true);
    }

    public void CloseChecklistItemWindow()
    {
        SetChecklistItemWindow(false);
    }

    public void ClearInput()
    {
        SetTitleInput(string.Empty);
        SetNoteInput(string.Empty);
        SetStatusInput(DefaultStatus);
    }

    public string GetTitleInput()
    {
        if (titleInput == null)
            return string.Empty;

        return titleInput.text.Trim();
    }

    public string GetNoteInput()
    {
        if (noteInput == null)
            return string.Empty;

        return noteInput.text.Trim();
    }

    public string GetStatusInput()
    {
        return GetDropdownValue(statusDropdown, DefaultStatus);
    }

    public void SetTitleInput(string value)
    {
        if (titleInput != null)
            titleInput.text = value;
    }

    public void SetNoteInput(string value)
    {
        if (noteInput != null)
            noteInput.text = value;
    }

    public void SetStatusInput(string value)
    {
        SetDropdownValue(statusDropdown, value, DefaultStatus);
    }

    public void AddChecklistButton(int checklistIndex, string title)
    {
        if (checklistIndex < 0)
            return;

        if (createdChecklistButtons.Count > 0 && checklistIndex < createdChecklistButtons.Count)
        {
            SetButtonAsChecklist(createdChecklistButtons[checklistIndex], checklistIndex, title);
            ApplyCurrentChecklistFilter();
            return;
        }

        CreateChecklistButton(checklistIndex, title);
        ApplyCurrentChecklistFilter();
    }

    public void UpdateChecklistButtonTitle(int checklistIndex, string title)
    {
        if (checklistIndex < 0 || checklistIndex >= createdChecklistButtons.Count)
            return;

        SetButtonAsChecklist(createdChecklistButtons[checklistIndex], checklistIndex, title);
        ApplyCurrentChecklistFilter();
    }

    public void RemoveChecklistButtonAndRebind(int removeIndex, IReadOnlyList<QAChecklistData> checklists)
    {
        if (removeIndex < 0 || removeIndex >= createdChecklistButtons.Count)
            return;

        Button targetButton = createdChecklistButtons[removeIndex];

        if (targetButton != null)
            UnityEngine.Object.Destroy(targetButton.gameObject);

        createdChecklistButtons.RemoveAt(removeIndex);

        RebindChecklistButtons(checklists);
        ApplyCurrentChecklistFilter();
    }

    public void SetWindowTitleToNewChecklistItem()
    {
        if (checklistItemWindowTitleText != null)
            checklistItemWindowTitleText.text = "New Check Item";
    }

    public void ShowMessage(string message)
    {
        if (messageView != null)
            messageView.ShowMessage(message);
    }

    private void CreateChecklistButton(int checklistIndex, string title)
    {
        Button newButton = CreateRawButton();

        if (newButton == null)
            return;

        createdChecklistButtons.Add(newButton);
        SetButtonAsChecklist(newButton, checklistIndex, title);
    }

    private Button CreateRawButton()
    {
        if (checklistButtonTemplate == null || checklistContent == null)
            return null;

        Button newButton = UnityEngine.Object.Instantiate(checklistButtonTemplate, checklistContent);
        newButton.gameObject.SetActive(true);

        return newButton;
    }

    private void SetButtonAsChecklist(Button button, int checklistIndex, string title)
    {
        if (button == null)
            return;

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>(true);

        if (buttonText != null)
            buttonText.text = title;

        button.onClick.RemoveAllListeners();

        int cachedIndex = checklistIndex;
        button.onClick.AddListener(() => onSelectChecklist?.Invoke(cachedIndex));
    }

    private void RebindChecklistButtons(IReadOnlyList<QAChecklistData> checklists)
    {
        currentChecklists = checklists;

        if (checklists == null)
            return;

        while (createdChecklistButtons.Count > checklists.Count)
        {
            int lastIndex = createdChecklistButtons.Count - 1;

            if (createdChecklistButtons[lastIndex] != null)
                UnityEngine.Object.Destroy(createdChecklistButtons[lastIndex].gameObject);

            createdChecklistButtons.RemoveAt(lastIndex);
        }

        while (createdChecklistButtons.Count < checklists.Count)
        {
            Button newButton = CreateRawButton();

            if (newButton == null)
                return;

            createdChecklistButtons.Add(newButton);
        }

        for (int i = 0; i < createdChecklistButtons.Count; i++)
        {
            SetButtonAsChecklist(createdChecklistButtons[i], i, checklists[i].title);
        }

        ApplyCurrentChecklistFilter();
    }

    private void ClearCreatedButtons()
    {
        for (int i = 0; i < createdChecklistButtons.Count; i++)
        {
            if (createdChecklistButtons[i] != null)
                UnityEngine.Object.Destroy(createdChecklistButtons[i].gameObject);
        }

        createdChecklistButtons.Clear();
    }

    private void ApplyCurrentChecklistFilter()
    {
        if (createdChecklistButtons == null || currentChecklists == null)
            return;

        string keyword = GetSearchKeyword();
        string statusFilter = GetDropdownValue(statusFilterDropdown, AllFilter);

        for (int i = 0; i < createdChecklistButtons.Count; i++)
        {
            Button button = createdChecklistButtons[i];

            if (button == null)
                continue;

            bool isVisible = false;

            if (i < currentChecklists.Count)
            {
                QAChecklistData checklist = currentChecklists[i];

                isVisible = IsTitleMatched(checklist, keyword) && IsStatusMatched(checklist, statusFilter);
            }

            button.gameObject.SetActive(isVisible);
        }

        RefreshChecklistListLayout();
    }

    private string GetSearchKeyword()
    {
        if (checklistSearchInputField == null)
            return string.Empty;

        return checklistSearchInputField.text.Trim();
    }

    private bool IsTitleMatched(QAChecklistData checklist, string keyword)
    {
        if (checklist == null)
            return false;

        if (string.IsNullOrWhiteSpace(keyword))
            return true;

        if (string.IsNullOrWhiteSpace(checklist.title))
            return false;

        return checklist.title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool IsStatusMatched(QAChecklistData checklist, string statusFilter)
    {
        if (checklist == null)
            return false;

        if (string.IsNullOrWhiteSpace(statusFilter) || statusFilter == AllFilter)
            return true;

        return string.Equals(checklist.status, statusFilter, StringComparison.OrdinalIgnoreCase);
    }

    private void RefreshChecklistListLayout()
    {
        if (checklistContent == null)
            return;

        RectTransform rectTransform = checklistContent as RectTransform;

        if (rectTransform != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
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

    private void SetChecklistWindow(bool value)
    {
        if (checklistWindow != null)
            checklistWindow.SetActive(value);
    }

    private void SetChecklistItemWindow(bool value)
    {
        if (checklistItemWindow != null)
            checklistItemWindow.SetActive(value);
    }
}