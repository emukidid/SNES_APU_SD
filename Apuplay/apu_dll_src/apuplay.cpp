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





#include "parport.h"
#include "apuplay.h"

int ByteTransferCount=0;
int ParallelPortCount=0;

int bootcode=0;


int port0=0;

//void InstallPortTalkDriver(void);
//unsigned char StartPortTalkDriver(void);

/*****************************************************************************

Parallel port - 8 data, 4 control, 5 status
data
	D0 - pin2 - MOSI
	D1 - pin3 - SCK
	D6 - pin8 - /CART (pad 49 of cartritge connector)
	D7 - pin9 - /RESET (of Atmel)

status
	S4 - pin13 - MISO
	S5 - pin12 - ready to receive (from atmel)

gnd
	pins 18-25
	connect to ground of board

*********************

SERIAL COMMUNICATION:

MOSI (PC -> Atmel)		Atmel - 1 input, and ParallelPort - 1 output
MISO (PC <- Atmel)		Atmel - 1 output, and ParallelPort - 1 input
SCK  (PC -> Atmel)		Atmel - 1 input, and ParallelPort - 1 output

MSB is sent/received first...

Atmel chip is expected to have the data ready by the time the
the clock goes high (so it should be sitting there, ready with its
first bit on the line before the computer initiates the transfer).
It should transition its data when the clock is low.

The computer is expected to have data on the line by the time it 
puts the clock high.
It should transition its data when the clock is low.

Clock up and clock down must not be less than 2 clock cycles.
(not a problem ... one bit set takes less 1 us ... 8 clock cycles).

Start up state... SCK is low.

The sequence 0x01, 0x02 ... 0x0F is sent and received to make sure the systems 
are synchronized.  This is needed because the Atmel chip may be running before the 
parallel port gets initialized.  This will prevent any false commands when the 
parallelport is being plugged in, etc.

************************************************************************/

/******************** D E F I N I T I O N S ****************************/

#define OUTPUT_PIN	0x01
#define SCK_PIN		0x02
#define CART_PIN	0x40
#define RESET_PIN	0x80

#define _RD_PIN		0x08
#define WR_PIN		0x04
#define _ADDR1		0x02
#define _ADDR0		0x01

#define INPUT_PIN	0x10
#define RDY_PIN		0x20

/******************** S T R U C T U R E S ******************************/


/******************** G L O B A L S ************************************/


int ready_line=0;     //holds the last state of the "ready to receive line"

/******************** F U N C T I O N S ********************************/

void sleep(int msec)
{
	clock_t time;

	//wait for a number of msec.
	time = clock();
	while ( clock()-time < 0.001*msec*CLOCKS_PER_SEC) {}
} //end of sleep()


int xfer_byte(int send_data)
{
	int i;
	int byte=0,temp;
	int data_in=0;

	/*****************************************************************
	;xfer_byte - uses SPI, Serial Communication
	;Input: 
	;	data to send to Atmel chip
	;
	;Returns: 
	;	data received from Atmel chip
	;
	;Note:
	;Expects MSB first...
	;
	;Atmel chip is expected to have the data ready by the time the
	;the clock goes high (so it should be sitting there, ready with its
	;first bit on the line before the computer initiates the transfer).
	;It should transition its data when the clock is low.
	;
	;The computer is expected to have data on the line by the time it 
	;puts the clock high.
	;It should transition its data when the clock is low.
	;
	;*****************************************************************/

	for(i=0x80; i!=0x00; i=(i>>1) )
	{
		ParallelPortCount+=4;

		ClrPins(data_pins,SCK_PIN);  //put clock low

		if(send_data & i)
			SetPins(data_pins,OUTPUT_PIN);	//put MOSI (data out) high
		else
			ClrPins(data_pins,OUTPUT_PIN);  //put MOSI (data out) low

		_outp(DATA,data_pins);		//send data + Clock low

		SetPins(data_pins,SCK_PIN);	//put clock high

//		_outp(DATA,data_pins);		//send data + Clock high
		_outp(DATA,data_pins);		//send again (as a kind of pause)

		//get bit from Atmel chip...
		temp=_inp(STATUS);
		if(temp & INPUT_PIN)
			data_in += i;
		
		ClrPins(data_pins,SCK_PIN);	//put clock low
	//	_outp(DATA,data_pins);		//send data, clock low
	}
	_outp(DATA,data_pins);
	ByteTransferCount+=1;
	return data_in;
} //end of xfer_byte()

