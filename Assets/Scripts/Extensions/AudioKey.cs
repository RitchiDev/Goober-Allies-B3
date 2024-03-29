using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Audio Key", menuName = "Audio/Create New Audio Key")]
public class AudioKey : ScriptableObject
{
    public enum SoundClipPlayOrder
    {
        Random = 0,
        InOrder = 1,
        Reverse = 2,
    }

    #region Config

    private static readonly float SEMITONES_TO_PITCH_CONVERSION_UNIT = 1.05946f;

    [SerializeField] private AudioMixerGroup m_MixerGroup;

    [SerializeField] AudioClip[] m_Clips;

    [Tooltip("Random volume range between these 2 values")][SerializeField] Vector2 m_Volume = new Vector2(0.5f, 0.5f);

    //Pitch / Semitones
    [SerializeField] private bool m_UseSemitones;

    [SerializeField] private Vector2Int m_Semitones = new Vector2Int(0, 0);

    [SerializeField] private bool m_Loop = false;

    [SerializeField] private Vector2 m_Pitch = new Vector2(1, 1);

    [SerializeField] private SoundClipPlayOrder m_PlayOrder;

    [SerializeField] private int m_PlayIndex = 0;

    private AudioSource m_Source;

    #endregion

    #region Code

    public void SyncPitchAndSemitones()
    {
        if (m_UseSemitones)
        {
            m_Pitch.x = Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, m_Semitones.x);
            m_Pitch.y = Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, m_Semitones.y);
        }
        else
        {
            m_Semitones.x = Mathf.RoundToInt(Mathf.Log10(m_Pitch.x) / Mathf.Log10(SEMITONES_TO_PITCH_CONVERSION_UNIT));
            m_Semitones.y = Mathf.RoundToInt(Mathf.Log10(m_Pitch.y) / Mathf.Log10(SEMITONES_TO_PITCH_CONVERSION_UNIT));
        }
    }

    private AudioClip GetAudioClip()
    {
        // get current clip
        AudioClip clip = m_Clips[m_PlayIndex >= m_Clips.Length ? 0 : m_PlayIndex];

        // find next clip
        switch (m_PlayOrder)
        {
            case SoundClipPlayOrder.InOrder:
                m_PlayIndex = (m_PlayIndex + 1) % m_Clips.Length;
                break;
            case SoundClipPlayOrder.Random:
                m_PlayIndex = Random.Range(0, m_Clips.Length);
                break;
            case SoundClipPlayOrder.Reverse:
                m_PlayIndex = (m_PlayIndex + m_Clips.Length - 1) % m_Clips.Length;
                break;
        }

        // return clip
        return clip;
    }

    public AudioSource Play(AudioSource audioSourceParam = null)
    {
        if (m_Clips.Length == 0)
        {
            Debug.LogWarning($"Missing sound clips for {name}");
            return null;
        }

        if (m_Source != null)
        {
            m_Source.Play();
            return m_Source;
        }

        AudioSource source = audioSourceParam;
        if (source == null)
        {
            GameObject sourceObject = new GameObject($"Source of: {name}", typeof(AudioSource));
            source = sourceObject.GetComponent<AudioSource>();
        }

        // set source config:
        source.outputAudioMixerGroup = m_MixerGroup;
        source.clip = GetAudioClip();
        source.volume = Random.Range(m_Volume.x, m_Volume.y);
        source.pitch = m_UseSemitones
            ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(m_Semitones.x, m_Semitones.y))
            : Random.Range(m_Pitch.x, m_Pitch.y);
        source.loop = m_Loop;
        source.Play();

        if (source.loop)
        {
            m_Source = source;
        }
        else
        {
            Destroy(source.gameObject, source.clip.length / source.pitch);
        }

        return source;
    }

    public AudioSource Stop()
    {
        if (m_Source == null)
        {
            Debug.LogWarning("Cant stop this source");
            return null;
        }

        m_Source.Stop();

        return m_Source;
    }

    #endregion
}
