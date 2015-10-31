copy bin\apuplay.exe apuplay.exe
copy bin\APU_DLL.DLL APU_DLL.DLL
mkdir SNES_APU_SD
copy ..\*.ino SNES_APU_SD\
copy ..\*.cpp SNES_APU_SD\
copy ..\*.h SNES_APU_SD\
7z a -tZip -y ..\apuplay_binary.zip apuplay.exe APU_DLL.DLL SNES_APU_SD
del /Q SNES_APU_SD\*.*
rmdir SNES_APU_SD
del apuplay.exe
del APU_DLL.DLL
