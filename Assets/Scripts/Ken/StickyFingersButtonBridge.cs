using UnityEngine;

public class StickyFingersButtonBridge : MonoBehaviour
{
    public TurnManager turnManager;

    [Header("Audio")]
    [SerializeField] private AudioSource buttonAudioSource; // AudioSource for button click SFX
    [SerializeField] private AudioClip buttonClickClip; // Button click sound clip
    [SerializeField][Range(0f, 1f)] private float buttonSfxVolume = 0.5f; // Volume for button click SFX

    private void Awake()
    {
        // Initialize AudioSource
        if (buttonAudioSource == null)
        {
            buttonAudioSource = gameObject.AddComponent<AudioSource>();
            buttonAudioSource.playOnAwake = false;
            buttonAudioSource.loop = false;
            buttonAudioSource.spatialBlend = 0f; // 2D sound for UI

            // Assign SFX mixer group
            AudioManager audioManager = FindFirstObjectByType<AudioManager>();
            if (audioManager != null && audioManager.GetMixer() != null)
            {
                buttonAudioSource.outputAudioMixerGroup = audioManager.GetMixer().FindMatchingGroups("SFX")[0];
            }
            else
            {
                Debug.LogWarning("StickyFingersButtonBridge could not find AudioManager or AudioMixer. Ensure AudioManager is in the scene and myMixer is assigned.");
            }
        }
    }

    public void OnStickyFingersButtonClicked()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play button click sound: AudioSource or buttonClickClip is missing.");
        }

        // Checks whose turn it is
        int currentIndex = turnManager.GetCurrentPlayerIndex();
        // Finds their StickyFingers script
        StickyFingers currentPlayerSF = StickyFingers.GetStickyFingersByPlayerIndex(currentIndex);

        if (currentPlayerSF != null)
        {
            // Opens their UI
            currentPlayerSF.ActivateStickyFingersUI();
        }
        else
        {
            Debug.LogWarning("StickyFingers not found for current player!");
        }
    }
}