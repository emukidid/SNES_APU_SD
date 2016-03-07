/****
 * SHVC-SOUND Arduino Player by emu_kidid in 2015
 * based on code by CaitSith2
 * 
 * Loads SPC tracks from SD to the SHVC-SOUND (aka SNES APU)
 */

 
#include <Firmata.h>
#include <EEPROM.h>
#include "APU.h"
APU apu(APU_TYPE_XMEM);

#ifdef ARDUINO_MEGA
#include <SPI.h>
#include <SdFat.h>
SdFat SD;
#include "LCD.h"
#include <Wire.h>
#include "RTClib.h"

void handleLCD(bool force_refresh);

LiquidCrystal lcd;
RTC_DS1307 rtc;

File spcFile;
File root;
int filedepth=0;
File files[10];
int filecounts[10];
int filecurrent[10];
int rowlen[2];
unsigned char rtc_found=1;
unsigned char is_spc2;
unsigned short spc2_total_songs;
unsigned short songnum  __attribute__ ((section (".noinit")));

static unsigned char boot_code[] =
{
    0x8F, 0x00, 0x00, //      Mov [0], #byte_0
    0x8F, 0x00, 0x01, //      Mov [1], #byte_1
    0x8F, 0xB0, 0xF1, //      Mov [0F1h], #0B0h   ;Clear the IO ports
    0xCD, 0x53,       //      Mov X, #Ack_byte
    0xD8, 0xF4,       //      Mov [0F4h], X
    
    0xE4, 0xF4,       //IN0:  Mov A, [0F4h]
    0x68, 0x00,       //      Cmp A, #IO_Byte_0
    0xD0, 0xFA,       //      Bne IN0
    
    0xE4, 0xF7,       //IN3:  Mov A, [0F7h] 
    0x68, 0x00,       //      Cmp A, #IO_Byte_3 
    0xD0, 0xFA,       //      Bne IN3 

    0x8F, 0x31, 0xF1, //      Mov [0F1h], #ctrl_byte
    
    0x8F, 0x6C, 0xF2, //      Mov [0F2h], 6Ch
    0x8F, 0x00, 0xF3, //      Mov [0F3h], #echo_control_byte
    0x8F, 0x4C, 0xF2, //      Mov [0F2h], 4Ch
    0x8F, 0x00, 0xF3, //      Mov [0F3h], #key_on_byte
    0x8F, 0x7F, 0xF2, //      Mov [0F2h], #dsp_control_register_byte
    0xAE,             //      Pop A
    0xCE,             //      Pop X
    0xEE,             //      Pop Y
    0x7F,             //      RetI
} ;
#endif

static unsigned char spcdata[256];           // 0x100  (256 bytes (of 64kb)) (header)
static unsigned char dspdata[128];           // 0x10100  (128 bytes)

FILE serial_stdout;

static unsigned char DSPdata[] =
{  //For loading the 128 byte DSP ram. DO NOT CHANGE.
  0xC4, 0xF2,       //START:  Mov [0F2h], A
  0x64, 0xF4,       //LOOP:   Cmp A, [0F4h]
  0xD0, 0xFC,       //        Bne LOOP
  0xFA, 0xF5, 0xF3, //        Mov [0F3h], [0F5h]
  0xC4, 0xF4,       //        Mov [0F4h], A
  0xBC,             //        Inc A
  0x10, 0xF2,       //        Bpl START

  0x8F, 0xFF, 0xFC, //      Mov [0FCh], #timer_2
  0x8F, 0xFF, 0xFB, //      Mov [0FBh], #timer_1
  0x8F, 0x4F, 0xFA, //      Mov [0FAh], #timer_0
  
  0xCD, 0xF5,       //      Mov X, #stack_pointer
  0xBD,             //      Mov SP, X
  
  0x2F, 0xAB,       //        Bra 0FFC9h  ;Right when IPL puts AA-BB on the IO ports and waits for CC.
};

char filename[64];
char line2[32];


// Function that printf and related will use to print
int serial_putchar(char c, FILE* f) {
    if (c == '\n') serial_putchar('\r', f);
    return Serial.write(c) == 1? 0 : 1;
}

/*
 * PRESCALER of 0 = 16Mhz Clock
 * PRESCALER of 1 = 8Mhz Clock
 * PRESCALER of 2 = 4Mhz Clock //Couldn't operate my PC interface any slower than this.
 * PRESCALER of 3 = 2Mhz Clock
 * PRESCALER of 4 = 1Mhz Clock //Baud rates 230400 and 250000 no longer possible
 * PRESCALER of 5 = 500Khz Clock //Baud rate 115200 no longer possible.
 */

