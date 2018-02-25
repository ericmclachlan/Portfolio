#!/bin/sh
#
# Run the following commands:

exDir=~/dropbox/17-18/572/hw8/examples
echo "Examples directory: $exDir"

# -------
# Hw8: Q1
# -------

svm-train -t 0 $exDir/train model.0
svm-predict $exDir/train model.0 output.0.train
svm-predict $exDir/test model.0 output.0.test

svm-train -t 1 -g 1 -r 0 -d 2 $exDir/train model.1
svm-predict $exDir/train model.1 output.1.train
svm-predict $exDir/test model.1 output.1.test

svm-train -t 1 -g 0.1 -r 0.5 -d 2 $exDir/train model.2
svm-predict $exDir/train model.2 output.2.train
svm-predict $exDir/test model.2 output.2.test

svm-train -t 2 -g 0.5 $exDir/train model.3
svm-predict $exDir/train model.3 output.3.train
svm-predict $exDir/test model.3 output.3.test

svm-train -t 3 -g 0.5 -r -0.2 $exDir/train model.4
svm-predict $exDir/train model.4 output.4.train
svm-predict $exDir/test model.4 output.4.test

grep total_sv model.*

# N[0]=1
# N[1]=5
# N[2]=10
# N[3]=20
# N[4]=50
# N[5]=100
# N[6]=200

# maxN=6

# for (( index=0; i <= $maxN; ++index )); do
		
	# outputDir=output_${N[$index]}
	# mkdir -p $outputDir

	# ./TBL_train.sh    $exDir/train2.txt $outputDir/model_file 1
	# ./TBL_classify.sh $exDir/train2.txt $outputDir/model_file $outputDir/sys_output_train $N[$index] > $outputDir/acc_train
	# ./TBL_classify.sh $exDir/test2.txt  $outputDir/model_file $outputDir/sys_output_test  $N[$index] > $outputDir/acc_test
	
# done
