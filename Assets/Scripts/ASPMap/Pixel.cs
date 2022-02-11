using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public List<string> Notes;
    public void SetPixel(float x, float y, Color color)
    {
        transform.localPosition = new Vector2(x, y);
        GetComponent<SpriteRenderer>().color = color;
    }

    public void AddNote(string note)
    {
        Notes.Add(note);
    }
    public void AddNote(List<string> noteList)
    {
        string note = "";
        foreach (string n in noteList)
        {
            note += n + " ";
        }
        Notes.Add(note);
    }
}
