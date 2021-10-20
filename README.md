# LineBurst
<p align="center">
  <img src="https://github.com/bassmit/images/blob/master/LineBurst/lineburst02.png?raw=true">
</p>
<p align="center">
  <img src="https://github.com/bassmit/images/blob/master/LineBurst/lineburst03.png?raw=true">
</p>

## Introduction
Plot functions and draw large amounts of debug lines, shapes and text to the Unity Game and Scene View, from BURST compiled jobs or MonoBehaviours.

## Installation
If [OpenUpm](https://openupm.com/packages/com.bassmit.lineburst/) is not installed, ensure node.js is installed so you have access to npm and run:
    
    npm install -g openupm-cli
    
To add LineBurst to a Unity project, open a prompt in the project root and run:

    openupm add com.bassmit.lineburst
 
## Getting Started
Create a LineBurstRenderer behaviour, then use the thread safe static APIs like:
 
    Draw.Line(a, b, Color.red);
     
When the amount of elements to be drawn is known allocate the required buffer in one operation:

    var spheres = new Draw.Spheres(amount);
    for (int i = 0; i < amount; i++)
        spheres.Draw(points[i], radii[i], colors[i]);
        
For additional examples install the Samples through the package manager, or check out the master branch which contains a 2020.3 LTS Unity project.

## Notes
The default font is missing lower case letters and many signs, a basic editor is provided in the scene view when a font is selected, pull requests are welcome ; ) The Unity Physics package forms the basis of this package.
