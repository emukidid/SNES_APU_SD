/******************** I N C L U D E S **********************************/
#define VC_EXTRALEAN		// Exclude rarely-used stuff from Windows headers
#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>			// MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT

#include <stdio.h>
#include <conio.h>
#include <string.h>
#include <time.h>

#include "apuplay.h"

/******************** D E F I N I T I O N S ****************************/

/******************** S T R U C T U R E S ******************************/

/******************** G L O B A L S ************************************/


int ready_line=0;     //holds the last state of the "ready to receive line"
int bootcode=0;
int port0=0;

/******************** F U N C T I O N S ********************************/

void sleep(int msec)
{
	clock_t time;

	//wait for a number of msec.
	time = clock();
	while ( clock()-time < 0.001*msec*CLOCKS_PER_SEC) {}
} //end of sleep()



static HANDLE comm_handle=NULL;

int _stdcall OpenPort(CString port)
{
	port="\\\\.\\"+port;

	comm_handle=CreateFile(port,GENERIC_READ|GENERIC_WRITE,0,NULL,OPEN_EXISTING,0,NULL);

	if(comm_handle==INVALID_HANDLE_VALUE) {
		const DWORD err=GetLastError();
		//TRACE2("Error opening port: %s (%i)\n",port,err);
		comm_handle=NULL;
		return (int)err;
	}
	//TRACE2("Port % s open - handle: %08X\n",port,comm_handle);

	// SetupComm(m_hComm,RxSize,TxSize);	// Setup Rx/Tx buffer sizes (optional)

	DCB dcb;
	dcb.DCBlength=sizeof(dcb);
	GetCommState(comm_handle,&dcb);
	dcb.BaudRate=250000;	// bit rate 
	dcb.ByteSize=8;			// number of bits/byte, 4-8 
	dcb.Parity=0;			// 0-4=no,odd,even,mark,space 
	dcb.StopBits=0;			// 0,1,2 = 1, 1.5, 2 
	dcb.fBinary=1;			// binary mode, no EOF check
	dcb.fParity=0;			// enable parity checking 
	dcb.fDtrControl=DTR_CONTROL_DISABLE; // DTR flow control type 
	dcb.fRtsControl=RTS_CONTROL_DISABLE; // RTS flow control 
	//dcb.fRtsControl=RTS_CONTROL_HANDSHAKE;
	dcb.fOutxCtsFlow=0;		// CTS output flow control 
	//dcb.fOutxCtsFlow=TRUE;
	dcb.fOutxDsrFlow=0;		// DSR output flow control 
	dcb.fDsrSensitivity=0;	// DSR sensitivity 
	dcb.fTXContinueOnXoff=0;// XOFF continues Tx 
	dcb.fOutX=0;			// XON/XOFF out flow control 
	dcb.fInX=0;				// XON/XOFF in flow control 
	dcb.fNull=0;			// enable null stripping 
	dcb.fAbortOnError=0;	// abort reads/writes on error 
	dcb.fErrorChar=0;		// enable error replacement 
	dcb.ErrorChar=(char)0xDE; // error replacement character
	//dcb.XonLim;			// transmit XON threshold 
	//dcb.XoffLim;			// transmit XOFF threshold 
	//dcb.XonChar;			// Tx and Rx XON character 
	//dcb.XoffChar;			// Tx and Rx XOFF character 
	//dcb.EofChar;			// end of input character 
	//dcb.EvtChar;			// received event character 
	SetCommState(comm_handle,&dcb);

	
	COMMTIMEOUTS to;
	GetCommTimeouts(comm_handle,&to);
	//to.ReadIntervalTimeout=(mode<2)?50:600;
	to.ReadIntervalTimeout=50;
	//to.ReadTotalTimeoutConstant=3000;
	to.ReadTotalTimeoutConstant=100;
	//to.ReadTotalTimeoutMultiplier=1;
	to.ReadTotalTimeoutMultiplier=0;
	//to.WriteTotalTimeoutMultiplier=;
	//to.WriteTotalTimeoutConstant=;
	SetCommTimeouts(comm_handle,&to);

	SetCommMask(comm_handle,0);

	//Sleep(500);

	return 0;
}

