using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bkgMusicPlayer : MonoBehaviour
{
    public AudioSource Track1;
    public AudioSource Track2;
    public AudioSource Track3;
    public AudioSource Track4;

    public int TrackSelector;
    public int TrackHistory;


    // Start is called before the first frame update
    void Start()
    {
        TrackSelector = Random.Range(0, 4);
        StopAllTracks();

        if (TrackSelector == 0)
        {
            Track1.Play();
            TrackHistory = 1;
        }
        else if (TrackSelector == 1)
        {
            Track2.Play();
            TrackHistory = 2;
        }
        else if (TrackSelector == 2)
        {
            Track3.Play();
            TrackHistory = 3;
        }
        else if (TrackSelector == 3)
        {
            Track4.Play();
            TrackHistory = 4;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Track1.isPlaying == false && Track2.isPlaying == false && Track3.isPlaying == false && Track4.isPlaying == false)
        {
            TrackSelector = Random.Range(0, 4);

            if (TrackSelector == 0 && TrackHistory != 1)
            {
                StopAllTracks();
                Track1.Play();
                TrackHistory = 1;
            }
            else if (TrackSelector == 1 && TrackHistory != 2)
            {
                StopAllTracks();
                Track2.Play();
                TrackHistory = 2;
            }
            else if (TrackSelector == 2 && TrackHistory != 3)
            {
                StopAllTracks();
                Track3.Play();
                TrackHistory = 3;
            }
            else if (TrackSelector == 3 && TrackHistory != 4)
            {
                StopAllTracks();
                Track4.Play();
                TrackHistory = 4;
            }
        }
    }

    private void StopAllTracks()
    {
        if (Track1.isPlaying) Track1.Stop();
        if (Track2.isPlaying) Track2.Stop();
        if (Track3.isPlaying) Track3.Stop();
        if (Track4.isPlaying) Track4.Stop();
    }

}
