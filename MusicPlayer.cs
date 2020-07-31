using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using static RPGGame.GlobalVariables;

namespace RPGGame
{
    internal static class MusicPlayer
    {
        #region Fields
        public static List<Note> noteList = new List<Note>();                                                       //This stores the list of notes + silences to be playeed
        private static readonly string musicFile = Directory.GetCurrentDirectory() + "\\Tune.dat";                  //This sets the expected directory of the music file
        public static Thread musicThread;                                                                           //This thread plays the music (to allow independent Thread.Sleep silences)
        private static bool stopMusic = false;                                                                      //This signals when to stop playing the music/end the thread
        public static void StopMusic() => stopMusic = true;                                                         //Pubic trigger for stopping the song+thread
        #endregion

        //This function sets up all that is needed before the game starts.
        public static void Initialize()
        {
            if (!File.Exists(musicFile))                                                                            //If the music file doesn't exist (in the format we need)
                ConvertCSV();                                                                                       //Convert it from the CSV file on hand

            noteList = ImportMusic();                                                                               //Imports the music from the file

            musicThread = new Thread(new ThreadStart(Play));                                                        //Starts a new thread to play the music
            musicThread.Start();                                                                                    //Start the thread
        }

        //This function plays the music!
        public static void Play()
        {
            do                                                                  //Play the song repeatedly (breaks internally)
            {
                foreach (Note currNote in noteList)                             //Play each note in the list
                {
                    if (stopMusic == true)                                      //Check for a stop trigger
                        break;                                                  //Break the loop if stopped
                    if (Mute == true)                                           //If it is muted
                    {
                        Thread.Sleep(100);                                      //Sleep 100ms
                        continue;                                               //And then try again
                    }
                    if (currNote.dur == 0)                                      //If the duration of the note is 0, skip
                        continue;                                               //Try again
                    if (currNote.freq != 0)                                     //If the frequency is not listed as zero (freq zero means silence)
                        Console.Beep(currNote.freq, currNote.dur);              //Play the note via Beep
                    else
                        Thread.Sleep(currNote.dur);                             //Otherwise play the silence
                }
                if (stopMusic == true)                                          //If the song ends or the loop is broken, and stopmusic is true
                    break;                                                      //End the loop (and by extension, the thread)
            }
            while (true);
        }

        /*This function - all written by me - converts a CSV file as formatted by MIDICSV by John Walker at http://www.fourmilab.ch/webtools/midicsv/
        to a file playable by the this class! The midi must be monophonic, situated on one track and channel - all else is ignored!
        Also note that MIDIs run by on-off triggers, and both Sleep and Beep require a length specified at the start, so these triggers must be
        converted into a list of durations instead.*/
        private static void ConvertCSV()
        {
            #region Read and parse CSV, setup intials
            string dataFile = Directory.GetCurrentDirectory() + "\\Tune.csv";                                                       //Sets expected csv directory
            string[] lines = File.ReadAllLines(dataFile);                                                                           //Reads the lines into file
            double tempoShift = 1000000 / int.Parse(lines.ToList().Find(x => Regex.Match(x, "Tempo").Success).Split(", ")[3]);      //Find and parse the first line that sets tempo in a file
            string firstNote = lines.ToList().Find(x => Regex.Match(x, "(Note_on_c)|(Note_off_c)").Success);                        //Finds the first note on/off command
            string firstTrack = firstNote.Split(", ")[0];                                                                           //Finds the first track mentioned
            string firstChannel = firstNote.Split(", ")[3];                                                                         //Finds the first channel mentioned
            string[] noteDataStrings = lines.ToList().FindAll(x => Regex.Match(x, "(Note_on_c)|(Note_off_c)").Success).ToArray();   //Reads all note on or off commands into an array
            var noteDataArray = new NoteData[noteDataStrings.Length];                                                               //Creates an array of noteData structs
            for (int i = 0; i < noteDataArray.Length; i++)                                                                          //For each note slot prepared
                noteDataArray[i] = new NoteData(noteDataStrings[i]);                                                                //Insert a new noteData from the data

            int currNote = 0;                                                                                                       //Initialize the current note as silence
            int startTime = 0;                                                                                                      //Initialize the start time
            #endregion

            #region Find and convert CSV event data to notes
            for (int num = 0; num < noteDataArray.Length - 1; num++)                                                                //For each noteData stored
            {
                if (currNote == 0)                                                                                                  //If there is currently silence
                {
                    if (noteDataArray[num].velocity == 0 || noteDataArray[num].onoff == "Note_off_c")                               //If you detect a break
                        continue;                                                                                                       //Skip
                    if (noteDataArray[num].time == startTime)                                                                       //If no time has passed
                        continue;                                                                                                       //Skip
                                                                                                                                    //Otherwise
                    noteList.Add(new Note(0, (int)((noteDataArray[num].time - startTime) * tempoShift)));                           //Create the silence
                    currNote = noteDataArray[num].note;                                                                             //Store the data
                    startTime = noteDataArray[num].time;
                    continue;                                                                                                       //And move on to the next
                }

                //If there is currently noise
                if (noteDataArray[num].onoff == "Note_on_c" && noteDataArray[num].velocity != 0)                                    //If you detect more noise
                    continue;                                                                                                           //Skip
                if (noteDataArray[num].note != currNote)                                                                            //If it's not stopping the right note
                    continue;                                                                                                           //Skip
                                                                                                                                    //Otherwise
                noteList.Add(new Note(ToFrequency(currNote), (int)((noteDataArray[num].time - startTime) * tempoShift)));           //Create the note
                currNote = 0;                                                                                                       //Store the data
                startTime = noteDataArray[num].time;
                continue;                                                                                                           //And move on to the next
            }
            #endregion

            #region Write note data to file
            using StreamWriter sw = File.CreateText(musicFile);                                                                     //Start writing to file
            foreach (Note note in noteList)                                                                                         //For each note
            {
                sw.WriteLine(note.freq);                                                                                            //Store the frequency
                sw.WriteLine(note.dur);                                                                                             //Store the length
                sw.WriteLine();                                                                                                     //Add a whitespace line (For readability)
            }
            sw.Close();                                                                                                             //Close it
            #endregion
        }