int xfer_byte_ready(int send_data)
{
	int data_in;
	clock_t time;

	/*****************************************************************
	;Input:
	; 	data to send to Atmel chip
	;
	;Returns: 
	;	data received from Atmel chip
	;
	;Note:
	;Sends data using xfer_byte() but waits for "ready to send" signal
	;from the Atmel chip first.
	;
	;Will not send/receive byte until the "ready to send" line from the 
	;Atmel chip is toggled.  First byte should not be sent until this
	;line is set high.
	;
	;*****************************************************************/

	//wait for Atmel to be ready ... time out after 250 ms
	time = clock();
	while( ready_line == (_inp(STATUS) & RDY_PIN) )
	{
		ParallelPortCount+=1;
		if(clock()-time > 1.250*CLOCKS_PER_SEC)
		{
//			printf("\nERROR: Atmel timed out.  ");
//			printf("No Ready-to-Receive signal was sent.\n");
			return -1;		
		}
	}
	
	ready_line = (_inp(STATUS) & RDY_PIN);

	data_in = xfer_byte(send_data);

	return data_in;
} //end of xfer_byte_ready()

void ResetChip(void)
{
	//reset the Atmel chip
//	ClrPins(data_pins, RESET_PIN + SCK_PIN + OUTPUT_PIN);
//	_outp(DATA, data_pins);

	ClrPins(data_pins, SCK_PIN + OUTPUT_PIN);
	_outp(DATA, data_pins);
	_outp(DATA, data_pins);
	ClrPins(data_pins, RESET_PIN);
	_outp(DATA, data_pins);


	//wait about 50 msec.
	sleep(250);

	//turn Atmel on
	SetPins(data_pins,RESET_PIN);	//put #RESET high
	_outp(DATA, data_pins);


	//wait about 50 msec for it to setup..
	sleep(250);
} //end of ResetChip()


int _stdcall InitSerialSend(void)
{
	int i, data_in;

	//reset the Atmel chip
	ResetChip();

	//initialize the serial send
	ready_line=0;
	for(i=0x01;i<=0x0F;i++)
	{
		data_in=xfer_byte_ready(i);
		if(i!=data_in)
		{
//			printf("\nERROR: Serial initialization failed.  ");
//			printf("Sent: 0x%.2X    Received: 0x%.2X\n", i, data_in);
/*
			if (data_in==0xFF)
			{
//				printf("\nIs the Device Connected to the parallel port?");
//				printf("\nIs the Device connected to power?");
//				printf("\nIs the Power turned on?");
			}

			if (data_in==0x00)
			{
				printf("\nIs MOSI/MISO/SCK shorted to ground?");
			}

*/
			return 0;
		}
	}

	return -1;
} //end of InitSerialSend()

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
	dcb.BaudRate=115200;	// bit rate 
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

	//EscapeCommFunction(comm_handle,CLRDTR);
	//EscapeCommFunction(comm_handle,CLRRTS);

	CloseHandle(comm_handle);

	comm_handle=NULL;
	return 0;

}

unsigned char init_data[]={
	'S','P','C','7',
	'0','0',' ','D',
	'A','T','A',' ',
	'L','O','A','D',
	'E','R',' ','V',
	'1','.','0',0x0D,
	0x0A,0x00
};

unsigned char sd_arduino_init[]={
	'S','H','V','C',
	'-','S','O','U',
	'N','D',' ','A',
	'r','d','u','i',
	'n','o',' ','P',
	'l','a','y','e',
	'r',' ','v','0',
	'.','1',0x0D,0x0A,
	0x00
};

int _stdcall init_port(char *port)
{
	int i;
	BYTE command[2];
	DWORD result;
	if(!comm_handle)
	{
		
		if(OpenPort(port))
			return 1;
		EscapeCommFunction(comm_handle,SETDTR);
		EscapeCommFunction(comm_handle,SETRTS);
		sleep(2500);
		for(i=0;i<25;i++)
		{
			ReadFile(comm_handle,command,1,&result,NULL);
			if(((command[0]!=init_data[i])&&(command[0]!=sd_arduino_init[i]))||(result!=1))
			{
				//ClosePort();
				//return 2;
				//ReadFile(comm_handle,command,40,&result,NULL);
				while((command[0]!=0)&&(result==1))
					ReadFile(comm_handle,command,1,&result,NULL);	//Fix for a mysterious ", DEBUG" string in the startup.
				command[0]='S';
				WriteFile(comm_handle,command,1,&result,NULL);
				if(result!=1)
				{
					ClosePort();
					return 3;
				}
				for(i=0;i<25;i++)
				{
					ReadFile(comm_handle,command,1,&result,NULL);
					if((command[0]!=init_data[i])||(result!=1))
					{
						ClosePort();
						if(i==0)
							return 2;
						else
							return 3+i;
					}
				}
				break;
			}
		}
		command[0]='D';
		command[1]='0';
		WriteFile(comm_handle,command,2,&result,NULL);
		if(result!=2)
		{
			ClosePort();
			return 3;
		}
		while((command[0]!=0)&&(result==1))
			ReadFile(comm_handle,command,1,&result,NULL);	//SHVC-SOUND string is longer than 25 characters. Finish reading it out.

		sleep(1);
	}
	
	return 0;
}




