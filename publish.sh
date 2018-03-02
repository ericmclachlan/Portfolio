#!/bin/sh

exDir=~/dropbox/17-18/572/hw8
echo "Examples directory: $exDir"

# Removes files:
rm -fv condor.log
rm -fv condor.out
rm -fv condor.err
rm -fv snippets.sh
rm -fv output.*
rm -fv acc.*
rm -fv model.2
rm -fv sys.2
rm -fv model.3
rm -fv sys.3
rm -fv model.5
rm -fv sys.5

# Zip it!
tar -czf hw.tar.gz .

# Validation
bash $exDir/check_hw8.sh $exDir/submit-file-list
