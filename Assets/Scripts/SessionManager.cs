using System;
using System.Collections;

using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private static SessionManager _instance;
    
    public static SessionManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindFirstObjectByType<SessionManager>();

                if (!_instance)
                {
                    // Optionally, create a new GameObject with this component
                    GameObject singletonObj = new GameObject(nameof(SessionManager));
                    _instance = singletonObj.AddComponent<SessionManager>();
                }
            }
            return _instance;
        }
    }
    
    public event Action OnSessionStarted;
    public event Action OnSessionEnded;
    public bool isSessionActive => _sessionUpdateCoroutine != null;
    [field: SerializeField] public float sessionTimeLimit { get; private set; } = 300;
    public float currentSessionTime {get; private set; }
    private Coroutine _sessionUpdateCoroutine;

    private IEnumerator SessionUpdate()
    {
        while (currentSessionTime < sessionTimeLimit)
        {
            currentSessionTime += Time.deltaTime;
            yield return null;
        }
        EndSession();
    }
    
    private SessionManager()
    {
    }
    
    public void StartSession()
    {
        currentSessionTime = 0;
        _sessionUpdateCoroutine = StartCoroutine(SessionUpdate());
        OnSessionStarted?.Invoke();
        ScoreManager.Instance.ResetScore();
    }
    
    public void EndSession()
    {
        if (_sessionUpdateCoroutine != null)
            StopCoroutine(_sessionUpdateCoroutine);
        OnSessionEnded?.Invoke();
    }

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
}