#define PRESCALER 0
void SetupPrescaler(int prescaler)
{
  Serial.flush();
  uint32_t baud_rate = 250000 << prescaler;  //Bacuase we are slowing the clock down
  //250000 baud at prescaler 1 actually means 125000 baud.  We have to compensate for this.
  
  noInterrupts();  //Since the following writes are timing sensitve, disable interrupts.
  CLKPR = 0x80;  //Write CLKPCE = 1 and CLKPSx = 0
  CLKPR = prescaler;  //Write CLKPCE = 0 and CLKPSx with desired value
  interrupts();
  delay(5);
  Serial.begin(baud_rate);  //Now we begin serial. :)
}
 

void setup()
{
  Serial.begin(250000);
  SetupPrescaler(PRESCALER);

  lcd.begin(16,2);

   // Set up stdout
  fdev_setup_stream(&serial_stdout, serial_putchar, NULL, _FDEV_SETUP_WRITE);
  stdout = &serial_stdout;
  
  Serial.println(F("SHVC-SOUND Arduino Player v0.1"));
#ifdef ARDUINO_MEGA

  Serial.print(F("Initializing RTC... "));
  if (! rtc.begin()) {
    Serial.println(F("Couldn't find RTC :("));
    rtc_found=0;
  }
  else
  {
    if (! rtc.isrunning()) {
      Serial.println(F("RTC is NOT running!, Setting it now."));
      // following line sets the RTC to the date & time this sketch was compiled
      rtc.adjust(DateTime(F(__DATE__), F(__TIME__)));
      // This line sets the RTC with an explicit date & time, for example to set
      // January 21, 2014 at 3am you would call:
      // rtc.adjust(DateTime(2014, 1, 21, 3, 0, 0));
    }
    else
      Serial.println(F("RTC is running! :)"));
  }

  delay(250);
  if (Serial.available()) return;
  
  Serial.print(F("Initializing SD card... "));

  if (!SD.begin(53)) {
    Serial.print(F("initialization failed!\n"));
    return;
  }
  Serial.print(F("initialization done.\n"));

  root = SD.open("dir/");
  if(!root)
  {
    Serial.print(F("Could not open root directory.\n"));
    Serial.print(F("Put all spc files/folders in directory named \"dir\""));
    return;
  }
  filedepth = 0;
  CountFiles();
  GetNextFile();
#endif
}

#ifdef ARDUINO_MEGA

int len(char *string, int maxlen)
{
  for(int i=0;i<maxlen;i++)
    if(string[i]==0)
      return i;
  return maxlen;
}

int lcd_delay=125;
int lcd_pos=0;
void refreshLCD()
{
  rowlen[0]=filename[0]=rowlen[1]=line2[0]=0;
  lcd_pos=0;
  lcd_delay=250;

  if(files[filedepth])
  {
    files[filedepth].getName(filename,64);
    Serial.print(filename);
    rowlen[0] = len(filename,64);
    if(files[filedepth].isDirectory())
    {
      Serial.println(F("/"));
      filename[rowlen[0]++] = '/';
      filename[rowlen[0]] = 0;
    }
    else
    {
      if(files[filedepth].size() < 66048) return;
      spcFile = files[filedepth];
      readSPCRegion((unsigned char*)line2, 0x2EL, 0x20);
      rowlen[1] = len(line2,32);
      Serial.print(F("SPC Track Name: ")); Serial.println(line2);
    }
  }
  handleLCD(true);
}

void CountFiles()
{
  File dir = (filedepth == 0) ? root : files[filedepth-1];
  filecurrent[filedepth] = -1;
  filecounts[filedepth] = 0;
  
  while (true)
  {
    files[filedepth] = dir.openNextFile();
    if(files[filedepth])
    {
      filecounts[filedepth]++;
      filecurrent[filedepth]++;
      files[filedepth].close();
    }
    else
    {
      return;
    }
  }
}

