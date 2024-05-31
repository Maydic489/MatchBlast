using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenuGroup;
    [SerializeField] GameObject gameGroup;
    [SerializeField] TMPro.TextMeshProUGUI movesNumText;
    [SerializeField] TMPro.TextMeshProUGUI gameOverText;

    public static UIManager instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void StartGame()
    {
        mainMenuGroup.SetActive(false);
        gameGroup.SetActive(true);
    }

    public void UpdateMovesText(int movesNum)
    {
        if (movesNumText.gameObject.activeSelf == false)
            movesNumText.gameObject.SetActive(true);

        movesNumText.text = movesNum.ToString();
    }

    public IEnumerator WonScreen()
    {
        gameOverText.text = "You  Won!";
        gameOverText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        gameOverText.gameObject.SetActive(false);

        gameGroup.SetActive(false);
        mainMenuGroup.SetActive(true);

        MatchBlastManager.instance.StopGame();

    }

    public IEnumerator GameOverScreen()
    {
        yield return new WaitForSeconds(2f);

        //in case player use the last move to win, waiting for star to fall
        if(MatchBlastManager.instance.starNum <= 0)
        {
            StartCoroutine(WonScreen());
            yield break;
        }
        
        gameOverText.text = "Game Over!";
        gameOverText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        gameOverText.gameObject.SetActive(false);
        gameGroup.SetActive(false);
        mainMenuGroup.SetActive(true);

        MatchBlastManager.instance.StopGame();
    }
}

