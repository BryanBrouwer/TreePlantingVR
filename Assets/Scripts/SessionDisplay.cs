using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionTimeText;
    [SerializeField] private string sessionTimeTextPrefix;
    [SerializeField] private string noSessionText;
    [SerializeField] private Button sessionStartButton;
    
    private Coroutine _sessionTimerCoroutine;

    private IEnumerator SessionTimer()
    {
        var sessionManager = SessionManager.Instance;
        while (sessionManager.isSessionActive)
        {
            Debug.Log("Session time remaining: " + sessionManager.currentSessionTime);
            var timeRemaining = sessionManager.sessionTimeLimit - sessionManager.currentSessionTime;
            sessionTimeText.text = sessionTimeTextPrefix + timeRemaining.ToString("F0");
            yield return new WaitForSeconds(1.0f);
        }
    }
    private void StartSession()
    {
        _sessionTimerCoroutine = StartCoroutine(SessionTimer());
        sessionStartButton.gameObject.SetActive(false);
    }
    
    private void EndSession()
    {
        if (_sessionTimerCoroutine != null)
            StopCoroutine(_sessionTimerCoroutine);
        sessionStartButton.gameObject.SetActive(true);
        sessionTimeText.text = noSessionText;
    }
    
    private void OnEnable()
    {
        sessionStartButton.onClick.AddListener(SessionManager.Instance.StartSession);
        SessionManager.Instance.OnSessionStarted += StartSession;
        SessionManager.Instance.OnSessionEnded += EndSession;
    }
    
    private void OnDisable()
    {
        sessionStartButton.onClick.RemoveListener(StartSession);
    }
}