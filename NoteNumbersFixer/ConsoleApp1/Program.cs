using System;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    class Program
    {
        private static string fileName = "KickAssBrass_Master.uvip";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int countChanged = 0;
            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            XmlElement root = document.DocumentElement;
            var keyGroups = document.GetElementsByTagName("Keygroup").Cast<XmlElement>();
            Console.WriteLine("Keygroups total: : " + keyGroups.Count());
            foreach (var keyGroup in keyGroups)
            {
                var lowKey = keyGroup.Attributes["LowKey"].InnerText;
                var highKey = keyGroup.Attributes["HighKey"].InnerText;
                var name = keyGroup.Attributes["Name"].InnerText;
                var samplePlayer = keyGroup.SelectSingleNode(".//Oscillators/SamplePlayer");
                var baseNote = samplePlayer.Attributes["BaseNote"].InnerText;
                if (highKey != baseNote)
                {
                    Console.WriteLine(name);
                    Console.WriteLine("lowKey: " + lowKey);
                    Console.WriteLine("highKey: " + highKey);
                    Console.WriteLine("baseNote: " + baseNote);
                    Console.WriteLine("Changing basenote to: " + highKey);
                    samplePlayer.Attributes["BaseNote"].InnerText = highKey;
                    var newBaseNot = samplePlayer.Attributes["BaseNote"].InnerText;
                    Console.WriteLine("New basenote : " + newBaseNot);
                    countChanged++;
                }
            }
            Console.WriteLine("Total Changed: " + countChanged);

            document.Save("KickAssBrass.uvip");
            Console.ReadLine();
        }
    }
}
