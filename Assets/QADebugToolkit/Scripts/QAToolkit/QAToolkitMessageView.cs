using System.Collections;
using TMPro;
using UnityEngine;

public class QAToolkitMessageView : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;

    [Header("Message Settings")]
    [SerializeField] private float showTime = 1f;
    [SerializeField] private float fadeTime = 0.5f;

    private Coroutine messageCoroutine;

    private void Awake()
    {
        HideImmediately();
    }

    public void ShowMessage(string _message)
    {
        if (messageText == null)
            return;

        if (string.IsNullOrWhiteSpace(_message))
        {
            HideImmediately();
            return;
        }

        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageText.text = _message;
        messageText.gameObject.SetActive(true);
        SetAlpha(1f);

        messageCoroutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitForSecondsRealtime(showTime);

        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            SetAlpha(alpha);

            yield return null;
        }

        HideImmediately();
        messageCoroutine = null;
    }

    private void HideImmediately()
    {
        if (messageText == null)
            return;

        messageText.text = string.Empty;
        SetAlpha(0f);
        messageText.gameObject.SetActive(false);
    }

    private void SetAlpha(float _alpha)
    {
        Color color = messageText.color;
        color.a = _alpha;
        messageText.color = color;
    }
}