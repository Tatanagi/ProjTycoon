using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalDecreeManager : MonoBehaviour
{
    public ResourceType favoredType;
    public int multiplier = 3;

    public void GenerateNewDecree()
    {
        favoredType = (ResourceType)Random.Range(0, 3);
        multiplier = Random.Range(2, 5);
        Debug.Log($"Royal Decree: {favoredType} is worth x{multiplier}!");
    }

    public int GetValueExchange(ResourceType type, int amount)
    {
        int baseValue = type switch
        {
            ResourceType.Bronze => 1,
            ResourceType.Silver => 3,
            ResourceType.Gold => 6,
            _ => 0
        };

        if (type == favoredType)
            baseValue *= multiplier;
        return baseValue * amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
