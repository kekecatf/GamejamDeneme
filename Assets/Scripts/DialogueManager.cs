using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
public Text questionText;
public Button[] answerButtons;
public Text timerText;
public float timeLimit = 15f;
private Coroutine timerCoroutine;
private bool choiceMade = false;
private Dictionary<string, DialogueNode> dialogueTree;

// Parkur sahnesi için özel düğme numarası, -1 ise özellik devre dışı
public int parkurButtonIndex = -1; // Örneğin 0, ilk butonun Parkur'a yönlendireceği anlamına gelir

void Start()
{
    InitializeDialogueTree();
    ShowDialogue(DiyalogGameManager.Instance.currentNodeID);
}

void InitializeDialogueTree()
{
    dialogueTree = new Dictionary<string, DialogueNode>();

    dialogueTree.Add("start", new DialogueNode(
        "Merhaba. Nasılsın, ne yapıyorsun?",
        new string[] {
            "İyiyim, hazırlanıyorum. Sen ne yapıyorsun?",
            "İyiyim, oturuyorum.",
            "Kötüyüm."
        },
        new string[] {
            "hazirlaniyorum",
            "oturuyorum1",
            "kotu"
        }));

    dialogueTree.Add("hazirlaniyorum", new DialogueNode(
        "Ben de oturuyorum. Neye hazırlanıyorsun?",
        new string[] {
            "Dışarı çıkacağım.",
            "Yürüyüş yapacağım. Katılmak ister misin?"
        },
        new string[] {
            "disari",
            "katilmak"
        }));

    dialogueTree.Add("disari", new DialogueNode(
        "Yalnız mısın?",
        new string[] {
            "Evet.",
            "Hayır."
        },
        new string[] {
            "evet1",
            "hayir"
        }));

    dialogueTree.Add("evet1", new DialogueNode(
        "Ne yapacaksın?",
        new string[] {
            "Yürüyüş yapacağım. Katılmak ister misin?"
        },
        new string[] {
            "evet2"
        }));

    dialogueTree.Add("evet2", new DialogueNode(
        "Evet süper olur. Hemen hazırlanıyorum.",
        new string[] {
            "Harika! Senin oraya doğru geliyorum."
        },
        new string[] {
            "harika"
        }));

    dialogueTree.Add("harika", new DialogueNode(
        "Tamam. Ben de iniyorum yavaştan.",
        new string[] { },
        new string[] {
            "gameover"
        }));

    dialogueTree.Add("katilmakIsterMisin", new DialogueNode(
        "Evet süper olur hemen hazırlanıyorum.",
        new string[] {
            "Harika! Senin oraya doğru geliyorum."
        },
        new string[] {
            "geliyorum"
        }));

    dialogueTree.Add("geliyorum", new DialogueNode(
        "Tamam ben de iniyorum yavaştan🫡",
        new string[] { },
        new string[] {
            "gameover"
        }));

    dialogueTree.Add("evet3", new DialogueNode(
        "Harika, senin oraya doğru geliyorum.",
        new string[] {
            "Tamam bende iniyorum yavaştan 🫡"
        },
        new string[] {
            "tamam20dk"
        }));

    dialogueTree.Add("tamam20dk", new DialogueNode(
        "Tamam 20 dakikaya oradayım.",
        new string[] {
            "🫡"
        },
        new string[] {
            "gameover"
        }));

    dialogueTree.Add("bilmiyorum", new DialogueNode(
        "Üff ne bu soğukluk bb.",
        new string[] { },
        new string[] {
            "gameover"
        }));

    dialogueTree.Add("oturuyorum1", new DialogueNode(
        "Anladım, bugün bir planın var mı?",
        new string[] {
            "Hayır, bir şeyler yapalım mı?",
            "Evet, bir arkadaşımla yemek yiyeceğim."
        },
        new string[] {
            "birseylerYapalim",
            "arkadasYemek"
        }));

    dialogueTree.Add("arkadasYemek", new DialogueNode(
        "Anladım. Yarın eğer boşsan bir şeyler yaparız.",
        new string[] {
            "Olur yapabiliriz.",
            "Sana ayıracak zamanım yok.",
            "Ne olur yapmak istersin? Yemek yiyelim mi?"
        },
        new string[] {
            "yapabiliriz",
            "esek",
            "yemekYiyelim"
        }));

    dialogueTree.Add("yapabiliriz", new DialogueNode(
        "Tamam yarın haberleşelim.",
        new string[] {
            "Tamam."
        },
        new string[] {
            "gameover"
        }));

    dialogueTree.Add("esek", new DialogueNode(
        "Esek.",
        new string[] { },
        new string[] {
            "gameover"
        }));

    dialogueTree.Add("yemekYiyelim", new DialogueNode(
        "Olur fark etmez. (Tamam o zaman saat ve mekânı sen belirle.)",
        new string[] {
            "Tamam."
        },
        new string[] {
            "gameover"
        }));

    dialogueTree.Add("kotu", new DialogueNode(
        "Neden ne oldu?",
        new string[] {
            "Sanane."
        },
        new string[] {
            "sanane"
        }));

    dialogueTree.Add("sanane", new DialogueNode(
        "Peki.",
        new string[] { },
        new string[] {
            "gameover"
        }));
}

