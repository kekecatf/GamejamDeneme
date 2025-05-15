using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button startGameButton;       // Oyuna başlama butonu
    public Button quitButton;            // Oyundan çıkış butonu
    
    [Header("Scene Settings")]
    public string gameSceneName = "Parkur";  // Oyun sahnesinin adı
    
    void Start()
    {
        // Butonlara event listener ekle
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Start Game butonu atanmamış!");
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        
        // Fare imlecini göster (varsa)
        Cursor.visible = true;
        
        // Zaman ölçeğini normal yaparak oyunun normal hızda çalıştığından emin ol
        Time.timeScale = 1f;
    }
    
    // Oyunu başlat
    public void StartGame()
    {
        Debug.Log("Oyun başlatılıyor: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Oyundan çık
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 