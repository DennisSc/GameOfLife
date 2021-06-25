# Conway's Game of Life

A variant of the famous "Game of Life" algorithm (cellular automata that were discovered by John Horton Conway in 1970; see https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life). 
Implementation as WinForms-app (written in C#, with no other frameworks or GFX engine besides .NET involved).

No HashLife or other fancy algorithms are used for computing next cell iterations - all work is done just by lookup and calculation of two-dimensional boolean arrays. Frames (or iterations) are pre-computed and drawn to bitmap structure in memory, then applied to the form when the bitmap is ready. When done this way, pretty fast rendering can be achieved even using native 2D graphics in a WinForms app.

Screenshot showing application window and some random seeded automata doing their work
![Screenshot1](https://github.com/DennisSc/GameOfLife/blob/master/pictures/golscene2.PNG)

Screenshot of "glider gun" and "stopper" automaton structures in action:<br>
![Screenshot2](https://github.com/DennisSc/GameOfLife/blob/master/pictures/golscene1.PNG)