void ShowDialogue(string nodeID)
{
    if (!dialogueTree.ContainsKey(nodeID))
    {
        SceneManager.LoadScene("Parkur");
        return;
    }

    DialogueNode node = dialogueTree[nodeID];
    questionText.text = node.question;
    choiceMade = false;

    if (node.answers.Length == 0)
    {
        StartCoroutine(DelayedSceneLoad("Parkur", 2f));
    }

    for (int i = 0; i < answerButtons.Length; i++)
    {
        if (i < node.answers.Length && !string.IsNullOrEmpty(node.answers[i]))
        {
            answerButtons[i].gameObject.SetActive(true);
            Text btnText = answerButtons[i].GetComponentInChildren<Text>();
            btnText.text = node.answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            
            // Butonun indeksini geçici değişkende sakla (closure için)
            int buttonIndex = i;
            
            // Tüm butonlar Parkur sahnesine yönlendirilsin
            answerButtons[i].onClick.AddListener(() => {
                // YALNIZCA seçilen cevap indeksini kaydet, currentNodeID'yi değiştirme
                DiyalogGameManager.Instance.selectedAnswerIndex = buttonIndex;
                choiceMade = true;
                GotoParkurScene();
            });
        }
        else
        {
            answerButtons[i].gameObject.SetActive(false);
        }
    }

    if (timerCoroutine != null)
        StopCoroutine(timerCoroutine);

    timerCoroutine = StartCoroutine(StartTimer());
}

// Parkur sahnesine geçiş yap
public void GotoParkurScene()
{
    // SceneController varsa onu kullan
    if (SceneController.Instance != null)
    {
        SceneController.Instance.GoToParkourScene();
    }
    else
    {
        // Eski yöntem, SceneController yoksa
        SceneManager.LoadScene("Parkur");
    }
}

IEnumerator StartTimer()
{
    float currentTime = timeLimit;

    while (currentTime > 0f)
    {
        if (choiceMade) yield break;
        currentTime -= Time.deltaTime;
        timerText.text = "Süre: " + Mathf.Ceil(currentTime).ToString();
        yield return null;
    }

    if (!choiceMade)
    {
        SceneManager.LoadScene("Parkur");
    }
}

IEnumerator DelayedSceneLoad(string sceneName, float delay)
{
    yield return new WaitForSeconds(delay);
    SceneManager.LoadScene(sceneName);
}

// Bir sonraki diyalog düğümünün ID'sini döndür
public string GetNextNodeID(string currentNodeID, int answerIndex)
{
    if (dialogueTree.ContainsKey(currentNodeID))
    {
        DialogueNode node = dialogueTree[currentNodeID];
        if (node.nextNodeIDs != null && answerIndex >= 0 && answerIndex < node.nextNodeIDs.Length)
        {
            return node.nextNodeIDs[answerIndex];
        }
    }
    
    return null;
}

public class DialogueNode
{
    public string question;
    public string[] answers;
    public string[] nextNodeIDs;

    public DialogueNode(string question, string[] answers, string[] nextNodeIDs)
    {
        this.question = question;
        this.answers = answers;
        this.nextNodeIDs = nextNodeIDs;
    }
}
}