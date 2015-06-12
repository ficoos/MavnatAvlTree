using System;
using System.Collections;
using System.Collections.Generic;

namespace StudentTree
{
	public class AvlTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
	{
		private sealed class Node
		{
			public Node Parent { get; set; }

			public Node Left{ get; set; }

			public Node Right { get; set; }

			public TKey Key { get; set; }

			public TValue Value { get; set; }

			public int Balance { get; set; }

			public override string ToString()
			{
				return string.Format("{0} ({1})", Key, Balance);
			}
		}

		private Node m_Root;

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			if (this.m_Root != null)
			{
				foreach (var value in this.iterate(this.m_Root))
				{
					yield return value;
				}
			}
		}

		private IEnumerable<KeyValuePair<TKey, TValue>> iterate(Node i_Node)
		{
			if (i_Node.Left != null)
			{
				foreach (var value in this.iterate(i_Node.Left))
				{
					yield return value;
				}
			}

			if (i_Node.Right != null)
			{
				foreach (var value in this.iterate(i_Node.Right))
				{
					yield return value;
				}
			}

			yield return new KeyValuePair<TKey, TValue>(i_Node.Key, i_Node.Value);
		}

		public void Clear()
		{
			this.m_Root = null;
		}

		public bool IsEmpty
		{
			get
			{
				return m_Root == null;
			}
		}

		// Function is different from assignment to be more idiomatic
		// If TValue is a struct there would be no way to differentiate between
		// not finding a value and the value being default(TValue).
		// So instead of returning null we return a bool and using an out parameter
		// for the value.
		public bool Find(TKey i_Key, out TValue i_Value)
		{
			Node node = this.m_Root;

			while (node != null)
			{
				if (i_Key.CompareTo(node.Key) < 0)
				{
					node = node.Left;
				}
				else if (i_Key.CompareTo(node.Key) > 0)
				{
					node = node.Right;
				}
				else
				{
					i_Value = node.Value;

					return true;
				}
			}

			i_Value = default(TValue);

			return false;
		}

		public void Insert(TKey i_Key, TValue i_Value)
		{
			if (this.m_Root == null)
			{
				this.m_Root = new Node { Key = i_Key, Value = i_Value };
			}
			else
			{
				Node node = this.m_Root;

				while (node != null)
				{
					int compare = i_Key.CompareTo(node.Key);

					if (compare < 0)
					{
						Node left = node.Left;

						if (left == null)
						{
							node.Left = new Node { Key = i_Key, Value = i_Value, Parent = node };

							this.insertBalance(node, 1);

							return;
						}
						else
						{
							node = left;
						}
					}
					else if (compare > 0)
					{
						Node right = node.Right;

						if (right == null)
						{
							node.Right = new Node { Key = i_Key, Value = i_Value, Parent = node };

							this.insertBalance(node, -1);

							return;
						}
						else
						{
							node = right;
						}
					}
					else
					{
						node.Value = i_Value;

						return;
					}
				}
			}
		}

		private void insertBalance(Node i_Node, int i_Balance)
		{
			while (i_Node != null)
			{
				i_Balance = i_Node.Balance += i_Balance;

				if (i_Balance == 0)
				{
					return;
				}
				else if (i_Balance == 2)
				{
					if (i_Node.Left.Balance == 1)
					{
						this.rotateRight(i_Node);
					}
					else
					{
						this.rotateLeftRight(i_Node);
					}

					return;
				}
				else if (i_Balance == -2)
				{
					if (i_Node.Right.Balance == -1)
					{
						this.rotateLeft(i_Node);
					}
					else
					{
						this.rotateRightLeft(i_Node);
					}

					return;
				}

				Node parent = i_Node.Parent;

				if (parent != null)
				{
					i_Balance = parent.Left == i_Node ? 1 : -1;
				}

				i_Node = parent;
			}
		}

