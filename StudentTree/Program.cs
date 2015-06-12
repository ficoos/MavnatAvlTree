using System;
using System.Collections.Generic;
using System.Text;

namespace StudentTree
{
	using System.Security.Permissions;

	public class Program
	{

        public static void Main()
        {
			AvlTree<StringKey, Student> dictionary = new AvlTree<StringKey, Student>();
            Student currStudent;
            List<string> Names = new List<string>();
            uint numOfStudentsToFind;
            uint numOfStudentsToInsert = UserInputHelper.GetUintFromUser("How many students do you want to add?");

            for (int i = 0; i < numOfStudentsToInsert; i++)
            {
                currStudent = Student.Read();
                dictionary.Insert(currStudent.Name, currStudent);
            }
            numOfStudentsToFind = UserInputHelper.GetUintFromUser("How many names do you want to search?");
            for (int i = 0; i < numOfStudentsToFind; i++)
            {
                Names.Add(UserInputHelper.AskForNonEmptyString("type a name."));
            }
            foreach (string name in Names)
            {
				if (dictionary.Find(new StringKey(name), out currStudent))
                {
                    currStudent.Print();
                }

            }
            Console.ReadKey();
        }
	}
}
