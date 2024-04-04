using System;
using System.Linq;
using SteamAudio;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using Vector3 = UnityEngine.Vector3;
using Ray = UnityEngine.Ray;
using Random = UnityEngine.Random;

public class AudioSourceLoudness : MonoBehaviour
{
    struct SoundRay
    {
        public Ray Ray;
        public float Loudness;
        public float Distance;
    }

    [Title("Sound awareness class")]

    #region Data
    [SerializeField] private int _numberOfRays = 100;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _maxPropagationDistance = 100f;
    [ShowInInspector][SerializeField] private static float _minLoudness = 0.0001f;
    [SerializeField] private float _castRate = 0.1f;
    [ReadOnly][ShowInInspector] private bool _inProgress;
    [HideInInspector] public UnityEvent<Vector3> OnSoundDetected;
    #endregion

    #region References
    private AudioSource _audioSource;
    #endregion

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!_inProgress) StartCoroutine(PropagateSoundRays());
    }

    public float CalculateInitialLoudness()
    {
        float clipLoudness = 0f;
        int sampleDataLength;
        float[] samples;
        if (GetComponent<IStreamedAudio>() != null)
        {
            var streamedAudio = GetComponent<IStreamedAudio>();
            samples = streamedAudio.ClipBuffer;
            sampleDataLength = streamedAudio.ClipBufferSize;
        }
        else
        {
            if (_audioSource.clip == null) return 0f;
            else
            {
                sampleDataLength = (int)(_audioSource.clip.frequency * _castRate);
                samples = new float[sampleDataLength];
                _audioSource.clip.GetData(samples, _audioSource.timeSamples);
            }
        }

        foreach (var sample in samples)
            clipLoudness += Mathf.Abs(sample);

        return clipLoudness /= sampleDataLength;
    }

    List<SoundRay> InitilizeSoundRays(Vector3[] directions, float loudness)
    {
        List<SoundRay> soundRays = new();
        foreach (var direction in directions)
            soundRays.Add(new SoundRay { Ray = new Ray(transform.position, direction), Loudness = loudness, Distance = 0 });
        return soundRays;
    }

    IEnumerator PropagateSoundRays()
    {
        _inProgress = true;
        yield return new WaitForSeconds(_castRate);
        Vector3[] directions = GetRayDirectionsAroundObject().Concat(GetRayDirectionsToListeners()).ToArray();
        float loudness = CalculateInitialLoudness();
        //Debug.Log(loudness);
        List<SoundRay> soundRays = InitilizeSoundRays(directions, loudness);
        while (true)
        {
            //Debug.Log(soundRays.Count);
            if (soundRays.Count > 0) soundRays = CheckCollisionWithEnemy(soundRays);
            else break;
            
        }
        _inProgress = false;
    }

    List<SoundRay> CheckCollisionWithEnemy(List<SoundRay> soundRays)
    {
        void DetectListener(float distance)
        {
            float distanceOffset = 0.1f * distance;
            var positionOffset = new Vector3(Random.Range(-distanceOffset, distanceOffset), 0, Random.Range(-distanceOffset, distanceOffset));
            var position = transform.position + positionOffset;
            OnSoundDetected.Invoke(position);
            _inProgress = false;
            StopCoroutine(PropagateSoundRays());
        }

        List<SoundRay> newSoundRays = new();
        for (int i = 0; i < soundRays.Count; i++)
        {
            //RaycastHit[] hits = Physics.RaycastAll(soundRays[i].Ray, _maxPropagationDistance, _layerMask);
            RaycastHit[] hits = Physics.SphereCastAll(soundRays[i].Ray, 0.5f,_maxPropagationDistance, _layerMask).Reverse().ToArray(); //теперь 0 элемент это 1 хит
            if (hits.Length == 0) continue;

            //Debug.DrawRay(transform.position, soundRays[i].Ray.direction*50, Color.red, 0.1f);
            var enemy = hits[0].transform.GetComponentInParent<IHearing>();
            var soundRay0 = soundRays[i];
            var distance = CalculateLoudnessAfterRolloff(GetLogarithmicRolloffRate, ref soundRay0, transform.position, hits[0].point);

            if (enemy != null)
            {
                //Debug.Log("Spartialized Loudness " + soundRay0.Loudness);
                if (enemy.HearingThreshold < soundRay0.Loudness)
                    DetectListener(distance);
            }
            else 
            {
                var reflectedSoundRay = ReflectSoundRay(ref soundRay0, hits[0]);
                if (reflectedSoundRay == null) continue;
                newSoundRays.Add((SoundRay)reflectedSoundRay);

                if (hits.Length > 1) //использовать extension для рассчёта точки выхода ray из объекта hit.GetExitPosition()
                {
                    enemy = hits[1].transform.GetComponent<EnemyHearing>();
                    var soundRay1 = soundRay0;
                    CalculateLoudnessAfterTransmission(ref soundRay1, hits[0]);
                    distance = CalculateLoudnessAfterRolloff(GetLogarithmicRolloffRate, ref soundRay1, hits[0].transform.position, hits[0].point);

                    if (enemy != null)
                    {
                        if (enemy.HearingThreshold < soundRay1.Loudness)
                            DetectListener(distance);
                    }
                }
            }
        }
        return newSoundRays;
    }

    #region CalculateLoudnessAfter
    float CalculateLoudnessAfterRolloff(Func<float, float> GetRolloffRate, ref SoundRay soundRay, Vector3 startPoint, Vector3 endPoint)
    {
        //Debug.Log("Original Loudness " + soundRay.Loudness);

        float distance = Vector3.Distance(startPoint, endPoint);
        soundRay.Distance += distance;
        soundRay.Loudness *= GetRolloffRate(soundRay.Distance);
        return distance;
        //Debug.Log("Distance " + soundRay.Distance);
        //Debug.Log("Rolloff Rate " + GetRolloffRate(soundRay.Distance));
        //Debug.Log("After Rolloff " + soundRay.Loudness);
    }

    void CalculateLoudnessAfterTransmission(ref SoundRay soundRay, RaycastHit hit)
    {
        var steamAudioGeometry = hit.transform.GetComponentInParent<SteamAudioGeometry>();
        soundRay.Loudness *= steamAudioGeometry ? steamAudioGeometry.material.midFreqTransmission : 1;
    }
        

    void CalculateLoudnessAfterAbsorption(ref SoundRay soundRay, RaycastHit hit)
    {
        var steamAudioGeometry = hit.transform.GetComponentInParent<SteamAudioGeometry>();
        soundRay.Loudness -= steamAudioGeometry ? soundRay.Loudness * steamAudioGeometry.material.midFreqAbsorption : 0f;
    }
        
    #endregion

    SoundRay? ReflectSoundRay(ref SoundRay soundRay, RaycastHit hit)
    {
        CalculateLoudnessAfterAbsorption(ref soundRay, hit);
        //Debug.Log("After Absorbtion " + soundRay.Loudness);
        if (_minLoudness > soundRay.Loudness) return null;
        //Debug.DrawRay(hit.point, Vector3.Reflect(soundRay.Ray.direction, hit.normal) * 50, Color.blue, 0.1f);
        return new SoundRay { Ray = new Ray(hit.point, Vector3.Reflect(soundRay.Ray.direction, hit.normal)), Loudness = soundRay.Loudness, Distance = soundRay.Distance };
    }

    #region GetRolloffRate
    float GetLogarithmicRolloffRate(float distance) =>
        Mathf.Clamp01(1 / (Mathf.Log10(8 * distance + 10)));

    float GetLinearRolloffRate(float distance) =>
        Mathf.Clamp01(-0.01f * distance + 1);
    #endregion

    Vector3[] GetRayDirectionsAroundObject()
    {
        Vector3[] directions = new Vector3[_numberOfRays];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2; // Равномерное распределение лучей вокруг объекта
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < _numberOfRays; i++)
        {
            float t = i/_numberOfRays;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            // Преобразование сферических координат в декартовы
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            directions[i] = new Vector3(x, y, z);
        }
        return directions;
    }

    Vector3[] GetRayDirectionsToListeners()
    {
        var listeners = gameObject.FindObjectsWithInterface<IHearing>();
        Vector3[] directions = new Vector3[listeners.Count];
        for (int i = 0; i < listeners.Count; i++)
            directions[i] = listeners[i].transform.position - transform.position;
        return directions;
    }

    #region Debug
    void OnDrawGizmosSelected() =>
        DrawRaysGizmos();

    void DrawRaysGizmos()
    {
        Vector3[] directions = GetRayDirectionsAroundObject();

        foreach (Vector3 direction in directions)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, direction * 50);
        }
    }
    #endregion
}
