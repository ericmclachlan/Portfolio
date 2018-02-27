#!/bin/sh
#
# Run the following commands:

exDir=~/dropbox/17-18/572/hw8/examples
echo "Examples directory: $exDir"

# -------
# Hw8: Q1
# -------

svm-train -t 0 $exDir/train model.1
svm-predict $exDir/train model.1 output.1.train
svm-predict $exDir/test model.1 output.1.test

svm-train -t 1 -g 1 -r 0 -d 2 $exDir/train model.2
svm-predict $exDir/train model.2 output.2.train
svm-predict $exDir/test model.2 output.2.test

svm-train -t 1 -g 0.1 -r 0.5 -d 2 $exDir/train model.3
svm-predict $exDir/train model.3 output.3.train
svm-predict $exDir/test model.3 output.3.test

svm-train -t 2 -g 0.5 $exDir/train model.4
svm-predict $exDir/train model.4 output.4.train
svm-predict $exDir/test model.4 output.4.test

svm-train -t 3 -g 0.5 -r -0.2 $exDir/train model.5
svm-predict $exDir/train model.5 output.5.train
svm-predict $exDir/test model.5 output.5.test

grep total_sv model.*


# -------
# Hw8: Q2
# -------


for (( index=1; i <= 5; ++index )); do
	
	./svm_classify.sh $exDir/test model.$N[$index] sys_output.$N[$index]

done
