using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Question : MonoBehaviour
{
    private enum QuestionType { onePiece, twoPieces, threePieces, inputString };

    [SerializeField]
    private QuestionType questionType;

    [SerializeField]
    private List<Evidence> correctEvidenceList;
    [SerializeField]
    private string questionText;
    //[SerializeField]
    //private int numberOfQuestions;

    [SerializeField]
    private AudioClip[] audioClips;
    public AudioClip[] reaction;

    private int score;
    private Questions questions;
    [SerializeField]
    private int questionNumber;

    //Subtitles

    [SerializeField]
    private Subtitles subs;
    [SerializeField]
    private List<string> reactionSubtitle, questionSubtitle = new List<string>();
    public List<float> questionSubtitleTime = new List<float>();
    public Queue<string> conversation, sentences = new Queue<string>();
    int sentenceIterator;

    public void Start()
    {

        questions = GetComponentInParent<Questions>();
        subs = FindObjectOfType<Subtitles>();

        conversation = new Queue<string>();
        sentences = new Queue<string>();

        Invoke("Subs", 0.1f); //Offset needed because otherwise subtitles won't load
    }

    private IEnumerable OldQuestionSubtitleCoroutine;

    public void PlayQuestion(int i)
    {
        StartCoroutine(questions.SetClip(i));
        StartCoroutine(GetNextLine());
    }
    public bool RequiresInput()
    {
        return questionType == QuestionType.inputString;
    }
    //UI
    public string GetQuestionName()
    {
        return questionText;
    }
    public List<Evidence> GetCorrectEvidenceList()
    {
        return correctEvidenceList;
    }
    public AudioClip[] GetClip()
    {
        return audioClips;
    }
    //Retrieves the score of the question, 0 = false, 1 = correct, 2 = perfect
    public int GetQuestionScore()
    {
        return score;
    }

    //Sets the score of the question, 0 = false, 1 = correct, 2 = perfect
    public void SetQuestionScore(int i)
    {
        StopCoroutine(questions.PlayReaction(questionNumber - 1, score));
        score = i;
        StartCoroutine(questions.PlayReaction(questionNumber, score));
    }


    //Get the number of answers needed for the question
    public int GetNumOfRequiredAnswers()
    {
        if (questionType == QuestionType.twoPieces)
        {
            return 2;
        }
        else if (questionType == QuestionType.threePieces)
        {
            return 3;
        }
        else
        {
            return -1;
        }
    }



    private void Subs()
    {

        foreach (string sub in subs.AllSentences("Question" + (questionNumber + 1)))
        {
            questionSubtitle.Add(sub);
        }
        subs.sentences.Clear();// needs to be done every time, so the sentences before won't added again
        foreach (float sub in subs.AllTimes("Question" + (questionNumber + 1)))
        {
            questionSubtitleTime.Add(sub);
        }
        subs.times.Clear();// needs to be done every time, so the sentences before won't added again

        foreach (string reactionSub in subs.AllSentences("Question" + (questionNumber + 1).ToString() + "reaction"))
        {
            reactionSubtitle.Add(reactionSub);
        }
        subs.sentences.Clear();// needs to be done every time, so the sentences before won't added again
    }

    public string GetReactionSubtitle(int score)
    {

        return reactionSubtitle[score];
    }


    public IEnumerator GetNextLine()
    { //set sentence Iterator to 0, because each converstion starts with sentence 0 
        sentenceIterator = 0;

        foreach (string sentence in questionSubtitle)
        {
            try
            {
                conversation.Enqueue(sentence);
                DisplayNextSentence();
            }
            catch (NullReferenceException ex) { Debug.Log("Check if sentences available"); }
            yield return null; //If this Coroutine is stopped
            yield return new WaitForSeconds(questionSubtitleTime[sentenceIterator]); //wait a special amount of time until next line is shown
            //yield return new WaitForSeconds(questions.mainVoiceSource.clip.length);
            sentenceIterator++;
        }

        ResetSubtitles(); //Reset Subtitles

    }
    private void DisplayNextSentence()
    {
        if (conversation.Count == 0)
        {
            ResetSubtitles();
            return;
        }
        string sentence = conversation.Dequeue();
        questions.GetSubtitlesTextbox().text = sentence;
    }

    public void ResetSubtitles()
    {
        questions.GetSubtitlesTextbox().text = null;
    }



}