		public bool Delete(TKey i_Key)
		{
			Node node = this.m_Root;

			while (node != null)
			{
				if (i_Key.CompareTo(node.Key) < 0)
				{
					node = node.Left;
				}
				else if (i_Key.CompareTo(node.Key) > 0)
				{
					node = node.Right;
				}
				else
				{
					Node left = node.Left;
					Node right = node.Right;

					if (left == null)
					{
						if (right == null)
						{
							if (node == this.m_Root)
							{
								this.m_Root = null;
							}
							else
							{
								Node parent = node.Parent;

								if (parent.Left == node)
								{
									parent.Left = null;

									this.deleteBalance(parent, -1);
								}
								else
								{
									parent.Right = null;

									this.deleteBalance(parent, 1);
								}
							}
						}
						else
						{
							replace(node, right);

							this.deleteBalance(node, 0);
						}
					}
					else if (right == null)
					{
						replace(node, left);

						this.deleteBalance(node, 0);
					}
					else
					{
						Node successor = right;

						if (successor.Left == null)
						{
							Node parent = node.Parent;

							successor.Parent = parent;
							successor.Left = left;
							successor.Balance = node.Balance;

							if (left != null)
							{
								left.Parent = successor;
							}

							if (node == this.m_Root)
							{
								this.m_Root = successor;
							}
							else
							{
								if (parent.Left == node)
								{
									parent.Left = successor;
								}
								else
								{
									parent.Right = successor;
								}
							}

							this.deleteBalance(successor, 1);
						}
						else
						{
							while (successor.Left != null)
							{
								successor = successor.Left;
							}

							Node parent = node.Parent;
							Node successorParent = successor.Parent;
							Node successorRight = successor.Right;

							if (successorParent.Left == successor)
							{
								successorParent.Left = successorRight;
							}
							else
							{
								successorParent.Right = successorRight;
							}

							if (successorRight != null)
							{
								successorRight.Parent = successorParent;
							}

							successor.Parent = parent;
							successor.Left = left;
							successor.Balance = node.Balance;
							successor.Right = right;
							right.Parent = successor;

							if (left != null)
							{
								left.Parent = successor;
							}

							if (node == this.m_Root)
							{
								this.m_Root = successor;
							}
							else
							{
								if (parent.Left == node)
								{
									parent.Left = successor;
								}
								else
								{
									parent.Right = successor;
								}
							}

							this.deleteBalance(successorParent, -1);
						}
					}

					return true;
				}
			}

			return false;
		}

		private void deleteBalance(Node i_Node, int i_Balance)
		{
			while (i_Node != null)
			{
				i_Balance = i_Node.Balance += i_Balance;

				if (i_Balance == 2)
				{
					if (i_Node.Left.Balance >= 0)
					{
						i_Node = this.rotateRight(i_Node);

						if (i_Node.Balance == -1)
						{
							return;
						}
					}
					else
					{
						i_Node = this.rotateLeftRight(i_Node);
					}
				}
				else if (i_Balance == -2)
				{
					if (i_Node.Right.Balance <= 0)
					{
						i_Node = this.rotateLeft(i_Node);

						if (i_Node.Balance == 1)
						{
							return;
						}
					}
					else
					{
						i_Node = this.rotateRightLeft(i_Node);
					}
				}
				else if (i_Balance != 0)
				{
					return;
				}

				Node parent = i_Node.Parent;

				if (parent != null)
				{
					i_Balance = parent.Left == i_Node ? -1 : 1;
				}

				i_Node = parent;
			}
		}

		private Node rotateLeft(Node i_Node)
		{
			Node right = i_Node.Right;
			Node rightLeft = right.Left;
			Node parent = i_Node.Parent;

			right.Parent = parent;
			right.Left = i_Node;
			i_Node.Right = rightLeft;
			i_Node.Parent = right;

			if (rightLeft != null)
			{
				rightLeft.Parent = i_Node;
			}

			if (i_Node == this.m_Root)
			{
				this.m_Root = right;
			}
			else if (parent.Right == i_Node)
			{
				parent.Right = right;
			}
			else
			{
				parent.Left = right;
			}

			right.Balance++;
			i_Node.Balance = -right.Balance;

			return right;
		}

		private Node rotateRight(Node i_Node)
		{
			Node left = i_Node.Left;
			Node leftRight = left.Right;
			Node parent = i_Node.Parent;

			left.Parent = parent;
			left.Right = i_Node;
			i_Node.Left = leftRight;
			i_Node.Parent = left;

			if (leftRight != null)
			{
				leftRight.Parent = i_Node;
			}

			if (i_Node == this.m_Root)
			{
				this.m_Root = left;
			}
			else if (parent.Left == i_Node)
			{
				parent.Left = left;
			}
			else
			{
				parent.Right = left;
			}

			left.Balance--;
			i_Node.Balance = -left.Balance;

			return left;
		}

