/****
 * SHVC-SOUND Arduino Player by emu_kidid in 2015
 * based on code by CaitSith2
 * 
 * Loads SPC tracks from SD to the SHVC-SOUND (aka SNES APU)
 */

#define RESET_PIN 54

#include <SPI.h>
//#include <SD.h>
#include <SdFat.h>
SdFat SD;
#include <Firmata.h>
#include "LCD.h"  //Using it for the defines only.

FILE serial_stdout;
File spcFile;
static unsigned char spcdata[256];           // 0x100  (256 bytes (of 64kb)) (header)
static unsigned char spcbuf[256];           // 0x100  (256 bytes (of 64kb)) (temp buffer)
static unsigned char dspdata[128];           // 0x10100  (128 bytes)

static unsigned char DSPdata[16] =
{  //For loading the 128 byte DSP ram. DO NOT CHANGE.
  0xC4, 0xF2, 0x64, 0xF4, 0xD0, 0xFC, 0xFA, 0xF5, 0xF3, 0xC4, 0xF4, 0xBC, 0x10, 0xF2, 0x2F, 0xB7, 
};

static unsigned char boot_code[] =
{
    0x8F, 0x00, 0x00, 0x8F, 0x00, 0x01, 0x8F, 0xFF, 0xFC, 0x8F, 0xFF, 0xFB, 0x8F, 0x4F, 0xFA, 0x8F, 
    0x31, 0xF1, 0xCD, 0x53, 0xD8, 0xF4, 0xE4, 0xF4, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xF5, 0x68, 0x00, 
    0xD0, 0xFA, 0xE4, 0xF6, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xF7, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xFD, 
    0xE4, 0xFE, 0xE4, 0xFF, 0x8F, 0x6C, 0xF2, 0x8F, 0x00, 0xF3, 0x8F, 0x4C, 0xF2, 0x8F, 0x00, 0xF3, 
    0x8F, 0x7F, 0xF2, 0xCD, 0xF5, 0xBD, 0xE8, 0xFF, 0x8D, 0x00, 0xCD, 0x00, 0x7F, 
} ;

// Function that printf and related will use to print
int serial_putchar(char c, FILE* f) {
    if (c == '\n') serial_putchar('\r', f);
    return Serial.write(c) == 1? 0 : 1;
}

void setup()
{
  PORTH = 0x60;
  DDRH = 0x03;  //Digital pins 8-9.
  PORTB = 0x30;
  DDRB |= 0x30;  //Digital pins 10-11
  PORTF = 0xFF;
  DDRF = 0xFF; //Analog Pin 0
  Serial.begin(250000);

   // Set up stdout
  fdev_setup_stream(&serial_stdout, serial_putchar, NULL, _FDEV_SETUP_WRITE);
  stdout = &serial_stdout;
  
  Serial.println("SHVC-SOUND Arduino Player v0.1");
  delay(3500);  //
  if (Serial.available()) return;
  
  printf("Initializing SD card...\n");

  if (!SD.begin(53)) {
    printf("initialization failed!\n");
    return;
  }
  printf("initialization done.\n");

  if (SD.exists("file.spc")) {
    printf("file.spc exists.\n");
  }
  else {
    printf("file.spc doesn't exist.\n");
  }

  // open a new file and immediately close it:
  printf("Opening file.spc...\n");
  spcFile = SD.open("file.spc", FILE_READ);
  LoadAndPlaySPC();
}


unsigned char ReadByteFromAPU(unsigned char address)
{  
  unsigned char data = 0;
  DDRB |= 0x30;    //APU RD/WR
  DDRH |= 0x60;    //APU A0-A1
  DDRB &= ~0xC0;  //APU D0-D1
  DDRE &= ~0x38;  //APU D2-D3, D5
  DDRG &= ~0x20;  //APU D4
  DDRH &= ~0x18;  //APU D6-D7
  PORTH &= ~0x60;
  PORTH |= ((address & 0x03)<<5);

  PORTB &= ~0x10;
  __asm__ __volatile__ ("nop");
  __asm__ __volatile__ ("nop");
  data=((PINB&0xC0)>>6) | ((PINE&0x30)>>2) | ((PING&0x20)>>1) | ((PINE&0x08)<<2) | ((PINH&0x18)<<3);
  PORTB |= 0x10;
  return data;
}

