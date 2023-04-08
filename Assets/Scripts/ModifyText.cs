using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModifyText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string newText = "New text content";
    private string[] clauseSample = new string[] { "Sample 1", "Sample 2", "Sample 3" }; //clause samples
    private string clauseA;
    private string clauseB;
    private Color clauseACol;
    private Color clauseBCol;
    List<Color> highlightColor = new List<Color> { Color.black, Color.red, Color.blue }; //list of colors
    Dictionary<string, int> colorCombinationCounts = new Dictionary<string, int>();
    Color[] colorCombination = new Color[] { Color.red, Color.blue };
    Color[] newColors;
    
    void Start()
    {
        // Set the initial text content
        textComponent.text = "Press SPACE to start";
        
        newColors = new Color[3];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Change the text content
            ISB();
            SetNewColors();
            textComponent.text = newText;
        }
    }

    public void ISB() //Initial Sentence - Before
    {
        clauseA = clauseSample[Random.Range(0, clauseSample.Length)]; //Choose the clause for A
        clauseB = clauseSample[Random.Range(0, clauseSample.Length)]; //Choose the clause for B

        clauseACol = highlightColor[Random.Range(0, highlightColor.Count)]; //changing color for clause A
        
        if (clauseACol == highlightColor[0]) //change color for clause B considering the color of clause A
        {
            clauseBCol = highlightColor[0];
        }
        else if (clauseACol == highlightColor[1])
        {
            clauseBCol = highlightColor[2];
        }
        else if (clauseACol == highlightColor[2])
        {
            clauseBCol = highlightColor[1];
        }

        newText = "Before " + //apply together to the text
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseACol) + ">" + clauseA + "</color>" +
                  ", " +
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseBCol) + ">" + clauseB + "</color>";
    }

    void SetNewColors()
    {
        string colorCombinationKey = string.Join(",", colorCombination);

        if (!colorCombinationCounts.ContainsKey(colorCombinationKey) || colorCombinationCounts[colorCombinationKey] < 3)
        {
            newColors[0] = colorCombination[0];
            newColors[1] = colorCombination[1];
            //newColors[2] = highlightColor[Random.Range(0, highlightColor.Count)];

            if (colorCombinationCounts.ContainsKey(colorCombinationKey))
            {
                colorCombinationCounts[colorCombinationKey]++;
            }
            else
            {
                colorCombinationCounts[colorCombinationKey] = 1;
            }
        }
        else
        {
            Debug.Log("Color combination used too many times!");
        }
    }
}
