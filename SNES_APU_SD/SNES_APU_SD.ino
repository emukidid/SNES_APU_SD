/****
 * SHVC-SOUND Arduino Player by emu_kidid in 2015
 * based on code by CaitSith2
 * 
 * Loads SPC tracks from SD to the SHVC-SOUND (aka SNES APU)
 */
#include <Firmata.h>
#include "APU.h"

#ifdef ARDUINO_MEGA
#include <SPI.h>
#include <SdFat.h>
SdFat SD;
#include "LCD.h"

File spcFile;
unsigned char is_spc2;
unsigned short spc2_total_songs;
unsigned short songnum  __attribute__ ((section (".noinit")));

static unsigned char spcbuf[256];           // 0x100  (256 bytes (of 64kb)) (temp buffer)
static unsigned char boot_code[] =
{
    0x8F, 0x00, 0x00, 0x8F, 0x00, 0x01, 0x8F, 0xFF, 0xFC, 0x8F, 0xFF, 0xFB, 0x8F, 0x4F, 0xFA, 0x8F, 
    0x31, 0xF1, 0xCD, 0x53, 0xD8, 0xF4, 0xE4, 0xF4, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xF5, 0x68, 0x00, 
    0xD0, 0xFA, 0xE4, 0xF6, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xF7, 0x68, 0x00, 0xD0, 0xFA, 0xE4, 0xFD, 
    0xE4, 0xFE, 0xE4, 0xFF, 0x8F, 0x6C, 0xF2, 0x8F, 0x00, 0xF3, 0x8F, 0x4C, 0xF2, 0x8F, 0x00, 0xF3, 
    0x8F, 0x7F, 0xF2, 0xCD, 0xF5, 0xBD, 0xE8, 0xFF, 0x8D, 0x00, 0xCD, 0x00, 0x7F, 
} ;
#endif

static unsigned char spcdata[256];           // 0x100  (256 bytes (of 64kb)) (header)
static unsigned char dspdata[128];           // 0x10100  (128 bytes)

FILE serial_stdout;
APU apu;

static unsigned char DSPdata[16] =
{  //For loading the 128 byte DSP ram. DO NOT CHANGE.
  0xC4, 0xF2, 0x64, 0xF4, 0xD0, 0xFC, 0xFA, 0xF5, 0xF3, 0xC4, 0xF4, 0xBC, 0x10, 0xF2, 0x2F, 0xB7, 
};



// Function that printf and related will use to print
int serial_putchar(char c, FILE* f) {
    if (c == '\n') serial_putchar('\r', f);
    return Serial.write(c) == 1? 0 : 1;
}

void setup()
{
  Serial.begin(250000);

   // Set up stdout
  fdev_setup_stream(&serial_stdout, serial_putchar, NULL, _FDEV_SETUP_WRITE);
  stdout = &serial_stdout;
  
  Serial.println("SHVC-SOUND Arduino Player v0.1");
#ifdef ARDUINO_MEGA
  delay(250);
  if (Serial.available()) return;
  
  printf("Initializing SD card...\n");

  if (!SD.begin(53)) {
    printf("initialization failed!\n");
    return;
  }
  printf("initialization done.\n");

  OpenSPCFile("file.spc");
  if(!spcFile)
    OpenSPCFile("file.sp2");

  if(spcFile)
  {
    if ((songnum+1) > spc2_total_songs)
    {
      songnum = 0;
    }
    LoadAndPlaySPC(songnum++);
  }
#endif
}

bool OpenSPCFile(char *filename)
{
  if(SD.exists(filename))
  {
    if(spcFile)
    {
      printf("Closing previous file\n");
      spcFile.close();
    }
    printf("Opening %s...\n",filename);
    spcFile = SD.open(filename,FILE_READ);
    if(!spcFile)
    {
      printf("Could not open %s\n",filename);
      return false;
    }
    is_spc2 = 0;
    if((readSPC(0) == 'K') && (readSPC(1) == 'S') && (readSPC(2) == 'P') && (readSPC(3) == 'C') && (readSPC(4) == 0x1A))
    {
      printf("%s is an spc2 file\n");
      is_spc2 = 1;
      spc2_total_songs = (readSPC(7) | (readSPC(8) << 8));
    }
    return true;
  }
  else
  {
    printf("%s doesn't exist.\n",filename);
    return false;
  }
}