void WriteByteToAPU(unsigned char address, unsigned char data)
{
  DDRB |= 0xF0;
  DDRE |= 0x38;
  DDRG |= 0x20;
  DDRH |= 0x18;

  PORTB &= ~0xC0;
  PORTB |= ((data & 0x03)<<6);
  
  PORTE &= ~0x38;
  PORTE |= ((data & 0x0C)<<2);
  
  PORTG &= ~0x20;
  PORTG |= ((data&0x10)<<1);
  
  PORTE |= ((data & 0x20)>>2);
  
  PORTH &= ~0x78;
  PORTH |= ((address & 0x03)<<5);
  PORTH |= ((data&0xC0)>>3);
  
  PORTB &= ~0x20;
  __asm__ __volatile__ ("nop");
  __asm__ __volatile__ ("nop");
  PORTB |= 0x20;
  
}

/* IO Pin Mapping.
 ** Digital Pin 0 - RX
 ** Digital Pin 1 - TX
 ** Digital Pin 2-7 - SNES APU D2-7
 ** Digital Pin 8 - SNES APU address 0
 ** Digital Pin 9 - SNES APU address 1
 ** Digital Pin 10 - SNES APU /RD
 ** Digital Pin 11 - SNES APU /WR
 ** Digital Pin 12-13 - SNES APU D0-1
 ** Analog Pin 0 (AKA Digital Pin 54) - SNES APU /RESET
 ** Vcc - SNES APU address 6, SNES APU Vcc, SNES APU audio Vcc
 ** Gnd - SNES APU Gnd, SNES APU audio Gnd.
 */

void APU_StartWrite(unsigned short address, unsigned char *data, int len)
{
  int i;
  uint8_t rdata;
  digitalWrite(RESET_PIN,LOW);
  delay(1);
  digitalWrite(RESET_PIN,HIGH);

  while(ReadByteFromAPU(0)!=0xAA);
  while(ReadByteFromAPU(1)!=0xBB);


  WriteByteToAPU(3,address>>8);
  WriteByteToAPU(2,address&0xFF);
  WriteByteToAPU(1,1);
  WriteByteToAPU(0,0xCC);
  while(ReadByteFromAPU(0)!=0xCC);

  for(i=0;i<len;i++)
  {
    WriteByteToAPU(1,data[i]);
    WriteByteToAPU(0,i&0xFF);
    while(ReadByteFromAPU(0)!=(i&0xFF));
  }
}

void APU_Reset()
{
  printf("APU Reset\n");
  digitalWrite(RESET_PIN,LOW);
  delay(1);
  digitalWrite(RESET_PIN,HIGH);
}

void APU_Wait(unsigned char address, unsigned char data)
{
  while(ReadByteFromAPU(address)!=data);
}

void APU_StartSPC700(unsigned char *dspdata, unsigned char *spcdata, int SD_mode=1)
{
  //'G'
  int port0state = 0, i = 0;

  if (SD_mode) printf("Uploading DSP Register\n");
  //Upload the DSP register, and the first page of SPC data at serial port speed.
  APU_StartWrite(0x0002,DSPdata,16);
  WriteByteToAPU(2,0x02);
  WriteByteToAPU(3,0x00);
  WriteByteToAPU(1,0x00);
  WriteByteToAPU(0,0x11);
  while(ReadByteFromAPU(0)!=0x11); 
  if (SD_mode) 
  {
    printf("Done\n"); 
    printf("Uploading dspdata\n");
  } 
  // Send dspdata[0] to dspdata[127]
  for(i=0;i<128;i++)
  {
    if(i == 0x4C)
      WriteByteToAPU(1,0x00);
    else if(i == 0x6C)
      WriteByteToAPU(1,0x60);
    else
      WriteByteToAPU(1,dspdata[i]);
    WriteByteToAPU(0,port0state);
    if(i<127)
        while(ReadByteFromAPU(0)!=port0state);
    port0state++;
  }
  while(ReadByteFromAPU(0)!=0xAA);
  if (SD_mode) 
  {
    printf("Done\n"); 
    printf("Uploading spcdata\n"); 
  }
  port0state=0;
  WriteByteToAPU(2,0x02);
  WriteByteToAPU(3,0x00);
  WriteByteToAPU(1,0x01);
  WriteByteToAPU(0,0xCC);
  while(ReadByteFromAPU(0)!=0xCC);
  // Send spcdata[0] to spcdata[255]
  for(i=0;i<256;i++)
  {
    if(i<2)
      continue;
    if(i>=0xF0)
      continue;
    WriteByteToAPU(1,spcdata[i]);
    WriteByteToAPU(0,port0state);
    while(ReadByteFromAPU(0)!=port0state);
    port0state++;
  }
  if (SD_mode) printf("Done\n"); 
}

