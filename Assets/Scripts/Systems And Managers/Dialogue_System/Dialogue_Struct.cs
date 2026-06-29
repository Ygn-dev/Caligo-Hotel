using System;
using System.Collections.Generic;

[Serializable]
public class Dialogue_Struct
{
    public string dialogueId;
    public int startNode;
    public List<Dialogue_Node> nodes;
}

[Serializable]
public class Dialogue_Node
{
    public int nodeId;
    public string personaje;
    public string text;
    public int nextNodeId;

    //public List<Dialogue_Option> options;
}

