using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public EffectEventWithData effectEvent;

    private List<StatusEffect> statusEffects = new();
    private List<PerkEffect> perkEffects = new();
    public GameObject TestEffect;
    public GameObject TestPerk;

    private void OnEnable()
    {
        effectEvent.onStatusReverted.AddListener(RemoveStatus);
        effectEvent.onPerkReverted.AddListener(RemovePerk);
    }

    private void OnDisable()
    {
        effectEvent.onStatusReverted.RemoveListener(RemoveStatus);
    }

    private void Start()
    {
        if (TestEffect != null)
            AddStatus(TestEffect);
        
        if (TestPerk != null)
            AddPerk(TestPerk);

    }

    // Instantiates the passed StatusEffect prefab, adds it to the statusEffects list, and runs the effect's Apply() method
    public void AddStatus(GameObject statusPrefab)
    {
        var status = Instantiate(statusPrefab, gameObject.transform).GetComponent<StatusEffect>();

        status.Apply(GetComponent<IDamageable>());

        Debug.Log("Status: " + status);
        statusEffects.Add(status);
    }

    // De-list given Status Effect and destroy the game object. Should be signaled by Effect's Remove() method
    public void RemoveStatus(StatusEffect status)
    {
        statusEffects.Remove(status);
        Destroy(status.gameObject);
    }

    public void AddPerk(GameObject perkPrefab)
    {
        var perk = Instantiate(perkPrefab, gameObject.transform).GetComponent<PerkEffect>();

        perk.Apply(GetComponent<IDamageable>());

        perkEffects.Add(perk);
    }

    public void RemovePerk(PerkEffect perk)
    {
        perkEffects.Remove(perk);
        Destroy(perk.gameObject);
    }
}