//Write a single IO port.
void APU_WriteSPC700(unsigned short address, unsigned char data) 
{
    switch(address>>2)
    {
      case 1:  //Multiple games use this song select driver.
        while(ReadByteFromAPU(1)!=((ReadByteFromAPU(0)+1)&0xFF));
        break;
      default:    //Any other driver selection not put here.
        WriteByteToAPU(address,data);
        break;
    }
}

void WriteByteToAPUAndWaitForState(unsigned char data, unsigned char state) {
 // printf("Byte %02X\n", data);
  WriteByteToAPU(1, data);
  WriteByteToAPU(0, state);
  while(ReadByteFromAPU(0)!=state);
}

// APU_WaitIoPort
int APU_WaitIoPort(uint8_t address, uint8_t data, int timeout)
{
  int i=0;
  while((ReadByteFromAPU(address)!=data)&&(i<timeout)) i++;
  if(i<timeout)
    return 0;
  return 1;
}

// Reads raw from a SPC file
unsigned char readSPC(long addr)
{
  spcFile.seek(addr);
  return spcFile.read();
}

// Reads raw from the SPC File
void readSPCRegion(unsigned char *buf, long addr, unsigned short len)
{
  spcFile.seek(addr);
  spcFile.read(buf, len);
}

// Reads from the spcdata region of a file (adjusts for header)
unsigned char readSPCData(unsigned short addr)
{
  if(spcFile.position() != (unsigned long)(addr+0x100)) {
    if(!spcFile.seek((unsigned long)(addr+0x100))) { // Adjust for SPC Header
      printf("Seek failed at addr %08X\n",(unsigned long)(addr+0x100));
      while(1);
    }
  }
  return spcFile.read();
}

// Clears a buffer (memset)
void clearBuffer(unsigned char *buf, long len)
{
  int i = 0;
  for(i = 0; i < len; i++) {
    buf[i] = 0;
  }
}

