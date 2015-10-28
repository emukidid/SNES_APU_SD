int _stdcall InitSerialSend(void);
int _stdcall ReadSPC700(int address);
int _stdcall WriteSPC700(int address, int data);
int _stdcall SetPort0 (short data);
int _stdcall WriteSPC700_WP0I (int address, int data);
int WrSPC700_WP0I (int address, int data);
int _stdcall Write16bytes (char byte1, char byte2, char byte3, char byte4, char byte5, char byte6, char byte7, char byte8, char byte9, char byte10, char byte11, char byte12, char byte13, char byte14, char byte15, char byte16);
int _stdcall ResetAPU(void);
int _stdcall OpenPort(CString port);
int _stdcall OpenPort_VB(char *port);	//Identical to OpenPort, except that VB programs can use this one, to detect available com ports.
int _stdcall init_port(void);
int _stdcall ClosePort(void);
int _stdcall FlushRead(void);

int _stdcall GetID6xTag(char ID666tag[], char XID6tag[], short tagID);