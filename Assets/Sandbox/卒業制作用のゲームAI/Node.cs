using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Node
{
    Node _parent;
    List<Node> _children;

    public Node(string id, string content, Node parent = null, params Node[] children)
    {
        ID = id;
        Content = content;
        _parent = parent;
        _children = children.ToList();
    }

    public string ID { get; }
    public string Content { get; }

    public void SetParent(Node parent)
    {
        _parent = parent;
    }

    public Node GetParent()
    {
        return _parent;
    }

    public void AddChild(Node child)
    {
        _children ??= new List<Node>();
        _children.Add(child);
    }

    public Node GetChild(int index)
    {
        if (IsLeaf()) return null;
        else return _children[index];
    }

    public bool IsLeaf()
    {
        return _children == null || _children.Count == 0;
    }
}