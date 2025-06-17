using System.Text;

public enum TokenType
{
    Keyword,
    Identifier,
    EOF,
    Arrow,
    LeftBrace,
    RightBrace,
    LeftBracket,
    RightBracket,
    Semicolon,
    Comma,
    Equals,
    String,
    Number
}
public class Token
{
    public TokenType Type;
    public string Value;
    public int Line, Column;

    public Token(TokenType type, int line, int col, string value="")
    {
        Type = type;
        Line = line;
        Column = col;
        Value = value;
    }
}

public class Tokeniser
{
    private readonly string Input;
    private int Position;
    private int Line = 1, Column = 1;

    private char Current => Position < Input.Length ? Input[Position] : '\0';

    public Tokeniser(string input)
    {
        Input = input;
    }

    private void Advance(int length = 1)
    {
        for (int i = 0; i < length; i++)
        {
            if (Position >= Input.Length) return;
            if (Input[Position] == '\n')
            {
                Line++;
                Column = 1;
            }
            else
            {
                Column++;
            }
            Position++;
        }
    }

    private char Peek(int length)
    {
        int newPosition = Position + length;
        return newPosition < Input.Length ? Input[newPosition] : '\0';
    }

    public Token NextToken()
    {
        ProcessCommentsAndWhitespace();
        if (Position >= Input.Length) return new Token(TokenType.EOF, Line, Column);

        char c = Current;

        if (c == '{') { Advance(); return new Token(TokenType.LeftBrace, Line, Column, "{"); }
        if (c == '}') { Advance(); return new Token(TokenType.RightBrace, Line, Column, "}"); }
        if (c == '[') { Advance(); return new Token(TokenType.LeftBracket, Line, Column, "["); }
        if (c == ']') { Advance(); return new Token(TokenType.RightBracket, Line, Column, "]"); }
        if (c == ';') { Advance(); return new Token(TokenType.Semicolon, Line, Column, ";"); }
        if (c == ',') { Advance(); return new Token(TokenType.Comma, Line, Column, ","); }
        if (c == '=') { Advance(); return new Token(TokenType.Equals, Line, Column, "="); }
        if (c == '-' && Peek(1) == '>') { Advance(2); return new Token(TokenType.Arrow, Line, Column, "->"); }

        if (c == '"')
        {
            int startLine = Line, startCol = Column;
            Advance();
            StringBuilder sb = new();

            while (Current != '"' && Current != '\0')
            {
                if (Current == '\\' && Peek(1) == '"')
                {
                    sb.Append('"');
                    Advance(2);
                }
                else
                {
                    sb.Append(Current);
                    Advance();
                }
            }
            if (Current == '"') Advance();

            return new Token(TokenType.String, startLine, startCol, sb.ToString());
        }

        if (char.IsLetterOrDigit(c) || c == '_')
        {
            int startLine = Line, startCol = Column;
            StringBuilder sb = new();

            while (char.IsLetterOrDigit(Current) || Current == '_')
            {
                sb.Append(Current);
                Advance();
            }

            string value = sb.ToString();

            string valueLowerCase = value.ToLower();
            if (valueLowerCase == "digraph" || valueLowerCase == "graph" || valueLowerCase == "subgraph" || valueLowerCase == "strict")
            {
                return new Token(TokenType.Keyword, startLine, startCol, value);
            }
            return new Token(TokenType.Identifier, startLine, startCol, value);
        }

        throw new System.Exception($"Unexpected character '{c}' at {Line}:{Column}");
    }

    public void ProcessCommentsAndWhitespace()
    {
        while (true)
        {
            while (char.IsWhiteSpace(Current)) Advance();

            if (Current == '/' && Peek(1) == '/')
            {
                Advance(2);
                while (Current != '\n' && Current != '\0') Advance();
                continue;
            }

            if (Current == '#')
            {
                Advance(1);
                while (Current != '\n' && Current != '\0') Advance();
                continue;
            }

            if (Current == '/' && Peek(1) == '*')
            {
                Advance(2);
                while (!(Current == '*' && Peek(1) == '/'))
                {
                    if (Current == '\0') return;
                    Advance();
                }
                Advance(2);
                continue;
            }
            break;
        }
    }
}