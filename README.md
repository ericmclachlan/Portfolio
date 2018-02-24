# Greetings

My name is Eric McLachlan and I am a software engineer and linguist.

Welcome to my portfolio. Please feel free to browse project at your leisure. This code is largely based on assignments I have received as part of my post-graduate studies at the University of Washington. While the design and, to an extent, the methodology are prescribed by the university, these C# implementations are completely my own work. For each assignment I received, I create one or more commands (see below) that are added to the library. The specific commands are probably of limited general use; however the underlying data structures, helper classes, and classifiers themselves are fairly generic and reusable.

Anyway, I hope that you enjoy browsing my wares, that you find it easy to read, and get a sense of the quality of code I can produce.

As time goes by, I will continue to add to and improve this library. In the meantime, I hope you find it interesting; and possibly even useful.

Thank you for taking the time to check this out.

Wishing you the best,

Eric McLachlan

Email: mailto:ericmclachlan@gmail.com

LinkedIn: www.linkedin.com/in/ericmclachlan


# Application Overview

The main project defines a "Command Platform". The CommandPlatform acts as a delegate, accepting parameters from the console and invoking the ExecuteCommand(...) method of the command specified in the first parameter passed to the application. The remaining parameters are passed to the relevant command.

So, for example, calling "CommandPlatform.exe help" will cause the HelpCommand.ExecuteCommand(...) method to be invoked. Similarly, calling "CommandPlatform.exe calc_emp_exp parameter1" will delegate execution ot the Command_calc_emp_exp.ExecuteCommand(...) method after initializing the relevant property of the Command_calc_emp_exp class to the value of parameter1.

The purpose of this project was to make it very easy to add new commands to the platform. To add a new command, simply add a new class (e.g. YourCommand) that implements ICommand. Be sure to set the CommandName property to something like "Your_Command_Name". Then, calling "CommandPlatform.exe Your_Command_Name". This will invoke YourCommand.ExecuteCommand(...). Marking properties of YourCommand with the CommandParameterAttribute will allow you to define parameters for the command, which are validated before control is passed to the ExecuteCommand(...) method.


# File Structure

The source code, which can be found in the "source" subdirectory, is written in C# and defines a simple console application.

The main directory is very light with only the project file required by visual studio and two other files.
- Program.cs, which starts up the command platform and delegates to the invoked commands.
- ProgramOutput.cs, which contains output specifically related to the commands being executed. (These routines are not general purpose and will likely be relocated to a position outside the core library at a later date.)

The code is further organized into the following folders:
- DataStructures
- Helpers
- Classifiers
- Commands

Each of these are introduced very briefly below:

## Data Structures

This folder contains class libraries of commonly used data structures, such as ValueIdMapper and IdValuePair which bind numerical values to arbitrary types. (This is a useful optimization in many circumstances as it avoids unnecessary text operations.)

In some cases, classes are included where they provide functionality that can be applied generally, such as the beam-search implementation, where the beam embodies a structure particular to that algorithm.

## Helpers

This folder contains implementations of static classes that provide very generic, but useful, re-usable code libraries. 

## Classifiers

This folder contains classifiers implemented as part of a course involving machine learning related to natural language processing (NLP).

## Commands

The application includes a number of example commands. These commands can be thought of as use-cases, where each command uses the libraries discussed above to satisfy some use case. (These commands are not general purpose and will likely be relocated to a position outside the core library at a later date.)