void APU_StartWrite(unsigned short address, unsigned char *data, int len)
{
  int i;
  uint8_t rdata;
  apu.reset();

  while(apu.read(0)!=0xAA);
  while(apu.read(1)!=0xBB);


  apu.write(3,address>>8);
  apu.write(2,address&0xFF);
  apu.write(1,1);
  apu.write(0,0xCC);
  while(apu.read(0)!=0xCC);

  for(i=0;i<len;i++)
  {
    apu.write(1,data[i]);
    apu.write(0,i&0xFF);
    while(apu.read(0)!=(i&0xFF));
  }
}

void APU_Wait(unsigned char address, unsigned char data)
{
  while(apu.read(address)!=data);
}

void APU_StartSPC700(unsigned char *dspdata, unsigned char *spcdata, int SD_mode=1)
{
  //'G'
  int port0state = 0, i = 0;

  if (SD_mode) printf("Uploading DSP Register\n");
  //Upload the DSP register, and the first page of SPC data at serial port speed.
  APU_StartWrite(0x0002,DSPdata,16);
  apu.write(2,0x02);
  apu.write(3,0x00);
  apu.write(1,0x00);
  apu.write(0,0x11);
  while(apu.read(0)!=0x11); 
  if (SD_mode) 
  {
    printf("Done\n"); 
    printf("Uploading dspdata\n");
  } 
  // Send dspdata[0] to dspdata[127]
  for(i=0;i<128;i++)
  {
    if(i == 0x4C)
      apu.write(1,0x00);
    else if(i == 0x6C)
      apu.write(1,0x60);
    else
      apu.write(1,dspdata[i]);
    apu.write(0,port0state);
    if(i<127)
        while(apu.read(0)!=port0state);
    port0state++;
  }
  while(apu.read(0)!=0xAA);
  if (SD_mode) 
  {
    printf("Done\n"); 
    printf("Uploading spcdata\n"); 
  }
  port0state=0;
  apu.write(2,0x02);
  apu.write(3,0x00);
  apu.write(1,0x01);
  apu.write(0,0xCC);
  while(apu.read(0)!=0xCC);
  // Send spcdata[0] to spcdata[255]
  for(i=0;i<256;i++)
  {
    if(i<2)
      continue;
    if(i>=0xF0)
      continue;
    apu.write(1,spcdata[i]);
    apu.write(0,port0state);
    while(apu.read(0)!=port0state);
    port0state++;
  }
  if (SD_mode) printf("Done\n"); 
}

void WriteByteToAPUAndWaitForState(unsigned char data, unsigned char state) {
 // printf("Byte %02X\n", data);
  apu.write(1, data);
  apu.write(0, state);
  while(apu.read(0)!=state);
}

// APU_WaitIoPort
int APU_WaitIoPort(uint8_t address, uint8_t data, int timeout)
{
  int i=0;
  while((apu.read(address)!=data)&&(i<timeout)) i++;
  if(i<timeout)
    return 0;
  return 1;
}

#ifdef ARDUINO_MEGA
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


void get_spc2_page(unsigned char *buf, unsigned short song, unsigned char page, unsigned short count=256)
{
  unsigned long spc2_offset = 16 + ((unsigned long)song * 1024);
  unsigned long spcram_start = 16 + ((unsigned long)spc2_total_songs * 1024);
  unsigned short page_number = readSPC(spc2_offset + (page * 2)) | (readSPC(spc2_offset + (page * 2) + 1) << 8);
  spcram_start += page_number * 256;
  readSPCRegion(buf,spcram_start,count);
}

