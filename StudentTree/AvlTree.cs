using System;
using System.Diagnostics;

namespace StudentTree
{
	public class AvlTree<K, V> where K : IComparable<K>
	{
		private class Node
		{
			private readonly V r_Value;

			public V Value
			{
				get
				{
					return r_Value;
				}
			}

			private readonly K r_Key;

			public K Key
			{
				get
				{
					return r_Key;
				}
			}

			public Node LeftChild { get; set; }

			public Node RightChild { get; set; }

			private int m_BalanceFactor;

			public int BalanceFactor
			{
				get
				{
					return m_BalanceFactor;
				}

				set
				{
					Debug.Assert(Math.Abs(value) > 2, "Balance Factor out of range");
					m_BalanceFactor = 2;
				}
			}

			public bool LeftChildTaller
			{
				get
				{
					return BalanceFactor > 0;
				}
			}

			public bool RightChildTaller
			{
				get
				{
					return BalanceFactor < 0;
				}
			}

			public Node(K i_Key, V i_Value)
			{
				r_Key = i_Key;
				r_Value = i_Value;
				LeftChild = null;
				RightChild = null;
				BalanceFactor = 0;
			}
		}

		private Node m_Root;

		public AvlTree()
		{
			m_Root = null;
		}

		private bool insert(Node i_Node, K i_Key, V i_Value)
		{
			bool wasInsert = false;
			int comparison = i_Node.Key.CompareTo(i_Key);
			if (comparison < 0)
			{
				if (i_Node.LeftChild == null)
				{
					i_Node.LeftChild = new Node(i_Key, i_Value);
					wasInsert = true;
				}
				else
				{
					wasInsert = this.insert(i_Node.LeftChild, i_Key, i_Value);
				}

				if (wasInsert)
				{
					i_Node.BalanceFactor++;
				}
			}
			else if (comparison > 0)
			{
				if (i_Node.RightChild == null)
				{
					i_Node.RightChild = new Node(i_Key, i_Value);
					wasInsert = true;
				}
				else
				{
					wasInsert = this.insert(i_Node.RightChild, i_Key, i_Value);
				}

				if (wasInsert)
				{
					i_Node.BalanceFactor--;
				}
			}

			if (wasInsert)
			{
				rebalance(i_Node);
			}

			return wasInsert;
		}

		private static void rebalance(Node i_Node)
		{
			if (Math.Abs(i_Node.BalanceFactor) <= 1)
			{
				return;
			}


			if (i_Node.LeftChildTaller)
			{
				if (i_Node.LeftChild.RightChildTaller)
				{
					Node originalLeftChild = i_Node.LeftChild;
					i_Node.LeftChild = i_Node.LeftChild.RightChild;
					i_Node.LeftChild.BalanceFactor++;
					Node originalRightGrandson = i_Node.LeftChild.LeftChild;
					i_Node.LeftChild.LeftChild = originalLeftChild;
					i_Node.LeftChild.LeftChild.RightChild = originalRightGrandson;
					i_Node.LeftChild.LeftChild.BalanceFactor--;
				}
				else
				{
					V tmpValue = i_Node.Value;
					i_Node.Value = i_Node.LeftChild.Value;
					i_Node.LeftChild.Value = tmpValue;
				}
			}
		}

		public bool TryInsert(K i_Key, V i_Value)
		{
			bool insertSuccessful;

			if (m_Root == null)
			{
				m_Root = new Node(i_Key, i_Value);
				insertSuccessful = true;
			}
			else
			{
				insertSuccessful = insert(m_Root, i_Key, i_Value);
			}

			return insertSuccessful;
		}

		public bool Find(K i_Key, out V o_Value)
		{
			bool wasFound = false;

			if (m_Root == null)
			{
				o_Value = default(V);
			}
			else
			{
				Node closestNode = findClosestNode(m_Root, i_Key);
				if (closestNode.Key.CompareTo(i_Key) == 0)
				{
					o_Value = closestNode.Value;
					wasFound = true;
				}
				else
				{
					o_Value = default(V);
				}
			}

			return wasFound;
		}
		
		private static Node findClosestNode(Node i_Node, K i_Key)
		{
			Node returnNode;
			int comparison = i_Node.Key.CompareTo(i_Key);
			Node nodeToCheck = null;
			if (comparison < 0)
			{
				nodeToCheck = i_Node.LeftChild;
			}
			else if (comparison > 0)
			{
				nodeToCheck = i_Node.RightChild;
			}

			if (nodeToCheck == null)
			{
				returnNode = i_Node;
			}
			else
			{
				returnNode = findClosestNode(nodeToCheck, i_Key);
			}
				
			return returnNode;
		}
	}
}