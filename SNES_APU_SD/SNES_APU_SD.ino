/****
   SHVC-SOUND Arduino Player by emu_kidid in 2015
   based on code by CaitSith2

   Loads SPC tracks from SD to the SHVC-SOUND (aka SNES APU)
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
#include "spc_struct.h"

void handleLCD(bool force_refresh = false);
void GetFile(bool print_info = false, bool rewind_dir = true, bool update_lcd = true);
void GetNextFile(bool print_info = false);
void GetPrevFile(bool print_info = false);
void PlaySPC(int SD_mode = 1);

LiquidCrystal lcd;
RTC_DS1307 rtc;

File spcFile;
File root;

#define MAX_FILE_DEPTH 20
int filedepth = 0;
File files[MAX_FILE_DEPTH];
int filecounts[MAX_FILE_DEPTH];
int filecurrent[MAX_FILE_DEPTH];
int rowlen[2];
unsigned char rtc_found = 1;
unsigned char is_spc2;
unsigned short spc2_total_songs;
unsigned short songnum  __attribute__ ((section (".noinit")));
bool debug_print = false;
spc_idx6_table tags  __attribute__ ((section (".noinit")));
bool auto_play;
u32 play_time, total_time;
int auto_play_start;

int song_depth=-1;
int song_current[MAX_FILE_DEPTH];
bool file_changed = false;
bool just_uploaded = false;

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

bool dataModeText = false;

FILE serial_stdout;

static unsigned char DSPdata[] =
{ //For loading the 128 byte DSP ram. DO NOT CHANGE.
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

// Function that printf and related will use to print
int serial_putchar(char c, FILE* f) {
  if (c == '\n') serial_putchar('\r', f);
  return Serial.write(c) == 1 ? 0 : 1;
}

/*
   PRESCALER of 0 = 16Mhz Clock
   PRESCALER of 1 = 8Mhz Clock
   PRESCALER of 2 = 4Mhz Clock //Couldn't operate my PC interface any slower than this.
   PRESCALER of 3 = 2Mhz Clock
   PRESCALER of 4 = 1Mhz Clock //Baud rates 230400 and 250000 no longer possible
   PRESCALER of 5 = 500Khz Clock //Baud rate 115200 no longer possible.
*/

#define PRESCALER 0
void SetupPrescaler(int prescaler)
{
  Serial.flush(); //Any and all serial output currently in the buffer will be garbage, from
  //the time it takes to change the prescaler till the time serial begins again.
  //As such, we Have to wait till ALL serial has been output before we change the pre-scaler.

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

  // Set up stdout
  fdev_setup_stream(&serial_stdout, serial_putchar, NULL, _FDEV_SETUP_WRITE);
  stdout = &serial_stdout;

  Serial.println(F("SHVC-SOUND Arduino Player v0.1"));
#ifdef ARDUINO_MEGA
  clear_current_song();
  lcd.begin(16, 2);

  Serial.print(F("Initializing RTC... "));
  if (! rtc.begin()) {
    Serial.println(F("Couldn't find RTC :("));
    rtc_found = 0;
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
    {
      Serial.println(F("RTC is running! :)"));
      readButtons();
      if (IsButtonPressed(LEFT) && IsButtonPressed(RIGHT))
      {
        rtc.adjust(DateTime(F(__DATE__), F(__TIME__)));
        Serial.println(F("Forcing an RTC Adjustment!"));
        while (IsButtonPressed(LEFT) || IsButtonPressed(RIGHT));
      }
    }
  }

  delay(250);
  if (Serial.available()) return;

  dataModeText = true;
  Serial.print(F("Initializing SD card... "));

  if (!SD.begin(53)) {
    Serial.print(F("initialization failed!\n"));
    return;
  }
  Serial.print(F("initialization done.\n"));

  root = SD.open("dir/");
  if (!root)
  {
    Serial.print(F("Could not open root directory.\n"));
    Serial.print(F("Put all spc files/folders in directory named \"dir\""));
    return;
  }
  filedepth = 0;
  CountFiles();
  for (filecurrent[filedepth] = 0; filecurrent[filedepth] < filecounts[filedepth]; filecurrent[filedepth]++)
    GetFile(true, !filecurrent[filedepth], false);
  GetFile();
#endif
}

#ifdef ARDUINO_MEGA

int len(char *string, int maxlen)
{
  for (int i = 0; i < maxlen; i++)
    if (string[i] == 0)
      return i;
  return maxlen;
}

#define LCD_DISPLAY_MODE_NORMAL 0
#define LCD_DISPLAY_MODE_DATETIME 1

#define LCD_SCROLL_DELAY_LONG 128
#define LCD_SCROLL_DELAY_SHORT 8
int lcd_delay[2] = {LCD_SCROLL_DELAY_LONG,LCD_SCROLL_DELAY_LONG};
int lcd_pos[2] = {0,0};
#define LINE_0_LEN 448
#define LINE_1_LEN 64
int line_len_max[] = {LINE_0_LEN, LINE_1_LEN};
char lcd_line_0[LINE_0_LEN];
char lcd_line_1[LINE_1_LEN];
char* lcd_buffer[] = {lcd_line_0, lcd_line_1};
char file_index[5];
void refreshLCD()
{
  rowlen[0] = lcd_buffer[0][0] = rowlen[1] = lcd_buffer[1][0] = 0;
  lcd_pos[0] = lcd_pos[1] = 0;
  lcd_delay[0] = lcd_delay[1] = LCD_SCROLL_DELAY_LONG;

  if (files[filedepth])
  {
    files[filedepth].getName(lcd_buffer[0], line_len_max[0]);
    rowlen[0] = len(lcd_buffer[0], line_len_max[0]);
    if (files[filedepth].isDirectory())
    {
      lcd_buffer[0][rowlen[0]++] = '/';
      lcd_buffer[0][rowlen[0]] = 0;
      switch_lcd_display_mode(LCD_DISPLAY_MODE_DATETIME);
    }
    else
    {
      if (IsSPC())
      {
        switch_lcd_display_mode(LCD_DISPLAY_MODE_NORMAL);
        spcFile = files[filedepth];
        readSPCRegion((unsigned char*)lcd_buffer[1], 0x2EL, 0x20);
        rowlen[1] = len(lcd_buffer[1], line_len_max[1]);
      }
      else
        switch_lcd_display_mode(LCD_DISPLAY_MODE_DATETIME);
    }
  }
}

