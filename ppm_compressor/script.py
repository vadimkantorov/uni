import os
from itertools import product, repeat
import shutil
import subprocess
import filecmp
import time

all_params_file = r"H:\Projects\Dfyz\ARC\allparams"
test_dir = r"H:\Projects\Dfyz\ARC\Release"
output_dir = r"H:\Projects\Dfyz\ARC\Report\res"
corpus_dir = r"H:\Projects\Dfyz\ARC\Report\corpus"

def get_all_params(params):
	return product(*[zip(repeat(k, len(v)), v) for k, v in params.items()])

if not os.path.exists(output_dir):
	os.mkdir(output_dir)

params = {}
with open(all_params_file) as pf:
	for line in pf:
		if line.startswith('==='):
			break
		tokens = [t.strip() for t in line.split('=')]
		param_name = tokens[0]
		param_value = tokens[1].split(',')
		params[param_name] = param_value

def Fmt(secs):
	s = secs % 60
	secs /= 60
	m = secs % 60
	secs /= 60
	h = secs
	return ":".join("%02d" % t for t in [h, m, s])

os.chdir(test_dir)
fnames = list(os.listdir(corpus_dir))
ap = list(get_all_params(params))
times = []
for i, params in enumerate(ap):
	time1 = int(time.time())
	should_count = True
	for j, fn in enumerate(fnames):
		with open('params.ini', 'w') as pf:
			pf.writelines("%s = %s\n" % (k, v) for k, v in params)
		param_str = "-".join([v for k, v in params])
		out_dir = os.path.join(output_dir, param_str)
		if not os.path.exists(out_dir):
			os.mkdir(out_dir)
		cfn = fn + '.out'
		dfn = fn + '.in'
	       	out_file = os.path.join(out_dir, cfn)
	       	if os.path.exists(out_file):
	       		print "%s already exists; skipping" % out_file
	       		should_count = False
	       		continue

		print "%d:%d - %s" % (i, j, os.path.join(param_str, fn))
		shutil.copy(os.path.join(corpus_dir, fn), os.path.join(test_dir, fn))
		if subprocess.call(['PPMComp.exe', '--code', fn, cfn]):
			print "Encoding failed"
			exit(1)
		if subprocess.call(['PPMComp.exe', '--decode', cfn, dfn]):
			print "Decoding failed"
			exit(1)
		if not filecmp.cmp(fn, dfn, shallow = False):
			print "Files differ"
			exit(1)
		shutil.copy(cfn, out_file)
		os.remove(cfn)
		os.remove(dfn)
	if should_count:
		time2 = int(time.time())
		time_diff = time2 - time1
		times.append(time_diff)
		print "Time consumed for current batch: %s, estimated time left: %s" % (Fmt(time_diff), Fmt(int(sum(times)/len(times)) * (len(ap) - i - 1)))