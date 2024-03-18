using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioSound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 5f)]
    public float volume = 1.0f;

    [Range(.1f, 3f)]
    public float pitch = 1.0f;

    public bool loop;
    public bool randomize;

    [HideInInspector]
    public List<AudioSource> sources;

    public void Play()
    {
        if (randomize)
        {
            sources[UnityEngine.Random.Range(0, sources.Count)].Play();
        }
        else
        {
            sources[0].Play();
        }
    }

    public void PlayOneShot()
    {
        if (randomize)
        {
            sources[UnityEngine.Random.Range(0, sources.Count)].PlayOneShot(clip);
        }
        else
        {
            sources[0].PlayOneShot(this.clip);
        }
    }

    public void Stop()
    {
        foreach (var audioSource in sources)
        {
            audioSource.Stop();
        }
    }

    public void Initialize(GameObject parent)
    {
        sources = new List<AudioSource>(3);

        var source = parent.AddComponent<AudioSource>();
        source.clip = clip;

        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;

        sources.Add(source);
        if (randomize)
        {
            for (int i = 0; i < 2; i++)
            {
                var sourceAnother = parent.AddComponent<AudioSource>();
                sourceAnother.clip = clip;
                sourceAnother.volume = volume + UnityEngine.Random.Range(-.1f, .1f);
                sourceAnother.pitch = volume + UnityEngine.Random.Range(-.1f, .1f);
                sourceAnother.loop = loop;
                sources.Add(sourceAnother);
            }
        }
    }

    public void ChangeVolumeByMaster(float masterVolume)
    {
        foreach (var source in sources)
        {
            if (source == null) continue;
            source.volume = volume * masterVolume;
        }
    }
}

[System.Serializable]
public class AudioSoundCombined
{
    public string name;

    [SerializeField]
    public AudioClip[] clips;

    [Range(0f, 5f)]
    public float volume = 1.0f;

    [Range(.1f, 3f)]
    public float pitch = 1.0f;

    public bool loop;

    [HideInInspector]
    public List<AudioSource> sources;

    public void Play()
    {
        sources[UnityEngine.Random.Range(0, sources.Count)].Play();
    }

    public void PlayOneShot()
    {
        int index = UnityEngine.Random.Range(0, sources.Count);
        sources[index].PlayOneShot(clips[index]);
    }

    public void Stop()
    {
        foreach (var audioSource in sources)
        {
            audioSource.Stop();
        }
    }

    public void Initialize(GameObject parent)
    {
        sources = new List<AudioSource>(clips.Length);

        foreach (AudioClip clip in clips)
        {
            var source = parent.AddComponent<AudioSource>();
            source.clip = clip;

            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            sources.Add(source);
        }
    }

    public void ChangeVolumeByMaster(float masterVolume)
    {
        foreach (var source in sources)
        {
            if (source == null) continue;
            source.volume = masterVolume * volume;
        }
    }
}


public class AudioManager : MonoBehaviour
{
    public float volumeAll = 1.0f;
    public AudioSound[] sounds;
    public AudioSoundCombined[] soundsCombined;

    public static AudioManager instance;
    private Dictionary<string, int> soundNameToIndex;
    private Dictionary<string, int> soundCombinedNameToIndex;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        soundNameToIndex = new Dictionary<string, int>();
        for (int i = 0; i < sounds.Length; i++)
        {
            AudioSound s = sounds[i];
            soundNameToIndex.Add(s.name, i);

            s.Initialize(gameObject);
        }


        soundCombinedNameToIndex = new Dictionary<string, int>();
        for (int i = 0; i < soundsCombined.Length; i++)
        {
            AudioSoundCombined s = soundsCombined[i];
            soundCombinedNameToIndex.Add(s.name, i);

            s.Initialize(gameObject);
        }

    }
    public void Play(string name)
    {
        AudioSound s = sounds[soundNameToIndex[name]];
        s.Play();
    }

    public void PlayOneShot(string name)
    {
        AudioSound s = sounds[soundNameToIndex[name]];
        s.PlayOneShot();
    }

    public void Stop(string name)
    {
        AudioSound s = sounds[soundNameToIndex[name]];
        s.Stop();
    }

    public void PlayCombined(string name)
    {
        AudioSoundCombined s = soundsCombined[soundCombinedNameToIndex[name]];
        s.Play();
    }

    public void PlayOneShotCombined(string name)
    {
        AudioSoundCombined s = soundsCombined[soundCombinedNameToIndex[name]];
        s.PlayOneShot();
    }

    public void StopCombined(string name)
    {
        AudioSoundCombined s = soundsCombined[soundCombinedNameToIndex[name]];
        s.Stop();
    }

    public void ChangeVolumeAll(float newValue)
    {
        volumeAll = newValue;
        PropagateVolumeChange();
    }


    private void OnValidate()
    {
        PropagateVolumeChange();
    }

    private void PropagateVolumeChange()
    {
        foreach (var sound in sounds)
        {
            if (sound == null) continue;
            sound.ChangeVolumeByMaster(volumeAll);
        }

        foreach (var sound in soundsCombined)
        {
            if (sound == null) continue;
            sound.ChangeVolumeByMaster(volumeAll);
        }
    }

}
