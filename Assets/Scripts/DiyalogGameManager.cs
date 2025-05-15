using UnityEngine;
using UnityEngine.SceneManagement;

public class DiyalogGameManager : MonoBehaviour
{
    public static DiyalogGameManager Instance;
    public string currentNodeID = "start";
    public int dialogueState = 0; // Diyalog durumu
    
    // Diyalog sonrası geçilecek sahne
    public string nextSceneName = "Parkur";
    
    [Header("Audio Settings")]
    public AudioClip gameStartSound;    // Oyun başlangıç sesi
    [Range(0f, 1f)]
    public float startSoundVolume = 0.7f;  // Ses seviyesi
    private AudioSource audioSource;
    
    // Kullanıcının seçtiği diyalog seçeneği indeksi
    public int selectedAnswerIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // AudioSource ekle
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Oyun başlangıç sesini çal
        PlayGameStartSound();
    }
    
    // Başlangıç sesini çal
    public void PlayGameStartSound()
    {
        if (gameStartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameStartSound, startSoundVolume);
            Debug.Log("Oyun başlangıç sesi çalınıyor: " + gameStartSound.name);
        }
    }
    
    // Diyalog durumunu ilerlet
    public void AdvanceDialogueState()
    {
        dialogueState++;
        Debug.Log("Diyalog durumu ilerledi: " + dialogueState);
        
        // DialogueManager'dan diyalog bilgilerini al ve bir sonraki diyalog düğümüne geç
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null && dialogueManager.GetNextNodeID(currentNodeID, selectedAnswerIndex) != null)
        {
            string nextNodeID = dialogueManager.GetNextNodeID(currentNodeID, selectedAnswerIndex);
            currentNodeID = nextNodeID;
            Debug.Log("Diyalog ağacı ilerledi: " + currentNodeID);
        }
    }
    
    // Bir sonraki sahneye geç
    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
