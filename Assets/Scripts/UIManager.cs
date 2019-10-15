using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    private int _score = 0;
    [SerializeField]
    private Text _bestScoreText;
    private int _bestScore;
    [SerializeField]
    private Text _ammoCountText;
    private int _ammoCount = 0;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Image _livesIMG;
    [SerializeField]
    private Text _gameOver;
    private bool _isGameOver = false;
    [SerializeField]
    private GameObject _pauseMenu;
    private Animator _pauseMenuAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + _score;
        _bestScore = PlayerPrefs.GetInt("BestScore", 0);
        _bestScoreText.text = "Hi-Score: " + _bestScore;
        _gameOver.gameObject.SetActive(false);
        _pauseMenuAnimator = GameObject.Find("Pause_Menu_Panel").GetComponent<Animator>();
        _pauseMenuAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _pauseMenuAnimator.SetBool("isPaused", true);
            _pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void UpdateScore(int points)
    {
        _score += points;
        _scoreText.text = "Score: " + _score;
    }

    public void UpdateAmmo(int ammo)
    {
        _ammoCount = ammo;
        _ammoCountText.text = "Ammo: " + _ammoCount;
    }

    public void CheckBestScore()
    {
        if (_score > _bestScore)
        {
            _bestScore = _score;
            _bestScoreText.text = "Hi-Score: " + _bestScore;
            PlayerPrefs.SetInt("BestScore", _bestScore);
        }
    }

    public void UpdateLives(int currentLives)
    {
        Debug.Log("current Lives = " + currentLives);
        _livesIMG.sprite = _livesSprites[currentLives];
    }

    public void GameOver()
    {
        _gameOver.gameObject.SetActive(true);
        _isGameOver = true;
        CheckBestScore();
        StartCoroutine(GameOverFade());
    }

    IEnumerator GameOverFade()
    {
        while (true)
        {
            _gameOver.text = "Game Over";
            yield return new WaitForSeconds(0.5f);
            _gameOver.text = "";
            yield return new WaitForSeconds(0.5f);
        }

    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        _pauseMenuAnimator.SetBool("isPaused", false);
        _pauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
