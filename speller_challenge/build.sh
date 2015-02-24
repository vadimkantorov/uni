#!/usr/bin/env bash

fsharpc --target:library Speller/*.fs --out:Service/bin/speller.dll --standalone
