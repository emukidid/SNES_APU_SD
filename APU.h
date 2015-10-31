#ifndef APU_h
#define APU_h

#include <inttypes.h>

#if (defined(__AVR_ATmega2560__) || defined(__AVR_ATmega1280__))
  #define RESET_PIN_0 54
  #define RESET_PIN_1 43
  #define ARDUINO_MEGA
#else
  #define RESET_PIN 14
#endif

class APU
{
public:
  APU();
  void init();
  uint8_t read(uint8_t address);
  void write(uint8_t address, uint8_t data);
  void reset();
private:
  uint8_t read_arduino(uint8_t address);
  uint8_t read_arduino_mega(uint8_t address);
  uint8_t read_arduino_mega_xmem(uint8_t address);
  void write_arduino(uint8_t address, uint8_t data);
  void write_arduino_mega(uint8_t address, uint8_t data);
  void write_arduino_mega_xmem(uint8_t address, uint8_t data);
  void reset_arduino();
  void reset_arduino_mega();
  void reset_arduino_mega_xmem();
  void init_mega();
  uint8_t mega_type;
  
  
};

#endif
