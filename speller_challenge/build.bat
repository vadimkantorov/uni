@echo off
fsc --target:library Speller\common.fs Speller\norvig.fs --out:Service\bin\speller.dll --standalone