void GetFile()
{
  if(filecurrent[filedepth] < 0)
    filecurrent[filedepth] += filecounts[filedepth];
  if(filecurrent[filedepth] == filecounts[filedepth])
    filecurrent[filedepth] = 0;
    
  File dir = (filedepth == 0) ? root : files[filedepth-1];
  dir.rewindDirectory();
  for(int i=0; i<=filecurrent[filedepth];i++)
  {
    if(files[filedepth])
      files[filedepth].close();
    files[filedepth] = dir.openNextFile();
  }
  refreshLCD();
}

void GetNextFile()
{
  filecurrent[filedepth]++;
  GetFile();
}

void GetPrevFile()
{
  filecurrent[filedepth]--;
  GetFile();
}

void EnterDirectory()
{
  if(files[filedepth].isDirectory())
  {
    filedepth++;
    CountFiles();
    GetNextFile();
  }
}

void LeaveDirectory()
{
  filedepth--;
  files[filedepth].getName(filename,64);
  refreshLCD();
}

#endif



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

int APU_StartSPC700(unsigned char *dspdata, unsigned char *spcdata, int SD_mode=1)
{
  //'G'
  int port0state = 0, i = 0, j = 0;

  DSPdata[15] = spcdata[0xFC];  //Helps to shrink the bootcode by 30 bytes.
  DSPdata[18] = spcdata[0xFB];  //bootcode was formerly 77 bytes long.
  DSPdata[21] = spcdata[0xFA];
  DSPdata[24] = spcdata[0xFF];

  if (SD_mode) Serial.println(F("Uploading DSP Register"));
  //Upload the DSP register, and the first page of SPC data at serial port speed.
  APU_StartWrite(0x0002,DSPdata,sizeof(DSPdata));
  apu.write(2,0x02);
  apu.write(3,0x00);
  apu.write(1,0x00);
  apu.write(0,sizeof(DSPdata)+1);
  while(apu.read(0)!=(sizeof(DSPdata)+1)); 
  if (SD_mode) 
  {
    Serial.println(F("Done"));
    Serial.println(F("Uploading dspdata"));
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
    j=0;
    if(i<127)
        while(apu.read(0)!=port0state)
        {
          if(j++ > 512)
            return 1;
        }
    port0state++;
  }
  j=0;
  while(apu.read(0)!=0xAA)
  {
    if(j++ > 512)
    return false;
  }
  if (SD_mode) 
  {
    Serial.println(F("Done"));
    Serial.println(F("Uploading spcdata"));
  }
  port0state=0;
  apu.write(2,0x02);
  apu.write(3,0x00);
  apu.write(1,0x01);
  apu.write(0,0xCC);
  j=0;
  while(apu.read(0)!=0xCC)
  {
    if(j++ > 512)
    return 2;
  }
  // Send spcdata[0] to spcdata[255]
  for(i=0;i<256;i++)
  {
    if(i<2)
      continue;
    if(i>=0xF0)
      continue;
    apu.write(1,spcdata[i]);
    apu.write(0,port0state);
    j=0;
    while(apu.read(0)!=port0state)
    {
      if(j++ > 512)
      return 3;
    }
    port0state++;
  }
  if (SD_mode) printf("Done\n"); 
  return 0;
}

