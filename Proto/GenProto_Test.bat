cd ".\Protogen"
start cmd.exe /k protogen.exe --proto_path="../Proto" --csharp_out="../CS" **/*.proto
pause