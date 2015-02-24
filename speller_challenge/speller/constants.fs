module Speller.Constants

open System.IO

let dataPath = @"../Data"
let spellingCorpusPath = Path.Combine(dataPath, "wikipedia_spelling.txt")
let trainingSpellingCorpusPath = Path.Combine(dataPath, "wikipedia_spelling_train.txt")
let testSpellingCorpusPath = Path.Combine(dataPath, "wikipedia_spelling_test.txt")
let probabilitiesPath = Path.Combine(dataPath, "probs.txt")
let dictionaryPath = Path.Combine(dataPath, "english_words_fixed.tsv")
let unigramsBingCache = Path.Combine(dataPath, "BingUnigramsCache")

let misspellingRate = 0.05