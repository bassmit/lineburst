# Line Burst

Unity package for drawing large amounts of lines in the game and scene view, from burst compiled jobs or monobehaviours. The contents of this package are largely taken from the Unity Physics package.

## Install
The recommended way of installing LineBurst is through the [OpenUPM](https://openupm.com/packages/com.bassmit.lineburst/) unitypackage installer (top right). After downloading the installer add the unitypackage to your project and LineBurst will be installed, including setting up the scoped registry required.

The package manager ui then needs to be augmented to show and install updates by installing [UpmGitExtension](https://openupm.com/packages/com.coffee.upm-git-extension/).

Alternatively, open the package manager, choose Add package from git URL and enter the url below. Note that you will not be notified of updates to LineBurst, or other custom packages installed in this way.

    https://github.com/bassmit/lineburst.git

## Getting Started
Create a LineBurstRenderer behaviour, then use the thread safe static APIs like Line.Draw.