bool WriteByteToAPUAndWaitForState(unsigned char data, unsigned char state) {
  int i=4096;
  apu.write(1, data);
  apu.write(0, state);
  while(apu.read(0)!=state)
  {
    i--;
    if(i==0)
      return false;
  }
  return true;
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
      Serial.print(F("Seek failed at addr ")); printf("%08X\n",(unsigned long)(addr+0x100));
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
  lcd.clear();
  lcd.setCursor(0,0);
  lcd.print("Uploading");
  lcd.setCursor(0,1);
  lcd.print("Reading from SD");
  
  unsigned long spc2_offset = 16 + ((unsigned long)song * 1024);
  unsigned long spc2_page_offset = 0;
  unsigned char PCL = readSPC(is_spc2?spc2_offset+704:0x25);    // 0x25     (1 byte)
  unsigned char PCH = readSPC(is_spc2?spc2_offset+705:0x26);    // 0x26     (1 byte)
  unsigned char A = readSPC(is_spc2?spc2_offset+706:0x27);      // 0x27     (1 byte)
  unsigned char X = readSPC(is_spc2?spc2_offset+707:0x28);      // 0x28     (1 byte)
  unsigned char Y = readSPC(is_spc2?spc2_offset+708:0x29);      // 0x29     (1 byte)
  unsigned char ApuSW = readSPC(is_spc2?spc2_offset+709:0x2A);  // 0x2A     (1 byte)
  unsigned char ApuSP = readSPC(is_spc2?spc2_offset+710:0x2B);  // 0x2B     (1 byte)
  unsigned char echo_clear = 0;
  unsigned short i,j,bootptr,spcinportiszero = 0;
  unsigned short count;
  

  // Read some ID tag stuff
  unsigned char tagBuffer[64];
  clearBuffer(&tagBuffer[0], 64);
  readSPCRegion(&tagBuffer[0], is_spc2?spc2_offset+768:0x2EL, 0x20);
  Serial.print(F("SPC Track Name: ")); Serial.println((char*)&tagBuffer[0]);
  clearBuffer(&tagBuffer[0], 64);
  readSPCRegion(&tagBuffer[0], is_spc2?spc2_offset+800:0x4EL, 0x20);
  Serial.print(F("SPC Track Game: ")); Serial.println((char*)&tagBuffer[0]);
  

  if(is_spc2)
  {
    for(i=0;i<128;i++)
    {
      digitalWrite(SRAM_PIN_1,LOW);
      digitalWrite(SRAM_PIN_0,LOW);
      get_spc2_page((unsigned char*)_SFR_MEM8(0x8000+(i<<8)), song, i);
      digitalWrite(SRAM_PIN_0,HIGH);
      get_spc2_page((unsigned char*)_SFR_MEM8(0x8000+(i<<8)), song, i+0x80);
    }
  }
  else
  {
    digitalWrite(SRAM_PIN_1,LOW);
    digitalWrite(SRAM_PIN_0,LOW);
    readSPCRegion((unsigned char*)&_SFR_MEM8(0x8000),0x100,0x8000);
    digitalWrite(SRAM_PIN_0,HIGH);
    readSPCRegion((unsigned char*)&_SFR_MEM8(0x8000),0x8100,0x8000);
  }

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

  
  lcd.setCursor(0,1);
  lcd.print("Manipulating SPC");
  
  // SPC In Port Is Zero?
  spcinportiszero = (!spcdata[0xF4] && !spcdata[0xF5] && !spcdata[0xF6] && !spcdata[0xF7]);

  //Calculate out echo region
  unsigned short echo_region = dspdata[0x6D] << 8;
  unsigned short echo_size = (dspdata[0x7D] & 0x0F) * 2048;
  if(echo_size==0) 
    echo_size=4;

  
  Serial.print(F("SPC echo region is: ")); printf("%04X\n", echo_region);
  Serial.print(F("SPC echo size is: ")); printf("%04X\n", echo_size);
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
    for(i=255;i<256;i-=255) 
    {
      unsigned long compare = i | (i<<8);
      compare <<= 16;
      compare |= i | (i<<8);
      count=0;
      digitalWrite(SRAM_PIN_0,HIGH);
      for(j=0xFFBC;j>=0x100;j-=4)
      {
        if(j==0x7FFC)
          digitalWrite(SRAM_PIN_0,LOW);
        if((j>(echo_region+(echo_size-1)))||(j<echo_region))
        {
          if(_SFR_MEM32(0x8000+(j&0x7FFF))==compare)
            count+=4;
          else
            count=0;
          if(count>=sizeof(boot_code))
          {
            count=sizeof(boot_code);
            break;
          }
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
        Serial.print(F("Type 1 search found enough\n"));
        break;
      }
    }
  }
  
  if(freespacesearch == 0 || freespacesearch == 2)
  {
    if(count != sizeof(boot_code))
    {
      unsigned long compare_zero = 0;
      unsigned long compare_one = 0xFFFFFFFF;
      digitalWrite(SRAM_PIN_0,HIGH);
      for(j = 0xFFBC; j >= 0x100; j-=4)
      {
        if(j==0x7FFC)
          digitalWrite(SRAM_PIN_0,LOW);
        if((j>(echo_region+(echo_size-1)))||(j<echo_region))
        {
          if(((j % 64) > 31) && _SFR_MEM32(0x8000+(j&0x7FFF))==compare_one) 
          {
            count+=4;
          }
          else if(((j % 64) <= 31) && _SFR_MEM8(0x8000+(j&0x7FFF))==compare_zero) 
          {
            count+=4;
          }
          else
          {
            count = 0;
          }
          if(count >= sizeof(boot_code)) {
            Serial.print(F("Type 2 search found enough\n"));
            count=sizeof(boot_code);
            break;
          }
        }
      }
    }
  }
  
  if(freespacesearch == 0 || freespacesearch == 3)
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
        Serial.print(F("Type 3 search found enough\n"));
      }
    }
  }
  
  if(count != sizeof(boot_code))
  {
    Serial.print(F("Couldn't find good spot for bootloader!\n"));
    count = sizeof(boot_code);
    j = 0xFF00;
  }

  unsigned short boot_code_dest = j;
  unsigned short boot_code_size = sizeof(boot_code);
  Serial.print(F("boot_code_dest: ")); printf("%04X\n", boot_code_dest);
  Serial.print(F("boot_code_size: ")); printf("%04X\n", boot_code_size);

  // Adjust the boot code with values from this SPC
  boot_code[ 1] = spcdata[0x00];  // SPCRam Address 0x0000
  boot_code[ 4] = spcdata[0x01];  // SPCRam Address 0x0001
  boot_code[16] = spcdata[0xF4] + (spcinportiszero ? 1:0);  // Inport 0
  boot_code[22] = spcdata[0xF7];  // Inport 3
  boot_code[26] = spcdata[0xF1] & 0xCF;  // Control Register
  boot_code[32] = dspdata[0x6C];      // DSP Echo Control Register
  boot_code[38] = dspdata[0x4C];      // DSP KeyON Register
  boot_code[41] = spcdata[0xF2];  // Current DSP Register Address

  spcdata[0xFF] = ApuSP;
  spcdata[0xFF] -= 6;

  lcd.clear();
  lcd.setCursor(0,0);
  lcd.print("Uploading SPC   ");
  lcd.setCursor(0,1);
  lcd.print("Page 0x00");

  //Prescaler of 0 seems to work reliably for everthing except SPC loading.
  //SPC loading has been found to work most reliably with a prescaler of 1 or slower.
  SetupPrescaler(1);
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
      Serial.print(F("Expected AABB0000, Got "));
      printf("%.2X%.2X%.2X%.2X",apu.read(0),apu.read(1),apu.read(2),apu.read(3));
      Serial.print(F(" :( - Failed to reset the APU\n"));
      while(1);
    }
    resetAttempts++;
  }
  Serial.print(F("APU Reset Complete\n"));

  // Initialise it with the DSP data and SPC data for this track
  i=APU_StartSPC700(dspdata, spcdata);
  if(i>0)
  {
    Serial.print(F("Upload failed in APU_StartSCP700: Result ")); printf("%d\n",i);
    return;     
  }
  Serial.print(F("Time to upload!\n"));

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
    Serial.print(F("Upload failed ad address 0x0100"));
    return;
  }

  // Write out the SPC Data (echo, boot_code, etc are interleaved here into the stream)
  digitalWrite(SRAM_PIN_0,LOW);
  for(i=0x100;i!=0;i++) // Thank you, overflow :)
  {
    if(i==0x8000)
      digitalWrite(SRAM_PIN_0,HIGH);
    
    if((i % 0x100) == 0) {
      lcd.setCursor(0,1);
      lcd.print("Page 0x");
      if((i>>8) < 16) lcd.print("0");
      lcd.print(i>>8,HEX);

      // Clear out echo region instead of copying it
      if(((i >= echo_region) && (i <= echo_region+(echo_size-1))) && 
          ((((dspdata[0x6C] & 0x20)==0)&&(echo_clear==0))||(echo_clear==1)))
      {
        if (echo_size == 4)
        {
          _SFR_MEM8(0x8000+(i&0x7FFF)+0)=0;
          _SFR_MEM8(0x8000+(i&0x7FFF)+1)=0;
          _SFR_MEM8(0x8000+(i&0x7FFF)+2)=0;
          _SFR_MEM8(0x8000+(i&0x7FFF)+3)=0;
        }
        else
        {
          clearBuffer((unsigned char*)&_SFR_MEM8(0x8000+(i&0x7FFF)),256);
        }
        if(i == echo_region)
        {
          Serial.print(F("Write echo start: ")); printf("%04X\n", i);
        }
        if(i == ((echo_region+echo_size-1) & 0xFF00))
        {
          Serial.print(F("Write echo end: ")); printf("%04X\n", (echo_region+echo_size-1));
        }
      }
      else if (i == 0x100)
      {
        // Prepare the stack
        _SFR_MEM8(0x8000+0x100+((ApuSP+0x00) & 0xFF)) = PCH;
        _SFR_MEM8(0x8000+0x100+((ApuSP+0xFF) & 0xFF)) = PCL;
        _SFR_MEM8(0x8000+0x100+((ApuSP+0xFE) & 0xFF)) = ApuSW;
        _SFR_MEM8(0x8000+0x100+((ApuSP+0xFD) & 0xFF)) = Y;
        _SFR_MEM8(0x8000+0x100+((ApuSP+0xFC) & 0xFF)) = X;
        _SFR_MEM8(0x8000+0x100+((ApuSP+0xFB) & 0xFF)) = A;
        Serial.print(F("Stack Pointer: ")); printf("%02X\n", ApuSP);
        Serial.print(F("Write PCH: ")); printf("%04X=%02X\n", 0x100+((ApuSP+0x00) & 0xFF), PCH);   // Program Counter High Address
        Serial.print(F("Write PCL: ")); printf("%04X=%02X\n", 0x100+((ApuSP+0xFF) & 0xFF), PCL);   // Program Counter Low Address
        Serial.print(F("Write PSW: ")); printf("%04X=%02X\n", 0x100+((ApuSP+0xFE) & 0xFF), ApuSW); // Program Status Word
        Serial.print(F("Write A: ")); printf("%04X=%02X\n", 0x100+((ApuSP+0xFD) & 0xFF), A);       // A Register
        Serial.print(F("Write X: ")); printf("%04X=%02X\n", 0x100+((ApuSP+0xFC) & 0xFF), X);       // X Register
        Serial.print(F("Write Y: ")); printf("%04X=%02X\n", 0x100+((ApuSP+0xFB) & 0xFF), Y);       // Y Register
      }
      else if (i == 0xFF00)
      {
        // Select proper SPC upper 64 RAM
        if (spcdata[0xF1] & 0x80)
        {
          readSPCRegion((unsigned char*)&_SFR_MEM8(0xFFC0),0x101C0,64);
        }
      }
    }
    unsigned char port0state = i & 0xFF;

    // Boot code section
    if(bootcode_offset != 2)
    {
      if((i >= boot_code_dest) && (i < (boot_code_dest+boot_code_size))) {
        if(!WriteByteToAPUAndWaitForState(boot_code[i-boot_code_dest], port0state))
        {
          Serial.print(F("Upload failed at address 0x")); printf("%.4X:(\n",i);
          return;     
        }
        if(i == boot_code_dest)
        {
          Serial.print(F("Write bootcode start: ")); printf("%04X\n", i);
        }
        if(i == (boot_code_dest+boot_code_size-1))
        {
          Serial.print(F("Write bootcode end: ")); printf("%04X\n", i);
        }
        continue;
      }
    }
    
    // Normal data write
    if(!WriteByteToAPUAndWaitForState(_SFR_MEM8(0x8000+(i&0x7FFF)), port0state))
    {
      Serial.print(F("Upload failed at address 0x")); printf("%.4X\n",i);
      return;     
    }
  }
  Serial.print(F("Upload complete!\n"));

  apu.write(3, (unsigned char)(boot_code_dest >> 8));
  apu.write(2, boot_code_dest & 0xFF);
  apu.write(1, 0);
  
  i = apu.read(0);
  i = i + 2;
  apu.write(0, i);
  i = 0;
  if(bootcode_offset != 2)
  {
    Serial.print(F("Wait for Play\n"));
    if(spcinportiszero) {
      apu.write(3, 1);
      apu.write(0, 1);
    }
    while(apu.read(0) != 0x53) {
      i++;
      if(i > 512) {
        Serial.print(F("Error loading SPC\n"));
        break;
      }
    }
    apu.write(0, spcdata[0xF4]);
    apu.write(1, spcdata[0xF5]);
    apu.write(2, spcdata[0xF6]);
    apu.write(3, spcdata[0xF7]);
  }
  Serial.print(F("Playing!\n"));
  SetupPrescaler(0);
  handleLCD(true);
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

