namespace NS.CalviScript
{
    public enum TokenType
    {
        None,
        Error,

        Number,

        Plus,
        Minus,
        Mult,
        Div,
        Modulo,

        LeftParenthesis,
        RightParenthesis,
        OpenCurly,
        CloseCurly,
        Coma,
        QuestionMark,
        Colon,
        Equal,
        SemiColon,

        Identifier,
        Var,
        While,
        Function,

        End
    }
}
