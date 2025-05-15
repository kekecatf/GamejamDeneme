using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public string name;                  // Yetenek adı
        public float cooldownTime = 5f;      // Bekleme süresi (saniye)
        public float currentCooldown = 0;    // Şu anki bekleme süresi
        public KeyCode activationKey;        // Aktifleştirme tuşu
        
        // Yetenek hazır mı?
        public bool IsReady()
        {
            return currentCooldown <= 0;
        }
    }
    
    public List<Skill> skills = new List<Skill>();
    
    void Update()
    {
        // Her yeteneğin bekleme süresini azalt
        foreach (Skill skill in skills)
        {
            if (skill.currentCooldown > 0)
            {
                skill.currentCooldown -= Time.deltaTime;
            }
            
            // Yetenekleri kullanma kontrolleri
            if (Input.GetKeyDown(skill.activationKey) && skill.IsReady())
            {
                UseSkill(skill);
            }
        }
    }
    
    void UseSkill(Skill skill)
    {
        // Yeteneği kullan
        Debug.Log(skill.name + " yeteneği kullanıldı!");
        
        // Bekleme süresini başlat
        skill.currentCooldown = skill.cooldownTime;
        
        // Burada yeteneğe özgü efektler eklenebilir
        // Örneğin: SpecialAttack(), SpeedBoost(), Shield() vb.
    }
    
    // Nörotransmitter güçlendirmesi için çağrılır
    public void ReduceAllCooldowns(float reductionAmount)
    {
        foreach (Skill skill in skills)
        {
            // Halihazırda bekleme durumunda olan yeteneklerin süresini azalt
            if (skill.currentCooldown > 0)
            {
                skill.currentCooldown *= (1 - reductionAmount);
                Debug.Log(skill.name + " yeteneğinin bekleme süresi azaldı: " + skill.currentCooldown);
            }
        }
    }
    
    // Editörde test için yetenek ekleme
    void OnValidate()
    {
        // Eğer hiç yetenek yoksa, örnek olarak bir tane ekle
        if (skills.Count == 0)
        {
            skills.Add(new Skill { 
                name = "Özel Saldırı", 
                cooldownTime = 5f, 
                activationKey = KeyCode.Space 
            });
        }
    }
} 