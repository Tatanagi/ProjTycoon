using UnityEngine;
using System;

public class WrongCurrency : MonoBehaviour
{
    private ResourceType devaluedResource;
    private bool isActive = false;

    // Explicitly define eligible resources for devaluation
    private readonly ResourceType[] eligibleResources = { ResourceType.Bronze, ResourceType.Silver, ResourceType.Gold };

    public static WrongCurrency Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ActivateEffect()
    {
        if (!isActive)
        {
            // Randomly select a resource to devalue from eligibleResources
            devaluedResource = eligibleResources[UnityEngine.Random.Range(0, eligibleResources.Length)];
            isActive = true;

            string title = "Wrong Currency!";
            string description = $"For this round, {devaluedResource} coins have no value and cannot be used at the Royal Fickle Mint!";

            UIManager.Instance.ShowCellAction(
                title,
                description,
                null,
                () => { Debug.Log($"Wrong Currency effect activated: {devaluedResource} coins are worthless this round and cannot be used at Royal Fickle Mint."); }
            );

            Debug.Log($"WrongCurrency effect: {devaluedResource} is devalued for this round.");
        }
    }

    public bool IsResourceDevalued(ResourceType type)
    {
        return isActive && type == devaluedResource;
    }

    public void ResetEffect()
    {
        isActive = false;
        devaluedResource = ResourceType.Bronze; // Reset to a default
        Debug.Log("WrongCurrency effect reset for the new round.");
    }

    public ResourceType GetDevaluedResource()
    {
        return devaluedResource;
    }
}