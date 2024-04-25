@echo off
setlocal

REM Define the path to the `.proto` files and the output directory relative to the batch file location
set PROTO_BASE_DIR=..\backend\CodeSprint.Common
set PROTO_DIR=%PROTO_BASE_DIR%\Protos
set OUT_DIR=.\src\app\generated

REM Create the output directory if it doesn't exist
if not exist "%OUT_DIR%" mkdir "%OUT_DIR%"

REM Find the path to the protoc-gen-ts plugin
set PROTOC_GEN_TS_PLUGIN=node_modules\.bin\protoc-gen-ts.cmd

REM Generate TypeScript service definitions
for %%f in ("%PROTO_DIR%\*.proto") do (
    protoc ^
        --plugin=protoc-gen-ts="%PROTOC_GEN_TS_PLUGIN%" ^
        --ts_out="%OUT_DIR%" ^
        -I "%PROTO_BASE_DIR%" ^
        %%f
)

echo TypeScript service definitions generated.

endlocal