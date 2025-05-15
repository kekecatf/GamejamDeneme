using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    
    [Header("Sahne Ayarları")]
    public string dialogueSceneName = "DiyalogSahnesi";
    public string parkourSceneName = "Parkur";
    
    // Aktif sahne izleme
    public string activeSceneName;
    private bool isTransitioning = false;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // İlk açılan sahneyi kaydet
            activeSceneName = SceneManager.GetActiveScene().name;
            
            // Sahne değişikliğini dinle
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Yüklenen sahneyi kaydet
        if (!isTransitioning)
        {
            activeSceneName = scene.name;
        }
    }
    
    // Diyalog sahnesinden Parkur sahnesine geçiş
    public void GoToParkourScene()
    {
        if (isTransitioning) return;
        
        StartCoroutine(TransitionToParkour());
    }
    
    // Parkur sahnesinden Diyalog sahnesine geçiş
    public void ReturnToDialogueScene()
    {
        if (isTransitioning) return;
        
        StartCoroutine(TransitionToDialogue());
    }
    
    // Diğer sahneye geçiş yap
    public void SwitchToScene(string sceneName)
    {
        if (isTransitioning) return;
        
        if (sceneName == parkourSceneName)
        {
            GoToParkourScene();
        }
        else if (sceneName == dialogueSceneName)
        {
            ReturnToDialogueScene();
        }
        else
        {
            // Farklı bir sahneye geçiş
            StartCoroutine(LoadNewScene(sceneName));
        }
    }
    
    // Diyalog sahnesinden Parkur sahnesine geçiş için coroutine
    private IEnumerator TransitionToParkour()
    {
        isTransitioning = true;
        
        // Diyalog sahnesi yoksa yükle (ilk açılışta olabilir)
        if (!IsSceneLoaded(dialogueSceneName))
        {
            yield return StartCoroutine(LoadSceneIfNeeded(dialogueSceneName));
        }
        
        // Parkur sahnesini yükle veya aktifleştir
        if (!IsSceneLoaded(parkourSceneName))
        {
            yield return StartCoroutine(LoadSceneAsync(parkourSceneName, LoadSceneMode.Additive));
        }
        
        // Diyalog sahnesini pasif yap, Parkur sahnesini aktif yap
        SetActiveScene(parkourSceneName);
        
        // İşaretçiyi kaldır
        isTransitioning = false;
        activeSceneName = parkourSceneName;
    }
    
    // Parkur sahnesinden Diyalog sahnesine geçiş için coroutine
    private IEnumerator TransitionToDialogue()
    {
        isTransitioning = true;
        
        // Diyalog sahnesini kontrol et ve yükle
        if (!IsSceneLoaded(dialogueSceneName))
        {
            yield return StartCoroutine(LoadSceneAsync(dialogueSceneName, LoadSceneMode.Additive));
        }
        
        // Parkur sahnesini pasif yap, Diyalog sahnesini aktif yap
        SetActiveScene(dialogueSceneName);
        
        // Diyalog ilerlemesini sağla
        if (DiyalogGameManager.Instance != null)
        {
            DiyalogGameManager.Instance.AdvanceDialogueState();
        }
        
        // İşaretçiyi kaldır
        isTransitioning = false;
        activeSceneName = dialogueSceneName;
    }
    
    // Yeni bir sahne yükle
    private IEnumerator LoadNewScene(string sceneName)
    {
        isTransitioning = true;
        
        // Mevcut tüm sahneleri boşalt
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != sceneName && scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }
        
        // Yeni sahneyi yükle
        if (!IsSceneLoaded(sceneName))
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        
        // İşaretçiyi kaldır
        isTransitioning = false;
        activeSceneName = sceneName;
    }
    
    // Sahnenin yüklü olup olmadığını kontrol et
    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }
    
    // Sahne yüklemesi için coroutine
    private IEnumerator LoadSceneIfNeeded(string sceneName)
    {
        if (!IsSceneLoaded(sceneName))
        {
            yield return StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive));
        }
    }
    
    // Sahneyi asenkron olarak yükle
    private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    
    // Aktif sahneyi değiştir ve diğerlerini pasifleştir
    private void SetActiveScene(string sceneToActivate)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.isLoaded)
            {
                // Sahne objelerini bul
                GameObject[] rootObjects = scene.GetRootGameObjects();
                
                // Sahneyi aktif veya pasif yap
                foreach (GameObject obj in rootObjects)
                {
                    obj.SetActive(scene.name == sceneToActivate);
                }
            }
        }
        
        // Aktif sahneyi ayarla
        Scene newActiveScene = SceneManager.GetSceneByName(sceneToActivate);
        if (newActiveScene.isLoaded)
        {
            SceneManager.SetActiveScene(newActiveScene);
        }
    }
} 