        /*This method reads the custom music file into memory, returning a list of the notes*/
        private static List<Note> ImportMusic()
        {
            var temp = new List<Note>();                                                                                            //Initialize list
            string[] noteData = File.ReadAllLines(musicFile);                                                                       //Read the lines into file
            for (int i = 0; i < noteData.Length - 1; i += 3)                                                                        //For each note
                temp.Add(new Note(int.Parse(noteData[i]), int.Parse(noteData[i + 1])));                                             //Create it from the data
            return temp;                                                                                                            //Return the finalised list
        }

        //This function converts a MIDI format note to a sound frequency for use with Beep
        private static int ToFrequency(int inData) => (int)Math.Round((Math.Pow(2, (inData - 69) / 12.0) * 440));
    }

}

/* This struct allows easy storage and referal of a note or silence */
internal struct Note
{
    public int freq;                                                                                                                //Frequency (0 is silence)
    public int dur;                                                                                                                 //Duration

    public Note(int freq, int dur)                                                                                                  //Constructor accepts two ints as defined above
    {
        if (((freq < 37) && (freq != 0)) || (freq > 32767))                                                                         //Checks that the note is playable - in bounds, or silence.
            throw new ArgumentException("Frequency out of bounds!");                                                                //Reports otherwise

        if (dur <= 0)                                                                                                               //If the duration is zero (shouldn't be possible)
            throw new ArgumentException("Duration out of bounds!");                                                                 //Report the error

        this.freq = freq;                                                                                                           //Assigns the frequency
        this.dur = dur;                                                                                                             //Assigns the duration
    }
}

/* This struct allows easy creation of and referal to a recieved MIDI trigger event */
internal struct NoteData
{
    public int track;                                                                                                               //What track is it on?
    public int time;                                                                                                                //What time is the trigger?
    public string onoff;                                                                                                            //Is it turning something on or off?
    public int channel;                                                                                                             //What channel is it on?
    public int note;                                                                                                                //What MIDI note does it play?
    public int velocity;                                                                                                            //What volume does it play at?

    public NoteData(string inData)                                                                                                  //Constructor takes in a string and then parses and assigns.
    {
        #region Parsing
        string[] data = inData.Split(", ");
        track = int.Parse(data[0]);
        time = int.Parse(data[1]);
        onoff = data[2];
        channel = int.Parse(data[3]);
        note = int.Parse(data[4]);
        velocity = int.Parse(data[5]);
        #endregion
    }
}