using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{


    public string nextLevel;

    [SerializeField] private Billy_EnemyAI billyEnemyAI;

    [SerializeField] private float delayBeforeFade = 2f;

    // Start is called before the first frame update
    void Start()
    {
        billyEnemyAI = GameObject.FindObjectOfType<Billy_EnemyAI>();

        if(billyEnemyAI != null)
        {
            billyEnemyAI.OnEnemyDeath += LoadTheNextLevel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTheNextLevel()
    {
        StartCoroutine(FadeAndLoadLevel());
    }

    private IEnumerator FadeAndLoadLevel()
    {

        UserInput.instance.DisableInput();
        // Wait for death animation
        yield return new WaitForSeconds(delayBeforeFade);

        // Fade out
        if (ScreenFade.Instance != null)
        {
            ScreenFade.Instance.FadeIn();
            yield return new WaitForSeconds(ScreenFade.Instance.defaultFadeDuration);
        }

        // Load the next level
        SceneManager.LoadScene(nextLevel);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            LoadTheNextLevel();
        }
    }

    private void OnDestroy()
    {
        if(billyEnemyAI != null)
        {
            billyEnemyAI.OnEnemyDeath -= LoadTheNextLevel;
        }
    }
}