		private Node rotateLeftRight(Node i_Node)
		{
			Node left = i_Node.Left;
			Node leftRight = left.Right;
			Node parent = i_Node.Parent;
			Node leftRightRight = leftRight.Right;
			Node leftRightLeft = leftRight.Left;

			leftRight.Parent = parent;
			i_Node.Left = leftRightRight;
			left.Right = leftRightLeft;
			leftRight.Left = left;
			leftRight.Right = i_Node;
			left.Parent = leftRight;
			i_Node.Parent = leftRight;

			if (leftRightRight != null)
			{
				leftRightRight.Parent = i_Node;
			}

			if (leftRightLeft != null)
			{
				leftRightLeft.Parent = left;
			}

			if (i_Node == this.m_Root)
			{
				this.m_Root = leftRight;
			}
			else if (parent.Left == i_Node)
			{
				parent.Left = leftRight;
			}
			else
			{
				parent.Right = leftRight;
			}

			if (leftRight.Balance == -1)
			{
				i_Node.Balance = 0;
				left.Balance = 1;
			}
			else if (leftRight.Balance == 0)
			{
				i_Node.Balance = 0;
				left.Balance = 0;
			}
			else
			{
				i_Node.Balance = -1;
				left.Balance = 0;
			}

			leftRight.Balance = 0;

			return leftRight;
		}

		private Node rotateRightLeft(Node i_Node)
		{
			Node right = i_Node.Right;
			Node rightLeft = right.Left;
			Node parent = i_Node.Parent;
			Node rightLeftLeft = rightLeft.Left;
			Node rightLeftRight = rightLeft.Right;

			rightLeft.Parent = parent;
			i_Node.Right = rightLeftLeft;
			right.Left = rightLeftRight;
			rightLeft.Right = right;
			rightLeft.Left = i_Node;
			right.Parent = rightLeft;
			i_Node.Parent = rightLeft;

			if (rightLeftLeft != null)
			{
				rightLeftLeft.Parent = i_Node;
			}

			if (rightLeftRight != null)
			{
				rightLeftRight.Parent = right;
			}

			if (i_Node == this.m_Root)
			{
				this.m_Root = rightLeft;
			}
			else if (parent.Right == i_Node)
			{
				parent.Right = rightLeft;
			}
			else
			{
				parent.Left = rightLeft;
			}

			if (rightLeft.Balance == 1)
			{
				i_Node.Balance = 0;
				right.Balance = -1;
			}
			else if (rightLeft.Balance == 0)
			{
				i_Node.Balance = 0;
				right.Balance = 0;
			}
			else
			{
				i_Node.Balance = 1;
				right.Balance = 0;
			}

			rightLeft.Balance = 0;

			return rightLeft;
		}

		private static void replace(Node i_Target, Node i_Source)
		{
			Node left = i_Source.Left;
			Node right = i_Source.Right;

			i_Target.Balance = i_Source.Balance;
			i_Target.Key = i_Source.Key;
			i_Target.Value = i_Source.Value;
			i_Target.Left = left;
			i_Target.Right = right;

			if (left != null)
			{
				left.Parent = i_Target;
			}

			if (right != null)
			{
				right.Parent = i_Target;
			}
		}

		public void PrintDictionary()
		{
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				Console.WriteLine("{0}:{1}", keyValuePair.Key, keyValuePair.Value);
			}
		}

		public void PrintTree()
		{
			this.printRoot(this.m_Root);
		}

		private void printRoot(Node i_Root)
		{
			if (i_Root.Left != null)
			{
				this.printNode(i_Root.Left, string.Empty, true);
			}
			Console.WriteLine("{0}", i_Root);
			if (i_Root.Right != null)
			{
				this.printNode(i_Root.Right, string.Empty, false);
			}
		}

		private void printNode(Node i_Node, string i_Indent, bool i_IsLeftChild)
		{
			if (i_Node.Left != null)
			{
				string indent = i_Indent + (i_IsLeftChild ? "  " : "| ");
				this.printNode(i_Node.Left, indent, true);
			}
			else
			{
				Console.WriteLine("{0}{1}", i_Indent, !i_IsLeftChild ? "|" : string.Empty);
			}

			Console.WriteLine("{0} {2}{1}", i_Indent, i_Node, i_IsLeftChild ? "/" : "\\");

			if (i_Node.Right != null)
			{
				string indent = i_Indent + (i_IsLeftChild ? "| " : "  ");
				this.printNode(i_Node.Right, indent, false);
			}
			else
			{
				Console.WriteLine("{0}{1}", i_Indent, i_IsLeftChild ? "|" : string.Empty);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
