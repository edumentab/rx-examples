# Rx Sample Applications

This repository includes a couple of simple Rx sample applications developed
at Edument AB and used in various seminars. If you're interested to learn more
on these topics, check out our Parallel and Asynchronous Programming course:

http://www.edument.se/en/training/dotnet/parallel-and-asynchronous-programming-in-csharp-5

## Airport Announcements

Example of Rx on some basic domain events related to gate assignments and
changes as well as flight delays/cancellations at an airport. Should just
build and run.

## Twitter Sentiment Analysis

Turns a Twitter live stream into an `Observable`, and then does some really
basic sentiment analysis on it. Also demonstrates using the awesome Switch
combinator to handle moving between observables based on user input.

To run this one, you'll need to grab a (free) Twitter key and put it into
app.config.
