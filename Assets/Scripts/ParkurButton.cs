using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ParkurButton : MonoBehaviour
{
    public void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(GoToParkur);
        }
        else
        {
            Debug.LogError("ParkurButton, Button bileşeni olmayan bir GameObject'e eklenmiş!");
        }
    }
    
    public void GoToParkur()
    {
        Debug.Log("Parkur sahnesine geçiliyor...");
        SceneManager.LoadScene("Parkur");
    }
} 