unsigned char ReadHexFromSerial()
{
  while ((Serial.available() < 2));
  unsigned char data = 0;
  unsigned char data2 = Serial.read();
  if(data2 >= 'a')
  {
    data2 -= 'a';
    data2 += 10;
  }
  else if (data2 >= 'A')
  {
    data2 -= 'A';
    data2 += 10;
  }
  else
  {
    data2 -= '0';
  }
  data = data2 << 4;
  data2 = Serial.read();
  if(data2 >= 'a')
  {
    data2 -= 'a';
    data2 += 10;
  }
  else if (data2 >= 'A')
  {
    data2 -= 'A';
    data2 += 10;
  }
  else
  {
    data2 -= '0';
  }
  data |= data2;
  return data;
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
    //printf("Data at port %d is 0x%.2X\n",address,data);
    break;
  case 'r':  //Read and returns 2 consecutive IO ports.
    address=ReadByteFromSerial()-'0';
    data=apu.read(address+1);
    Serial.write(data);
    data=apu.read(address);
    Serial.write(data);
    break; 
  case 'S':
    Serial.println(F("SPC700 DATA LOADER V1.0"));
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
    //data=ReadHexFromSerial();
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

int date_time=250;

bool refreshRTC(bool force_refresh=false)
{
  if(!rtc_found) return false;
  if(rowlen[1]) return false;
  
  date_time--;
  if(!date_time)
    date_time=250;

  if(!force_refresh&&(date_time%25)) return false;
  if(rowlen[1]==0)
  {
    DateTime now=rtc.now();
    if(date_time <= 125)
      sprintf(line2,"%d:%.2d:%.2d",now.hour(),now.minute(),now.second());
    else
      sprintf(line2,"%d/%.2d/%.2d",now.year(),now.month(),now.day());
  }
  return true;
}


void handleLCD(bool force_refresh=false)
{
  int lcd_pos_max = (rowlen[0]>rowlen[1]?rowlen[0]:rowlen[1])+4;
  lcd_delay--;
  force_refresh|=refreshRTC(force_refresh);
  if(lcd_delay<=0)
  {
    force_refresh|=rowlen[0]>16;
    force_refresh|=rowlen[1]>16;
    lcd_pos++;
    if(lcd_pos==lcd_pos_max)
      lcd_pos=-16;
    lcd_delay=(lcd_pos==0?125:10);
  }
  if(force_refresh)
  {
    lcd.clear();
    for(int k=0; k<2; k++) 
    {
      if(rowlen[k] <= 16)
      {
        lcd.setCursor(0,k);
        lcd.print(k==0?filename:line2);
      }
      else
      {
        int j=0-(lcd_pos<0?lcd_pos:0);
        lcd.setCursor(j,k);
        for(int i=(lcd_pos+j);i<rowlen[k]&&j<16;i++,j++)
          lcd.print(k==0?filename[i]:line2[i]);
      }
    }
  }
}


#define LEFT 0
#define UP 1
#define DOWN 2
#define RIGHT 3
#define SELECT 4

void readButtons(bool *buttons)
{
  for (int i=0;i<5;i++)
    buttons[i] = digitalRead(65+i) == LOW;
  delay(40);
  for (int i=0;i<5;i++)
    buttons[i] &= digitalRead(65+i) == LOW;
}

void handleButtons()
{
  bool buttons[5];
  readButtons(buttons);

  if(buttons[UP])
  {
    GetPrevFile();
    while(digitalRead(65+UP) == LOW);
  }
  if(buttons[DOWN])
  {
    GetNextFile();
    while(digitalRead(65+DOWN) == LOW);
  }
  if(buttons[LEFT])
  {
    if(filedepth>0)
      LeaveDirectory();
    while(digitalRead(65+LEFT) == LOW);
  }
  if(buttons[RIGHT])
  {
    spcFile=files[filedepth];
    if(spcFile)
    {
      if(spcFile.isDirectory())
        EnterDirectory();
      else if(spcFile.size() >= 66048)
        LoadAndPlaySPC(0);
    }
    while(digitalRead(65+RIGHT) == LOW);
  }

  if(buttons[SELECT])
  {
    char line2_temp[64];
    sprintf(line2_temp,"%s",line2);
    rowlen[1]=0;
    handleLCD(true);
    do
    {
      for(int i=0;i<250;i++)
      {
        readButtons(buttons);
        handleLCD();
        if((buttons[UP] || buttons[DOWN] || buttons[LEFT] || buttons[RIGHT]) && !buttons[SELECT])
          break;
      }
    } while(buttons[SELECT]);
    sprintf(line2,"%s",line2_temp);
    rowlen[1]=len(line2,64);
    handleLCD(true);
  }
}

void loop() //The main loop.  Define various subroutines, and call them here. :)
{
  ProcessCommandFromSerial();
  handleButtons();
  handleLCD();
}



