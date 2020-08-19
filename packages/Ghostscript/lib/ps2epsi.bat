@echo off 

if %1/==/ goto usage
if %2/==/ goto usage

call "%~dp0gssetgs.bat"
set infile=%~1
set outfile=%~2

rem First we need to determine the bounding box. ps2epsi.ps below will pick
rem the result up from %outfile%
%GSC% -q -dNOPAUSE -dBATCH -P- -sDEVICE=bbox -sOutputFile=NUL %1 2> %2

rem Ghostscript uses %outfile% to define the output file
%GSC% -q -dNOPAUSE -P- -sDEVICE=bit -sOutputFile=NUL ps2epsi.ps < %1

rem We bracket the actual file with a few commands to help encapsulation
echo %%%%Page: 1 1 >> %2
echo %%%%BeginDocument: %2 >> %2
echo /InitDictCount countdictstack def gsave save mark newpath >> %2
echo userdict /setpagedevice /pop load put >> %2

rem Append the original onto the preview header
rem cat.ps uses the %infile% and %outfile% environment variables for the filenames
%GSC% -q -dNOPAUSE -dBATCH -P- -sDEVICE=bit -sOutputFile=NUL cat.ps


echo %%%%EndDocument >> %2
echo countdictstack InitDictCount sub { end } repeat >> %2
echo cleartomark restore grestore >> %2

goto end

:usage
echo "Usage: ps2epsi <infile.ps> <outfile.epi>"

:end