void LoadAndPlaySPC()
{
  unsigned char PCL = readSPC(0x25);    // 0x25     (1 byte)
  unsigned char PCH = readSPC(0x26);    // 0x26     (1 byte)
  unsigned char A = readSPC(0x27);      // 0x27     (1 byte)
  unsigned char X = readSPC(0x28);      // 0x28     (1 byte)
  unsigned char Y = readSPC(0x29);      // 0x29     (1 byte)
  unsigned char ApuSW = readSPC(0x2A);  // 0x2A     (1 byte)
  unsigned char ApuSP = readSPC(0x2B);  // 0x2B     (1 byte)
  unsigned char echo_clear = 1;

  // Read some ID tag stuff
  unsigned char tagBuffer[64];
  clearBuffer(&tagBuffer[0], 64);
  readSPCRegion(&tagBuffer[0], 0x2EL, 0x20);
  printf("SPC Track Name: %s\n",&tagBuffer[0]);
  clearBuffer(&tagBuffer[0], 64);
  readSPCRegion(&tagBuffer[0], 0x4EL, 0x20);
  printf("SPC Track Game: %s\n",&tagBuffer[0]);

  // Read some stuff
  readSPCRegion(&spcdata[0], 0x100L, 256);
  readSPCRegion(&dspdata[0], 0x10100L, 128);
  
  unsigned short i,j,bootptr,spcinportiszero = 0;
  unsigned short count;

  // SPC In Port Is Zero?
  spcinportiszero = (!spcdata[0xF4] && !spcdata[0xF5] && !spcdata[0xF6] && !spcdata[0xF7]);

  //Calculate out echo region
  unsigned short echo_region = dspdata[0x6D] << 8;
  unsigned short echo_size = (dspdata[0x7D] & 0x0F) * 2048;
  if(echo_size==0) 
    echo_size=4;

  printf("SPC echo region is: %04X\n", echo_region);
  printf("SPC echo size is: %04X\n", echo_size);

  //Locate a spot to write the bootloader now.
  int freespacesearch = 0;
  if(freespacesearch < 2)
  {
    for(i=255;i!=0xFFFF;i--) 
    {
      count=0;
      readSPCRegion(&spcbuf[0], 0xFF00+0x100, 256);
      for(j=0xFFBF;j>=0x100;j--)
      {
        if((j>(echo_region+(echo_size-1)))||(j<echo_region))
        {
          if((j&0xFF)==0xFF)
            readSPCRegion(&spcbuf[0], (j&0xFF00)+0x100, 256);
          if(spcbuf[j&0xFF]==(i&0xFF))
            count++;
          else
            count=0;
          if(count==sizeof(boot_code))
            break;
        }
        else
        {
          count=0;
        }
      }
      if(count==sizeof(boot_code)) {
        printf("Type 1 search found enough\n");
        break;
      }
    }
  }
  
  if(freespacesearch == 0 || freespacesearch == 2)
  {
    if(count!=sizeof(boot_code))
    {
      if(echo_size < sizeof(boot_code) || echo_region == 0)
      {
        count = 0;
      }
      else 
      {
        count = sizeof(boot_code);
        j = echo_region;
        printf("Type 2 search found enough\n");
      }
    }
  }
  if(freespacesearch = 0 || freespacesearch == 3)
  {
    if(count != sizeof(boot_code))
    {
      readSPCRegion(&spcbuf[0], 0xFF00+0x100, 256);
      for(j = 0xFFBF; j >= 0x100; j--)
      {
        if((j&0xFF)==0xFF)
            readSPCRegion(&spcbuf[0], (j&0xFF00)+0x100, 256);
        if(((j % 64) > 31) && readSPCData(j)==0xFF) 
        {
          count++;
        }
        else if(((j % 64) <= 31) && readSPCData(j)==0) 
        {
          count++;
        }
        else
        {
          count = 0;
        }
        if(count == sizeof(boot_code)) {
          printf("Type 3 search found enough\n");
          break;
        }
      }
    }
  }
  if(count != sizeof(boot_code))
  {
    printf("Couldn't find good spot for bootloader!\n");
    count = sizeof(boot_code);
    j = 0xFF00;
  }

  unsigned short boot_code_dest = j;
  unsigned short boot_code_size = sizeof(boot_code);
  printf("boot_code_dest: %04X\n", boot_code_dest);
  printf("boot_code_size: %04X\n", boot_code_size);

  // Adjust the boot code with values from this SPC
  boot_code[0x19] = spcdata[0xF4] + (spcinportiszero ? 1:0);  // Inport 0
  boot_code[0x1F] = spcdata[0xF5];  // Inport 1
  boot_code[0x25] = spcdata[0xF6];  // Inport 2
  boot_code[0x2B] = spcdata[0xF7];  // Inport 3
  boot_code[0x01] = spcdata[0x00];  // SPCRam Address 0x0000
  boot_code[0x04] = spcdata[0x01];  // SPCRam Address 0x0001
  boot_code[0x07] = spcdata[0xFC];  // Timer 2
  boot_code[0x0A] = spcdata[0xFB];  // Timer 1
  boot_code[0x0D] = spcdata[0xFA];  // Timer 0
  boot_code[0x10] = spcdata[0xF1];  // Control Register
  boot_code[0x38] = dspdata[0x6C];      // DSP Echo Control Register
  boot_code[0x3E] = dspdata[0x4C];      // DSP KeyON Register
  boot_code[0x41] = spcdata[0xF2];  // Current DSP Register Address
  boot_code[0x47] = A;  // A Register
  boot_code[0x49] = Y;  // Y Register
  boot_code[0x4B] = X;  // X Register
  if(ApuSP < 3)
    boot_code[0x44] = ApuSP + 0x100-3;  // Stack Pointer
  else
    boot_code[0x44] = ApuSP - 3;        // Stack Pointer

  // Reset the APU
  int resetAttempts = 0;
  APU_Reset();
  delay(50);
  while(ReadByteFromAPU(0) != 0xAA || ReadByteFromAPU(1) != 0xBB
     || ReadByteFromAPU(2) != 0x00 || ReadByteFromAPU(3) != 0x00)
  {
    APU_Reset();
    delay(50);
    if(resetAttempts > 20)
    {
      printf("Failed to reset the APU\n");
      while(1);
    }
    resetAttempts++;
  }
  printf("APU Reset Complete\n");

  // Initialise it with the DSP data and SPC data for this track
  APU_StartSPC700(dspdata, spcdata); 
  printf("Time to upload!\n");
  printf("ApuSP %08X\n",ApuSP);

  // Here we upload the SPC, tip-toe around regions where we can't just stream raw from file  
  // First we must set the write address to 0x100
  WriteByteToAPU(1,1);
  WriteByteToAPU(2,0x00);
  WriteByteToAPU(3,0x01);
  int rb=ReadByteFromAPU(0);
  rb+=2;
  WriteByteToAPU(0,rb&0xFF);
  j=0;
  while((ReadByteFromAPU(0)!=rb)&&(j<500))
    j++;
  if(j==500){
    printf("Error setting next 16 byte write up for %04X\n", i);
    return;
  }
  // Write out the SPC Data (echo, boot_code, etc are interleaved here into the stream)
  for(i=0x100;i!=0;i++) // Thank you, overflow :)
  {
    if((i % 0x100) == 0) {
      // Clear out echo region instead of copying it
      if(((i >= echo_region) && (i <= echo_region+(echo_size-1))) && 
          ((((dspdata[0x6C] & 0x20)==0)&&(echo_clear==0))||(echo_clear==1)))
      {
        if (echo_size == 4)
        {
          readSPCRegion(&spcbuf[0], i+0x100, 256);
          spcbuf[0]=spcbuf[1]=spcbuf[2]=spcbuf[3]=0;
        }
        else
        {
          clearBuffer(&spcbuf[0],256);
        }
        if(i == echo_region)
          printf("Write echo start: %04X\n", i);
        if(i == ((echo_region+echo_size-1) & 0xFF00))
          printf("Write echo end: %04X\n", (echo_region+echo_size-1));
      }
      else if (i == 0x100)
      {
        // Prepare the stack
        readSPCRegion(&spcbuf[0], i+0x100, 256);
        spcbuf[(ApuSP + 0x00) & 0xFF] = PCH;
        spcbuf[(ApuSP + 0xFF) & 0xFF] = PCL;
        spcbuf[(ApuSP + 0xFE) & 0xFF] = ApuSW;
        printf("Write PCH: %04X=%02X\n", i, PCH);   // Program Counter High Address
        printf("Write PCL: %04X=%02X\n", i, PCL);   // Program Counter Low Address
        printf("Write PSW: %04X=%02X\n", i, ApuSW); // Program Status Word
      }
      else if (i == 0xFF00)
      {
        // Select proper SPC upper 64 RAM
        if (spcdata[0xF1] & 0x80)
        {
          readSPCRegion(&spcbuf[0], i+0x100, 192);
          readSPCRegion(&spcbuf[0xC0],0x101C0,64);
          printf("Write spcram: FFC0-FFFF\n");
        }
        else
        {
          readSPCRegion(&spcbuf[0], i+0x100, 256);
        }
      }
      else
      { 
        readSPCRegion(&spcbuf[0], i+0x100, 256);
      }
    }
    unsigned char port0state = i & 0xFF;

    // Boot code section
    if((i >= boot_code_dest) && (i < (boot_code_dest+boot_code_size))) {
      WriteByteToAPUAndWaitForState(boot_code[i-boot_code_dest], port0state);     
      if(i == boot_code_dest)
        printf("Write bootcode start: %04X\n", i);
      if(i == (boot_code_dest+boot_code_size-1))
        printf("Write bootcode end: %04X\n", i);
      continue;
    }
    
    // Normal data write
    WriteByteToAPUAndWaitForState(spcbuf[i&0xFF], port0state);
    //printf("Write Norm: %04X\n", i);
  }
  printf("Upload complete!\n");

  delay(250);

  APU_WriteSPC700(3, (unsigned char)(boot_code_dest >> 8));
  APU_WriteSPC700(2, boot_code_dest & 0xFF);
  APU_WriteSPC700(1, 0);
  
  i = ReadByteFromAPU(0);
  i = i + 2;
  APU_WriteSPC700(0, i);
  i = 0;
  printf("Wait for Play\n");
  if(spcinportiszero) {
    APU_WriteSPC700(3, 1);
    APU_WriteSPC700(0, 1);
  }
  while(ReadByteFromAPU(0) != 0x53) {
    i++;
    if(i > 512) {
      printf("Error loading SPC\n");
      break;
    }
  }
  APU_WriteSPC700(0, spcdata[0xF4]);
  APU_WriteSPC700(1, spcdata[0xF5]);
  APU_WriteSPC700(2, spcdata[0xF6]);
  APU_WriteSPC700(3, spcdata[0xF7]);
  printf("Playing!\n");
}

