#!/bin/sh

# Compile the program using the Unix compiler for .net:
ApplicationName=CommandPlatform.exe

rm -fv ${ApplicationName}

if [ "$#" -eq  "0" ]
   then
		# Compile without debugging:
		mcs source/*.cs -out:${ApplicationName}
 else
		# Compile with debugging:
		mcs source/*.cs -out:${ApplicationName} -d:DEBUG
 fi

mono ${ApplicationName} generate_scripts
chmod a+x *.sh
 