int _stdcall WriteSPC700 (int address, int data)
{
	int dummy=0;
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
	

	
/*
	SetPins(data_pins, RESET_PIN);
	ClrPins(data_pins, SCK_PIN);

	address = (address << 2) + 0x02;
	xfer_byte_ready ( address );
	xfer_byte_ready ( data ); 
*/

	/*

	ClrPins(control_pins, INPUT_MODE);
		_outp(CONTROL, control_pins);

	SetPins(control_pins, _ADDR0 + _ADDR1);
	ClrPins(control_pins, address);
	_outp(CONTROL, control_pins);
	ClrPins(control_pins, WR_PIN);
	ClrPins(control_pins, _RD_PIN);
	
	_outp(CONTROL, control_pins);

	_outp(DATA, data);

	SetPins(control_pins, WR_PIN);
	_outp(CONTROL, control_pins);*/


	return dummy;
}
/*
int _stdcall WriteCompressedByte(unsigned char byte1,unsigned char byte2,unsigned char byte3,unsigned char byte4, int len)
{
	static int num_writes=0;
	int i=0;
	int dummy=0;
	BYTE command[5];
	DWORD result;
	command[0]='F';
	command[1]=byte1;
	command[2]=byte2;
	command[3]=byte3;
	command[4]=byte4;
	
	if(comm_handle==NULL)
	{
		num_writes = 0; //Reset the state, as the hardware port was closed early.
		return 1;
	}
	if(num_writes == 0)
	{
		WriteFile(comm_handle,command,5,&result,NULL);
		if(result!=5)
			return 2;
	}
	else
	{
		WriteFile(comm_handle,&command[1],4,&result,NULL);
		if(result!=4)
			return 2;
	}

	if(num_writes >= (len-4))
	{
		BYTE data[1];
		ReadFile(comm_handle,data,1,&result,NULL);
		while(result==0)
		{
			sleep(1);
			i++;
			if(i>1000)
				return 2;
			ReadFile(comm_handle,data,1,&result,NULL);
		}
		//while(result!=0)
		//	ReadFile(comm_handle,data,1,&result,NULL);
	}
	num_writes+=4;
	if(num_writes>=len)
		num_writes=0;
	return 0;
}*/

int _stdcall ReadSPC700 (int address)
{
	
	
	/*
	SetPins(data_pins, RESET_PIN);
	ClrPins(data_pins, SCK_PIN);

	address = (address << 2) + 0x02;
	if (xfer_byte_ready ( address + 0x80 )==-1)
	{
		TogglePins(ready_line,RDY_PIN);
		if (xfer_byte_ready ( address + 0x80 )==-1)
		{
			return -1;
		}
	}
	return xfer_byte_ready ( 0 ); 
/*/
	/*if (!(control_pins & INPUT_MODE))
	{ 
		SetPins(control_pins, _ADDR0 + _ADDR1 + INPUT_MODE);
	}
	else
	{
		SetPins(control_pins, _ADDR0 + _ADDR1);
	} 
	SetPins(control_pins, WR_PIN);
	//SetPins(control_pins, _ADDR0 + _ADDR1);
	ClrPins(control_pins, address);
	//_outp(CONTROL, control_pins);
	SetPins(control_pins, _RD_PIN);
	_outp(CONTROL, control_pins);

	data=_inp(DATA);
	//ClrPins(control_pins, _RD_PIN + INPUT_MODE);
	ClrPins(control_pins, _RD_PIN);
	_outp(CONTROL, control_pins);*/
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
	/*
	address=(address<<2)+0x02;

	xfer_byte_ready ( address + 0x40 );
	xfer_byte_ready ( data0 );
	xfer_byte_ready ( data1 ); 
	/*/
	WriteSPC700(address, data1);
	WriteSPC700(address, data0);

}

