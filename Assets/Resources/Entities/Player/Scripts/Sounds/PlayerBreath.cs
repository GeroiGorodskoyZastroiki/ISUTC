using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System;

public class PlayerBreath : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private AudioSource _audioSource;

    [FoldoutGroup("Sounds")][SerializeField] private AudioClip _lowExhaustedWalk;
    [FoldoutGroup("Sounds")][SerializeField] private AudioClip _lowExhaustedRun;
    [FoldoutGroup("Sounds")][SerializeField] private AudioClip _mediumExhaustedWalk;
    [FoldoutGroup("Sounds")][SerializeField] private AudioClip _mediumExhaustedRun;
    [FoldoutGroup("Sounds")][SerializeField] private AudioClip _highExhaustedWalk;
    [FoldoutGroup("Sounds")][SerializeField] private AudioClip _highExhaustedRun;

    private float _lowExhaustedBreakpoint;
    private float _mediumExhaustedBreakpoint;
    private float _highExhaustedBreakpoint;

    private bool _isBreathing;

    private void Start()
    {
        if (!_lowExhaustedWalk) throw new NullReferenceException();
        _audioSource = GetComponent<AudioSource>();
        _playerMovement = GetComponentInParent<PlayerMovement>();

        _lowExhaustedBreakpoint = _playerMovement.MaxStamina * 0.6f;
        _mediumExhaustedBreakpoint = _playerMovement.MaxStamina * 0.45f;
        _highExhaustedBreakpoint = _playerMovement.MaxStamina * 0.15f;
    }

    private void Update()
    {
        if (!_isBreathing) StartCoroutine(Breathe());
    }

    private IEnumerator Breathe()
    {
        _isBreathing = true;
        yield return new WaitForSeconds(0.1f);
        AudioClip sound = GetBreatheSound();

        if (sound && _playerMovement.enabled)
        {
            Debug.Log(sound.name);
            _audioSource.PlayOneShot(sound);
        }
        _isBreathing = false;
    }

    private AudioClip GetBreatheSound()
    {
        if (_playerMovement.Stamina < _lowExhaustedBreakpoint && !_audioSource.isPlaying)
        {
            if (_playerMovement.Stamina < _mediumExhaustedBreakpoint)
            {
                if (_playerMovement.Stamina < _highExhaustedBreakpoint)
                {
                    return _playerMovement.TargetSpeed == _playerMovement.SprintSpeed ? _highExhaustedRun : _highExhaustedWalk;
                }
                return _playerMovement.TargetSpeed == _playerMovement.SprintSpeed ? _mediumExhaustedRun : _mediumExhaustedWalk;
            }
            return _playerMovement.TargetSpeed == _playerMovement.SprintSpeed ? _lowExhaustedRun : _lowExhaustedWalk;
        }
        return null;
    }
}
