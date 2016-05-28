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
        QuestionMark,
        Colon,
        Equal,
        SemiColon,
        OpenCurly,
        CloseCurly,

        Identifier,
        Var,
        While,
        Function,

        End
    }
}
