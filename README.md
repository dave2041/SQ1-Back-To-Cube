# SQ1-Back-To-Cube
Allows you to input the state of your SQ-1 and get a list of moves to bring it back to a cube

I took the cube states to solve from this website https://alchemistmatt.com/cube/square1list.html which is a great resource but i found myself scrolling through the list a lot...
If i were a web dev this could've beena  nice little wesite :)

Instead we have a windows form application, it reads in the states from the data text file and the images from the images folder. The Images have letters denoting their rotation.
This could have been done programatically but... It wasn't!

Every image with an 'a' indicating it is the first instance of that configuration is added to the first list. Once the user chooses the first state of their cube the second list is populated with all of the possible second states (or bottom to the top).

Finally once the user selects a second state, the state is looked up in the cubeStates list and we grab the next state and add it to the list until we find the final, solved state.

Feel free to message me with any questions or to mess around with this project in any way you like.

I've tried to comment but it should be self explanatory (famous last words)
