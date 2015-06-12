using System;

namespace StudentTree
{
	using System.Collections.Generic;
	using System.ComponentModel;

	internal class UserInputHelper
	{
		public static bool AskYesNoQuestion(string i_Question)
		{
			const string v_YesAnswer = "y";
			const string v_NoAnswer = "n";
			bool? result = null;

			while (!result.HasValue)
			{
				Console.Write("{2} ({0}/{1}): ", v_YesAnswer, v_NoAnswer, i_Question);
				string answer = Console.ReadLine().ToLower();
				switch (answer)
				{
					case v_YesAnswer:
						result = true;
						break;
					case v_NoAnswer:
						result = false;
						break;
				}

				if (!result.HasValue)
				{
					Console.WriteLine("Please answer in the from of '{0}' or '{1}", v_YesAnswer, v_NoAnswer);
				}
			}

			return result.Value;
		}

		public static string AskForNonEmptyString(string i_Question)
		{
			Console.Write("{0}: ", i_Question);
			string answer = Console.ReadLine().Trim();
			while (answer == string.Empty)
			{
				Console.WriteLine("Please enter a non empty string");
				Console.Write("{0}: ", i_Question);
				answer = Console.ReadLine().Trim();
			}

			return answer;
		}

		public delegate bool ValueParserDelegate<T>(string i_StringValue, out T i_OutValue);

        public static T GetValueFromUser<T>(string i_Question, ValueParserDelegate<T> i_ParserDelegate, string i_ErrorString)
        {
            bool gotValue = false;
	        T value = default(T);

	        while(!gotValue)
            {
                string input = AskForNonEmptyString(i_Question);
                gotValue = i_ParserDelegate(input, out value);
                if (!gotValue)
                {
                    Console.WriteLine(i_ErrorString);
                }
            }

            return value;
        }

	
        public static T SelectFromList<T>(string i_Prompt, IEnumerable<T> i_Options, IComparer<T> i_ComparisonFunction = null)
		{
			T result = default(T);
			bool isValidInput = false;

			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			if (!converter.CanConvertFrom(typeof(string)))
			{
				throw new ArgumentException("Type '{0}' can't be converted from string", typeof(T).ToString());
			}

			while (!isValidInput)
			{
				Console.Write("{0} (", i_Prompt);
				bool isFirstItem = true;
				foreach (T option in i_Options)
				{
					if (!isFirstItem)
					{
						Console.Write(", ");
					}

					isFirstItem = false;
					Console.Write(option);
				}
				Console.Write("): \n");

				string rawOption = Console.ReadLine();
				try
				{
					result = (T)converter.ConvertFromString(rawOption);
				}
				catch (Exception)
				{
					Console.WriteLine("Could not parse answer, please try again");
					continue;
				}

				foreach (T option in i_Options)
				{
					if (i_ComparisonFunction == null)
					{
						isValidInput = option.Equals(result);
					}
					else
					{
						isValidInput = i_ComparisonFunction.Compare(option, result) == 0;
					}

					if (isValidInput)
					{
						result = option;
						break;
					}
				}

				if (!isValidInput)
				{
					Console.WriteLine("Please select only from the provided values");
				}
			}

			return result;	
		}
	    
        
        public static uint GetUintFromUser(string i_Question)
        {
            string answer = AskForNonEmptyString(i_Question);
            uint input;
            bool ParseSuccess = uint.TryParse(answer, out input);
            if(ParseSuccess==false)
            {
                Console.WriteLine("Invalid input"); 
               //maybe:  throw new ArgumentException("Invalid input");
                Environment.Exit(1);
            }
            return input;
        }
    }
}
