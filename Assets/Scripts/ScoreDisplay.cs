using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string scoreTextPrefix;
    
    public void UpdateScore(int score)
    {
        scoreText.text = scoreTextPrefix + score;
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