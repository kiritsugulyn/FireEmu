# FireEmu
A file emulator of kancolle kai

__Only Support Single Hit In Day Battle currently__

## Usage
Compile

Cmd :
~~~
FireEmu.exe -input=<input json file> -output=<output result file> -time=<times you want to run>
~~~

## Notice
You can get a sample input json file in `FireEmu/test.json`

[Here](https://github.com/andanteyk/ElectronicObserver/blob/develop/ElectronicObserver/Other/Information/apilist.txt) you can get the list of `Ship.Stype` ( api_start2.api_mst_stype.api_name ) and `Slot.Api_mapbattle_type3` (api_start2.api_mst_slotitem.api_type[2]).

The `Yomi` of Bismarck should be `ビスマルク`

Italia   `リットリオ・イタリア`

Roma `ローマ`

Other ships' `Yomi` doesn't matter

`valance` in the json file mean the default hit prob of the environment, 90 in kancolle kai and 93 in web game (test).

## Todo
* Support a more commen input file format.
* Support more attack type in day battle.
* Support attack of CV
* Support midnight battle