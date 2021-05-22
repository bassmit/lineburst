# Line Burst

Unity package for drawing large amounts of lines in the game and scene view, from burst compiled jobs or monobehaviours. The contents of this package are largely taken from the Unity Physics package.

## Getting Started
Create a LineBurstRenderer behaviour, then use the thread safe static APIs like Draw.Line. When the amount of elements to be drawn is known use eg new Draw.Lines to allocate the required buffer as one operation.

