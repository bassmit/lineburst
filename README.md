# LineBurst
<p align="center">
  <img src="https://github.com/bassmit/images/blob/master/LineBurst/lineburst01.png?raw=true">
</p>

## Introduction
Draw large amounts of lines, shapes and text to the Unity Game and Scene View, from BURST compiled jobs or MonoBehaviours. The contents of this package are largely taken from the Unity Physics package.

## Installation
To have a quick look at LineBurst clone or download the master branch which contains a full Unity 2020.3 LTS project.

The recommended way of installing LineBurst is through the [OpenUPM](https://openupm.com/packages/com.bassmit.lineburst/) package installer (top right). Download and add the unitypackage to a project to install LineBurst, including setting up the scoped registry required.

Alternatively, open the package manager, choose Add package from git URL and enter:

    https://github.com/bassmit/lineburst.git#upm

Note that you will not be notified of updates to LineBurst, or other packages installed in this way.
 
## Getting Started
Create a LineBurstRenderer behaviour, then use the thread safe static APIs like:
 
    Draw.Line(a, b, Color.red);
     
When the amount of elements to be drawn is known allocate the required buffer in one operation:

    var spheres = new Draw.Spheres(amount);
    for (int i = 0; i < amount; i++)
        spheres.Draw(points[i], radii[i], colors[i]);
