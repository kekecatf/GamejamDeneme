using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Bu script butonlara direkt olarak eklenebilir ve Inspector üzerinden fonksiyonları atanabilir.
/// </summary>
public class ButtonHelper : MonoBehaviour
{
    [Header("Olaylar")]
    public UnityEvent onButtonClick;    // Butona basıldığında çalışacak olay
    
    [Header("Sahne Geçişleri")]
    public string targetSceneName;      // Geçiş yapılacak sahne adı
    public bool loadScene = false;      // Sahne yükleme aktif mi?
    
    private Button button;
    
    void Awake()
    {
        // Buton bileşenini al
        button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError("Bu GameObject'de Button bileşeni yok!");
            return;
        }
        
        // Butona tıklama olayı ekle
        button.onClick.AddListener(OnButtonClicked);
        
        Debug.Log(gameObject.name + " butonu hazırlandı.");
    }
    
    void OnButtonClicked()
    {
        Debug.Log(gameObject.name + " butonuna tıklandı!");
        
        // Özel olayları çalıştır
        if (onButtonClick != null)
        {
            onButtonClick.Invoke();
        }
        
        // Eğer sahne yükleme aktifse, belirtilen sahneyi yükle
        if (loadScene && !string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log(targetSceneName + " sahnesi yükleniyor...");
            SceneManager.LoadScene(targetSceneName);
        }
    }
    
    // Ana menüye dönme fonksiyonu - buton onClick olayından direkt çağrılabilir
    public void ReturnToMainMenu()
    {
        Debug.Log("Ana Menüye dönülüyor...");
        SceneManager.LoadScene("AnaMenu");
    }
    
    // Oyunu yeniden başlatma fonksiyonu - buton onClick olayından direkt çağrılabilir
    public void RestartCurrentScene()
    {
        Debug.Log("Sahne yeniden başlatılıyor...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Uygulamadan çıkış - buton onClick olayından direkt çağrılabilir
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