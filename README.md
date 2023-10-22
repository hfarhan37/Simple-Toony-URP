# Simple Toony URP

![Imgur](https://i.imgur.com/hTVtx7V.png)

Simple Toony URP is a simple toon shader package with custom toon lighting. Shaders are mostly developed in shader graph. This is mostly basic toon shader. However, several implementations such as adding support for point light and shadow mask might be useful for some people. For some features, HLSL codes are used. Outline shader uses ```URP Sample Buffer``` node.

![Imgur](https://i.imgur.com/8Q0NCRI.png)

## Unity Version Compatibility
* 2022.3.7f1
* Other versions may work for toon shader but not tested

## Rendering Pipeline Compatibility
* Universal Rendering Pipeline (URP)

## Features
* Multiple Directional Light support
* Multiple Point Light, Ambient Light, Baked Light, Reflection Probe support
* Tweaking baked light output through posterize parameter (Not great!)
* Outline shader
* Contains several subgraphs including for fetching Shadow Mask
* Contains samples
* Material converter tools to convert large amount of material

![](https://i.imgur.com/2ZAUWzM.gif)

## Installation
* Click ```Add packages from git url...``` from ```Package Manager```
* Paste ```https://github.com/hfarhan37/Simple-Toony-URP.git``` and click add

## Usages
* Use  ```SimpleURPToon/SimpleStandardToon``` shader in material for toon shader
* Outline shader:
  * Use  ```ToonFullScreen/ScreenSpaceOutline``` shader in material for outline shader
  * Add ```Full Screen Pass Renderer Feature``` in URP renderer settings
  * Drag the outline material in the ```Pass Material```
  * Set ```Requirements``` to ```Depth```, ```Normal``` & ```Color```

![Imgur](https://i.imgur.com/ozadZcO.png)

## Material Converter
* Open through ```Window/Simple Toony URP/Material Converter```
* Select the material type from ```Project Materials to Convert```
* A list of materials will be shown from selected material type
* You can select the materials that you want to exclude from the conversion
* Select the target material type from ```Target``` to which you want to convert
* Click ```Convert``` button to start conversion process
* You can also click ```Convert All Project Materials``` to convert all project materials. But it is NOT RECOMMENDED.
* You can undo the material conversion as well for accidental conversion

![Imgur](https://i.imgur.com/2VAglGq.png)

## License

Distributed under the MIT License. See [License](https://github.com/hfarhan37/Simple-Toony-URP/blob/master/LICENSE.md) for more information.
