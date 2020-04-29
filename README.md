# Thalatta
An open-source terrain generation suite.

## Introduction
After discovering there are no full open-source terrain generation programs, I started making one myself. It works with the Unity engine, and is incredibly easy to use.

## State
Thalatta can currently generate reasonable looking terrains. There are no advanced features like grass generation or town generation (yet).

## Contributing
Please feel free to contribute, I would love it if you helped out the gamedev community! If you would like to become a contributer, just create an issue, and I will add you.

## Tutorial
To get started, first add the repository to a seperate folder in your assets folder, then create an empty terrain, and add the terrain generation component to it. On the terrain, I would recommend to set the terrain height (on the terrain component) to about 200 max. Then set the depth to the terrain height, and the size to the size for your terrain. After this, you are ready to choose a noise algorithm. Diamond square produces reasonable results, exponential perlin noise only generates crude mountains/valleys. Turbulence might generate somewhat alien landscapes. Fractal generates plains with light hills. Hybrid combines fractal with exponential perlin noise.
Then configure your noise variables, and press generate, it should now generate some terrain, that you can easily edit.
