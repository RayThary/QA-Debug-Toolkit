using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QAChecklistManager : MonoBehaviour
{
    [Header("Toolkit")]
    [SerializeField] private QAToolkit qaToolkit;

    [Header("Checklist List")]
    [SerializeField] private GameObject checklistWindow;
    [SerializeField] private Transform checklistContent;
    [SerializeField] private Button checklistButtonTemplate;

    [Header("Checklist Filter")]
    [SerializeField] private TMP_InputField checklistSearchInputField;
    [SerializeField] private TMP_Dropdown statusFilterDropdown;

    [Header("Checklist Item Window")]
    [SerializeField] private GameObject checklistItemWindow;
    [SerializeField] private TextMeshProUGUI checklistItemWindowTitleText;

    [Header("Checklist Input")]
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField noteInput;
    [SerializeField] private TMP_Dropdown statusDropdown;
    [SerializeField] private QAToolkitMessageView messageView;

    [Header("Default Checklist")]
    [SerializeField] private List<string> defaultChecklistTitles = new List<string>();

    private QAChecklistDataModule dataModule;
    private QAChecklistViewModule viewModule;
    private QAChecklistStorageModule storageModule;

    private int selectedChecklistIndex = -1;
    private bool isNewChecklistMode;
    private bool isInitialized;

    private void Awake()
    {
        dataModule = new QAChecklistDataModule();
        viewModule = new QAChecklistViewModule();
        storageModule = new QAChecklistStorageModule();

        dataModule.Setup(defaultChecklistTitles);
        storageModule.Setup(qaToolkit);

        viewModule.Setup(checklistWindow, checklistItemWindow, checklistItemWindowTitleText,
            titleInput, noteInput, statusDropdown, messageView,
            checklistContent, checklistButtonTemplate,
            checklistSearchInputField, statusFilterDropdown,
            SelectChecklistItem, StartNewChecklistItem);

        QAChecklistSaveData saveData = storageModule.LoadChecklists();

        dataModule.LoadAndMergeChecklists(saveData);

        viewModule.InitializeChecklistButtons(dataModule.Checklists);

        SaveAllChecklistsToJson();

        isInitialized = true;
    }

    private void OnDisable()
    {
        if (!isInitialized)
            return;

        SaveCurrentChecklistItem();
        SaveAllChecklistsToJson();
    }

    private void OnApplicationQuit()
    {
        if (!isInitialized)
            return;

        SaveCurrentChecklistItem();
        SaveAllChecklistsToJson();
    }

    public void OpenChecklistWindow()
    {
        qaToolkit.SetToggleBlocked(true);
        viewModule.OpenChecklistWindow();
    }

    public void CloseChecklistWindow()
    {
        if (!SaveCurrentChecklistItem(true))
            return;

        qaToolkit.SetToggleBlocked(false);

        SaveAllChecklistsToJson();
        viewModule.CloseChecklistWindow();
    }

    public void StartNewChecklistItem()
    {
        if (!SaveCurrentChecklistItem(true))
            return;

        selectedChecklistIndex = -1;
        isNewChecklistMode = true;

        viewModule.ShowNewChecklistItemWindow();
    }

    private void SelectChecklistItem(int index)
    {
        if (!dataModule.IsValidIndex(index))
            return;

        if (!SaveCurrentChecklistItem(true))
            return;

        selectedChecklistIndex = index;
        isNewChecklistMode = false;

        QAChecklistData checklist = dataModule.GetChecklist(index);

        viewModule.ShowEditChecklistItemWindow(checklist);
    }

    public void CloseChecklistItemWindow()
    {
        if (!SaveCurrentChecklistItem(true))
            return;

        SaveAllChecklistsToJson();

        viewModule.CloseChecklistItemWindow();
    }

    public void ClearChecklistInput()
    {
        viewModule.ClearInput();
    }

    public void SaveData()
    {
        if (!SaveCurrentChecklistItem(true))
            return;

        SaveAllChecklistsToJson();

        viewModule.ShowMessage("Data Saved.");
    }

    public void DeleteCurrentChecklistItem()
    {
        if (isNewChecklistMode || !dataModule.IsValidIndex(selectedChecklistIndex))
        {
            viewModule.ShowMessage("No checklist item selected.");
            return;
        }

        dataModule.DeleteChecklist(selectedChecklistIndex);

        viewModule.RemoveChecklistButtonAndRebind(selectedChecklistIndex, dataModule.Checklists);

        selectedChecklistIndex = -1;
        isNewChecklistMode = false;

        viewModule.ClearInput();
        viewModule.SetWindowTitleToNewChecklistItem();
        viewModule.CloseChecklistItemWindow();
        viewModule.ShowMessage("Checklist Item Deleted.");

        SaveAllChecklistsToJson();
    }

    public void ExportChecklistReportToTxt()
    {
        if (!SaveCurrentChecklistItem(true))
            return;

        SaveAllChecklistsToJson();

        storageModule.ExportChecklistReportToTxt(dataModule.Checklists);
        storageModule.ExportChecklistSheetToTsv(dataModule.Checklists);

        viewModule.ShowMessage("Export Completed.");
    }

    private bool SaveCurrentChecklistItem(bool showMessage = false)
    {
        string title = viewModule.GetTitleInput();
        string note = viewModule.GetNoteInput();
        string status = viewModule.GetStatusInput();

        if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(note))
            return true;

        if (string.IsNullOrWhiteSpace(title))
            title = "Untitled Check Item";

        int ignoreIndex = dataModule.IsValidIndex(selectedChecklistIndex) ? selectedChecklistIndex : -1;

        if (dataModule.HasSameTitle(title, ignoreIndex))
        {
            if (showMessage)
                viewModule.ShowMessage("Same title already exists.");

            return false;
        }

        if (isNewChecklistMode || !dataModule.IsValidIndex(selectedChecklistIndex))
        {
            int newChecklistIndex = dataModule.AddChecklist(title, note, status);

            selectedChecklistIndex = newChecklistIndex;
            isNewChecklistMode = false;

            QAChecklistData newChecklist = dataModule.GetChecklist(newChecklistIndex);

            viewModule.AddChecklistButton(newChecklistIndex, newChecklist.title);
        }
        else
        {
            dataModule.UpdateChecklist(selectedChecklistIndex, title, note, status);

            QAChecklistData checklist = dataModule.GetChecklist(selectedChecklistIndex);

            viewModule.UpdateChecklistButtonTitle(selectedChecklistIndex, checklist.title);
        }

        return true;
    }

    private void SaveAllChecklistsToJson()
    {
        storageModule.SaveChecklists(dataModule.Checklists);
    }
}