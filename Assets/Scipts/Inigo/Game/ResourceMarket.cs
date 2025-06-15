using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Bronze, Silver, Gold }

[System.Serializable]
public class ResourceToken
{
    public ResourceType type;
    public int quantity;
}
public class ResourceMarket : MonoBehaviour
{
    public List<ResourceToken> availableTokens = new List<ResourceToken>();

    public void GenerateTokens(int round)
    {
        availableTokens.Clear();

        availableTokens.Add(new ResourceToken { type = ResourceType.Bronze, quantity = 5 + round });
        availableTokens.Add(new ResourceToken { type = ResourceType.Silver, quantity = 3 + round / 2 });
        availableTokens.Add(new ResourceToken { type = ResourceType.Gold, quantity = 1 + round / 3 });

        Debug.Log("new tokens added to the market");

    }

    public void GiveResourceToPlayer(PlayerController player, int roll)
    {
        if (roll <= 2)
            Debug.Log("Bronze x2");
        else if (roll <= 4)
            Debug.Log("Silver x1");
        else
            Debug.Log("Gold x1");
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
