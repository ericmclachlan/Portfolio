# Greetings

My name is Eric McLachlan and I am a software engineer and computational linguist.

Welcome to my portfolio. Please feel free to browse this project at your leisure. Some of the code is based on assignments I received as part of my post-graduate studies at the University of Washington. The university did influence the design of the commands (see below) and methodology, however these C# implementations are completely my own work.

I hope that, by reading my code, you:
- get a sense of the quality of code I can produce, and
- generally just enjoy seeing how I went about solving these problems.

As time goes by, I will continue to add to and improve this library. As it is, I think it contains implementations that are interesting; and hopefully even useful.

Thank you for taking the time to check out my portfolio.

Wishing you the best,

Eric McLachlan

Email: mailto:ericmclachlan@gmail.com

LinkedIn: www.linkedin.com/in/ericmclachlan

---

# Application Overview

The solution is written in C# and this package includes the following three main directories:
1. ericmclachlan.Portfolio.ConsoleApp
2. ericmclachlan.Portfolio.Core
3. ericmclachlan.Portfolio.Tests


## Portfolio/ericmclachlan.Portfolio.ConsoleApp

The project defines an executable called CommandPlatform.exe. The application consists of a command framework and a collection of commands. The framework acts as a delegate, accepting parameters from the console and invoking the ExecuteCommand(...) method of whichever command is specified in the first parameter that is passed to the application. The remaining parameters passed to the application are also automatically initialized and passed to the relevant command.

So, for example, calling "CommandPlatform.exe help" will cause the HelpCommand.ExecuteCommand(...) method to be invoked. Similarly, calling "CommandPlatform.exe calc_emp_exp parameter1" will delegate execution to the Command_calc_emp_exp.ExecuteCommand(...) method after initializing the relevant property of the Command_calc_emp_exp class to the value of parameter1.

The purpose of this project was to make it very easy to add new commands to the platform. To add a new command, simply add a new class (e.g. YourCommand) that inherits from the Command class. Be sure to set the CommandName property to something like "Your_Command_Name". Then, calling "CommandPlatform.exe Your_Command_Name" will invoke YourCommand.ExecuteCommand(...). You can define parameters for the command, which are validated before control is passed to the ExecuteCommand(...) method by marking properties of YourCommand with the CommandParameterAttribute.


## Portfolio/ericmclachlan.Portfolio.Core

This is the main library of code in this portfolio.

The code is subdivided into:
- Classifiers
- DataStructures
- Helpers

Each of these are introduced very briefly below:

### Classifiers

These machine learning implementations define classifiers useful for natural language processing (NLP).

### Data Structures

This folder contains class libraries of commonly used data structures. One example is IdValuePair which bind numerical values to arbitrary types. (This is a useful optimization in many circumstances as it avoids unnecessary text operations.)

In some cases, classes are included where they provide functionality that can be applied generally, such as the beam-search implementation, where the beam embodies a structure particular to that algorithm.

### Helpers

This folder contains implementations of static classes that provide very generic, but useful, re-usable code libraries. 


## Portfolio/ericmclachlan.Portfolio.Tests

This folder contains code used for testing the core library. Testing is implemented using the Microsoft Unit Test Framework. 

The code in this directory demonstrates how commands can be executed programatically, as well as providing some test cases for regression testing.