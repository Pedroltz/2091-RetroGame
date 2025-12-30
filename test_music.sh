#!/bin/bash

# Test script to verify music playback
echo "Testing music with proper environment variables..."
echo "DISPLAY=$DISPLAY"
echo "XDG_RUNTIME_DIR=$XDG_RUNTIME_DIR"
echo "User: $(whoami)"
echo "UID: $(id -u)"

# Run the game
cd /home/pedrolt/Documentos/Projs/2091-RetroGame
./bin/Debug/net8.0/2091
