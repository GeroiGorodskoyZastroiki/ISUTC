using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    Player player;
    AudioSource audioSource;

    [SerializeField] AudioClip lowExhaustedWalk;
    [SerializeField] AudioClip lowExhaustedRun;
    [SerializeField] AudioClip mediumExhaustedWalk;
    [SerializeField] AudioClip mediumExhaustedRun;
    [SerializeField] AudioClip highExhaustedWalk;
    [SerializeField] AudioClip highExhaustedRun;

    float lowExhaustedBreakpoint;
    float mediumExhaustedBreakpoint;
    float highExhaustedBreakpoint;

    bool isBreathing = false;

    void Start()
    {
        player = GetComponentInParent<Player>();
        audioSource = GetComponent<AudioSource>();
        
        lowExhaustedBreakpoint = player.Stamina * 0.6f;
        mediumExhaustedBreakpoint = player.Stamina * 0.45f;
        highExhaustedBreakpoint = player.Stamina * 0.15f;
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

        if (sound)
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
