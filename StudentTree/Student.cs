using System;
using System.Collections.Generic;
using System.Text;

namespace StudentTree
{
    public class Student
    {
        public StringKey Name { get; private set; }

        private readonly uint r_StudyYear;

        private readonly uint r_Id;

        private readonly float r_Average;

        public StringKey GetKey()
        {
            return this.Name;
        }

        public Student()
        {
        }

	    public Student(StringKey i_Name, uint i_StudyYear, uint i_ID, float i_Average)
        {
            this.Name = i_Name;
            this.r_StudyYear = i_StudyYear;
            this.r_Id = i_ID;
            this.r_Average = i_Average;
        }
        
        public static Student Read()
        {
            uint studyYear;
            uint id;
            float avarage;
            string input = UserInputHelper.AskForNonEmptyString("Please type the student's details.");
            List<string> dataFromUser = splitString(input);
            if (dataFromUser.Count != 4)
            {
                Environment.Exit(1);
            }
            StringKey name = new StringKey(dataFromUser[0]);
            bool parseSuccededStudyYear = uint.TryParse(dataFromUser[1], out studyYear);
            bool parseSuccededId = uint.TryParse(dataFromUser[2], out id);
            bool parseSuccededAvarage = float.TryParse(dataFromUser[3], out avarage);
            if (parseSuccededStudyYear == false || parseSuccededId == false || parseSuccededAvarage == false)
            {
                Console.WriteLine("Invalid input"); 
                Environment.Exit(1);
            }
            return new Student(name, studyYear, id, avarage);
        }

        private static List<string> splitString(string i_Input)
        {
            int i = 0;
            List<string> strings = new List<string>();
            int length = i_Input.Length;
	        while (i < length)
            {
                if (i < length && i_Input[i] != ' ')
                {
                    string tempstring = string.Empty;
                    while (i < length && i_Input[i] != ' ')
                    {
                        tempstring += i_Input[i];
                        i++;
                    }
                    strings.Add(tempstring);
                }
                else
                {
                    i++;
                }
            }
            return strings;
        }

        public void Print()
        {
	        Console.WriteLine(
@"Student Name:{0}
Study Year: {1}
ID:{2}
Avarage:{3}",
		        this.Name.Key,
		        this.r_StudyYear,
		        this.r_Id,
		        this.r_Average);
        }
    }
}