int _stdcall OpenPort_VB(char *port)
{
	return OpenPort(port);
}

int _stdcall ClosePort(void)
{
	if(!comm_handle) return 0;
	CloseHandle(comm_handle);

	comm_handle=NULL;
	return 0;
}

unsigned char init_data[]=			"SPC700 DATA LOADER V1.0\r\n";				//Original Arduino interface startup string
unsigned char init_data_mega[]=		"SPC700 DATA LOADER V1.0, DEBUGGING\r\n";	//For some strange reason, I appended ", DEBUGGING" to the startup string on the mega.
unsigned char sd_arduino_init[]=	"SHVC-SOUND Arduino Player v0.1\r\n";		//Startup string of emu_kidid's firmware derivative.

int _stdcall init_port(char *port)
{
	int i=0;
	clock_t time;
	time = clock();
	BYTE command[2];
	DWORD result;
	if(!comm_handle)
	{
		
		if(OpenPort(port))
			return 1;

		DCB dcb;
		dcb.DCBlength=sizeof(dcb);
		GetCommState(comm_handle,&dcb);

	retry_init_port:
		EscapeCommFunction(comm_handle,SETDTR);
		EscapeCommFunction(comm_handle,SETRTS);
		//sleep(2500);
		do {ReadFile(comm_handle,command,1,&result,NULL); } while ((result != 1) && (clock()-time < 6*CLOCKS_PER_SEC));
		for(i=0;i<25;i++)
		{
			if(((command[0]!=init_data[i])&&
				(command[0]!=init_data_mega[i])&&
				(command[0]!=sd_arduino_init[i]))
				||(result!=1))
			{
				if(dcb.BaudRate == 250000)	//Fallback to 115200 and try again, in case one of the legacy sketches are loaded.
				{
					do {ReadFile(comm_handle,command,1,&result,NULL); } while (result == 1);
					dcb.BaudRate=115200;
					SetCommState(comm_handle,&dcb);
					EscapeCommFunction(comm_handle,CLRDTR);
					EscapeCommFunction(comm_handle,CLRRTS);
					sleep(50);
					goto retry_init_port;
				}
				ClosePort();
				return 2;
			}
			ReadFile(comm_handle,command,1,&result,NULL);
		}
		command[0]='D';
		command[1]='0';
		WriteFile(comm_handle,command,2,&result,NULL);
		if(result!=2)
		{
			ClosePort();
			return 3;
		}

		sleep(1);
	}
	do {ReadFile(comm_handle,command,1,&result,NULL); } while (result == 1);
	
	return 0;
}




int _stdcall WriteSPC700 (int address, int data)
{
	BYTE command[3];
	DWORD result;
	command[0]='W';
	command[1]=address+'0';
	command[2]=data;
	if(comm_handle==NULL)
		return 1;
	WriteFile(comm_handle,command,3,&result,NULL);
	if(result!=3)
		return 2;
	
	return 0;
}

int _stdcall ReadSPC700 (int address)
{
	BYTE command[2];
	static BYTE data[4];
	int i=0;

	command[0]='R';
	command[1]=address+'0';

	if(comm_handle==NULL)
		return -1;
	DWORD result;
	DWORD total_result;

	if(address<4)
	{
		WriteFile(comm_handle,command,2,&result,NULL);
		ReadFile(comm_handle,data,1,&result,NULL);
		while(result==0)
		{
			sleep(1);
			i++;
			if(i>1000)
				return -2;
			ReadFile(comm_handle,data,1,&result,NULL);
		}
		return data[0];
	}
	else
	{
		if((address&3)==0)
		{
			command[0]='Q';
			WriteFile(comm_handle,command,1,&result,NULL);
			ReadFile(comm_handle,data,4,&result,NULL);
			total_result=result;
			while(total_result<4)
			{
				sleep(1);
				i++;
				if(i>1000)
					return -2;
				ReadFile(comm_handle,&data[total_result],4-total_result,&result,NULL);
				total_result+=result;
			}
		}
		return data[address&3];
	}
	return data[0];
}

