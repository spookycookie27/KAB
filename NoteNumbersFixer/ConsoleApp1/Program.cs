using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    class Program
    {
        private static string fileName = "KickAssBrass_master.uvip";
        private static Dictionary<string, string> _noteMap;
        private static string[] problems = new[]
        {
            "TNR SW5 C#5C", "TPT FF B4 +", "TPT FF D5+", "TPT FF F5+", "TPT FF G5+", "TPT FF A#5+", "TPT PP F3B"
        };

        static void Main(string[] args)
        {
            _noteMap = GetNoteMapsDictionary();
            Console.WriteLine("Hello World!");
            var transposeUp = new[]
            {
                "TromboneFullProgram",
                "TromboneLongStabs",
                "TromboneShortStabs",
                "TromboneMixedStabs",
                "TromboneLowStabs",
                "TromboneLowLongStabs",
                "TromboneLowSHortStabs",
                "TromboneLowFalseHarmonic"
            };
            var transposeDown = new[]
            {
                "TromboneSwells1",
                "TromboneSwells2",
                "TromboneSwells3",
                "TromboneSwells4",
                "TromboneSwells5",
                "TenorSaxExpression",
                "TenorSaxFullProgram",
                "TenorSaxFullProgramV2",
                "TenorSaxSlurred",
                "TenorSaxLongStabs",
                "TenorSaxShortStabs",
                "TenorSaxMixedStabs",
                "TenorSaxVibrato",
                "TrumpetEnds",
                "TrumpetExpression",
                "TrumpetLongFalls",
                "TrumpetShortFalls",
                "TrumpetRips+Falls",
                "TrumpetShortRips",
                "TrumpetLongRips",
                "TrumpetLongRips",
                "TrumpetFullProgram",
                "TrumpetHard",
                "TrumpetSoft",
                "TrumpetFullProgramV2",
                "TrumpetSlurred",
                "TrumpetSlides",
                "TrumpetShortStabs",
                "TrumpetLongStabs",
                "TrumpetMixedStabs",
                "TrumpetSwells1",
                "TrumpetSwells2",
                "TrumpetSwells3",
                "TrumpetSwells4",
                "TrumpetSwells5",
                "TrumpetVibrato",
                "TrumpetTrills",
                "TrumpetFullProgramV2"
            };
            var lowerRange5Octaves = new[]
            {
                "TenorSaxFullProgram",
                "TenorSaxFullProgramV2",
                "TrumpetFullProgramV2"
            };
            var countChanged = 0;
            
            var document = new XmlDocument();
            document.Load(fileName);
            var root = document.DocumentElement;
            var totalKeyGroups = root.GetElementsByTagName("Keygroup").Cast<XmlElement>().ToList();
            Console.WriteLine("totalKeyGroups: " + totalKeyGroups.Count);
            var layers = document.GetElementsByTagName("Layer").Cast<XmlElement>().ToList();
            foreach (var layer in layers)
            {
                var layerDisplayName = layer.Attributes["DisplayName"].InnerText;


                var keyGroups = layer.GetElementsByTagName("Keygroup").Cast<XmlElement>().ToList();
                //Console.WriteLine($"Layer {layerDisplayName} KeyGroups total: : {keyGroups.Count()}");
                foreach (var keyGroup in keyGroups)
                {
                    var lowerRangeOffset = 48;
                    var noteNumberOffset = 0;
                    if (transposeUp.Contains(layerDisplayName))
                    {
                        noteNumberOffset = 12;
                    }
                    if (transposeDown.Contains(layerDisplayName))
                    {
                        noteNumberOffset = -12;
                    }
                    if (lowerRange5Octaves.Contains(layerDisplayName))
                    {
                        lowerRangeOffset = lowerRangeOffset + 12;
                    }
                    var sampleFileName = keyGroup.Attributes["DisplayName"].InnerText;
                    var lowKey = Convert.ToInt32(keyGroup.Attributes["LowKey"].InnerText);
                    //var highKey = Convert.ToInt32(keyGroup.Attributes["LowKey"].InnerText);
                    var sampleName = sampleFileName.Substring(0, sampleFileName.Length - 5);
                    var keyGroupName = keyGroup.Attributes["DisplayName"].InnerText;
                    var samplePlayerNode = keyGroup.SelectSingleNode(".//Oscillators/SamplePlayer");
                    var baseNote = Convert.ToInt32(samplePlayerNode?.Attributes?["BaseNote"].InnerText);
                    //var calcdNoteNumber = GetNoteNumber(sampleName);
                    var noteNumber = GetNoteNumber(sampleName);
                    var diffBetweenNoteNumberAndLowKey = FindDifference(noteNumber, lowKey);
                    var isLowerRange = diffBetweenNoteNumberAndLowKey > 24;
                    if (isLowerRange)
                    {
                        noteNumber = noteNumber - lowerRangeOffset;
                    }
                    else
                    {
                        noteNumber = noteNumber + noteNumberOffset;
                    }
                    //var diffBetweenNoteNumberAndBaseKey = FindDifference(noteNumber, baseNote);
                    //if (diffBetweenNoteNumberAndBaseKey > 8)
                    //{
                    //    Console.WriteLine(layerDisplayName);
                    //}
                    if (baseNote == noteNumber || samplePlayerNode == null) continue;

                    Console.WriteLine($"Changing {layerDisplayName} -  {keyGroupName} basenote from: {baseNote} to {noteNumber}");
                    if (samplePlayerNode.Attributes != null)
                    {
                        samplePlayerNode.Attributes["BaseNote"].InnerText = noteNumber.ToString();
                    }
                    countChanged++;
                }
            }

            Console.WriteLine("Total Changed: " + countChanged);

            document.Save("KickAssBrass.uvip");
        }

        private static decimal FindDifference(int nr1, int nr2)
        {
            return Math.Abs(nr1 - nr2);
        }

        private static string GetKey(string sampleName)
        {
            var nameForNoteNumber = "";
            var key = "";
            if (problems.Contains(sampleName) || sampleName.EndsWith("L"))
            {
                nameForNoteNumber = sampleName.TrimEnd(sampleName[^1]).Trim();
            }
            else
            {
                nameForNoteNumber = sampleName.Trim();
            }

            key = nameForNoteNumber.Substring(nameForNoteNumber.Length - 3).Replace(" ", "");
            return key;
        }

        private static int GetNoteNumber(string sampleName)
        {
            var key = GetKey(sampleName);
            return Convert.ToInt16(_noteMap[key.ToUpper()]);
        }

        private Dictionary<string, string> noteNumbers = new Dictionary<string, string>
        {
            {"0", "C-1"},
            {"1", "C#-1"},
            {"2", "D-1"},
            {"3", "D#-1"},
            {"4", "E-1"},
            {"5", "F-1"},
            {"6", "F#-1"},
            {"7", "G-1"},
            {"8", "G#-1"},
            {"9", "A-1"},
            {"10", "A#-1"},
            {"11", "B-1"},
            {"12", "C0"},
            {"13", "C#0"},
            {"14", "D0"},
            {"15", "D#0"},
            {"16", "E0"},
            {"17", "F0"},
            {"18", "F#0"},
            {"19", "G0"},
            {"20", "G#0"},
            {"21", "A0"},
            {"22", "A#0"},
            {"23", "B0"},
            {"24", "C1"},
            {"25", "C#1"},
            {"26", "D1"},
            {"27", "D#1"},
            {"28", "E1"},
            {"29", "F1"},
            {"30", "F#1"},
            {"31", "G1"},
            {"32", "G#1"},
            {"33", "A1"},
            {"34", "A#1"},
            {"35", "B1"},
            {"36", "C2"},
            {"37", "C#2"},
            {"38", "D2"},
            {"39", "D#2"},
            {"40", "E2"},
            {"41", "F2"},
            {"42", "F#2"},
            {"43", "G2"},
            {"44", "G#2"},
            {"45", "A2"},
            {"46", "A#2"},
            {"47", "B2"},
            {"48", "C3"},
            {"49", "C#3"},
            {"50", "D3"},
            {"51", "D#3"},
            {"52", "E3"},
            {"53", "F3"},
            {"54", "F#3"},
            {"55", "G3"},
            {"56", "G#3"},
            {"57", "A3"},
            {"58", "A#3"},
            {"59", "B3"},
            {"60", "C4"},
            {"61", "C#4"},
            {"62", "D4"},
            {"63", "D#4"},
            {"64", "E4"},
            {"65", "F4"},
            {"66", "F#4"},
            {"67", "G4"},
            {"68", "G#4"},
            {"69", "A4"},
            {"70", "A#4"},
            {"71", "B4"},
            {"72", "C5"},
            {"73", "C#5"},
            {"74", "D5"},
            {"75", "D#5"},
            {"76", "E5"},
            {"77", "F5"},
            {"78", "F#5"},
            {"79", "G5"},
            {"80", "G#5"},
            {"81", "A5"},
            {"82", "A#5"},
            {"83", "B5"},
            {"84", "C6"},
            {"85", "C#6"},
            {"86", "D6"},
            {"87", "D#6"},
            {"88", "E6"},
            {"89", "F6"},
            {"90", "F#6"},
            {"91", "G6"},
            {"92", "G#6"},
            {"93", "A6"},
            {"94", "A#6"},
            {"95", "B6"},
            {"96", "C7"},
            {"97", "C#7"},
            {"98", "D7"},
            {"99", "D#7"},
            {"100", "E7"},
            {"101", "F7"},
            {"102", "F#7"},
            {"103", "G7"},
            {"104", "G#7"},
            {"105", "A7"},
            {"106", "A#7"},
            {"107", "B7"}
        };

        private static Dictionary<string, string> GetNoteMapsDictionary()
        {
            return new Dictionary<string, string>
            {
                {"F0", "29"},
                {"F#0", "30"},
                {"G0", "31"},
                {"G#0", "32"},
                {"A0", "33"},
                {"A#0", "34"},
                {"B0", "35"},
                {"C1", "36"},
                {"C#1", "37"},
                {"D1", "38"},
                {"D#1", "39"},
                {"E1", "40"},
                {"F1", "41"},
                {"F#1", "42"},
                {"G1", "43"},
                {"G#1", "44"},
                {"A1", "45"},
                {"A#1", "46"},
                {"B1", "47"},
                {"C2", "48"},
                {"C#2", "49"},
                {"D2", "50"},
                {"D#2", "51"},
                {"E2", "52"},
                {"F2", "53"},
                {"F#2", "54"},
                {"G2", "55"},
                {"G#2", "56"},
                {"A2", "57"},
                {"A#2", "58"},
                {"B2", "59"},
                {"C3", "60"},
                {"C#3", "61"},
                {"D3", "62"},
                {"D#3", "63"},
                {"E3", "64"},
                {"F3", "65"},
                {"F#3", "66"},
                {"G3", "67"},
                {"G#3", "68"},
                {"A3", "69"},
                {"A#3", "70"},
                {"B3", "71"},
                {"C4", "72"},
                {"C#4", "73"},
                {"D4", "74"},
                {"D#4", "75"},
                {"E4", "76"},
                {"F4", "77"},
                {"F#4", "78"},
                {"G4", "79"},
                {"G#4", "80"},
                {"A4", "81"},
                {"A#4", "82"},
                {"B4", "83"},
                {"C5", "84"},
                {"C#5", "85"},
                {"D5", "86"},
                {"D#5", "87"},
                {"E5", "88"},
                {"F5", "89"},
                {"F#5", "90"},
                {"G5", "91"},
                {"G#5", "92"},
                {"A5", "93"},
                {"A#5", "94"},
                {"B5", "95"},
                {"C6", "96"},
                {"C6B", "96"},
                {"C#6", "97"},
                {"D6", "98"},
                {"D#6", "99"},
                {"E6", "100"},
                {"F6", "101"},
                {"F#6", "102"},
                {"G6", "103"},
                {"G#6", "104"},
                {"A6", "105"},
                {"A#6", "106"}
            };
        }
    }
}
