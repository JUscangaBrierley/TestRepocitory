using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Brierley.FrameWork.CampaignManagement
{

	public class Tree<T> : TreeNode<T>
	{
		public Tree() { }

		public Tree(T RootValue)
		{
			Value = RootValue;
		}
	}


	public class TreeNodeList<T> : List<TreeNode<T>>
	{
		public TreeNode<T> Parent;

		public TreeNodeList(TreeNode<T> Parent)
		{
			this.Parent = Parent;
		}

		public new TreeNode<T> Add(TreeNode<T> Node)
		{
			base.Add(Node);
			Node.Parent = Parent;
			return Node;
		}

		public TreeNode<T> Add(T Value)
		{
			return Add(new TreeNode<T>(Value));
		}
	}


	public class TreeNode<T>
	{
		public TreeNode(T Value)
		{
			_value = Value;
		}

		public TreeNode()
		{
		}

		private TreeNode<T> _parent;
		public TreeNode<T> Parent
		{
			get { return _parent; }
			set
			{
				if (_parent == value)
				{
					return;
				}
				if (_parent != null)
				{
					_parent.Children.Remove(this);
				}
				if (value != null && !value.Children.Contains(this))
				{
					value.Children.Add(this);
				}
				_parent = value;
			}
		}

		public TreeNode<T> Root
		{
			get
			{
				TreeNode<T> node = this;
				while (node.Parent != null)
				{
					node = node.Parent;
				}
				return node;
			}
		}


		private TreeNodeList<T> _children;
		public TreeNodeList<T> Children
		{
			get
			{
				if (_children == null)
				{
					_children = new TreeNodeList<T>(this);
				}
				return _children;
			}
			private set { _children = value; }
		}

		private T _value;
		public T Value
		{
			get { return _value; }
			set
			{
				_value = value;

			}
		}
	}
}