int _stdcall WriteSPC700_WP0I (int address, int data)
{
	/*
	address=(address<<2)+0x02;
	xfer_byte_ready ( address + 0x20 );
	return xfer_byte_ready ( data ); 

	/*/
	int i;
	int result;
	i = 0;
	if(i=WriteSPC700(address, data))
		return i;
	if(i=WriteSPC700(0,port0))
		return i;
	i=0;
	//while (ReadSPC700(0)!=port0)
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

int WrSPC700_WP0I (int address, int data)
{
	address=(address<<2)+0x02;
	xfer_byte_ready ( address + 0x20 );
	return xfer_byte_ready ( data );

}

int _stdcall SetPort0 (short data)
{
	
//	xfer_byte_ready ( 0x10 );
//	return xfer_byte_ready ( data );
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
//	WriteSPC700_16(0x02,0x02,0x00);
	WriteSPC700(0x01,0x01);
	WriteSPC700(0x00,0xCC);
//	WriteSPC700_16(0x00,0xCC,0x01);
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
	/*
	int address=1;
	int xfer_error;
	address=(address<<2)+0x02;
	xfer_error = xfer_byte_ready ( address + 0x01 );
	if (xfer_error == -1)
		return xfer_error;
	xfer_byte_ready ( byte1 );
	xfer_byte_ready ( byte2 );
	xfer_byte_ready ( byte3 );
	xfer_byte_ready ( byte4 );
	xfer_byte_ready ( byte5 );
	xfer_byte_ready ( byte6 );
	xfer_byte_ready ( byte7 );
	xfer_byte_ready ( byte8 );
	xfer_byte_ready ( byte9 );
	xfer_byte_ready ( byte10 );
	xfer_byte_ready ( byte11 );
	xfer_byte_ready ( byte12 );
	xfer_byte_ready ( byte13 );
	xfer_byte_ready ( byte14 );
	xfer_byte_ready ( byte15 );
	return xfer_byte_ready ( byte16 ); 

	/*/

	/*int i;
	i = 0;
	i+=WriteSPC700_WP0I (1, byte1);
	i+=WriteSPC700_WP0I (1, byte2);
	i+=WriteSPC700_WP0I (1, byte3);
	i+=WriteSPC700_WP0I (1, byte4);

	if (i > 0)
		return i;
	
	i+=WriteSPC700_WP0I (1, byte5);
	i+=WriteSPC700_WP0I (1, byte6);
	i+=WriteSPC700_WP0I (1, byte7);
	i+=WriteSPC700_WP0I (1, byte8);

	if (i > 0)
		return i;
	
	i+=WriteSPC700_WP0I (1, byte9);
	i+=WriteSPC700_WP0I (1, byte10);
	i+=WriteSPC700_WP0I (1, byte11);
	i+=WriteSPC700_WP0I (1, byte12);

	if (i > 0)
		return i;
	
	i+=WriteSPC700_WP0I (1, byte13);
	i+=WriteSPC700_WP0I (1, byte14);
	i+=WriteSPC700_WP0I (1, byte15);
	i+=WriteSPC700_WP0I (1, byte16);
	
	return i;*/


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
	/*if(num_writes == 0)
	{
		max_writes=((byte3<<8)|(byte4))>>4;
		max_writes++;
		WriteFile(comm_handle,command,5,&result,NULL);
		if(result!=5)
			return 2;
	}
	else
	{*/
		WriteFile(comm_handle,&command[1],16,&result,NULL);
		if(result!=16)
			return 2;
	//}

	/*if(num_writes == (max_writes-1))
	{
		BYTE data[1];
		ReadFile(comm_handle,data,1,&result,NULL);
		while(result==0)
		{
			sleep(1);
			i++;
			if(i>1000)
				return 2;
			ReadFile(comm_handle,data,1,&result,NULL);
		}
		//while(result!=0)
		//	ReadFile(comm_handle,data,1,&result,NULL);
	}
	num_writes++;
	if(num_writes==max_writes)
		num_writes=0;
	port0+=16;*/
	return 0;
	
	return i; 
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
	
	return i; 
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


	/*
	ClrPins(control_pins,WR_PIN);		//Once the OR gate is in place
	SetPins(control_pins,_RD_PIN);		//Pulling both /RD and /WR low
		_outp(CONTROL, control_pins);	//Will Reset the APU, since
	sleep(50);							// /RESET will be tied to the
	SetPins(control_pins,WR_PIN);		//output of the OR gate.
	ClrPins(control_pins,_RD_PIN);
		_outp(CONTROL, control_pins);
	sleep(50);*/
	
	return 0;
	return InitSerialSend();
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
	//return 0;
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


	/*
	ClrPins(control_pins,WR_PIN);		//Once the OR gate is in place
	SetPins(control_pins,_RD_PIN);		//Pulling both /RD and /WR low
		_outp(CONTROL, control_pins);	//Will Reset the APU, since
	sleep(50);							// /RESET will be tied to the
	SetPins(control_pins,WR_PIN);		//output of the OR gate.
	ClrPins(control_pins,_RD_PIN);
		_outp(CONTROL, control_pins);
	sleep(50);*/
	
	return 0;
	return InitSerialSend();
}



int _stdcall GetID6xTag(char ID666tag[], char XID6tag[], short tagID)
{
	
	return ID666tag[0] + ID666tag[1] + ID666tag[2] + ID666tag[3];
}