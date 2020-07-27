using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Linq;
using static RPGGame.GlobalVariables;

namespace RPGGame
{
    static class MusicPlayer
    {

        public static List<Note> noteList = new List<Note>();
        private static readonly string musicFile = Directory.GetCurrentDirectory() + "\\Tune.dat";
        public static Thread musicThread;
        private static Boolean stopMusic = false;
        public static void Initialize()
        {
            if (!File.Exists(musicFile))
                ConvertCSV();

            noteList = InterpretDat();

            musicThread = new Thread(new ThreadStart(Play));
            musicThread.Start();
        }

        private static int ToFrequency(int inData)
        {
            return (int)Math.Round((Math.Pow(2, (inData - 69) / 12.0) * 440));
        }

        public static void Play()
        {
            do {
                foreach (Note currNote in noteList)
                {
                    if (stopMusic == true)
                        break;
                    if (Mute == true){
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

        public static void StopMusic()
        {
            stopMusic=true;
        }

        private static void ConvertCSV()
        {
            string dataFile = Directory.GetCurrentDirectory() + "\\Tune.csv";
            String[] lines = File.ReadAllLines(dataFile);
            double tempoShift = 1000000/Int32.Parse(lines.ToList().Find(x => Regex.Match(x, "Tempo").Success).Split(", ")[3]);
            string firstNote = lines.ToList().Find(x => Regex.Match(x, "(Note_on_c)|(Note_off_c)").Success);
            string firstTrack = firstNote.Split(", ")[0];
            string firstChannel = firstNote.Split(", ")[3];
            String[] noteDataStrings = lines.ToList().FindAll(x => Regex.Match(x, "(Note_on_c)|(Note_off_c)").Success).ToArray();
            NoteData[] noteDataArray = new NoteData[noteDataStrings.Length];
            for (int i = 0; i < noteDataArray.Length; i++)
                noteDataArray[i] = new NoteData(noteDataStrings[i]);

            int currNote = 0;
            int startTime = 0;

            for (int num=0; num < noteDataArray.Length - 1; num++)
            {
                if (currNote==0)                                                                            //If there is currently silence
                {
                    if (noteDataArray[num].velocity == 0 || noteDataArray[num].onoff == "Note_off_c")       //If you detect a break
                        continue;                                                                           //Skip
                    if (noteDataArray[num].time == startTime)                                               //if no time has passed
                        continue;                                                                           //Skip
                                                                                                            //Otherwise
                    noteList.Add(new Note(0, (int)((noteDataArray[num].time - startTime)*tempoShift)));                         //Create the silence
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
                noteList.Add(new Note(ToFrequency(currNote), (int)((noteDataArray[num].time - startTime)*tempoShift)));                      //Create the note
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

        private static List<Note> InterpretDat(){
            List<Note> temp = new List<Note>();
            String[] noteData = File.ReadAllLines(musicFile);
            for (int i = 0; i < noteData.Length - 1; i += 3)
                temp.Add(new Note(Int32.Parse(noteData[i]), Int32.Parse(noteData[i+1])));
            return temp;
        }
}
}

struct Note
{
    public int freq;
    public int dur;

    public Note(int freq, int dur)
    {
        this.freq = freq;
        this.dur = dur;
    }
}

struct NoteData
{
    public int track;
    public int time;
    public string onoff;
    public int channel;
    public int note;
    public int velocity;

    public NoteData(string inData)
    {
        String[] data = inData.Split(", ");
        track = Int32.Parse(data[0]);
        time = Int32.Parse(data[1]);
        onoff = data[2];
        channel = Int32.Parse(data[3]);
        note = Int32.Parse(data[4]);
        velocity = Int32.Parse(data[5]);
    }
}