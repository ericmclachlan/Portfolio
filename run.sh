#!/bin/sh
#
# Run the following commands:

exDir=~/dropbox/17-18/572/hw7/examples
echo "Examples directory: $exDir"

# ------
# Hw6
# ------

N[0]=1
N[1]=5
N[2]=10
N[3]=20
N[4]=50
N[5]=100
N[6]=200

maxN=6

for (( index=0; i <= $maxN; ++index )); do
		
	outputDir=output_${N[$index]}
	mkdir -p $outputDir

	./TBL_train.sh    $exDir/train2.txt $outputDir/model_file 1
	./TBL_classify.sh $exDir/train2.txt $outputDir/model_file $outputDir/sys_output_train $N[$index] > $outputDir/acc_train
	./TBL_classify.sh $exDir/test2.txt  $outputDir/model_file $outputDir/sys_output_test  $N[$index] > $outputDir/acc_test
	
done

# Prepare files for submission:
cp output_20/sys_output_20 .
cp output_50/sys_output_50 .
cp output_100/sys_output_100 .