void  WriteSPC700_16 (int address, int data0, int data1)
{
	WriteSPC700(address, data1);
	WriteSPC700(address, data0);
}

int _stdcall WriteSPC700_WP0I (int address, int data)
{
	int i;
	int result;
	i = 0;
	if(i=WriteSPC700(address, data))
		return i;
	if(i=WriteSPC700(0,port0))
		return i;
	i=0;
	do
	{
		result=ReadSPC700(0);
		if(result==-1)
			return 1;
		i++;
		if (i > 1024)
			return 1;
	} while (result!=port0);
	port0++;
	if (port0 == 256)
		port0 = 0;

	return 0;
	

}

int _stdcall SetPort0 (short data)
{
	int dummy=0;
	BYTE command[2];
	DWORD result;
	command[0]='f';
	command[1]=data;
	
	
	if(comm_handle==NULL)
		return 1;
	WriteFile(comm_handle,command,2,&result,NULL);
	if(result!=2)
		return 2;
	port0=data;
	return 0;
}

void InitTrans (void)
{
	while (ReadSPC700(0x01) != 0xBB);
	WriteSPC700(0x03,0x00);
	WriteSPC700(0x02,0x02);
	WriteSPC700(0x01,0x01);
	WriteSPC700(0x00,0xCC);
	while (ReadSPC700(0x00) != 0xCC);
}

#define MAX_WRITES 4080

int _stdcall StartWrite16bytes(unsigned char addresshigh, unsigned char address_low, unsigned char datahigh, unsigned char datalow)
{
	
	BYTE command[17];
	DWORD result;
	command[0]='F';
	command[1]=addresshigh;
	command[2]=address_low;
	command[3]=datahigh;
	command[4]=datalow;
	if(comm_handle==NULL)
	{
		//num_writes = 0; //Reset the state, as the hardware port was closed early.
		return 1;
	}
	WriteFile(comm_handle,command,5,&result,NULL);
	if(result!=5)
		return 2;
	return 0;

}

int _stdcall FinishWrite16bytes()
{
	DWORD result;
	int i=0;
	if(comm_handle==NULL)
	{
		//num_writes = 0; //Reset the state, as the hardware port was closed early.
		return 1;
	}
	BYTE data[1];
	ReadFile(comm_handle,data,1,&result,NULL);
	while(result==0)
	{
		return 2;
		sleep(1);
		i++;
		if(i>1000)
			return 2;
		ReadFile(comm_handle,data,1,&result,NULL);
	}
	return 0;
}

int _stdcall Write16bytes (unsigned char byte1, unsigned char byte2, char byte3, char byte4, char byte5, char byte6, char byte7, char byte8, char byte9, char byte10, char byte11, char byte12, char byte13, char byte14, char byte15, char byte16)
{
	static int num_writes=0;
	static int max_writes=MAX_WRITES;
	int i=0;
	int dummy=0;
	BYTE command[17];
	DWORD result;
	command[0]='F';
	command[1]=byte1;
	command[2]=byte2;
	command[3]=byte3;
	command[4]=byte4;
	command[5]=byte5;
	command[6]=byte6;
	command[7]=byte7;
	command[8]=byte8;
	command[9]=byte9;
	command[10]=byte10;
	command[11]=byte11;
	command[12]=byte12;
	command[13]=byte13;
	command[14]=byte14;
	command[15]=byte15;
	command[16]=byte16;
	
	if(comm_handle==NULL)
	{
		num_writes = 0; //Reset the state, as the hardware port was closed early.
		return 1;
	}
	WriteFile(comm_handle,&command[1],16,&result,NULL);
	if(result!=16)
		return 2;
	return 0;
}

