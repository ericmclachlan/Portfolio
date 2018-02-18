#!/bin/sh
#
# Run the following commands:

exDir=~/dropbox/17-18/572/hw7/examples
echo "Examples directory: $exDir"

# ------
# Hw6
# ------

beam_size[0]=0
beam_size[1]=1
beam_size[2]=2
beam_size[3]=3

topN[0]=1
topN[1]=3
topN[2]=5
topN[3]=10

topK[0]=1
topK[1]=5
topK[2]=10
topK[3]=100

for index in 0 1 2  3; do
		
	outputDir=output_${beam_size[$index]}_${topN[$index]}_${topK[$index]}
	mkdir -p $outputDir
	
	./beamsearch_maxent.sh	$exDir/sec19_21.txt $exDir/sec19_21.boundary $exDir/m1.txt $outputDir/sys_output "${beam_size[$index]}" "${topN[$index]}" "${topK[$index]}" > $outputDir/acc
	cat $outputDir/acc
	
done
