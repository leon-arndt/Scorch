﻿// This code automatically generated by TableCodeGen
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Subtitles : MonoBehaviour
{
    //Created with CSV2Table asset from asset store -> Gets Data from CSV Files
    #region
    public class Row
    {
        public string ID;
        public string Name;
        public string Person;
        public string Sentence;
        public string Time;
    }
    #endregion

    List<Row> rowList = new List<Row>();
    bool isLoaded = false;

    //getter by other classes
    List<string> ids;
    List<string> names;
    public List<string> sentences;
    public List<float> times;
    [SerializeField]
    public TextAsset engSub;
    [SerializeField]
    public TextAsset gerSub;

    private int m_convertlanguage;
    string dialoguename;

    private void Start()
    {
        ids = new List<string>();
        names = new List<string>();
        sentences = new List<string>();
        times = new List<float>();


        m_convertlanguage = PlayerPrefs.GetInt("language"); //Gets the right Language from Playerprefs and loads the according language
        SetLanguage(m_convertlanguage); // For loading according language

    }

    //Code created with CSV2Table asset from asset store -> Gets Data from CSV Files
    #region 
    public bool IsLoaded()
    {
        return isLoaded;
    }

    public List<Row> GetRowList()
    {
        return rowList;
    }

    public void Load(TextAsset csv)
    {
        rowList.Clear();
        string[][] grid = CsvParser2.Parse(csv.text);
        for (int i = 1; i < grid.Length; i++)
        {
            Row row = new Row();
            row.ID = grid[i][0];
            row.Name = grid[i][1];
            row.Person = grid[i][2];
            row.Sentence = grid[i][3];
            row.Time = grid[i][4];

            rowList.Add(row);
        }
        isLoaded = true;
    }

    public int NumRows()
    {
        return rowList.Count;
    }

    public Row GetAt(int i)
    {
        if (rowList.Count <= i)
            return null;
        return rowList[i];
    }

    public Row Find_ID(string find)
    {
        return rowList.Find(x => x.ID == find);
    }
    public List<Row> FindAll_ID(string find)
    {
        return rowList.FindAll(x => x.ID == find);
    }
    public Row Find_Name(string find)
    {
        return rowList.Find(x => x.Name == find);
    }
    public List<Row> FindAll_Name(string find)
    {
        return rowList.FindAll(x => x.Name == find);
    }
    public Row Find_Person(string find)
    {
        return rowList.Find(x => x.Person == find);
    }
    public List<Row> FindAll_Person(string find)
    {
        return rowList.FindAll(x => x.Person == find);
    }
    public Row Find_Sentence(string find)
    {
        return rowList.Find(x => x.Sentence == find);
    }
    public List<Row> FindAll_Sentence(string find)
    {
        return rowList.FindAll(x => x.Sentence == find);
    }
    public Row Find_Time(string find)
    {
        return rowList.Find(x => x.Time == find);
    }
    public List<Row> FindAll_Time(string find)
    {
        return rowList.FindAll(x => x.Time == find);
    }
    #endregion

    public List<string> AllSentences(string findname)
    {
        foreach (Row row in FindAll_Name(findname))
        {
            sentences.Add(row.Sentence);
        }
        return sentences;
    }

    public List<float> AllTimes(string findname) 
    {
        foreach (Row row in FindAll_Name(findname))
        {
            times.Add(float.Parse(row.Time));
        }
        return times;
    }

    public int StringToIntConverter(string findName) // For conversation Class, Convert Calls into numbers
    {
        int number = 1;
        if (findName == "Introdialogue") number = 0;
        else if (findName == "Autopsyreport") number = 1;
        else if (findName == "Alcoholproblem") number = 2;
        else if (findName == "PickedUpImpatient") number = 3;
        else if (findName == "NotPickedUpImpatient") number = 4;

        return number;
    }
 
    public string IntToStringConverter(int number) // For conversation Class, Convert Numbers into Calls 
    {
        if (number == 0) dialoguename = "Introdialogue";
        else if (number == 1) dialoguename = "Autopsyreport";
        else if (number == 2) dialoguename = "Alcoholproblem";
        else if (number == 3) dialoguename = "PickedUpImpatient";
        else if (number == 4) dialoguename = "NotPickedUpImpatient";

        return dialoguename;
    }

    public void SetLanguage(int language) // 0= none; default english; 1= english; 2 = german // more can be added
    {
        if (language == 1)
        { Load(engSub); }
        else if (language == 2) { Load(gerSub); }
        else
        {
            Load(engSub);
        }
    }

}