namespace Speller

type ISpeller =
    abstract member CorrectQuery : string -> (string*float) []
    abstract member CorrectWord : string -> string[] * ((string*float) list)