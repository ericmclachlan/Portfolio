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

Copying Files
-------------

lcd C:\Development\Ling572\ericmcl.Ling572.hw7\work\
cd /home2/ericmcl/572/hw7/
mput -r *.sh
mput *.cmd
chmod a+x *.sh

lcd C:\Development\Ling572\ericmcl.Ling572.hw7\work\source
mkdir /home2/ericmcl/572/hw7/source
cd /home2/ericmcl/572/hw7/source
mput -r *.cs
lcd C:\Development\Ling572\ericmcl.Ling572.hw7\work
cd /home2/ericmcl/572/hw7


Run Scripts
-----------

cd /home2/ericmcl/572/hw7
clear
./clean.sh
ls
./compile.sh
ls
condor_submit condor.cmd
tail -F condor.err


Get Output
----------

cd /home2/ericmcl/572/hw7
lcd C:\Development\Ling572\ericmcl.Ling572.hw7\
mget -r output_*


lcd C:\Development\Ling572\ericmcl.Ling572.hw7\work\
get hw.tar.gz

