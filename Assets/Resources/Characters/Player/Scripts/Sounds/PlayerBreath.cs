using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System;

public class PlayerBreath : MonoBehaviour
{
    Player player;
    AudioSource audioSource;

    [FoldoutGroup("Sounds")][SerializeField] AudioClip lowExhaustedWalk;
    [FoldoutGroup("Sounds")][SerializeField] AudioClip lowExhaustedRun;
    [FoldoutGroup("Sounds")][SerializeField] AudioClip mediumExhaustedWalk;
    [FoldoutGroup("Sounds")][SerializeField] AudioClip mediumExhaustedRun;
    [FoldoutGroup("Sounds")][SerializeField] AudioClip highExhaustedWalk;
    [FoldoutGroup("Sounds")][SerializeField] AudioClip highExhaustedRun;

    [HideInInspector] float lowExhaustedBreakpoint;
    [HideInInspector] float mediumExhaustedBreakpoint;
    [HideInInspector] float highExhaustedBreakpoint;

    [HideInInspector] bool isBreathing = false;

    void Start()
    {
        if (!lowExhaustedWalk) throw new NullReferenceException();
        audioSource = GetComponent<AudioSource>();
        player = GetComponentInParent<Player>();

        lowExhaustedBreakpoint = player.MaxStamina * 0.6f;
        mediumExhaustedBreakpoint = player.MaxStamina * 0.45f;
        highExhaustedBreakpoint = player.MaxStamina * 0.15f;
    }

    private void Update()
    {
        if (!isBreathing) StartCoroutine(Breathe());
    }

    IEnumerator Breathe()
    {
        isBreathing = true;
        yield return new WaitForSeconds(0.1f);
        var sound = GetBreatheSound();

        if (sound && player.enabled)
        {
            Debug.Log(sound.name);
            audioSource.PlayOneShot(sound);
        }
        isBreathing = false;
        yield break;
    }

    AudioClip GetBreatheSound()
    {
        if (player.Stamina < lowExhaustedBreakpoint && !audioSource.isPlaying)
        {
            if (player.Stamina < mediumExhaustedBreakpoint)
            {
                if (player.Stamina < highExhaustedBreakpoint)
                {
                    return player.TargetSpeed == player.SprintSpeed ? highExhaustedRun : highExhaustedWalk;
                }
                return player.TargetSpeed == player.SprintSpeed ? mediumExhaustedRun : mediumExhaustedWalk;
            }
            return player.TargetSpeed == player.SprintSpeed ? lowExhaustedRun : lowExhaustedWalk;
        }
        return null;
    }
}
