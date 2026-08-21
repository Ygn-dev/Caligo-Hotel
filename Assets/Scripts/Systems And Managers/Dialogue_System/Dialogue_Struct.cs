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
public class TextoIdioma
{
    public string idioma; // "en", "es", etc.
    public string texto;  // "Hello", "Hola", etc.
}
[Serializable]
public class Dialogue_Node
{
    public int nodeId;
    public string personaje;
    public List<TextoIdioma> textosPorIdioma; //public string text;
    public int nextNodeId;

    //public List<Dialogue_Option> options;
    public string ObtenerTexto(string idiomaDeseado)
    {
        if (textosPorIdioma == null || textosPorIdioma.Count == 0) return "";

        // Buscar el idioma solicitado
        foreach (var item in textosPorIdioma)
        {
            if (item.idioma == idiomaDeseado) return item.texto;
        }

        // Si no lo encuentra, buscar idioma por defecto
        foreach (var item in textosPorIdioma)
        {
            if (item.idioma == "es") return item.texto;
        }

        return "";
    }
}

