#!/bin/sh

# dotnet build -c Release
rm -rf obj
mkdir -p obj/Assemblies

# visual
cp -r Resources/* obj/
# functional
cp bin/Content.Client/Content.Client.dll obj/Assemblies/
cp bin/Content.Client/Content.Client.pdb obj/Assemblies/
cp bin/Content.Client/Content.Shared.dll obj/Assemblies/
cp bin/Content.Client/Content.Shared.pdb obj/Assemblies/
# legal
cp RobustToolbox/legal.md obj/
cp RobustToolbox/LICENSE-GPLv3.TXT obj/
cp RobustToolbox/LICENSE-MIT.TXT obj/

rm -f kdc_paint.zip
cd obj
zip -r ../kdc_paint.zip *

# scp -P 28421 kdc_paint.zip 192.168.0.70:/mnt/drive/http/kdc_paint_2.zip
