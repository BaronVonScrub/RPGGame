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

        public static List<Note> noteList = new List<Note>();
        private static readonly string musicFile = Directory.GetCurrentDirectory() + "\\Tune.dat";
        public static Thread musicThread;
        private static bool stopMusic = false;
        public static void Initialize()
        {
            if (!File.Exists(musicFile))
                ConvertCSV();

            noteList = InterpretDat();

            musicThread = new Thread(new ThreadStart(Play));
            musicThread.Start();
        }

        private static int ToFrequency(int inData) => (int)Math.Round((Math.Pow(2, (inData - 69) / 12.0) * 440));

        public static void Play()
        {
            do
            {
                foreach (Note currNote in noteList)
                {
                    if (stopMusic == true)
                        break;
                    if (Mute == true)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    if (currNote.dur == 0)
                        break;
                    if (currNote.freq != 0)
                        Console.Beep(currNote.freq, currNote.dur);
                    else
                        Thread.Sleep(currNote.dur);
                }
                if (stopMusic == true)
                    break;
            }
            while (true);
        }

        public static void StopMusic() => stopMusic = true;

        private static void ConvertCSV()
        {
            string dataFile = Directory.GetCurrentDirectory() + "\\Tune.csv";
            string[] lines = File.ReadAllLines(dataFile);
            double tempoShift = 1000000 / int.Parse(lines.ToList().Find(x => Regex.Match(x, "Tempo").Success).Split(", ")[3]);
            string firstNote = lines.ToList().Find(x => Regex.Match(x, "(Note_on_c)|(Note_off_c)").Success);
            string firstTrack = firstNote.Split(", ")[0];
            string firstChannel = firstNote.Split(", ")[3];
            string[] noteDataStrings = lines.ToList().FindAll(x => Regex.Match(x, "(Note_on_c)|(Note_off_c)").Success).ToArray();
            var noteDataArray = new NoteData[noteDataStrings.Length];
            for (int i = 0; i < noteDataArray.Length; i++)
                noteDataArray[i] = new NoteData(noteDataStrings[i]);

            int currNote = 0;
            int startTime = 0;

            for (int num = 0; num < noteDataArray.Length - 1; num++)
            {
                if (currNote == 0)                                                                            //If there is currently silence
                {
                    if (noteDataArray[num].velocity == 0 || noteDataArray[num].onoff == "Note_off_c")       //If you detect a break
                        continue;                                                                           //Skip
                    if (noteDataArray[num].time == startTime)                                               //if no time has passed
                        continue;                                                                           //Skip
                                                                                                            //Otherwise
                    noteList.Add(new Note(0, (int)((noteDataArray[num].time - startTime) * tempoShift)));     //Create the silence
                    currNote = noteDataArray[num].note;                                                     //Store the data
                    startTime = noteDataArray[num].time;
                    continue;                                                                               //And continue
                }

                //If there is currently noise
                if (noteDataArray[num].onoff == "Note_on_c" && noteDataArray[num].velocity != 0)            //If you detect more noise
                    continue;                                                                               //Skip
                if (noteDataArray[num].note != currNote)                                                    //If it's not stopping the right note
                    continue;                                                                               //Skip
                                                                                                            //Otherwise
                noteList.Add(new Note(ToFrequency(currNote), (int)((noteDataArray[num].time - startTime) * tempoShift)));                      //Create the note
                currNote = 0;                                                                               //Store the data
                startTime = noteDataArray[num].time;
                continue;                                                                                   //And continue
            }

            using StreamWriter sw = File.CreateText(musicFile);
            foreach (Note note in noteList)
            {
                sw.WriteLine(note.freq);
                sw.WriteLine(note.dur);
                sw.WriteLine();
            }
            sw.Close();
        }

        private static List<Note> InterpretDat()
        {
            var temp = new List<Note>();
            string[] noteData = File.ReadAllLines(musicFile);
            for (int i = 0; i < noteData.Length - 1; i += 3)
                temp.Add(new Note(int.Parse(noteData[i]), int.Parse(noteData[i + 1])));
            return temp;
        }
    }
}

internal struct Note
{
    public int freq;
    public int dur;

    public Note(int freq, int dur)
    {
        if (((freq < 37) && (freq != 0)) || (freq > 32767))
            throw new ArgumentException("Frequency out of bounds!");

        if (dur <= 0)
            throw new ArgumentException("Duration out of bounds!");

        this.freq = freq;
        this.dur = dur;
    }
}

internal struct NoteData
{
    public int track;
    public int time;
    public string onoff;
    public int channel;
    public int note;
    public int velocity;

    public NoteData(string inData)
    {
        string[] data = inData.Split(", ");
        track = int.Parse(data[0]);
        time = int.Parse(data[1]);
        onoff = data[2];
        channel = int.Parse(data[3]);
        note = int.Parse(data[4]);
        velocity = int.Parse(data[5]);
    }
}