void CountFiles()
{
  File dir = (filedepth == 0) ? root : files[filedepth - 1];
  filecurrent[filedepth] = -1;
  filecounts[filedepth] = 0;

  while (true)
  {
    files[filedepth] = dir.openNextFile();
    if (files[filedepth])
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

void GetFile(bool print_info, bool rewind_dir, bool update_lcd)
{
  File dir = (filedepth == 0) ? root : files[filedepth - 1];
  if (rewind_dir)
  {
    if (filecurrent[filedepth] < 0)
      filecurrent[filedepth] += filecounts[filedepth];
    if (filecurrent[filedepth] == filecounts[filedepth])
      filecurrent[filedepth] = 0;

    dir.rewindDirectory();
    for (int i = 0; i <= filecurrent[filedepth]; i++)
    {
      if (files[filedepth])
        files[filedepth].close();
      files[filedepth] = dir.openNextFile();
    }
  }
  else
  {
    if (files[filedepth])
      files[filedepth].close();
    files[filedepth] = dir.openNextFile();
  }
  if (filedepth == 0)
    root = dir;
  else
    files[filedepth - 1] = dir;

  refreshLCD();
  if (print_info)
  {
    Serial.print('[');
    sprintf(file_index, (filecounts[filedepth] > 256 ? "%04X" : "%02X"), filecurrent[filedepth]);
    Serial.print(file_index);
    Serial.print(F("]|"));
    Serial.print(lcd_buffer[0]);
    if (IsSPC())
    {
      Serial.print(F(" - Track Name: ")); Serial.print(lcd_buffer[1]);
    }
    Serial.println();
  }
  if (update_lcd)
    handleLCD(true);
}

void GetNextFile(bool print_info)
{
  filecurrent[filedepth]++;
  GetFile(print_info, filecurrent[filedepth] >= filecounts[filedepth]);
}

void GetPrevFile(bool print_info)
{
  filecurrent[filedepth]--;
  GetFile(print_info);
}

void EnterDirectory()
{
  if (files[filedepth].isDirectory())
  {
    filedepth++;
    if(filedepth <= MAX_FILE_DEPTH)
    {
      CountFiles();
      GetNextFile();
    }
    else
    {
      filedepth--;
      Serial.println("Error: Max Directory depth reached.");
    }
  }
}

void LeaveDirectory()
{
  filedepth--;
  GetFile(dataModeText);
}

#endif



void APU_StartWrite(unsigned short address, unsigned char *data, int len)
{
  int i;
  uint8_t rdata;
  apu.reset();

  while (apu.read(0) != 0xAA);
  while (apu.read(1) != 0xBB);


  apu.write(3, address >> 8);
  apu.write(2, address & 0xFF);
  apu.write(1, 1);
  apu.write(0, 0xCC);
  while (apu.read(0) != 0xCC);

  for (i = 0; i < len; i++)
  {
    apu.write(1, data[i]);
    apu.write(0, i & 0xFF);
    while (apu.read(0) != (i & 0xFF));
  }
}

void APU_Wait(unsigned char address, unsigned char data)
{
  while (apu.read(address) != data);
}

int APU_StartSPC700(unsigned char *dspdata, unsigned char *spcdata, int SD_mode = 1)
{
  //'G'
  int port0state = 0, i = 0, j = 0;

  DSPdata[15] = spcdata[0xFC];  //Helps to shrink the bootcode by 30 bytes.
  DSPdata[18] = spcdata[0xFB];  //bootcode was formerly 77 bytes long.
  DSPdata[21] = spcdata[0xFA];
  DSPdata[24] = spcdata[0xFF];

  if (SD_mode) Serial.print(F("Uploading DSP Register Loader...."));
  //Upload the DSP register, and the first page of SPC data at serial port speed.
  APU_StartWrite(0x0002, DSPdata, sizeof(DSPdata));
  apu.write(2, 0x02);
  apu.write(3, 0x00);
  apu.write(1, 0x00);
  apu.write(0, sizeof(DSPdata) + 1);
  while (apu.read(0) != (sizeof(DSPdata) + 1));
  if (SD_mode)
  {
    Serial.println(F("Done"));
    Serial.print(F("Uploading dspdata...."));
  }
  // Send dspdata[0] to dspdata[127]
  for (i = 0; i < 128; i++)
  {
    if (i == 0x4C)
      apu.write(1, 0x00);
    else if (i == 0x6C)
      apu.write(1, 0x60);
    else
      apu.write(1, dspdata[i]);
    apu.write(0, port0state);
    j = 0;
    if (i < 127)
      while (apu.read(0) != port0state)
      {
        if (j++ > 512)
          return 1;
      }
    port0state++;
  }
  j = 0;
  while (apu.read(0) != 0xAA)
  {
    if (j++ > 512)
      return false;
  }
  if (SD_mode)
  {
    Serial.println(F("Done"));
    Serial.print(F("Uploading spcdata"));
  }
  port0state = 0;
  apu.write(2, 0x02);
  apu.write(3, 0x00);
  apu.write(1, 0x01);
  apu.write(0, 0xCC);
  j = 0;
  while (apu.read(0) != 0xCC)
  {
    if (j++ > 512)
      return 2;
  }
  // Send spcdata[0] to spcdata[255]
  for (i = 0; i < 256; i++)
  {
    if (i < 2)
      continue;
    if (i >= 0xF0)
      continue;
    apu.write(1, spcdata[i]);
    apu.write(0, port0state);
    j = 0;
    while (apu.read(0) != port0state)
    {
      if (j++ > 512)
        return 3;
    }
    port0state++;
  }
  return 0;
}

bool WriteByteToAPUAndWaitForState(unsigned char data, unsigned char state) {
  int i = 4096;
  apu.write(1, data);
  apu.write(0, state);
  while (apu.read(0) != state)
  {
    i--;
    if (i == 0)
      return false;
  }
  return true;
}

// APU_WaitIoPort
int APU_WaitIoPort(uint8_t address, uint8_t data, int timeout)
{
  int i = 0;
  while ((apu.read(address) != data) && (i < timeout)) i++;
  if (i < timeout)
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
  if (spcFile.position() != (unsigned long)(addr + 0x100)) {
    if (!spcFile.seek((unsigned long)(addr + 0x100))) { // Adjust for SPC Header
      Serial.print(F("Seek failed at addr ")); printf("%08X\n", (unsigned long)(addr + 0x100));
      while (1);
    }
  }
  return spcFile.read();
}

// Clears a buffer (memset)
void clearBuffer(unsigned char *buf, long len)
{
  int i = 0;
  for (i = 0; i < len; i++) {
    buf[i] = 0;
  }
}


void get_spc2_page(unsigned char *buf, unsigned short song, unsigned char page, unsigned short count = 256)
{
  unsigned long spc2_offset = 16 + ((unsigned long)song * 1024);
  unsigned long spcram_start = 16 + ((unsigned long)spc2_total_songs * 1024);
  unsigned short page_number = readSPC(spc2_offset + (page * 2)) | (readSPC(spc2_offset + (page * 2) + 1) << 8);
  spcram_start += page_number * 256;
  readSPCRegion(buf, spcram_start, count);
}

unsigned short bootcode_offset;
void ReadSPCFromSD()
{
  bootcode_offset = 0;
  digitalWrite(SRAM_PIN_1, LOW);
  digitalWrite(SRAM_PIN_0, LOW);
  readSPCRegion((unsigned char*)&_SFR_MEM8(0x8000), 0x100, 0x8000);
  digitalWrite(SRAM_PIN_0, HIGH);
  readSPCRegion((unsigned char*)&_SFR_MEM8(0x8000), 0x8100, 0x8000);
  digitalWrite(SRAM_PIN_1, HIGH);
  digitalWrite(SRAM_PIN_0, LOW);
  readSPCRegion((unsigned char*)&_SFR_MEM8(0x8000), 0x000, 0x100);
  readSPCRegion((unsigned char*)&_SFR_MEM8(0x8100), 0x10100, 0x100);
  _SFR_MEM8(0x8200) = 0;  //Fixes a bug caused by loading a no extended tag spc, after loading an spc with extended tags.
  readSPCRegion((unsigned char*)&_SFR_MEM8(0x8200), 0x10200, 0x7E00);
  ProcessSPCTags();
  spcFile.getName(tags.spc_filename, MAX_SPC_FILENAME);
}

void ReadSPC2FromSD(unsigned short song)
{
  unsigned long spc2_offset = 16 + ((unsigned long)song * 1024);
  unsigned long spc2_page_offset = 0;
  unsigned short i, j;

  clearBuffer((u8*)&tags, sizeof(spc_idx6_table));

  digitalWrite(SRAM_PIN_1, HIGH);
  digitalWrite(SRAM_PIN_0, LOW);

  //Read the registers.
  for (i = 0; i < 7; i++)
    _SFR_MEM8(0x8025 + i) = readSPC(spc2_offset + 704 + i);

  //Read DSP registers
  readSPCRegion((unsigned char*)&_SFR_MEM8(0x8100), spc2_offset + 512, 128);

  //Read IPL region
  readSPCRegion((unsigned char*)&_SFR_MEM8(0x81C0), spc2_offset + 640, 64);

  //Read SPC Ram
  digitalWrite(SRAM_PIN_1, LOW);
  for (i = 0; i < 128; i++)
    get_spc2_page((unsigned char*)_SFR_MEM8(0x8000 + (i << 8)), song, i);

  digitalWrite(SRAM_PIN_0, HIGH);
  for (i = 0; i < 128; i++)
    get_spc2_page((unsigned char*)_SFR_MEM8(0x8000 + (i << 8)), song, i + 0x80);

  bootcode_offset = readSPC(spc2_offset + 734) | (readSPC(spc2_offset + 735) << 8);

  //Process tags
  readSPCRegion((u8*)&tags.date, spc2_offset + 712, 4);
  readSPCRegion((u8*)&tags.intro_len, spc2_offset + 716, 32);
  readSPCRegion((u8*)&tags.fade_len, spc2_offset + 720, 32);
  readSPCRegion((u8*)&tags.emulator, spc2_offset + 728, 32);
  readSPCRegion((u8*)&tags.ost_disc, spc2_offset + 729, 32);
  readSPCRegion((u8*)&tags.ost_track, spc2_offset + 730, 32);
  readSPCRegion((u8*)&tags.copyright, spc2_offset + 732, 32);
  readSPCRegion((u8*)&tags.boot_code, spc2_offset + 734, 32);
  readSPCRegion((u8*)&tags.song_title, spc2_offset + 768, 32);
  readSPCRegion((u8*)&tags.game_title, spc2_offset + 800, 32);
  readSPCRegion((u8*)&tags.song_artist, spc2_offset + 832, 32);
  readSPCRegion((u8*)&tags.dumper_name, spc2_offset + 864, 32);
  readSPCRegion((u8*)&tags.comments, spc2_offset + 896, 32);
  readSPCRegion((u8*)&tags.ost_title, spc2_offset + 928, 32);
  readSPCRegion((u8*)&tags.pub_name, spc2_offset + 960, 32);
  readSPCRegion((u8*)&tags.spc_filename, spc2_offset + 992, 28);
  unsigned long extended;
  readSPCRegion((u8*)&extended, spc2_offset + 1020, 4);
  if (extended)
  {
    u8 buf[257];
    bool seen[10] = {false, false, false, false, false, false, false, false, false, false};

    do
    {
      readSPCRegion(buf, extended, 257);
      extended += 2;
      extended += buf[1];
      if (buf[0] > 9) break; //Invalid tag data, as per spc2 file specificaiton v1.3.
      if (!seen[buf[0]])
      {
        switch (buf[0])
        {
          case 1:
            memcpy((u8*)&tags.song_title[32], &buf[2], MAX_SONG_TITLE - 32);
            tags.song_title[MAX_SONG_TITLE - 1] = 0;
            break;
          case 2:
            memcpy((u8*)&tags.game_title[32], &buf[2], MAX_GAME_TITLE - 32);
            break;
          case 3:
            memcpy((u8*)&tags.song_artist[32], &buf[2], MAX_SONG_ARTIST - 32);
            break;
          case 4:
            memcpy((u8*)&tags.dumper_name[32], &buf[2], MAX_DUMPER_NAME - 32);
            break;
          case 5:
            memcpy((u8*)&tags.comments[32], &buf[2], MAX_COMMENTS - 32);
            break;
          case 6:
            memcpy((u8*)&tags.ost_title[32], &buf[2], MAX_OST_TITLE - 32);
            break;
          case 7:
            memcpy((u8*)&tags.pub_name[32], &buf[2], MAX_PUB_NAME - 32);
            break;
          case 8:
            memcpy((u8*)&tags.spc_filename[28], &buf[2], MAX_SPC_FILENAME - 32);
            break;
        }
        seen[buf[0]] = true;
      }
    } while (buf[0]);
  }

}

bool IsSPC()
{
  IsSPC(files[filedepth]);
}

bool IsSPC(File file)
{
  if (file.isDirectory()) return false;
  if (file.size() < 66048) return false;
  if (file.size() > 131592L) return false;
  u8 buf[0x1E];
  file.seek(0);
  file.read(buf, 0x1D);
  buf[0x1D] = 0;
  return !strncmp((char*)&buf[0], "SNES-SPC700 Sound File Data v", 0x1D);
}


void spc_push(unsigned char *sp, unsigned char data, const __FlashStringHelper *ifsh)
{
  if (debug_print) {
    Serial.print(ifsh);
    printf("%04X=%02X\n", 0x100 + sp[0], data);
  }
  _SFR_MEM8(0x8000 + 0x100 + sp[0]) = data;
  sp[0]--;
}

void LoadAndPlaySPC()
{
  ReadSPCFromSD();
  PlaySPC();
}


void PlaySPC(int SD_mode)
{
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(F("Uploading"));
  lcd.setCursor(0, 1);
  lcd.print(F("Reading from SD"));

  unsigned char PCL;    // 0x25     (1 byte)
  unsigned char PCH;    // 0x26     (1 byte)
  unsigned char A;      // 0x27     (1 byte)
  unsigned char X;      // 0x28     (1 byte)
  unsigned char Y;      // 0x29     (1 byte)
  unsigned char ApuSW;  // 0x2A     (1 byte)
  unsigned char ApuSP;  // 0x2B     (1 byte)
  unsigned char echo_clear = 0;
  unsigned short i, j, bootptr, spcinportiszero = 0;
  unsigned short count;
  unsigned char spcdata[256];           // 0x100  (256 bytes (of 64kb)) (header)
  unsigned char dspdata[128];           // 0x10100  (128 bytes)
  unsigned char ipl_buffer[64];

  digitalWrite(SRAM_PIN_1, HIGH);
  digitalWrite(SRAM_PIN_0, LOW);
  PCL = _SFR_MEM8(0x8025);
  PCH = _SFR_MEM8(0x8026);
  A = _SFR_MEM8(0x8027);
  X = _SFR_MEM8(0x8028);
  Y = _SFR_MEM8(0x8029);
  ApuSW = _SFR_MEM8(0x802A);
  ApuSP = _SFR_MEM8(0x802B);

  for (i = 0; i < 128; i++)
    dspdata[i] = _SFR_MEM8(0x8100 + i);

  for (i = 0; i < 64; i++)
    ipl_buffer[i] = _SFR_MEM8(0x81C0 + i);

  digitalWrite(SRAM_PIN_1, LOW);

  for (i = 0; i < 256; i++)
    spcdata[i] = _SFR_MEM8(0x8000 + i);

  // Read out 64 bytes of ram from the proper area of spc.
  if (spcdata[0xF1] & 0x80)
  {
    digitalWrite(SRAM_PIN_0, HIGH);
    for (i = 0; i < 64; i++)
      _SFR_MEM8(0xFFC0 + i) = ipl_buffer[i];
  }


  lcd.setCursor(0, 1);
  lcd.print(F("Manipulating SPC"));

  // SPC In Port Is Zero?
  spcinportiszero = (!spcdata[0xF4] && !spcdata[0xF5] && !spcdata[0xF6] && !spcdata[0xF7]);

  //Calculate out echo region
  unsigned short echo_region = dspdata[0x6D] << 8;
  unsigned short echo_size = (dspdata[0x7D] & 0x0F) * 2048;
  if (echo_size == 0)
    echo_size = 4;

  if (debug_print)
  {
    Serial.print(F("SPC echo region is: ")); printf("%04X\n", echo_region);
    Serial.print(F("SPC echo size is: ")); printf("%04X\n", echo_size);
  }
  //Locate a spot to write the bootloader now.
  int freespacesearch = 0;

  if (bootcode_offset > 0)
  {
    freespacesearch = 4;  //No need to search for space, spc2 already tells us where to put it. :)
    if (bootcode_offset == 1)
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


  if (freespacesearch < 2)
  {
    for (i = 255; i < 256; i -= 255)
    {
      unsigned long compare = i | (i << 8);
      compare <<= 16;
      compare |= i | (i << 8);
      count = 0;
      digitalWrite(SRAM_PIN_0, HIGH);
      for (j = 0xFFBC; j >= 0x100; j -= 4)
      {
        if (j == 0x7FFC)
          digitalWrite(SRAM_PIN_0, LOW);
        if ((j > (echo_region + (echo_size - 1))) || (j < echo_region))
        {
          if (_SFR_MEM32(0x8000 + (j & 0x7FFF)) == compare)
            count += 4;
          else
            count = 0;
          if (count >= sizeof(boot_code))
          {
            count = sizeof(boot_code);
            break;
          }
        }
        else
        {
          count = 0;
          j = echo_region;
          if (j == 0)
            break;
        }
      }
      if (count == sizeof(boot_code)) {
        if (debug_print) Serial.print(F("Type 1 search found enough\n"));
        break;
      }
    }
  }

  if (freespacesearch == 0 || freespacesearch == 2)
  {
    if (count != sizeof(boot_code))
    {
      unsigned long compare_zero = 0;
      unsigned long compare_one = 0xFFFFFFFF;
      digitalWrite(SRAM_PIN_0, HIGH);
      for (j = 0xFFBC; j >= 0x100; j -= 4)
      {
        if (j == 0x7FFC)
          digitalWrite(SRAM_PIN_0, LOW);
        if ((j > (echo_region + (echo_size - 1))) || (j < echo_region))
        {
          if (((j % 64) > 31) && _SFR_MEM32(0x8000 + (j & 0x7FFF)) == compare_one)
          {
            count += 4;
          }
          else if (((j % 64) <= 31) && _SFR_MEM8(0x8000 + (j & 0x7FFF)) == compare_zero)
          {
            count += 4;
          }
          else
          {
            count = 0;
          }
          if (count >= sizeof(boot_code)) {
            if (debug_print) Serial.print(F("Type 2 search found enough\n"));
            count = sizeof(boot_code);
            break;
          }
        }
      }
    }
  }

  if (freespacesearch == 0 || freespacesearch == 3)
  {
    if (count != sizeof(boot_code))
    {
      if (echo_size < sizeof(boot_code) || echo_region == 0)
      {
        count = 0;
      }
      else
      {
        count = sizeof(boot_code);
        j = echo_region;
        if (debug_print) Serial.print(F("Type 3 search found enough\n"));
      }
    }
  }

  if (count != sizeof(boot_code))
  {
    Serial.print(F("Upload failed - Couldn't find good spot for bootloader!\n"));
    count = sizeof(boot_code);
    j = 0xFF00;
  }

  unsigned short boot_code_dest = j;
  unsigned short boot_code_size = sizeof(boot_code);
  if (debug_print)
  {
    Serial.print(F("boot_code_dest: ")); printf("%04X\n", boot_code_dest);
    Serial.print(F("boot_code_size: ")); printf("%04X\n", boot_code_size);
  }

  // Boot code section
  if (bootcode_offset != 2)
  {
    // Adjust the boot code with values from this SPC
    boot_code[ 1] = spcdata[0x00];  // SPCRam Address 0x0000
    boot_code[ 4] = spcdata[0x01];  // SPCRam Address 0x0001
    boot_code[16] = spcdata[0xF4] + (spcinportiszero ? 1 : 0); // Inport 0
    boot_code[22] = spcdata[0xF7];  // Inport 3
    boot_code[26] = spcdata[0xF1] & 0xCF;  // Control Register
    boot_code[32] = dspdata[0x6C];      // DSP Echo Control Register
    boot_code[38] = dspdata[0x4C];      // DSP KeyON Register
    boot_code[41] = spcdata[0xF2];  // Current DSP Register Address

    digitalWrite(SRAM_PIN_0, boot_code_dest < 0x8000 ? LOW : HIGH);
    for (i = boot_code_dest; i < (boot_code_dest + boot_code_size); i++)
    {
      if (i == 0x8000)
        digitalWrite(SRAM_PIN_0, HIGH);
      _SFR_MEM8(0x8000 + (i & 0x7FFF) + 0) = boot_code[i - boot_code_dest];
    }
    if (debug_print)
    {
      Serial.print(F("Write bootcode start: ")); printf("%04X\n", boot_code_dest);
      Serial.print(F("Write bootcode end: ")); printf("%04X\n", (boot_code_dest + boot_code_size - 1));
    }
  }

  //Initialize the Stack
  digitalWrite(SRAM_PIN_0, LOW);
  spc_push(&ApuSP, PCH, F("Write PCH: "));
  spc_push(&ApuSP, PCL, F("Write PCL: "));
  spc_push(&ApuSP, ApuSW, F("Write PSW: "));
  spc_push(&ApuSP, Y, F("Write Y: "));
  spc_push(&ApuSP, X, F("Write X: "));
  spc_push(&ApuSP, A, F("Write A: "));
  spcdata[0xFF] = ApuSP;
  if (debug_print) {
    Serial.print(F("Stack Pointer: "));
    printf("%02X\n", ApuSP);
  }


  // Clear out echo region instead of copying it
  if ((((dspdata[0x6C] & 0x20) == 0) && (echo_clear == 0)) || (echo_clear == 1))
  {
    digitalWrite(SRAM_PIN_0, echo_region < 0x8000 ? LOW : HIGH);
    for (i = echo_region; i < (echo_region + echo_size); i++)
    {
      if (i == 0x8000) digitalWrite(SRAM_PIN_0, HIGH);
      _SFR_MEM8(0x8000 + (i & 0x7FFF) + 0) = 0;
    }
  }

  if (debug_print)
  {
    Serial.print(F("Write echo start: ")); printf("%04X\n", echo_region);
    Serial.print(F("Write echo end: ")); printf("%04X\n", (echo_region + echo_size - 1));
  }



  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(F("Uploading SPC   "));
  lcd.setCursor(0, 1);
  lcd.print(F("Page 0x00"));

  //Prescaler of 0 seems to work reliably for everthing except SPC loading.
  //SPC loading has been found to work most reliably with a prescaler of 1 or slower.
  SetupPrescaler(1);
  // Reset the APU
  int resetAttempts = 0;
  apu.reset();
  delay(50);
  while (apu.read(0) != 0xAA || apu.read(1) != 0xBB
         || apu.read(2) != 0x00 || apu.read(3) != 0x00)
  {
    apu.reset();
    delay(50);
    if (resetAttempts > 20)
    {
      if(SD_mode)
      {
        Serial.print(F("Expected AABB0000, Got "));
        printf("%.2X%.2X%.2X%.2X", apu.read(0), apu.read(1), apu.read(2), apu.read(3));
        Serial.print(F(" :( - Failed to reset the APU\n"));
      }
      else
      {
        Serial.write('0');
        for(i=0;i<4;i++)
          Serial.write(apu.read(i));
      }
      SetupPrescaler(0);
      return;
    }
    resetAttempts++;
  }
  if(SD_mode) Serial.println(F("APU Reset Complete"));

  // Initialise it with the DSP data and SPC data for this track
  i = APU_StartSPC700(dspdata, spcdata, SD_mode);
  if (i > 0)
  {
    if(SD_mode)
    {
      Serial.print(F("Upload failed in APU_StartSCP700: Result ")); printf("%d\n", i);
    }
    else
    {
      Serial.write('1'); Serial.write(i&0xFF);
    }
    SetupPrescaler(0);
    return;
  }

  // Here we upload the SPC, tip-toe around regions where we can't just stream raw from file
  // First we must set the write address to 0x100
  apu.write(1, 1);
  apu.write(2, 0x00);
  apu.write(3, 0x01);
  int rb = apu.read(0);
  rb += 2;
  apu.write(0, rb & 0xFF);
  j = 0;
  while ((apu.read(0) != rb) && (j < 500))
    j++;
  if (j == 500) {
    if(SD_mode)
    {
      Serial.print(F("Upload failed ad address 0x0100"));
    }
    else
    {
      Serial.write('2');
      Serial.write(0x01);
      Serial.write(0x00);
    }
    SetupPrescaler(0);
    return;
  }

  // Write out the SPC Data
  digitalWrite(SRAM_PIN_0, LOW);
  for (i = 0x100; i != 0; i++) // Thank you, overflow :)
  {
    unsigned char port0state = i & 0xFF;
    if (i == 0x8000) digitalWrite(SRAM_PIN_0, HIGH);

    if ((i % 0x400) == 0)
    {
      if(SD_mode) Serial.print('.');
      lcd.setCursor(7, 1);
      if ((i >> 8) < 16) lcd.print('0');
      lcd.print(i >> 8, HEX);
    }

    // Normal data write
    if (!WriteByteToAPUAndWaitForState(_SFR_MEM8(0x8000 + (i & 0x7FFF)), port0state))
    {
      if(SD_mode)
      {
        Serial.print(F("Upload failed at address 0x")); printf("%.4X\n", i);
      }
      else
      {
        Serial.write('2');
        Serial.write(i>>8);
        Serial.write(i&0xFF);
      }
      SetupPrescaler(0);
      return;
    }
  }
  if (SD_mode) Serial.println(F("Done!"));

  apu.write(3, (unsigned char)(boot_code_dest >> 8));
  apu.write(2, boot_code_dest & 0xFF);
  apu.write(1, 0);

  i = apu.read(0);
  i = i + 2;
  apu.write(0, i);
  i = 0;
  if (bootcode_offset != 2)
  {
    if (SD_mode) Serial.print(F("Wait for Play...."));
    if (spcinportiszero) {
      apu.write(3, 1);
      apu.write(0, 1);
    }
    while (apu.read(0) != 0x53) {
      i++;
      if (i > 512) {
        if(SD_mode)
        {
          Serial.println(F("Error loading SPC"));
        }
        else
        {
          Serial.write('3');
        }
        apu.write(0, spcdata[0xF4]);
        apu.write(1, spcdata[0xF5]);
        apu.write(2, spcdata[0xF6]);
        apu.write(3, spcdata[0xF7]);
        SetupPrescaler(0);
        return;
      }
    }
    apu.write(0, spcdata[0xF4]);
    apu.write(1, spcdata[0xF5]);
    apu.write(2, spcdata[0xF6]);
    apu.write(3, spcdata[0xF7]);
  }
  if(SD_mode)
  {
    Serial.println(F("Playing!"));
  }
  else
  {
    Serial.write('S');
  }
  SetupPrescaler(0);
  handleLCD(true);
  if (SD_mode)
  {
    Serial.println(F("----START OF SPC TAGS----"));
    if(tags.spc_filename[0]){Serial.print(F("SPC File name: ")); Serial.println(tags.spc_filename);}
    if(tags.song_title[0]){Serial.print(F("Song Name: ")); Serial.println(tags.song_title);}
    if(tags.game_title[0]){Serial.print(F("Game: ")); Serial.println(tags.game_title);}
    if(tags.song_artist[0]){Serial.print(F("Artists: ")); Serial.println(tags.song_artist);}
    if(tags.dumper_name[0]){Serial.print(F("Dumper: ")); Serial.println(tags.dumper_name);}
    if(tags.comments[0])
    {
      Serial.print(F("Comments: ")); 
      Serial.println(tags.comments);
      Serial.println(F("----End of Comments----"));
    }
    if(tags.ost_title[0]){Serial.print(F("Original Soundtrack Title: ")); Serial.println(tags.ost_title);}
    if(tags.ost_disc || tags.ost_track)
    {
      Serial.print(F("OST Disc/Track #: "));
      if(tags.ost_disc)
      {
        Serial.print(F("Disc: "));
        Serial.print(tags.ost_disc,DEC);
      }
      if(tags.ost_disc && tags.ost_track)
        Serial.print(F(" - "));
      if(tags.ost_track)
      {
        int track = tags.ost_track & 0xFF;
        track <<= 8;
        track |= (tags.ost_track >> 8);
        Serial.print(F("Track: "));
        Serial.print(track,DEC);
      }
      Serial.println();
    }
    if(tags.pub_name[0]){Serial.print(F("Publisher name: ")); Serial.println(tags.pub_name);}
    if(tags.copyright){Serial.print(F("Copyright Year: ")); Serial.println(tags.copyright);}
    if(tags.intro_len)
    {
      Serial.print(F("Play Time: "));
      int tlen = tags.intro_len / 64000;
      Serial.print(tlen / 60);
      Serial.print(':');
      if((tlen % 60) < 10) Serial.print('0');
      Serial.println(tlen % 60);
    }
    if(tags.fade_len)
    {
      Serial.print(F("Fadeout Time: "));
      int tlen = tags.fade_len / 64000;
      Serial.print(tlen / 60);
      Serial.print(':');
      if((tlen % 60) < 10) Serial.print('0');
      Serial.println(tlen % 60);
    }
    Serial.println(F("----END OF SPC TAGS----"));
  }
  play_time = 0;
  total_time = tags.intro_len;
  total_time += tags.fade_len;
  total_time /= 64;
  set_current_song();
  file_changed = true;
  just_uploaded = true;
}

void set_current_song()
{
  song_depth = filedepth;
  for(int i=0;i<MAX_FILE_DEPTH;i++)
    song_current[i] = filecurrent[i];
}

void clear_current_song()
{
  song_depth = -1;
  for(int i=0;i<MAX_FILE_DEPTH;i++)
    song_current[i] = -1;
}

bool is_current_song_selected()
{
  if(song_depth != filedepth) return false;
  for (int i=0;i<MAX_FILE_DEPTH;i++)
  {
    if(song_current[i] != filecurrent[i])
      return false;
  }
  return true;
}

#endif

/*--------------------------------------------------------*

   ProcessCommand() - Processes a serial port command

   --------------------------------------------------------
*/

void WriteByteToSerial(unsigned char data, bool forceRawByte = false)
{
  if (!dataModeText || forceRawByte)
    Serial.write(data);
  else
  {
    if (data < 16)
      Serial.print('0');
    Serial.print(data, HEX);
  }
}

unsigned char ReadByteFromSerial(bool forceRawByte = false)
{
  while ((Serial.available() == 0));
  if (forceRawByte) return Serial.read();
  return (dataModeText ? ReadHexFromSerial() : Serial.read());
}

unsigned char ReadHexFromSerial()
{
  while ((Serial.available() < 2));
  unsigned char data = 0;
  unsigned char data2 = Serial.read();
  if (data2 >= 'a')
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
  if (data2 >= 'a')
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

#ifdef ARDUINO_MEGA
bool newSPCinRAM=false;
void UploadSPCFromPC()
{
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print(F("  Loading SRAM  "));
  lcd.setCursor(0, 1);
  lcd.print(F("Page 0x00"));
  unsigned short i,j;
  digitalWrite(SRAM_PIN_1, LOW);
  digitalWrite(SRAM_PIN_0, LOW);

  for (i = 0; i < 0x8000; i++)
  {
    if (!(i & 0x3FF))
    {
      Serial.write('.');
      lcd.setCursor(7, 1);
      if ((i >> 8) < 16) lcd.print('0');
      lcd.print(i >> 8, HEX);
    }
    _SFR_MEM8(0x8000 + (i & 0x7FFF)) = ReadByteFromSerial(true);
  }

  digitalWrite(SRAM_PIN_0, HIGH);

  for (i = 0; i < 0x8000; i++)
  {
    if (!(i & 0x3FF))
    {
      Serial.write('.');
      lcd.setCursor(7, 1);
      lcd.print((i >> 8) + 0x80, HEX);
    }
    _SFR_MEM8(0x8000 + (i & 0x7FFF)) = ReadByteFromSerial(true);
  }

  digitalWrite(SRAM_PIN_1, HIGH);
  digitalWrite(SRAM_PIN_0, LOW);

  lcd.setCursor(0, 0);
  lcd.print(F("Loading Register"));
  
  for (i = 0; i < 0x200; i++)
    _SFR_MEM8(0x8000 + (i & 0x7FFF)) = ReadByteFromSerial(true);
  
  Serial.write('.');

  lcd.setCursor(0, 0);
  lcd.print(F("  Loading Tags  "));
  j=ReadByteFromSerial(true);
  j<<=8;
  j|=ReadByteFromSerial(true);
  
  if(j)
  {
    
    for(i = 0; i < j; i++)
    {
      _SFR_MEM8(0x8200 + (i & 0x7FFF)) = ReadByteFromSerial(true);
    }
    ProcessSPCTags();
  }
  Serial.write('.');
  
  
  lcd.clear();
  lcd.setCursor(6, 0);
  lcd.print(F("Done"));
  newSPCinRAM = true;
}
#endif

bool AbortOnDataModeText(unsigned char command)
{
  if (!dataModeText) return false;
  switch (command)
  {
    case 'f':
    case 'F':
    case 'G':
    case 'u':
    case 'U':
      Serial.println(F("This Command is meant for the PC interface program"));
      return true;
  }
  return false;
}

void ProcessCommandFromSerial()
{
  if (Serial.available() == 0)
    return;
  static unsigned char port0state = 0;
  uint16_t i;
  unsigned long j;
  char filename[256];
  unsigned char spcdata[256];
  unsigned char dspdata[128];
  static long time;
  int checksum;
  unsigned char address, data;
  unsigned char command = ReadByteFromSerial(true);
  unsigned short dir_num;
#ifdef ARDUINO_MEGA
  File upload;
#endif
  if (AbortOnDataModeText(command)) return;
  switch (command)
  {
    case 'D':    //Set datamode between ascii and binary.  In ascii mode,  each data byte is transmitted
      //as ascii hexadecimal notation.  In binary mode, the byte transmitted on the serial
      //line is taken literally for what it is.
      dataModeText = ReadByteFromSerial(true) == '1';
      // Scracth that deprecation.  This can still be useful when the interface is operating in standalone mode.

      break;


    case 'Q':  //Read all 4 IO ports in rapid fire succession.
      if (dataModeText) Serial.print(F("Reading All 4 APU Addresses: "));
      data = apu.read(0);
      WriteByteToSerial(data);
      data = apu.read(1);
      WriteByteToSerial(data);
      data = apu.read(2);
      WriteByteToSerial(data);
      data = apu.read(3);
      WriteByteToSerial(data);
      if (dataModeText) Serial.println();
      break;
    case 'R':  //Read a single IO port.  Takes an address as a parameter, returns one byte.
      if (dataModeText) Serial.println(F("Input an Address (0-3): "));
      address = ReadByteFromSerial(true) - '0';
      if (dataModeText)
      {
        Serial.print(F("Reading Address "));
        Serial.print(address, DEC);
        Serial.print(':');
      }
      data = apu.read(address);
      WriteByteToSerial(data);
      if (dataModeText) Serial.println();
      //printf("Data at port %d is 0x%.2X\n",address,data);
      break;
    case 'r':  //Read and returns 2 consecutive IO ports.
      if (dataModeText) Serial.println(F("Input an Address (0-3): "));
      address = ReadByteFromSerial(true) - '0';
      if (dataModeText)
      {
        Serial.print(F("Reading Address "));
        Serial.print(address, DEC);
        Serial.print(':');
      }
      data = apu.read(address + 1);
      WriteByteToSerial(data);
      data = apu.read(address);
      WriteByteToSerial(data);
      if (dataModeText) Serial.println();
      break;
    case 'S':
      Serial.println(F("SPC700 DATA LOADER V1.0"));
      break;
    case 's':  //Reset the Audio Processing Unit
      apu.reset();
      if (dataModeText)
        Serial.println(F("APU Reset :)"));
      break;
    case 'f':  //Set the expected state of PORT 0 for the next write.
      port0state = ReadByteFromSerial();
      break;
    case 'F':  //Upload SPC data at serial port speed.
      uint16_t ram_addr;
      uint16_t data_size;

      ram_addr = ReadByteFromSerial() << 8;
      ram_addr |= ReadByteFromSerial();

      data_size = ReadByteFromSerial() << 8;
      data_size |= ReadByteFromSerial();

      apu.write(1, 1);
      apu.write(2, ram_addr & 0xFF);
      apu.write(3, ram_addr >> 8);
      i = apu.read(0);
      i += 2;
      apu.write(0, i & 0xFF);
      while (apu.read(0) != (i & 0xFF));
      port0state = 0;

      for (i = 0; i < data_size; i++)
      {
        data = ReadByteFromSerial();
        apu.write(1, data);
        apu.write(0, port0state++);
        //Serial port is slow enough that the APU will be ready for the data,
        //when it finally comes in. :)
      }
      Serial.write(1);
      break;

    case 'G':  //Upload the DSP register, and the first page of SPC data at serial port speed.
      for (i = 0; i < 128; i++)
        dspdata[i] = ReadByteFromSerial();
      for (i = 0; i < 256; i++)
        spcdata[i] = ReadByteFromSerial();
      APU_StartSPC700(dspdata, spcdata, 0);
      Serial.write('F');
      break;

    case 'U': //Upload the spc to ram.
#ifdef ARDUINO_MEGA
      UploadSPCFromPC();
#else
      Serial.write('U');
#endif
      break;
    case 'P':
#ifdef ARDUINO_MEGA
      PlaySPC(dataModeText);
#endif
      break;


    case 'u': //Upload the spc as a temp file, and play that one.
#ifdef ARDUINO_MEGA
      for (i = 0, j = 0; i < 4; i++)
      {
        j <<= 8;
        j |= ReadByteFromSerial(true);
      }
      i = 0;
      do {
        filename[i] = ReadByteFromSerial(true);
      } while (filename[i++] && i < 255);
      filename[i] = 0;
      Serial.println(filename);

      upload = SD.open(filename, FILE_WRITE);
      if (upload)
      {
        Serial.write('O');
        i = 0;
        while (j--)
        {
          _SFR_MEM8(0x8000 + (i & 0x7FFF)) = ReadByteFromSerial(true);
          i++;
          if (!(i & 0x7FFF))
          {
            upload.write((unsigned char*)_SFR_MEM8(0x8000), 0x8000);
            Serial.write('A');
          }
        }
        if (i & 0x7FFF)
          upload.write((unsigned char*)_SFR_MEM8(0x8000), i & 0x7FFF);
        upload.close();
        Serial.write('F');
        upload = spcFile;
        spcFile = SD.open(filename);
        if (spcFile)
        {
          if (IsSPC(spcFile))
          {
            LoadAndPlaySPC();
          }
          spcFile.close();
        }
        spcFile = upload;
      }
      else
      {
        Serial.write('F');
      }
#else
      Serial.write('U');
#endif

      break;

    case 'W':  //Write a single IO port.
      if (dataModeText) Serial.println(F("Input an Address (0-3): "));
      address = ReadByteFromSerial(true) - '0';
      if (dataModeText) Serial.print(F("Input a Hex Byte (2 digits): "));
      data = ReadByteFromSerial();
      //data=ReadHexFromSerial();
      i = 0; //Must timeout any wait loops, if the respective drivers are not running.
      switch (address - 4)
      {
        case 0:
          while ((apu.read(1) != ((apu.read(0) + 1) & 0xFF)) && (i < 128)) i++; //Handle a blazeon track change.
          if (i < 128)
          {
            apu.write(0, data);
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
          apu.write(1, data);
          apu.write(0, 3);
          data = apu.read(3);
          data++;
          apu.write(3, data);
          if (APU_WaitIoPort(3, data, 2048))
            break;
          i = 0;
          apu.write(0, 4);
          data = apu.read(3);
          data++;
          apu.write(3, data);
          if (APU_WaitIoPort(3, data, 2048))
            break;
          apu.write(0, 0);
          break;
        case 2:
          apu.write(3, data);
          apu.write(0, 0x80);
          if (APU_WaitIoPort(0, 0, 2048))
            break;
          apu.write(0, 0);
          break;
        case 0xFC:  //0 - 4 = 0xFC.
        case 0xFD:  //1 - 4 = 0xFD.
        case 0xFE:  //2 - 4
        case 0xFF:  //3 - 4
        default:    //Any other driver selection not put here.
          if (dataModeText)
          {
            Serial.print(F("Writing "));
            WriteByteToSerial(data);
            Serial.print(F(" to address "));
            Serial.println(address, DEC);
          }
          /*
          ** Default, is just write the ports as is.
          */
          apu.write(address, data);
      }
      break;
    case 'w':  //Write 2 IO consecutive IO ports.
      if (dataModeText) Serial.println(F("Input an Address (0-3): "));
      address = ReadByteFromSerial(true) - '0';
      if (dataModeText) Serial.print(F("Input a Hex Word (4 digits): "));
      data = ReadByteFromSerial();
      if (dataModeText)
      {
        Serial.print(F("Writing "));
        WriteByteToSerial(data);
      }
      apu.write(address + 1, data);
      data = ReadByteFromSerial();
      if (dataModeText)
      {
        WriteByteToSerial(data);
        Serial.print(F(" to address "));
        Serial.println(address, DEC);
      }
      apu.write(address, data);
      break;
#ifdef ARDUINO_MEGA
    case 'A':
      auto_play = ReadByteFromSerial(true) == '1';
      if(dataModeText)
      {
        if(auto_play)
          Serial.println(F("Auto Play turned on"));
        else
          Serial.println(F("Auto Play turned off"));
      }
      break;
    case 'E':
    case 'e':
      Serial.println(F("Input a directory number:"));
      dir_num = ReadByteFromSerial();
      if (filecounts[filedepth] > 256)
      {
        dir_num <<= 8;
        dir_num |= ReadByteFromSerial();
      }
      filecurrent[filedepth] = dir_num;
      GetFile();
      spcFile = files[filedepth];
      if (spcFile)
      {
        if (spcFile.isDirectory())
        {
          EnterDirectory();
          for (filecurrent[filedepth] = 0; filecurrent[filedepth] < filecounts[filedepth]; filecurrent[filedepth]++)
            GetFile(true, !filecurrent[filedepth], false);
          GetFile();
          
        }
        else if (IsSPC())
        {
          LoadAndPlaySPC();
          auto_play = (command == 'E');
          auto_play_start = filecurrent[filedepth];
        }
      }
      break;
    case 'l':
      LeaveDirectory();
      if(dataModeText)
      {
        i = filecurrent[filedepth];
        for (filecurrent[filedepth] = 0; filecurrent[filedepth] < filecounts[filedepth]; filecurrent[filedepth]++)
          GetFile(true, !filecurrent[filedepth], false);
        filecurrent[filedepth] = i;
        GetFile();
      }
      break;
    case 'T':
      ProcessSPCTags();
      break;
#else
    case 'A':
    case 'T':
    case 'l':
    case 'e':
    case 'E':
      Serial.write('U');
      break;
#endif
    default:
      if (dataModeText)
      {
        Serial.println(F("Commands to use:"));
        Serial.println(F("\tQ - Read out all 4 APU ports at once"));
        Serial.println(F("\tR - Takes an Address (0-3) and reads that byte."));
        Serial.println(F("\tr - Takes an Address (0-3) and reads 2 consecutive bytes"));
        Serial.println(F("\ts - Resets the APU"));
        Serial.println(F("\tW - Takes an Address (0-3), and 2 hex characters, and writes it there."));
        Serial.println(F("\t\tThere are a few driver handlers implemented."));
        Serial.println(F("\tw - Writes 2 consecutive bytes on Addresses 0-3"));
      }
      break;
  }
}

#ifdef ARDUINO_MEGA
int IsNumeric(char *str, u32 length)
{
  u32 c = 0;
  while (c < length && isdigit(str[c])) c++;
  if (c == length || str[c] == 0)
    return c;
  else
    return -1;
}

int CountNumbers(char *str, u32 length)
{
  u32 c = 0;
  while (c < length && isdigit(str[c])) c++;
  return c;
}

int IsDate(char *str, u32 length)
{
  u32 c = 0;
  while (c < length && (isdigit(str[c]) || str[c] == '/' || str[c] == '-')) c++;
  if (c == length || str[c] == 0)
    return c;
  else
    return -1;
}


void smemcpy(u8 *dst, u8 *src, u32 len)
{
  u32 len_left = len;
  u8 buf[512];
  while (len_left)
  {
    digitalWrite(SRAM_PIN_0, LOW);
    memcpy(buf, src, len_left < 512 ? len_left : 512);
    digitalWrite(SRAM_PIN_0, HIGH);
    memcpy(dst, buf, len_left < 512 ? len_left : 512);
    len_left -= (len_left < 512) ? len_left : 512;
  }
  digitalWrite(SRAM_PIN_0, LOW);
}

void ProcessSPCTags()
{
  spc_sram_struct *s = (spc_sram_struct*)&_SFR_MEM8(0x8000);

  spc_header *h = &s->header;
  spc_id666_text *it = &s->tag_text;
  spc_id666_bin *ib = &s->tag_binary;

  spc_idx6_header *idx6h = &s->idx6;
  spc_idx6_sub_header *idx6sh;

  digitalWrite(SRAM_PIN_1, HIGH);
  digitalWrite(SRAM_PIN_0, LOW);

  clearBuffer((u8*)&tags, sizeof(spc_idx6_table));

  int tag_format = SPC_TAG_PREFER_BINARY;
  int i, j, k, l;
  int y, m, d;

  i = IsNumeric(it->song_length_secs, 3);
  j = IsNumeric(it->fade_length_ms, 5);
  k = IsDate(&it->date_dumped[0], 11);

  if (!(i | j | k))
  {
    if (it->channel_disable == 1 && it->emulator == 0)
      tag_format = SPC_TAG_BINARY;
  }
  else
  {
    if (i != -1 && j != -1)
    {
      if (k > 0)
        tag_format = SPC_TAG_TEXT;
      else
      {
        if (k == -1)
        {
          if (!((u32*)&it->date_dumped)[1])
            tag_format = SPC_TAG_BINARY;
          else
            tag_format = SPC_TAG_TEXT;
        }
        else
        {
          if ((it->fade_length_ms[4] == 0) && (isascii(it->song_artist[0]) && (it->song_artist[0] != 0)))
            tag_format = SPC_TAG_TEXT; //Conclusively text.
          if ((i == 3) || (j == 5))
            tag_format = SPC_TAG_TEXT;
        }
      }
    }
  }

  if (tag_format == SPC_TAG_TEXT)
  {
    //printf("tag format : text\n");
    i = CountNumbers(&it->date_dumped[0], 11);
    j = CountNumbers(&it->date_dumped[i + 1], 11 - i - 1);
    k = CountNumbers(&it->date_dumped[i + 1 + j + 1], 11 - j - 1);

    if ((i == 4 && j > 0 && j <= 2 && k > 0 && k <= 2) || (i > 0 && i <= 2 && j > 0 && j <= 2 && k == 4))
    {

      if (i == 4) //YYYY.MM.DD format.
      {
        for (l = 0, y = 0; l < i; l++)
        {
          y *= 10;
          y += (it->date_dumped[l] - 48);
        }
        for (l = (i + 1), m = 0; l < (i + 1 + j); l++)
        {
          m *= 10;
          m += (it->date_dumped[l] - 48);
        }
        for (l = (i + 1 + j + 1), d = 0; l < (i + 1 + j + 1 + k); l++)
        {
          d *= 10;
          d += (it->date_dumped[l] - 48);
        }
      }
      else
      {
        for (l = 0, m = 0; l < i; l++)
        {
          m *= 10;
          m += (it->date_dumped[l] - 48);
        }
        for (l = (i + 1), d = 0; l < (i + 1 + j); l++)
        {
          d *= 10;
          d += (it->date_dumped[l] - 48);
        }
        for (l = (i + 1 + j + 1), y = 0; l < (i + 1 + j + 1 + k); l++)
        {
          y *= 10;
          y += (it->date_dumped[l] - 48);
        }
      }
      if (m > 12)
      {
        i = m;
        m = d;
        d = i;
      }

      digitalWrite(SRAM_PIN_0, HIGH);
      if ((m == 0) || (m > 12) || (d == 0) || (d > 31) || (y < 1900) || (y > 2999))
        tags.date = 0;
      else
        tags.date = ((m / 10) << 28) |
                    ((m % 10) << 24) |
                    ((d / 10) << 20) |
                    ((d % 10) << 16) |
                    (((y / 100) / 10) << 12) |
                    (((y / 100) % 10) << 8) |
                    (((y % 100) / 10) << 4) |
                    (((y % 100) % 10) << 0);
    }
    else
    {
      digitalWrite(SRAM_PIN_0, HIGH);
      tags.date = 0;  //Impossible or zero.
    }

    // impossible, or zero
    if (tags.date > 0x99999999) tags.date = 0;
    //printf("date: %08x\n", s->date);

    u8 buf[8];

    digitalWrite(SRAM_PIN_0, LOW);
    memcpy(buf, &it->song_length_secs, 3); buf[4] = 0;
    digitalWrite(SRAM_PIN_0, HIGH);
    tags.intro_len = atoi((char*)buf) * 64000;
    digitalWrite(SRAM_PIN_0, LOW);
    memcpy(buf, &it->fade_length_ms, 5); buf[6] = 0;
    digitalWrite(SRAM_PIN_0, HIGH);
    tags.fade_len = atoi((char*)buf) * 64;

    smemcpy((u8*)&tags.song_title, (u8*)&it->song_title, 32);
    smemcpy((u8*)&tags.game_title, (u8*)&it->game_title, 32);
    smemcpy((u8*)&tags.dumper_name, (u8*)&it->dumper_name, 16);
    smemcpy((u8*)&tags.comments, (u8*)&it->comments, 32);
    smemcpy((u8*)&tags.song_artist, (u8*)&it->song_artist, 32);
  }
  else
  {
    // binary
    //printf("tag format : binary\n");

    y = (ib->date_dumped >> 16) & 0xffff;
    m = (ib->date_dumped >> 8) & 0xff;
    d = (ib->date_dumped >> 0) & 0xff;

    if (m > 12)
    {
      i = m;
      m = d;
      d = i;
    }

    digitalWrite(SRAM_PIN_0, HIGH);
    if ((m == 0) || (m > 12) || (d == 0) || (d > 31) || (y < 1900) || (y > 2999))
      tags.date = 0;
    else
      tags.date = ((m / 10) << 28) |
                  ((m % 10) << 24) |
                  ((d / 10) << 20) |
                  ((d % 10) << 16) |
                  (((y / 100) / 10) << 12) |
                  (((y / 100) % 10) << 8) |
                  (((y % 100) / 10) << 4) |
                  (((y % 100) % 10) << 0);

    //printf("date: %08x\n", s->date);

    smemcpy((u8*)&tags.intro_len, (u8*)&ib->song_length_secs, 4);
    tags.intro_len &= 0x00FFFFFF;
    tags.intro_len *= 64000;
    smemcpy((u8*)&tags.fade_len, (u8*)&ib->fade_length_ms, 4);
    tags.fade_len *= 64;
    smemcpy((u8*)&tags.song_title, (u8*)&ib->song_title, 32);
    smemcpy((u8*)&tags.game_title, (u8*)&ib->game_title, 32);
    smemcpy((u8*)&tags.dumper_name, (u8*)&ib->dumper_name, 16);
    smemcpy((u8*)&tags.comments, (u8*)&ib->comments, 32);
    smemcpy((u8*)&tags.song_artist, (u8*)&ib->song_artist, 32);
  }


  if (strncmp((char*)&idx6h->header, "xid6", 4) == 0) {
    unsigned short offset = 0;

    while ((offset < idx6h->size) && (offset < 32256))
    {
      digitalWrite(SRAM_PIN_0, LOW);
      idx6sh = (spc_idx6_sub_header*)&idx6h->data[offset];
      int tag_length = ((idx6sh->Type) ? idx6sh->Length : 2);
      switch (idx6sh->ID)
      {
        case IDX6_SONGNAME:
          smemcpy((u8*)&tags.song_title, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length > MAX_SONG_TITLE ? MAX_SONG_TITLE : tag_length);
          break;
        case IDX6_ARTISTNAME:
          smemcpy((u8*)&tags.song_artist, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length > MAX_SONG_ARTIST ? MAX_SONG_ARTIST : tag_length);
          break;
        case IDX6_COMMENTS:
          smemcpy((u8*)&tags.comments, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length > MAX_COMMENTS ? MAX_COMMENTS : tag_length);
          break;
        case IDX6_DUMPERNAME:
          smemcpy((u8*)&tags.dumper_name, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length > MAX_DUMPER_NAME ? MAX_DUMPER_NAME : tag_length);
          break;
        case IDX6_PUBNAME:
          smemcpy((u8*)&tags.pub_name, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length > MAX_PUB_NAME ? MAX_PUB_NAME : tag_length);
          break;
        case IDX6_GAMENAME:
          smemcpy((u8*)&tags.game_title, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length > MAX_GAME_TITLE ? MAX_GAME_TITLE : tag_length);
          break;
        case IDX6_OSTTITLE:
          smemcpy((u8*)&tags.ost_title, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length > MAX_OST_TITLE ? MAX_OST_TITLE : tag_length);
          break;
        case IDX6_OSTDISC:
          smemcpy((u8*)&tags.ost_disc, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), 1);
          break;
        case IDX6_OSTTRACK:
          smemcpy((u8*)&tags.ost_track, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), 2);
          break;
        case IDX6_COPYRIGHT:
          smemcpy((u8*)&tags.copyright, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), 2);
          break;
        case IDX6_INTROLEN:
          smemcpy((u8*)&tags.intro_len, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length);
          break;
        case IDX6_FADELEN:
          smemcpy((u8*)&tags.fade_len, ((idx6sh->Type) ? idx6sh->data : (u8*)&idx6sh->Length), tag_length);
          break;
      }
      offset += 4 + ((idx6sh->Type) ? ((idx6sh->Length + 3) & (~3)) : 0);

    }

  }
}

int date_time = 250;

char date_time_line[16];

int lcd_display_mode = LCD_DISPLAY_MODE_NORMAL;
void switch_lcd_display_mode(int mode)
{
  if(lcd_display_mode != mode)
  {
    lcd_display_mode = mode;
    switch(lcd_display_mode)
    {
      case LCD_DISPLAY_MODE_NORMAL:
        lcd_buffer[0] = lcd_line_0;
        lcd_buffer[1] = lcd_line_1;
        line_len_max[0] = LINE_0_LEN;
        line_len_max[1] = LINE_1_LEN;
        
        break;
      case LCD_DISPLAY_MODE_DATETIME:
        lcd_buffer[0] = lcd_line_0;
        lcd_buffer[1] = date_time_line;
        line_len_max[0] = LINE_0_LEN;
        line_len_max[1] = 16;
        break;
    }
    rowlen[0] = len(lcd_buffer[0],line_len_max[0]);
    rowlen[1] = len(lcd_buffer[1],line_len_max[1]);
  }
}

bool refreshRTC(bool force_refresh = false)
{
  if (!rtc_found) return false;
  if(lcd_display_mode != LCD_DISPLAY_MODE_DATETIME) return false;

  date_time--;
  if (!date_time)
    date_time = 250;

  if (!force_refresh && (date_time % 25)) return false;
  DateTime now = rtc.now();
  if (date_time <= 125)
    sprintf(date_time_line, "%d:%.2d:%.2d", now.hour(), now.minute(), now.second());
  else
    sprintf(date_time_line, "%d/%.2d/%.2d", now.year(), now.month(), now.day());
  return true;
}


u32 prev_play_time;
bool songUpdateLCD()
{
  if (is_current_song_selected())
  {
    bool time_changed = ((play_time / 1000) != prev_play_time);
    prev_play_time = play_time / 1000;

    if (file_changed)
    {
      if(tags.spc_filename[0])
      {
        sprintf(&lcd_buffer[0][0], "(%s) %s - %s - %s", tags.spc_filename, tags.game_title, tags.song_title, tags.song_artist);
      }
      else
      {
        sprintf(&lcd_buffer[0][0], "%s - %s - %s", tags.game_title, tags.song_title, tags.song_artist);
      }
      rowlen[0] = len(lcd_buffer[0], line_len_max[0]);
      lcd_delay[0] = LCD_SCROLL_DELAY_SHORT;
      time_changed = true;
      file_changed = false;
    }
    if (!time_changed) return false;

    u32 play_seconds = play_time / 1000;
    u32 total_seconds = total_time / 1000;

    rowlen[1] = 0;
    sprintf(&lcd_buffer[1][rowlen[1]], "%.2d", play_seconds / 60);
    rowlen[1] = len(lcd_buffer[1], line_len_max[1]);
    sprintf(&lcd_buffer[1][rowlen[1]], ":%.2d\0", play_seconds % 60);
    rowlen[1] = len(lcd_buffer[1], line_len_max[1]);
    sprintf(&lcd_buffer[1][rowlen[1]], " - %.2d\0", total_seconds / 60);
    rowlen[1] = len(lcd_buffer[1], line_len_max[1]);
    sprintf(&lcd_buffer[1][rowlen[1]], ":%.2d\0", total_seconds % 60);
    rowlen[1] = 16;
    return true;
  }
  else
  {
    if (file_changed)
    {
      file_changed = false;
      refreshLCD();
      return true;
    }
  }
  return false;
}


void handleLCD(bool force_refresh)
{
  force_refresh |= songUpdateLCD();
  force_refresh |= refreshRTC(force_refresh);
  for(int i = 0; i < 2; i++)
  {
    if(rowlen[i] <= 16)
      continue;
    lcd_delay[i]--;
    if (lcd_delay[i] <= 0)
    {
      force_refresh = true;
      
      lcd_pos[i]++;
      if (lcd_pos[i] == rowlen[i])
        lcd_pos[i] = -4;
      
      lcd_delay[i] = (lcd_pos[i] == 0 ? LCD_SCROLL_DELAY_LONG : LCD_SCROLL_DELAY_SHORT);
      if (is_current_song_selected())
        lcd_delay[i] = LCD_SCROLL_DELAY_SHORT;
    }
  }
  if (force_refresh)
  {
    lcd.clear();
    for (int k = 0; k < 2; k++)
    {
      if (rowlen[k] <= 16)
      {
        lcd.setCursor(0, k);
        lcd.print(lcd_buffer[k]);
      }
      else
      {
        int j = 0 - (lcd_pos[k] < 0 ? lcd_pos[k] : 0);
        lcd.setCursor(j, k);
        for (int i = (lcd_pos[k] + j); j < 16; i++, j++)
        {
          if (i < rowlen[k])
            lcd.print(lcd_buffer[k][i]);
          else if (i >= (rowlen[k]+4))
            lcd.print(lcd_buffer[k][i-(rowlen[k]+4)]);
          else
            lcd.print(' ');
        }
      }
    }
  }
}



bool buttons[5];

void clearButtons()
{
  for(int i = 0; i < 5; i++)
    buttons[i] = false;
}

void readButtons()
{
  for (int i = 0; i < 5; i++)
    buttons[i] = digitalRead(65 + i) == LOW;
  delay(40);
  for (int i = 0; i < 5; i++)
    buttons[i] &= digitalRead(65 + i) == LOW;
}

bool IsButtonPressed(int button)
{
  return digitalRead(65 + button) == LOW;
}

bool IsButtonReleased(int button)
{
  return !IsButtonPressed(button);
}

void handleButtons(bool skipRead = false)
{
  int count = 0;
  if(!skipRead)
    readButtons();

handleButtons_top:
  if (buttons[UP])
  {
    GetPrevFile(true);
    while (buttons[UP] && (count < 15))
    {
      count++;
      readButtons();
    }
    if (count == 15)
    {
      if (IsSPC())
        LoadAndPlaySPC();
      else
        file_changed = true;
    }
    else
    {
      file_changed = true;
    }
    while (IsButtonPressed(UP));
  }
  if (buttons[DOWN])
  {
    GetNextFile(true);
    while (buttons[DOWN] && (count < 15))
    {
      count++;
      readButtons();
    }
    if (count == 15)
    {
      if (IsSPC())
        LoadAndPlaySPC();
      else
        file_changed = true;
    }
    else
    {
      file_changed = true;
    }
    while (IsButtonPressed(DOWN));
  }
  if (buttons[LEFT])
  {
    while (buttons[LEFT] && (count < 15))
    {
      count++;
      readButtons();
    }
    if (count < 15)
    {
      if (filedepth > 0)
      {
        LeaveDirectory();
        file_changed = true;
      }
    }
    else
    {
      clear_current_song();
      auto_play = false;
      apu.reset();
    }
    while (IsButtonPressed(LEFT));
  }
  if (buttons[RIGHT])
  {
    spcFile = files[filedepth];
    if (spcFile)
    {
      if (spcFile.isDirectory())
      {
        EnterDirectory();
        GetFile(true);
        file_changed = true;
      }
      else if (IsSPC())
      {
        LoadAndPlaySPC();
        auto_play = IsButtonPressed(RIGHT);
        auto_play_start = filecurrent[filedepth];
      }
    }
    while (IsButtonPressed(RIGHT));
  }

  if (buttons[SELECT])
  {
    switch_lcd_display_mode(LCD_DISPLAY_MODE_DATETIME);
    handleLCD(true);
    do
    {
      for (int i = 0; i < 250; i++)
      {
        readButtons();
        handleLCD();
        if ((buttons[UP] || buttons[DOWN] || buttons[LEFT] || buttons[RIGHT]))
          break;
      }
      if (buttons[RIGHT] && buttons[SELECT])
        PlaySPC();
    } while (buttons[SELECT]);
    switch_lcd_display_mode(LCD_DISPLAY_MODE_NORMAL);
    handleLCD(true);
    if (!buttons[SELECT])
      goto handleButtons_top;
    else
    {
      //place to handle other menu code.
    }
  }
}

#endif

void loop() //The main loop.  Define various subroutines, and call them here. :)
{
  unsigned long loop_start, loop_end;
  loop_start = millis();
  ProcessCommandFromSerial();
#ifdef ARDUINO_MEGA
  handleButtons();
  handleLCD();
  loop_end = millis();
  loop_end -= loop_start;

  if (just_uploaded)
  {
    just_uploaded = false;
    if (!auto_play)
      play_time = 0;
    return;
  }

  play_time += loop_end;
  if (auto_play)
  {
    if(play_time >= total_time)
    {
      if(tags.spc_filename[0])
      {
        do
        {
          GetNextFile(true);
        } while (!IsSPC() && (auto_play_start < filecurrent[filedepth]));
        if (auto_play_start >= filecurrent[filedepth])
        {
          auto_play = false;
          clear_current_song();
          apu.reset();
          Serial.println(F("Last SPC file in current directory finished playing"));
          return;
        }
        LoadAndPlaySPC();
      }
      else if (newSPCinRAM)
      {
        PlaySPC();
        newSPCinRAM = false;
        Serial.println(F("Ready for next SPC"));
      }
      else
      {
        auto_play = false;
        clear_current_song();
        apu.reset();
        Serial.println(F("SPC in ram is done playing."));
      }
    }
  }
#endif
}


