using UnityEngine;

public class PauseScreenPanel : MonoBehaviour
{
    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}
