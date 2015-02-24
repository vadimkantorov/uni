#load "spellingCorpus.fs"
#load "constants.fs"

open System
open System.IO
open Speller.Constants
open Speller.SpellingCorpus

let (trainingLines, testLines) = splitSpellingCorpus (readSpellingCorpus spellingCorpusPath)
File.WriteAllLines(trainingSpellingCorpusPath, trainingLines)
File.WriteAllLines(testSpellingCorpusPath, testLines)