int _stdcall StartSPC700(unsigned char byte1)
{
	static int num_writes=0;
	int i=0;
	int dummy=0;
	BYTE command[2];
	DWORD result;
	command[0]='G';
	command[1]=byte1;
	
	
	if(comm_handle==NULL)
		return 1;
	if(num_writes == 0)
	{
		WriteFile(comm_handle,command,2,&result,NULL);
		if(result!=2)
			return 1;
	}
	else
	{
		WriteFile(comm_handle,&command[1],1,&result,NULL);
		if(result!=1)
			return 1;
	}

	if(num_writes == 383)
	{
		BYTE data[1];
		ReadFile(comm_handle,data,1,&result,NULL);
		while(result==0)
		{
			sleep(1);
			i++;
			if(i>1000)
				return 1;
			ReadFile(comm_handle,data,1,&result,NULL);
		}
	}
	num_writes++;
	if(num_writes==384)
		num_writes=0;
	return 0;
}

int _stdcall ResetAPU(void)	//Reset the APU by whatever means it needs reset.
{
	if(comm_handle==NULL)
		return 1;

	BYTE command[1];
	DWORD result;
	command[0]='s';

	WriteFile(comm_handle,command,1,&result,NULL);
	if(result!=1)
		return 2;

	while(result!=0)
		ReadFile(comm_handle,command,1,&result,NULL);	//Flush the read buffer completely.
	
	return 0;
}

unsigned char boot_code[] =
{
    0x8F, 0x00, 0x00, 0x8F, 0x00, 0x01, 0x8F, 0xFF, 0xFC, 0x8F, 0xFF, 0xFB, 0x8F, 0x4F, 0xFA, 0x8F, 
    0x31, 0xF1, 0xCD, 0x53, 0xD8, 0xF4, 0xE4, 0xF4, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xF5, 0x68, 0x00, 
    0xD0, 0xFA, 0xE4, 0xF6, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xF7, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xFD, 
    0xE4, 0xFE, 0xE4, 0xFF, 0x8F, 0x6C, 0xF2, 0x8F, 0x00, 0xF3, 0x8F, 0x4C, 0xF2, 0x8F, 0x00, 0xF3, 
    0x8F, 0x7F, 0xF2, 0xCD, 0xF5, 0xBD, 0xE8, 0xFF, 0x8D, 0x00, 0xCD, 0x00, 0x7F, 
} ;


