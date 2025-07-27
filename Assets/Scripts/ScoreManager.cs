using System;

public class ScoreManager
{
    private static ScoreManager _instance;

    public static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ScoreManager();
            return _instance;
        }
    }
        
    private int _score;
    private event Action<int> OnScoreChanged;
        
    private ScoreManager()
    {
    }
        
    public void AddScore(int score)
    {
        _score += score;
        OnScoreChanged?.Invoke(_score);
    }
        
    public int GetScore()
    {
        return _score;
    }
        
    public void ResetScore()
    {
        _score = 0;
        OnScoreChanged?.Invoke(_score);
    }
}