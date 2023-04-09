using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ModifyText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public Fixation fix;
    public WriteCSV csv;
    public TimeManager timeman;
    private bool isCoroutineRunning = false;
    public string newText = "";
    
    //Clauses
    private string[] clauseASample = new string[]
    {
        "Mary pulled a trigger", 
        "I cleaned my room", 
        "Bob came home",
        "Chester spilled the coffee"
    }; //clause A samples
    private string[] clauseBSample = new string[]
    {
        "the telephone rang", 
        "someone knocked at the door", 
        "I turned on the lights",
        "it started to rain"
    }; //clause B samples
    private string clauseA;
    private string clauseB;
    
    //Colors
    private Color clauseACol;
    private Color clauseBCol;
    List<Color> highlightColor = new List<Color> { Color.black, Color.red, Color.blue }; //list of colors
    int redBlueCount = 0; //color combination counters
    int blueRedCount = 0;
    int blackCount = 0;

    //Actions
    List<Action> actions = new List<Action>(); //A list for sentence structures Trial 1 and 2
    int i = 0; //the index of the item in the actions
    
    //Data lists
    public List<string> response = new List<string>(); //Collects responses
    public List<int> trialCounter = new List<int>(); //Collects trial numbers
    public List<string> answers = new List<string>(new[]
    {
        "B", "A", "A", "B", "A", "B", "B", "A", "A", "B", "A", "B", //Trial 1 answers ISB-ISA
        "A", "B", "B", "A", "B", "A", "A", "B", "B", "A", "B", "A" //Trial 2 answers FSB-FSA
    }); //Since actions are defined, answers are also defined

    public List<int> correctness = new List<int>(); //Collects correctness values
    public List<int> ifcolor = new List<int>(); //Collects data if the trial was colored
    public List<float> time = new List<float>(); //Collects _trialTime data
    
    //CSV Data
    List<string> allData = new List<string>();
    public string currData;
    public string header = "Trial Number, Trial Name, Color, Response, Correctness, RT"; //header

    void Start()
    {
        // Set the initial text content
        newText = "<color=red> Trial 1 </color> \n " +
                  "\n You will see the sentences in the format: \n" +
                  " <color=red> Clause 1, Clause 2 </color> \n" +
                  "\n Press 'S' to choose Clause 1 \n" +
                  "Press 'K' to choose Clause 2 \n" +
                  "\n Read each sentence carefully and choose the Clause that \n" +
                  "happened <color=red> EARLIER </color> \n" +
                  " \n Press SPACE to start";
        AddingActions();
    }

    void Update()
    {
        textComponent.text = newText;
        if (
            (Input.GetKeyDown(KeyCode.Space) || 
             Input.GetKeyDown(KeyCode.S) || 
             Input.GetKeyDown(KeyCode.K)) && 
            !isCoroutineRunning)
        {
            StartCoroutine(Order());

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            response.Add("0");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            timeman._timeResponse = Time.realtimeSinceStartup; //the time of the response
            timeman.callTime(); //calculate the difference
            time.Add(timeman._trialTime);
            
            response.Add("A");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            timeman._timeResponse = Time.realtimeSinceStartup; //the time of the response
            timeman.callTime(); //calculate the difference
            time.Add(timeman._trialTime);
            
            response.Add("B");
        }
    }

    IEnumerator Order() //Fixation --> Sentence
    {
        isCoroutineRunning = true;

        // Execute the first function
        //Debug.Log("Starting function 1...");
        yield return StartCoroutine(_fixation());

        // Execute the second function
        //Debug.Log("Starting function 2...");
        yield return StartCoroutine(_clauseManager());

        isCoroutineRunning = false;
    }

    IEnumerator _fixation() //Ienumerator executes Fixation
    {
        newText = ""; //the text does not overlap fixation
        yield return new WaitForSeconds(0.5f);
        fix.ActivateFixation();
        yield return new WaitForSeconds(1.0f);
    }
    
    IEnumerator _clauseManager() //IEnumerator executes void ClauseManager
    {
        ClauseManager();
        yield return null;
    }
    
    public void ClauseManager() 
    {
        //Assign samples to clauses
        clauseA = clauseASample[UnityEngine.Random.Range(0, clauseASample.Length)]; //Choose the clause for A
        clauseB = clauseBSample[UnityEngine.Random.Range(0, clauseBSample.Length)]; //Choose the clause for B
        
        //Assign colors to clauses
        while(true)
        {
            clauseACol = highlightColor[UnityEngine.Random.Range(0, highlightColor.Count)]; //changing color for clause A
        
            if (clauseACol == highlightColor[0] && blackCount < 6) //change color for clause B considering the color of clause A
            {
                clauseBCol = highlightColor[0];
                ChooseAction();
                blackCount++; //count number of color combination occurrences
                //Debug.Log("black = " + blackCount);
                ifcolor.Add(0);
                break;
            }
            else if (clauseACol == highlightColor[1] && redBlueCount < 3)
            {
                clauseBCol = highlightColor[2];
                ChooseAction();
                redBlueCount++;
                ifcolor.Add(1);
                //Debug.Log("redBlue = " + redBlueCount);
                break;
            }
            else if (clauseACol == highlightColor[2] && blueRedCount < 3)
            {
                clauseBCol = highlightColor[1];
                ChooseAction();
                blueRedCount++;
                ifcolor.Add(1);
                //Debug.Log("blueRed = " + blueRedCount);
                break;
            }
            
            else if (redBlueCount >= 3 && //Determine when is the end of a trial
                     blueRedCount >= 3 &&
                     blackCount >= 6)
            {
                ChooseAction();
                break;
            }

            else //Repeat the loop to fulfill the requirements for ending the trial
            {
                Debug.Log("Repeating...");
                continue;
            }
        }

    }

    public void ISB() //Initial Sentence - Before
    {
        newText = "Before " + 
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseACol) + ">" + clauseA + "</color>" + 
                  ", " + 
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseBCol) + ">" + clauseB + "</color>";
        
    }
    
    public void ISA() //Initial Sentence - After
    {
        newText = "After " + 
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseACol) + ">" + clauseA + "</color>" + 
                  ", " + 
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseBCol) + ">" + clauseB + "</color>";
        
    }
    
    public void FSB() //Final Sentence - Before
    {
        /*
        for (int i = 0; i < clauseA.Length; i++)
        {
            if (char.IsLetter(clauseA[i]))
            {
                clauseA = clauseA.Substring(0, i) + char.ToUpper(clauseA[i]) + clauseA.Substring(i + 1);
                break;
            }
        */    
        newText = "<color=#" + ColorUtility.ToHtmlStringRGB(clauseACol) + ">" + clauseA + "</color>" + 
                  ", " + "before " +
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseBCol) + ">" + clauseB + "</color>";
        
    }
    
    public void FSA() //Final Sentence - After
    {
        newText = "<color=#" + ColorUtility.ToHtmlStringRGB(clauseACol) + ">" + clauseA + "</color>" + 
                  ", " + "after " + 
                  "<color=#" + ColorUtility.ToHtmlStringRGB(clauseBCol) + ">" + clauseB + "</color>";
        
    }

    public void ChooseAction()
    {
        try
        {
            if (i == 12 && redBlueCount >= 3 &&
                blueRedCount >= 3 &&
                blackCount >= 6) //When Trial 1 ends
            {
                newText = "End of <color=red> Trial 1 </color>.\n" +
                          "\n Press SPACE to start <color=red> Trial 2 </color>";
                redBlueCount = 0; //Reset the counters
                blueRedCount = 0;
                blackCount = 0;

            }
            else if (i <= actions.Count)
            {
                timeman._timeStart = Time.realtimeSinceStartup; //set the new timer
                actions[i]();
                Debug.Log(i);
                i++;
                trialCounter.Add(i);
            }
            else
            {
                EndTrial(); //Note: for some reason skips that part, 
                            //so had to catch the error
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            //Debug.Log("I catched an Error");
            EndTrial();
        }

    } //Executes each action from list of actions for Trial 1 and 2
    
    public void EndTrial()
    {
        newText = "End of Trial\n" +
                  "\n Thank you for being a participant!";
        response.RemoveAt(0);
        response.RemoveAt(12); //delete the first Space Bar answer that starts the trial
                                //and the Space Bar answer to start the Trial 2
        
        CorrectCheck();
        SaveTrialResponses();
        csv.MakeCSV(allData, header);
        
        /*
        string myString = string.Join(", ", response);
        Debug.Log(myString);

        string trialString = string.Join(", ", trialCounter);
        Debug.Log(trialString); */

        string correctString = string.Join(", ", correctness);
        Debug.Log(correctString);
        
    }

    public void AddingActions()
    {
        //adding actions in the right order for Trial 1 to the list
        actions.Add(ISB); //0
        actions.Add(ISA); //1
        actions.Add(ISA);
        actions.Add(ISB);
        actions.Add(ISA);
        actions.Add(ISB);
        actions.Add(ISB);
        actions.Add(ISA);
        actions.Add(ISA);
        actions.Add(ISB);
        actions.Add(ISA);
        actions.Add(ISB);
        
        //adding actions in the right order for Trial 2 to the list
        actions.Add(FSB); //12
        actions.Add(FSA);
        actions.Add(FSA);
        actions.Add(FSB);
        actions.Add(FSA);
        actions.Add(FSB);
        actions.Add(FSB);
        actions.Add(FSA);
        actions.Add(FSA);
        actions.Add(FSB);
        actions.Add(FSA);
        actions.Add(FSB); //23
    }

    public void CorrectCheck()
    {
        for (int n = 0; n <= 23; n++)
        {
            if (response[n] == answers[n])
            {
                correctness.Add(1);
            }
            else
            {
                correctness.Add(0);
            }
        }
    }
    
    public void SaveTrialResponses()
    {
        List<string> actionsString = actions.Select(a => a.Method.Name).ToList();

        for (int a = 0; a <= 23; a++)
        {
            currData += trialCounter[a].ToString() + "," + actionsString[a].ToString() + "," 
                              + ifcolor[a].ToString() + "," + response[a] + ","
                              + correctness[a].ToString() + "," + time[a].ToString() 
                              + Environment.NewLine
                        ; 
            //trial number, trial name, color, response, correct, RT
        }
        
        Debug.Log(currData);
        allData.Add(currData);
    }

}
    