/*--------------------------------------------------------*
 * 
 * ProcessCommand() - Processes a serial port command
 * 
 * --------------------------------------------------------
 */

unsigned char ReadByteFromSerial()
{
  while ((Serial.available() == 0));
  return Serial.read();
}


void ProcessCommandFromSerial()
{
  if(Serial.available() == 0)
    return;
  static unsigned char port0state=0;
  uint16_t i;
  //static uint16_t j;
  static long time;
  int checksum;
  unsigned char address, data;
  unsigned char command = ReadByteFromSerial();
  switch(command)
  {
  case 'D':    //Set datamode between ascii and binary.  In ascii mode,  each data byte is transmitted
    //as ascii hexadecimal notation.  In binary mode, the byte transmitted on the serial
    //line is taken literally for what it is.
    ReadByteFromSerial();
    // Set datamode has been totally deprecated as of Oct 28, 2015.  It will always remain as binary
    // mode from here on out.  This is the no operation command to use, in order to initiate PC mode.
    break;


  case 'Q':  //Read all 4 IO ports in rapid fire succession.
    data=ReadByteFromAPU(0);
    Serial.write(data);
    data=ReadByteFromAPU(1);
    Serial.write(data);
    data=ReadByteFromAPU(2);
    Serial.write(data);
    data=ReadByteFromAPU(3);
    Serial.write(data);
    break;
  case 'R':  //Read a single IO port.  Takes an address as a parameter, returns one byte.
    address=ReadByteFromSerial()-'0';
    data=ReadByteFromAPU(address);
    Serial.write(data);
    break;
  case 'r':  //Read and returns 2 consecutive IO ports.
    address=ReadByteFromSerial()-'0';
    data=ReadByteFromAPU(address+1);
    Serial.write(data);
    data=ReadByteFromAPU(address);
    Serial.write(data);
    break; 
  case 'S':
    Serial.println("SPC700 DATA LOADER V1.0");
    break;
  case 's':  //Reset the Audio Processing Unit
    digitalWrite(RESET_PIN,LOW);
    delay(10);
    digitalWrite(RESET_PIN,HIGH);
    break;
  case 'f':  //Set the expected state of PORT 0 for the next write.
    port0state=ReadByteFromSerial();
    break;
  case 'F':  //Upload SPC data at serial port speed.
    uint16_t ram_addr;
    uint16_t data_size;
    
    ram_addr=ReadByteFromSerial()<<8;
    ram_addr|=ReadByteFromSerial();
    
    //SECSERIAL.print("Ram Address = ");
    //SECSERIAL.print(ram_addr,HEX);
    data_size=ReadByteFromSerial()<<8;
    data_size|=ReadByteFromSerial();
    //SECSERIAL.print(", Data Size = ");
    //SECSERIAL.println(data_size,HEX);
    WriteByteToAPU(1,1);
    WriteByteToAPU(2,ram_addr&0xFF);
    WriteByteToAPU(3,ram_addr>>8);
    i=ReadByteFromAPU(0);
    i+=2;
    WriteByteToAPU(0,i&0xFF);
    while(ReadByteFromAPU(0)!=(i&0xFF));
    port0state=0;
    //SECSERIAL.println("SPC Data Transfer Started");
    for(i=0;i<data_size;i++)
    //for(i=0;i<65280;i++)
    {
      /*if(!(i%16))
      {
        SECSERIAL.print(".");
      }*/
      //data=i&0xFF;
      data=ReadByteFromSerial();
      WriteByteToAPU(1,data);
      WriteByteToAPU(0,port0state);
      //while(ReadByteFromAPU(0)!=port0state);
      port0state++;
      //SECSERIAL.print("Received byte:");
      //SECSERIAL.println(data,HEX);
    }
    //SECSERIAL.print(millis()-time,DEC);
    //SECSERIAL.println(" milliseconds to upload SPC");
    ////SECSERIAL.println("Transfer Finished");
    Serial.write(1);

    break;

  case 'G':  //Upload the DSP register, and the first page of SPC data at serial port speed.
    for(i=0;i<128;i++)
      dspdata[i]=ReadByteFromSerial();
    for(i=0;i<256;i++)
      spcdata[i]=ReadByteFromSerial();
    APU_StartSPC700(dspdata,spcdata,0);
    Serial.write('F');
    break;
  case 'W':  //Write a single IO port.
    address=ReadByteFromSerial()-'0';
    data=ReadByteFromSerial();
    i=0;  //Must timeout any wait loops, if the respective drivers are not running.
    switch(address-4)
    {
      case 0:
        while((ReadByteFromAPU(1)!=((ReadByteFromAPU(0)+1)&0xFF))&&(i<128)) i++;  //Handle a blazeon track change.
        if(i<128)
        {
          WriteByteToAPU(0,data);
        }
        /*
        ** Blazeon uses the following port communication system.
        ** When Port 1 is Equal to Port 0 + 1, then it is safe to write a new value to any of the 4 ports.
        ** When Port 0 is Equal to Port 1 + 1, then the SPC side driver is reading the ports, then clearing
        ** the ports.  As a result, if we write during the latter time, it ends up being a race condition to
        ** get the write in the specific port, before that port is read out.
        ** Immediately following the readout of the ports, the in ports are automatically cleared and will read
        ** as 0 on SPC side.
        **
        ** There is absolutely no SPC reloading code stored in a Blazeon SPC.  Once it is running, it runs forever,
        ** with its preprogrammed songs/sfxs. Writing to any port changes songs/sfxs played instantly during the
        ** former condition.
        */
        break;
      case 1:  //Multiple games use this song select driver.
        WriteByteToAPU(1,data);
        WriteByteToAPU(0,3);
        data=ReadByteFromAPU(3);
        data++;
        WriteByteToAPU(3,data);
        if(APU_WaitIoPort(3,data,2048))
          break;
        i = 0;
        WriteByteToAPU(0,4);
        data=ReadByteFromAPU(3);
        data++;
        WriteByteToAPU(3,data);
        if(APU_WaitIoPort(3,data,2048))
          break;
        WriteByteToAPU(0,0);
        break;
      case 2:
        WriteByteToAPU(3,data);
        WriteByteToAPU(0,0x80);
        if(APU_WaitIoPort(0,0,2048))
          break;
        WriteByteToAPU(0,0);
        break;
      case 0xFC:  //0 - 4 = 0xFC.
      case 0xFD:  //1 - 4 = 0xFD.
      case 0xFE:  //2 - 4
      case 0xFF:  //3 - 4
      default:    //Any other driver selection not put here.
        /*
        ** Default, is just write the ports as is.
        */
        WriteByteToAPU(address,data);
    }
    break;
  case 'w':  //Write 2 IO consecutive IO ports.
    address=ReadByteFromSerial()-'0';
    data=ReadByteFromSerial();
    WriteByteToAPU(address+1,data);
    data=ReadByteFromSerial();
    WriteByteToAPU(address,data);
    break;
  }
}

void loop() //The main loop.  Define various subroutines, and call them here. :)
{
  ProcessCommandFromSerial();
  
  
}


