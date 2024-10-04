using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    private int _highScore;
    public UnityEvent OnScoreUpdated;
    public UnityEvent OnHighScoreUpdated;

    private void Start()
    {
        _score = 0;
        _highScore = PlayerPrefs.GetInt("HighScore");
        OnScoreUpdated?.Invoke();
        OnHighScoreUpdated?.Invoke();
    }

    public void SetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", _highScore);
    }
    public int GetHighScore()
    {
        return _highScore;
    }
    public int GetScore()
    {
        return _score;
    }
    public void IncrementScore()
    {
        _score++;
        OnScoreUpdated?.Invoke();
        if (_score > _highScore)
        {
            _highScore = _score;
            OnHighScoreUpdated?.Invoke();
        }
    }
}
