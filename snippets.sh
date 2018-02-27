Visual Studio Debugging Parameters
----------------------------------

# Hw2.Q3a: min_gain=0
build_dt ..\examples\train.vectors.txt ..\examples\test.vectors.txt 4 0.1 model_file sys_output

# Hw3.Q3b: min_gain=0.1
build_NB1 ..\examples\train.vectors.txt ..\examples\test.vectors.txt 0 0.5 model_file sys_output

# Hw1.Q1:
build_kNN "C:\Development\Ling572\ericmcl.Ling572.hw4\examples\train.vectors.txt" "C:\Development\Ling572\ericmcl.Ling572.hw4\examples\test.vectors.txt" 1 2 sys_output

# Hw4.Q2:
rank_feat_by_chi_square < ..\examples\train.vectors.txt

# Hw5.Q2:
maxent_classify ..\examples\test2.vectors.txt q1\m1.txt sys_output

# Hw5.Q3:
calc_emp_exp ..\examples\train2.vectors.txt

# Hw5.Q4:
calc_model_exp ..\examples\train2.vectors.txt
calc_model_exp ..\examples\train2.vectors.txt q1\m1.txt

# Hw6
beamsearch_maxent ..\examples\ex\test.txt ..\examples\ex\boundary.txt ..\examples\m1.txt sys_output 2 5 10 > acc
beamsearch_maxent ..\examples\sec19_21.txt ..\examples\sec19_21.boundary ..\examples\m1.txt sys_output 2 5 10 > acc

# Hw7
TBL_train	 ..\examples\train2.txt model_file 1
TBL_classify ..\examples\train2.txt model_file sys_output 1 > acc_train
TBL_classify ..\examples\test2.txt  model_file sys_output 1 > acc_test

# Hw8
svm_classify ..\examples\libSVM_test ..\examples\libSVM_model sys_output

Copying Files
-------------

lcd C:\Development\git\Portfolio\
cd /home2/ericmcl/572/hw8/
mput -r *.sh
mput *.cmd
chmod a+x *.sh

lcd C:\Development\git\Portfolio\source
mkdir /home2/ericmcl/572/hw8/source
cd /home2/ericmcl/572/hw8/source
mput -r *.cs
lcd C:\Development\git\Portfolio\
cd /home2/ericmcl/572/hw8


Run Scripts
-----------

cd /home2/ericmcl/572/hw8
clear
./clean.sh
ls
./compile.sh
ls
condor_submit condor.cmd
tail -F condor.err


Get Output
----------

cd /home2/ericmcl/572/hw8
lcd C:\Development\git\
mget -r model.*
mget -r output.*
mget -r sys.*


lcd C:\Development\git\Portfolio\
get hw.tar.gz

