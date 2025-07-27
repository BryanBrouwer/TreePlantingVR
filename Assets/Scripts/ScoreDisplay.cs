using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public void UpdateScore(int score)
    {
        Debug.Log(score);
    }
        
    private void OnEnable()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateScore;
    }
        
    private void OnDisable()
    {
        ScoreManager.Instance.OnScoreChanged -= UpdateScore;
    }
}