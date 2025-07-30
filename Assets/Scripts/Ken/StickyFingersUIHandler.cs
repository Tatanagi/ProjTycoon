using UnityEngine;

public class StickyFingersUIHandler : MonoBehaviour
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
                Debug.LogWarning("StickyFingersUIHandler could not find AudioManager or AudioMixer. Ensure AudioManager is in the scene and myMixer is assigned.");
            }
        }
    }

    // Called by the Confirm button
    public void ConfirmSteal()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play button click sound: AudioSource or buttonClickClip is missing.");
        }

        // Gets active player's StickyFingers script
        var currentPlayer = StickyFingers.GetStickyFingersByPlayerIndex(turnManager.GetCurrentPlayerIndex());
        // Call confirm logic
        if (currentPlayer != null)
        {
            currentPlayer.OnStickyFingersConfirmed();
        }
        else
        {
            Debug.LogWarning("Could not find current player's StickyFingers script.");
        }
    }

    // Called by the Cancel button
    public void CancelSteal()
    {
        if (buttonAudioSource != null && buttonClickClip != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickClip, buttonSfxVolume);
        }
        else
        {
            Debug.LogWarning("Cannot play button click sound: AudioSource or buttonClickClip is missing.");
        }

        var currentPlayer = StickyFingers.GetStickyFingersByPlayerIndex(turnManager.GetCurrentPlayerIndex());
        // Closes panel
        if (currentPlayer != null)
        {
            currentPlayer.CloseStickyFingersUI();
        }
        else
        {
            Debug.LogWarning("Could not find current player's StickyFingers script.");
        }
    }
}