void LoadAndPlaySPC(unsigned short song)
{
  unsigned long spc2_offset = 16 + ((unsigned long)song * 1024);
  unsigned long spc2_page_offset = 0;
  unsigned char PCL = readSPC(is_spc2?spc2_offset+704:0x25);    // 0x25     (1 byte)
  unsigned char PCH = readSPC(is_spc2?spc2_offset+705:0x26);    // 0x26     (1 byte)
  unsigned char A = readSPC(is_spc2?spc2_offset+706:0x27);      // 0x27     (1 byte)
  unsigned char X = readSPC(is_spc2?spc2_offset+707:0x28);      // 0x28     (1 byte)
  unsigned char Y = readSPC(is_spc2?spc2_offset+708:0x29);      // 0x29     (1 byte)
  unsigned char ApuSW = readSPC(is_spc2?spc2_offset+709:0x2A);  // 0x2A     (1 byte)
  unsigned char ApuSP = readSPC(is_spc2?spc2_offset+710:0x2B);  // 0x2B     (1 byte)
  unsigned char echo_clear = 1;

  // Read some ID tag stuff
  unsigned char tagBuffer[64];
  clearBuffer(&tagBuffer[0], 64);
  readSPCRegion(&tagBuffer[0], is_spc2?spc2_offset+768:0x2EL, 0x20);
  printf("SPC Track Name: %s\n",&tagBuffer[0]);
  clearBuffer(&tagBuffer[0], 64);
  readSPCRegion(&tagBuffer[0], is_spc2?spc2_offset+800:0x4EL, 0x20);
  printf("SPC Track Game: %s\n",&tagBuffer[0]);

  // Read some stuff
  if(is_spc2)
  {
    get_spc2_page(&spcdata[0], song, 0);
    readSPCRegion(&dspdata[0], spc2_offset+512, 128);
  }
  else
  {
    readSPCRegion(&spcdata[0], 0x100L, 256);
    readSPCRegion(&dspdata[0], 0x10100L, 128);
  }
  
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
  unsigned short bootcode_offset = is_spc2?readSPC(spc2_offset+734)|(readSPC(spc2_offset+735)<<8):0;
  if(bootcode_offset > 0)
  {
    freespacesearch = 4;  //No need to search for space, spc2 already tells us where to put it. :)
    if(bootcode_offset == 1)
    {
      count = 0;
    }
    else if (bootcode_offset == 2)
    {
      count = sizeof(boot_code);
      j = PCL | (PCH << 8);
    }
    else if (bootcode_offset > 255)
    {
      j = bootcode_offset;
      count = sizeof(boot_code);
    }
  }
  
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
          j=echo_region;
          if(j == 0)
            break;
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
  if(freespacesearch == 0 || freespacesearch == 3)
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
  apu.reset();
  delay(50);
  while(apu.read(0) != 0xAA || apu.read(1) != 0xBB
     || apu.read(2) != 0x00 || apu.read(3) != 0x00)
  {
    apu.reset();
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
  apu.write(1,1);
  apu.write(2,0x00);
  apu.write(3,0x01);
  int rb=apu.read(0);
  rb+=2;
  apu.write(0,rb&0xFF);
  j=0;
  while((apu.read(0)!=rb)&&(j<500))
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
          if(is_spc2)
            get_spc2_page(&spcbuf[0], song, i>>8);
          else
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
        if(is_spc2)
          get_spc2_page(&spcbuf[0], song, i>>8);
        else
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
          if(is_spc2)
          {
            get_spc2_page(&spcbuf[0], song, i>>8, 192);
            readSPCRegion(&spcbuf[0xC0],spc2_offset+640,64);
          }
          else
          {
            readSPCRegion(&spcbuf[0], i+0x100, 192);
            readSPCRegion(&spcbuf[0xC0],0x101C0,64);
          }
          printf("Write spcram: FFC0-FFFF\n");
        }
        else
        {
          if(is_spc2)
            get_spc2_page(&spcbuf[0], song, i>>8);
          else
            readSPCRegion(&spcbuf[0], i+0x100, 256);
        }
      }
      else
      { 
        if(is_spc2)
          get_spc2_page(&spcbuf[0], song, i>>8);
        else
          readSPCRegion(&spcbuf[0], i+0x100, 256);
      }
    }
    unsigned char port0state = i & 0xFF;

    // Boot code section
    if(bootcode_offset != 2)
    {
      if((i >= boot_code_dest) && (i < (boot_code_dest+boot_code_size))) {
        WriteByteToAPUAndWaitForState(boot_code[i-boot_code_dest], port0state);     
        if(i == boot_code_dest)
          printf("Write bootcode start: %04X\n", i);
        if(i == (boot_code_dest+boot_code_size-1))
          printf("Write bootcode end: %04X\n", i);
        continue;
      }
    }
    
    // Normal data write
    WriteByteToAPUAndWaitForState(spcbuf[i&0xFF], port0state);
    //printf("Write Norm: %04X\n", i);
  }
  printf("Upload complete!\n");

  apu.write(3, (unsigned char)(boot_code_dest >> 8));
  apu.write(2, boot_code_dest & 0xFF);
  apu.write(1, 0);
  
  i = apu.read(0);
  i = i + 2;
  apu.write(0, i);
  i = 0;
  if(bootcode_offset != 2)
  {
    printf("Wait for Play\n");
    if(spcinportiszero) {
      apu.write(3, 1);
      apu.write(0, 1);
    }
    while(apu.read(0) != 0x53) {
      i++;
      if(i > 512) {
        printf("Error loading SPC\n");
        break;
      }
    }
    apu.write(0, spcdata[0xF4]);
    apu.write(1, spcdata[0xF5]);
    apu.write(2, spcdata[0xF6]);
    apu.write(3, spcdata[0xF7]);
  }
  printf("Playing!\n");
}
#endif

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
    data=apu.read(0);
    Serial.write(data);
    data=apu.read(1);
    Serial.write(data);
    data=apu.read(2);
    Serial.write(data);
    data=apu.read(3);
    Serial.write(data);
    break;
  case 'R':  //Read a single IO port.  Takes an address as a parameter, returns one byte.
    address=ReadByteFromSerial()-'0';
    data=apu.read(address);
    Serial.write(data);
    break;
  case 'r':  //Read and returns 2 consecutive IO ports.
    address=ReadByteFromSerial()-'0';
    data=apu.read(address+1);
    Serial.write(data);
    data=apu.read(address);
    Serial.write(data);
    break; 
  case 'S':
    Serial.println("SPC700 DATA LOADER V1.0");
    break;
  case 's':  //Reset the Audio Processing Unit
    apu.reset();
    break;
  case 'f':  //Set the expected state of PORT 0 for the next write.
    port0state=ReadByteFromSerial();
    break;
  case 'F':  //Upload SPC data at serial port speed.
    uint16_t ram_addr;
    uint16_t data_size;
    
    ram_addr=ReadByteFromSerial()<<8;
    ram_addr|=ReadByteFromSerial();
    
    data_size=ReadByteFromSerial()<<8;
    data_size|=ReadByteFromSerial();

    apu.write(1,1);
    apu.write(2,ram_addr&0xFF);
    apu.write(3,ram_addr>>8);
    i=apu.read(0);
    i+=2;
    apu.write(0,i&0xFF);
    while(apu.read(0)!=(i&0xFF));
    port0state=0;

    for(i=0;i<data_size;i++)
    {
      data=ReadByteFromSerial();
      apu.write(1,data);
      apu.write(0,port0state++);
      //Serial port is slow enough that the APU will be ready for the data,
      //when it finally comes in. :)
    }
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
        while((apu.read(1)!=((apu.read(0)+1)&0xFF))&&(i<128)) i++;  //Handle a blazeon track change.
        if(i<128)
        {
          apu.write(0,data);
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
        apu.write(1,data);
        apu.write(0,3);
        data=apu.read(3);
        data++;
        apu.write(3,data);
        if(APU_WaitIoPort(3,data,2048))
          break;
        i = 0;
        apu.write(0,4);
        data=apu.read(3);
        data++;
        apu.write(3,data);
        if(APU_WaitIoPort(3,data,2048))
          break;
        apu.write(0,0);
        break;
      case 2:
        apu.write(3,data);
        apu.write(0,0x80);
        if(APU_WaitIoPort(0,0,2048))
          break;
        apu.write(0,0);
        break;
      case 0xFC:  //0 - 4 = 0xFC.
      case 0xFD:  //1 - 4 = 0xFD.
      case 0xFE:  //2 - 4
      case 0xFF:  //3 - 4
      default:    //Any other driver selection not put here.
        /*
        ** Default, is just write the ports as is.
        */
        apu.write(address,data);
    }
    break;
  case 'w':  //Write 2 IO consecutive IO ports.
    address=ReadByteFromSerial()-'0';
    data=ReadByteFromSerial();
    apu.write(address+1,data);
    data=ReadByteFromSerial();
    apu.write(address,data);
    break;
  }
}

void loop() //The main loop.  Define various subroutines, and call them here. :)
{
  ProcessCommandFromSerial();
}



