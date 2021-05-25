<p align="center">
  <img src="https://github.com/bassmit/images/blob/master/LineBurst/lineburst01.png?raw=true">
</p>

# LineBurst
Draw large amounts of lines and shapes to the Unity Game and Scene View, from BURST compiled jobs or MonoBehaviours. The contents of this package are largely taken from the Unity Physics package.

## Installation
To have a quick look at LineBurst clone or download the master branch which contains a full Unity 2020.3 LTS project.

The recommended way of installing LineBurst is through the [OpenUPM](https://openupm.com/packages/com.bassmit.lineburst/) package installer (top right). Download and add the unitypackage to a project to install LineBurst, including setting up the scoped registry required.

The package manager UI then needs to be augmented to show and install updates by installing [UpmGitExtension](https://openupm.com/packages/com.coffee.upm-git-extension/).

Alternatively, open the package manager and choose Add package from git URL:

![](https://github.com/bassmit/images/blob/master/DotsNav/image16.png?raw=true)

And enter the url:

    https://github.com/bassmit/lineburst.git#upm

Note that you will not be notified of updates to LineBurst, or other packages installed in this way.
 
## Getting Started
Create a LineBurstRenderer behaviour, then use the thread safe static APIs like:
 
    Draw.Line(a, b);
     
When the amount of elements to be drawn is known allocate the required buffer in one operation:

    var spheres = new Draw.Spheres(x);
    for (int i = 0; i < x; i++)
        spheres.Draw(/*...*/);