using System;
using System.Collections.Generic;
using System.Text;

namespace StudentTree
{
	public class Program
	{
		private static void assert(string i_TestName, bool i_Predicate)
		{
			if (i_Predicate)
			{
				Console.WriteLine("{0} - V", i_TestName);
			}
			else
			{
				Console.WriteLine("{0} - X", i_TestName);
			}
			
		}

		public static void Main()
		{
			AvlTree<string, int> tree = new AvlTree<string, int>();
			int value;
			const string v_RootNodeKey = "F";
			const int v_RootNodeValue = 0;
			assert("Insert root node", tree.TryInsert(v_RootNodeKey, v_RootNodeValue));
			assert("Find root node", tree.Find(v_RootNodeKey, out value));
			assert("Found root node value", value == v_RootNodeValue);
			tree.TryInsert("G", 1);
			tree.TryInsert("A", -1);
			tree.TryInsert("B", -1);
			tree.TryInsert("E", -1);
			assert("Find child (G)", tree.Find("G", out value));
			assert("Find child (E)", tree.Find("E", out value));
			assert("Find child (B)", tree.Find("B", out value));
			Console.ReadKey();
		}
	}
}
