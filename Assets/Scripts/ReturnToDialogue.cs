using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnToDialogue : MonoBehaviour
{
    public string targetScene = "Parkur"; // Varsayılan olarak Parkur sahnesine geçiş
    public bool advanceDialogueState = false; // Diyalog durumunu ilerletmek istiyor muyuz?
    
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadTargetScene);
        }
    }
    
    public void LoadTargetScene()
    {
        // Eğer diyalog durumunu ilerletmek istersek
        if (advanceDialogueState && DiyalogGameManager.Instance != null)
        {
            DiyalogGameManager.Instance.AdvanceDialogueState();
        }
        
        // Hedef sahneyi yükle
        SceneManager.LoadScene(targetScene);
    }
} 