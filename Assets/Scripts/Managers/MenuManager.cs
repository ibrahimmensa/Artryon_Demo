using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Image logo;
    public GameObject MainMenu;
    public GameObject splash;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        fadeInLogo();
    }

    public void fadeInLogo()
    {
        StartCoroutine(fadeInLogoStart());
    }

    IEnumerator fadeInLogoStart()
    {
        while(logo.color.a < 1)
        {
            yield return new WaitForSeconds(0.1f);
            logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, logo.color.a + 0.05f);
        }
        MainMenu.SetActive(true);
        splash.SetActive(false);
    }

    public void onClickTry()
    {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
