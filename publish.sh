#!/bin/sh

exDir=~/dropbox/17-18/572/hw7
echo "Examples directory: $exDir"

# Removes files:
rm -fv snippets.sh

# Zip it!
tar -czf hw.tar.gz .

# Validation
bash $exDir/check_hw7.sh $exDir/submit-file-list
