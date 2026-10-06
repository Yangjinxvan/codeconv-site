@echo off
if exist codeconv.zip ren codeconv.zip codeconv_old.zip
"C:\Program Files\WinRAR\WinRAR.exe" a -afzip -pcodeconv -m4 -r -x"打包ZIP.bat" -x".git" -x"codeconv_old.zip" -x"autorun.inf" -x".gitignore" -x"codeconv.iso" codeconv.zip *
echo 打包完成！
pause
