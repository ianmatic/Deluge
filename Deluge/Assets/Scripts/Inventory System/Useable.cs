using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Useable : Item
{
    // FIELDS
    // Basics
    public int usesRemaining = 0;
    Entity holder;

    // For Temporary Items
    public bool temporary = false;
    public int duration = 0;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Trigger the effects being applied from player script
        if (usesRemaining == 0)
        {
            Destroy(gameObject);
        }
    }


    // Applies the effects of the useable item to the item holder
    public void ApplyEffects(GameObject user)
    {
        // Get the holder component
        UpdateHolder(user);

        // Degrade the amount of uses
        usesRemaining--;

        // Apply stat bonuses
        holder.attack += bonusAtk;
        holder.defense += bonusDef;
        holder.health += bonusHP;
        holder.maxHealth += bonusMaxHP;
        holder.vamp += bonusVamp;

        // TODO: Apply VFX

        // Invoke a function to remove the changes after the duration of the effect should end
        if (temporary)
        {
            Invoke("RemoveEffects", duration);
        }
    }

    private void UpdateHolder(GameObject user)
    {
        holder = user.GetComponent<Entity>();
    }

    // Invokeable method to remove previously applied effects
    private void RemoveEffects()
    {
        // Remove the effects
        holder.attack -= bonusAtk;
        holder.defense -= bonusDef;
        holder.health -= bonusHP;
        holder.maxHealth -= bonusMaxHP;
        holder.vamp -= bonusVamp;
    }
}
