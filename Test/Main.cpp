#include <errno.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <iostream>

struct DumpData
{
public:
	int Var;
	int Mem;
};

static int writer(const void* data, size_t size, void* u) {
	return (fwrite(data, size, 1, (FILE*)u) != 1) && (size != 0);
}
static int DumpMem(const void* data, size_t n, size_t size, FILE* file) {
	return writer(data, n * size, file);
}
static int DumpVar(const void* data, FILE* file) {
	return DumpMem(&data, 1, sizeof(data), file);
}
static DumpData* DumpVector(const void* data, int n, size_t size, FILE* file) {
	DumpData* ddp = new DumpData();

	ddp->Var = DumpVar((void*)n, file);
	ddp->Mem = DumpMem(data, n, size, file);

	return ddp;
}

int main() {
	using namespace std;

	FILE* file = fopen("D:\\!a\\test.txt", "a");
	//int wtf = DumpVar((void*)1337, file);

	uint32_t* insts = new uint32_t[17];

	for (int i = 0; i < sizeof(insts); i++) {
		insts[i] = 1579;
	}

	DumpData* dd = DumpVector(insts, sizeof(insts), sizeof(uint32_t), file);

	cout << "Returned Var: '" << dd->Var << "' Mem: '" << dd->Mem << "'" << endl;

	delete dd;

	fclose(file);

	return 0;
}