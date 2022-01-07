using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SyntBoard : MonoBehaviour
{
    [SerializeField]
    private Oscillator[] oscillators;

    [SerializeField]
    private GameObject noteColumn;

    [SerializeField]
    private GameObject noteSign;

    [SerializeField]
    private bool stylophoneMode = false;

    [SerializeField]
    private float volume = 0.1f;

    private int touchCounter = 0;


    private float minFreq;
    private float maxFreq;

    private float minFreqOrig;
    private float maxFreqOrig;

    private float freqRange;

    private Dictionary<string, float> notes = new Dictionary<string, float>
    {

        {"C3", 130.81f},
        {"C#3", 138.59f},
        {"D3", 146.83f},
        {"D#3", 155.56f},
        {"E3", 164.81f},
        {"F3", 174.61f},
        {"F#3", 185.00f},
        {"G3", 196.00f},
        {"G#3", 207.65f},
        {"A3", 220.00f},
        {"A#3", 233.08f},
        {"B3", 246.94f},
        {"C4", 261.63f},
        {"C#4", 277.18f},
        {"D4", 293.66f},
        {"D#4", 311.13f},
        {"E4", 329.63f},
        {"F4", 349.23f},
        {"F#4", 369.99f},
        {"G4", 392.0f},
        {"G#4", 415.30f},
        {"A4", 440.0f},
        {"A#4", 466.16f},
        {"B4", 493.88f},
        {"C5", 523.25f},
        {"C#5", 554.37f},
        {"D5", 587.33f},
        {"D#5", 622.25f},
        {"E5", 659.25f},
        {"F5", 698.46f},
        {"F#5", 739.99f},
        {"G5", 783.99f},
        {"G#5", 830.61f},
        {"A5", 880.00f},
        {"A#5", 932.33f},
        {"B5", 987.77f}
    };

    private Dictionary<string, string> stylophoneNotesRepr = new Dictionary<string, string>
    {
        {"A3", "1"},
        {"A#3", "1.5"},
        {"B3", "2"},
        {"C4", "3"},
        {"C#4", "3.5"},
        {"D4", "4"},
        {"D#4", "4.5"},
        {"E4", "5"},
        {"F4", "6"},
        {"F#4", "6.5"},
        {"G4", "7"},
        {"G#4", "7.5"},
        {"A4", "8"},
        {"A#4", "8.5"},
        {"B4", "9"},
        {"C5", "10"},
        {"C#5", "10.5"},
        {"D5", "11"},
        {"D#5", "11.5"},
        {"E5", "12"}
    };


    private float WorldPosToFreq(Vector2 worldPos)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        float x_position_ratio = localPos.x + 0.5f;

        if (x_position_ratio < 0f)
            x_position_ratio = 0f;
        if (x_position_ratio > 1f)
            x_position_ratio = 1f;

        // return (x_position_ratio * freqRange) + minFreq;
        return Mathf.Pow(10, x_position_ratio * freqRange + minFreq);
    }

    public void InstantiateNotes()
    {
        foreach (var item in notes)
        {
            if ((item.Value > maxFreqOrig) || (item.Value < minFreqOrig))
            {
                continue;
            }

            float loc_x_position = (Mathf.Log10(item.Value) - minFreq) / freqRange;
            Debug.Log(loc_x_position);
            loc_x_position = loc_x_position - 0.5f;

            GameObject newNoteCol = Instantiate<GameObject>(noteColumn, transform, true);
            newNoteCol.transform.localPosition = new Vector2(loc_x_position, 0);
            newNoteCol.transform.localScale = new Vector2(newNoteCol.transform.localScale.x, 1);

            GameObject newNoteSign = Instantiate<GameObject>(noteSign, transform, true);
            newNoteSign.transform.localPosition = new Vector2(loc_x_position, -0.25f);

            if (stylophoneMode)
            {
                if (stylophoneNotesRepr.ContainsKey(item.Key))
                    newNoteSign.GetComponentInChildren<TextMeshPro>().text = stylophoneNotesRepr[item.Key];
                else
                    newNoteSign.GetComponentInChildren<TextMeshPro>().text = item.Key;
            }
            else
                newNoteSign.GetComponentInChildren<TextMeshPro>().text = item.Key;

            if (item.Key.Contains("#"))
            {
                newNoteCol.transform.localScale = new Vector2(newNoteCol.transform.localScale.x / 2, 1);
                newNoteCol.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);

                newNoteSign.transform.localScale /= 1.5f;
            }

        }
    }


    private void Start()
    {
        minFreq = notes["C4"];
        maxFreq = notes["C5"];

        minFreqOrig = minFreq;
        maxFreqOrig = maxFreq;

        minFreq = Mathf.Log10(minFreq);
        maxFreq = Mathf.Log10(maxFreq);

        freqRange = maxFreq - minFreq;

        InstantiateNotes();
    }

    private void Update()
    {

        touchCounter = 0;

        foreach (Touch touch in Input.touches)
        {
            if (touchCounter >= oscillators.Length)
                break;

            Vector2 touchWPosition = Camera.main.ScreenToWorldPoint(touch.position);
            Debug.DrawLine(Vector3.zero, touchWPosition, Color.red);
            oscillators[touchCounter].Frequency = WorldPosToFreq(touchWPosition);
            Debug.Log(oscillators[touchCounter].Frequency);
            oscillators[touchCounter].Volume = volume;
            touchCounter += 1;
        }

        for (int i = touchCounter; i < oscillators.Length; i++)
        {
            oscillators[i].Volume = 0f;
        }

    }

}