int _stdcall UploadSPC(unsigned char *spcdata, unsigned char *dspdata, unsigned char *spcram, unsigned char A, unsigned char X,
					   unsigned char Y, unsigned char SP, unsigned char SW, unsigned char PCL, unsigned char PCH, unsigned char echo_clear)
{
	int i,j,bootptr;
	int echo_region, echo_size;
	int count;
	if(comm_handle==NULL)
		return 1;

	//Select proper SPC upper 64 ram
	if(spcdata[0xF1] & 0x80)
	{
		for(i=0;i<64;i++)
			spcdata[0xFFC0+i] = spcram[i];
	}
	
	//Clear out echo region
	echo_region = dspdata[0x6D] << 8;
	echo_size = (dspdata[0x7D] & 0x0F) * 2048;
	if(echo_size==0) 
		echo_size=4;

	if((((dspdata[0x6C] & 0x20)==0)&&(echo_clear=0))||(echo_clear==1))
		for (i=echo_region;(i<0x10000)&&(i<echo_region+echo_size);i++)
			spcdata[i]=0;

	//Locate and write the bootloader now.
	for(i=255;i>=0;i--)	
	{
		count=0;
		for(j=0xFFBF;j>=0x100;j--)
		{
			if((j>(echo_region+echo_size))||(j<echo_region))
			{
				if(spcdata[j]=i)
					count++;
				else
					count=0;
				if(count==77)
					break;
			}
			else
			{
				count=0;
			}
		}
		if(count==77)
			break;
	}
	if(j==0xFF)
	{
		if(echo_size<77)
			return -1;	//Not enough SPCram for boot code.
		else
		{
			j=echo_region;
			count=77;
		}
	}
	for(i=j;i<(j+count);i++)
		spcdata[i]=boot_code[i-j];

	spcdata[j+0x19] = spcdata[0xF4];
	spcdata[j+0x1F] = spcdata[0xF5];
	spcdata[j+0x25] = spcdata[0xF6];
	spcdata[j+0x2B] = spcdata[0xF7];
	spcdata[j+0x01] = spcdata[0x00];
	spcdata[j+0x04] = spcdata[0x01];
	spcdata[j+0x07] = spcdata[0xFC];
	spcdata[j+0x0A] = spcdata[0xFB];
	spcdata[j+0x0D] = spcdata[0xFA];
	spcdata[j+0x10] = spcdata[0xF1];
	spcdata[j+0x38] = dspdata[0x6C];
	dspdata[0x6C] = 0x60;
	spcdata[j+0x3E] = dspdata[0x4C];
	dspdata[0x4C] = 0x00;
	spcdata[j+0x41] = spcdata[0xF2];
	spcdata[j+0x44] = SP - 3;
	spcdata[0x100 + SP - 0] = PCH;
	spcdata[0x100 + SP - 1] = PCL;
	spcdata[0x100 + SP - 2] = SW;
	spcdata[j+0x47] = A;
	spcdata[j+0x49] = Y;
	spcdata[j+0x4B] = X;

	bootptr=j;

	ResetAPU();
	for(i=0;i<128;i++)
		StartSPC700(dspdata[i]);
	for(i=0;i<256;i++)
		StartSPC700(spcdata[i]);
	WriteSPC700(1,1);
	WriteSPC700(2,0x00);
	WriteSPC700(3,0x01);
	i=ReadSPC700(0);
	i+=2;
	WriteSPC700(0,i);
	j=0;
	while((ReadSPC700(0)!=i)&&(j<500))
		j++;
	if(j==500)
		return 2;
	SetPort0(0);
	for(i=0x100;i<0x10000;i+=16)
	{
		Write16bytes(spcdata[i],spcdata[i+1],spcdata[i+2],spcdata[i+3],
					 spcdata[i+4],spcdata[i+5],spcdata[i+6],spcdata[i+7],
					 spcdata[i+8],spcdata[i+9],spcdata[i+10],spcdata[i+11],
					 spcdata[i+12],spcdata[i+13],spcdata[i+14],spcdata[i+15]);
	}

	/*FlushRead();
	WriteSPC700(3,bootptr>>8);
	WriteSPC700(2,bootptr&0xFF);
	WriteSPC700(1,0);
	i=ReadSPC700(0);
	i+=2;
	WriteSPC700(0,i);
	while((ReadSPC700(0)!=0x53)&&(j<500))
		j++;
	WriteSPC700(0,spcdata[0xF4]);
	WriteSPC700(1,spcdata[0xF5]);
	WriteSPC700(2,spcdata[0xF6]);
	WriteSPC700(3,spcdata[0xF7]);*/
	
	
	return bootptr;
}

int _stdcall FlushRead(void)	//Reset the APU by whatever means it needs reset.
{
	if(comm_handle==NULL)
		return 1;

	BYTE command[1];
	DWORD result;
	command[0]='s';

	result=1;

	while(result!=0)
		ReadFile(comm_handle,command,1,&result,NULL);	//Flush the read buffer completely.
	
	return 0;
}



int _stdcall GetID6xTag(char ID666tag[], char XID6tag[], short tagID)
{
	
	return ID666tag[0] + ID666tag[1] + ID666tag[2] + ID666tag[3];
}