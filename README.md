# Simple Toony URP

![](https://static.wixstatic.com/media/3a7fb2_eb8491021ff84a8aa0edd63cca429b10~mv2.gif)

Simple Toony URP is a simple toon shader package with custom toon lighting. Shaders are mostly developed in shader graph. For some features, HLSL file is used. Outline shader uses ```URP Sample Buffer``` node.

## Unity Version Compatibility
* 2022.3.7f1

## Features
* Multiple Directional Light support
* Point Light support
* Tweaking baked light output through posterize parameter (Not great!)
* Outline shader
* Contains several subgraphs including for Shader Mask
* Contains samples

## Usages
* Use  ```SimpleURPToon/SimpleStandardToon``` shader in material for toon shader
* Outline shader:
  * Use  ```ToonFullScreen/ScreenSpaceOutline``` shader in material for outline shader
  * Add ```Full Screen Pass Renderer Feature``` in URP renderer settings
  * Drag the outline material in the ```Pass Material```
  * Set ```Requirements``` to ```Depth```, ```Normal``` & ```Color```

![](https://static.wixstatic.com/media/3a7fb2_5ad48574360d4c5ea203bb4d80301676~mv2.png)

## License

Distributed under the MIT License. See `LICENSE.md` for more information.
