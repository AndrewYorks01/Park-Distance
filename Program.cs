using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace Park_Distance
{
    public class ParkInfo
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Region { get; set; }
        public int RegID { get; set; }

    }

    public class Operations
    {
        // Return distance in miles between two points
        public static int Distance(int x1, int x2, int y1, int y2)
        {
            double val = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) * 30;
            return (int)Math.Round(val, 0);
        }


    }

    class Program
    {
        public static bool Matches(int number, String region)
        {
            if (number == 1 && region == "South")
                return true;
            else if (number == 2 && region == "Northeast")
                return true;
            else if (number == 3 && region == "Midwest")
                return true;
            else if (number == 4 && region == "West")
                return true;
            else
                return false;
        }

        static void Main(string[] args)
        {
            string direc = Directory.GetCurrentDirectory();
            string delete = "bin-debug-net5.0";
            direc = direc.Substring(0, direc.Length - (delete.Length));
            //Console.WriteLine(direc);

            Console.WriteLine("PARK DISTANCE");

            Console.WriteLine("1. South");
            Console.WriteLine("2. Northeast");
            Console.WriteLine("3. Midwest");
            Console.WriteLine("4. West");
            int number = 0;
            while ( (number < 1) || (number > 4) )
            {
                Console.Write("In which region is your baseline park?: ");
                string input = Console.ReadLine();
                Int32.TryParse(input, out number);
            }
            //Console.WriteLine("number = " + number);
            string regionName;
            if (number == 1)
                regionName = "South";
            else if (number == 2)
                regionName = "Northeast";
            else if (number == 3)
                regionName = "Midwest";
            else
                regionName = "West";

            string fileSearch = direc + "XML\\parks.xml";

            if (File.Exists(fileSearch)){
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("The XML file parks.xml is missing.");
                System.Environment.Exit(1);
            }

            // Load the XML list
            XmlDocument doc = new XmlDocument();
            doc.Load(fileSearch);
            List<ParkInfo> Parks = new List<ParkInfo>();
            XmlNodeList nodeList;
            nodeList = doc.SelectNodes("/ParkData/Park");
            foreach (XmlNode node in nodeList)
            {
                ParkInfo myPark = new ParkInfo();
                foreach (XmlNode child in node.ChildNodes)
                {
                    // Add child nodes to myPark
                    if (child.Name == "Name")
                        myPark.Name = child.InnerText;
                    else if (child.Name == "City")
                        myPark.City = child.InnerText;
                    else if (child.Name == "State")
                        myPark.State = child.InnerText;
                    else if (child.Name == "X")
                        myPark.X = Convert.ToInt32(child.InnerText);
                    else if (child.Name == "Y")
                        myPark.Y = Convert.ToInt32(child.InnerText);
                    else if (child.Name == "Region")
                        myPark.Region = child.InnerText;
                    else if (child.Name == "RegID")
                        myPark.RegID = Convert.ToInt32(child.InnerText);

                } // end foreach for a single location

                // Add myPark to the main Parks list
                Parks.Add(myPark);

            } // end foreach for the entire list

            int maximum = 0;

            // Display all the parks that can be selected
            foreach (var parks in Parks)
            {
                if (Matches(number, parks.Region))
                {
                    if ((parks.RegID % 2 == 1) || (parks.RegID == 1))
                        Console.Write(parks.RegID + ". " + parks.Name + "   ");
                    else
                        Console.WriteLine(" " + parks.RegID + ". " + parks.Name);
                    maximum += 1;
                }
            }

            Console.WriteLine();
            // Ask for the user's baseline park
            int parkChoice = 0;
            while ((parkChoice < 1) || (parkChoice > maximum))
            {
                Console.Write("Which is your baseline park?: ");
                string input = Console.ReadLine();
                Int32.TryParse(input, out parkChoice);
            }

            // Determine the baseline park using the given information
            ParkInfo baseline = Parks.Find(x => (x.RegID == parkChoice) && (x.Region == regionName) );
            //Console.WriteLine(baseline.Name);

            int radius = 29;
            while ( (radius < 30))
            {
                Console.Write("Look at parks within how many miles (at least 30)?: ");
                string inp = Console.ReadLine();
                Int32.TryParse(inp, out radius);
            }

            
            Console.WriteLine();
            Console.WriteLine("Here are parks within " + radius + " miles:");
            for (int dis = 0; dis <= radius; dis++)
            {
                foreach (var parks in Parks)
                {
                    int theDist = Operations.Distance(parks.X, baseline.X, parks.Y, baseline.Y);
                    if (theDist == dis)
                    {
                        // Display parks within the radius, making sure not to show the baseline park.
                        if ((theDist < 30) && (parks.Name != baseline.Name))
                            Console.WriteLine("{0, -45} {1, -1}, {2, -1} - <30 mi", parks.Name, parks.City, parks.State);
                        else if (parks.Name != baseline.Name)
                            Console.WriteLine("{0, -45} {1, -1}, {2, -1} - {3} mi", parks.Name, parks.City, parks.State, theDist);
                    }
                } // end foreach
            } // end for

            // Output the same information to an output file
            using (StreamWriter outFile = new StreamWriter(Path.Combine(direc, "output.txt")))
            {
                outFile.WriteLine("Parks within " + radius + " miles of " + baseline.Name);
                for (int dis = 0; dis <= radius; dis++)
                {
                    foreach (var parks in Parks)
                    {
                        int theDist = Operations.Distance(parks.X, baseline.X, parks.Y, baseline.Y);
                        if (theDist == dis)
                        {
                            // Display parks within the radius, making sure not to show the baseline park.
                            if ((theDist < 30) && (parks.Name != baseline.Name))
                                outFile.WriteLine("{0, -45} {1, -1}, {2, -1} - <30 mi", parks.Name, parks.City, parks.State);
                            else if (parks.Name != baseline.Name)
                                outFile.WriteLine("{0, -45} {1, -1}, {2, -1} - {3} mi", parks.Name, parks.City, parks.State, theDist);
                        }
                    } // end foreach
                } // end for
            }

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        } // end of Main
    }
}
