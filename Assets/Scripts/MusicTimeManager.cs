using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MusicTimeManager : MonoBehaviour
{
    public static MusicTimeManager instance;
    // Default tempo
    public const float DEF_TEMPO = 120,
    MIN_TEMPO = 30,
    MAX_TEMPO = 360;
    // Number of beats in a measure
    private static int numerator = 3;
    // What type of note recieves the beat. i.e 4 = quarter note, 8 = eighth note
    private static int denominator = 4;
    // Tempo of the game (in bpm)
    public static float tempo = DEF_TEMPO;
    // Time in seconds that a beat takes
    public static float noteLength = .5f;
    // The next time mark that the metronome should click
    public static float nextTime = 0;
    // Time of the next deadline
    public static float deadline = numerator * noteLength;
    // Time in seconds that a measure takes
    public static float measLength = 2f;
    // Current beat of measure -- last beat played
    public static int beat = 0;
    // Current time in ms attached to audiosource
    public static float time;
    // Amount of leeway in seconds timing bonus has. 1/16nd note by default
    public static float grace = noteLength / 4f;

    // Audiosource in control of music
    public AudioSource musicAudio;
    public AudioSource clickAudio;
    // Position in measure.
    public static float measTime;
    // Volume of clicks
    public static float clickVolume = 1;
    // Sound played every beat except for the last beat
    public AudioClip beatClick;
    // Sound played on the last beat of every measure
    public AudioClip measClick;
    // Used for debugging to disconnect time events from audiosource
    public bool noMusic = false;
    
    public MetronomeMarker marker;
    public TMP_Text tempoDisplay;

    // Converts tempo to speed in audiosource. 1 : 120
    private float TempoToSpeed()
    {
        return tempo / DEF_TEMPO;
    }

    public void SetTempo(float t)
    {
        tempo = t;
        // notelength doesn't need to be assigned here because time is linked to the audiosource
        //noteLength = 60f / tempo;
        measTime = noteLength * numerator;
        measLength = numerator * noteLength;
        grace = noteLength / 4f;
        musicAudio.pitch = TempoToSpeed();
        tempoDisplay.text = t.ToString("#.00") + " BPM";
    }

    // Tells system to send actions when last beat of measure is reached
    private void SendActions()
    {
        Player.SendActions();
    }

    private void SendBonusActions()
    {
        Player.SendActions(true);
    }

    // TODO: THIS IS NOT FINISHED
    // Tracks time of music and handles rhythm events.
    private void TrackTime()
    {
        int beatOfMeasure = (beat % numerator) + 1; //1,2,3,4 for 4/4.
        if(noMusic)
        {
            time = Time.timeSinceLevelLoad;
        }
        else
        {
            // Prevents impossible clicks after song loops
            if(time > musicAudio.time) {
                nextTime = 0;
            }
            time = musicAudio.time;
        }
        measTime = time % measLength;
        marker.Track(measTime % measLength / measLength);
        if(time >= nextTime) {
            marker.Pulse(noteLength / 4);
            beat++;
            if(beatOfMeasure == numerator) {
                SendActions();
                clickAudio.PlayOneShot(measClick, clickVolume);
                deadline += numerator * noteLength;
            } else {
                SendBonusActions();
                clickAudio.PlayOneShot(beatClick, clickVolume);
            }
            nextTime += noteLength;
        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        musicAudio = GetComponent<AudioSource>();
        instance = this;
        SetTempo(tempo);
    }

    // Update is called once per frame
    void Update()
    {
        TrackTime();
    }
}
