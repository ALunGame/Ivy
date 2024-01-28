@echo off
cd ".\Protogen"
::start cmd.exe /k protogen.exe --proto_path="../Proto" --csharp_out="../CS" **/*.proto
::start protogen.exe --proto_path="../Proto" --csharp_out="../../IvyGame/Assets/Scripts/Network/GenProto" **/*.proto