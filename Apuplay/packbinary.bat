copy bin\apuplay.exe apuplay.exe
copy bin\APU_DLL.DLL APU_DLL.DLL
mkdir SNES_APU_SD
copy ..\SNES_APU_SD.ino SNES_APU_SD\SNES_APU_SD.ino
7z a -tZip -y ..\apuplay_binary.zip apuplay.exe APU_DLL.DLL SNES_APU_SD
del SNES_APU_SD\SNES_APU_SD.ino
rmdir SNES_APU_SD
del apuplay.exe
del APU_DLL.DLL
