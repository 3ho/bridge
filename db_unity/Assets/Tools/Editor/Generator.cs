using System;
using System.Text;

public class Generator
{
    protected StringBuilder indent = new StringBuilder();
    protected StringBuilder content;

    public Generator()
    {
        content = new StringBuilder();
    }

    public Generator(StringBuilder sb)
    {
        content = sb;
    }

    public StringBuilder GetContent()
    {
        return content;
    }

    public String GetIndent()
    {
        return indent.ToString();
    }

    public void AddIndent()
    {
        indent.Append('\t');
    }

    public void ReduceIndent()
    {
        indent.Remove(this.indent.Length - 1, 1);
    }

    public void ResetIndent()
    {
        indent = new StringBuilder();
    }

    public void Print(char x)
    {
        content.Append(x);
    }

    public void Print(String str)
    {
        content.Append(str);
    }

    public void Println()
    {
        content.AppendLine();
    }

    public void Println(char x)
    {
        content.Append(indent.ToString());
        content.AppendLine("" + x);
    }

    public void Println(String str)
    {
        content.Append(indent.ToString());
        content.AppendLine(str);
    }

    public void Printf(String format, params System.Object[] args)
    {
        content.AppendFormat(format, args);
    }

    public void Printfln(String format, params System.Object[] args)
    {
        content.Append(indent.ToString());
        content.AppendFormat(format, args);
        content.AppendLine();
    }
}
