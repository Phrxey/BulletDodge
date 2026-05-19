using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    public void UpdateHearts(int currentLives)
    {
        heart1.SetActive(currentLives >= 1);
        heart2.SetActive(currentLives >= 2);
        heart3.SetActive(currentLives >= 3);
    }
}