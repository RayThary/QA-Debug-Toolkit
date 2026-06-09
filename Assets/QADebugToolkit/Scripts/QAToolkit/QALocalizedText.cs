using UnityEngine;

public class QALocalizedText : MonoBehaviour
{
    [SerializeField] private string localizedText;
    public string GetLocalizedText { get { return localizedText; } }
}
