# Thalatta
An open-source terrain generation suite.

## Introduction
After discovering there are no full open-source terrain generation programs, I started making one myself. It works with the Unity engine, and is incredibly easy to use.

## State
Thalatta can currently generate reasonable looking terrains. There are no advanced features like grass generation or town generation (yet).

## Contributing
Please feel free to contribute, I would love it if you helped out the gamedev community! If you would like to become a contributer, just create a pull request.

## Tutorial
### Terrain from scratch
To get started, first add the repository to a seperate folder in your assets folder, then create an empty terrain, and add the terrain generation component to it. On the terrain, I would recommend to set the terrain height (on the terrain component) to about 200 max. Then set the depth to the terrain height, and the size to the size for your terrain. After this, you are ready to choose a noise algorithm. Diamond square produces decent results, but of varying quality, exponential perlin noise works well but might look boring in valeys. Turbulence might generate somewhat alien landscapes. Fractal generates plains with light hills. Hybrid combines fractal with exponential perlin noise, which gives the best results in my opinion.

### Erosion
You can enable erosion, which, well, simulates erosion (code written by Sebastian Lague). You can also choose to only enable erosion when generating the terrain, which will use the existing terrain.

### Texture terrain
If you enable texture terrain, it will use the first texture for dirt, the  for rock, the third for snow, and the fourth for grass

### Tree generation
If you enable tree generation, it will use the first tree in the tree slot to place across the map.
