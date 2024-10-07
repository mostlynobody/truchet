# Truchet
Multi-scale Truchet tile pattern generator, based on a paper by Christopher Carlson.

[christophercarlson.com MULTI-SCALE TRUCHET PATTERNS](https://christophercarlson.com/portfolio/multi-scale-truchet-patterns/) </br>
[Bridges 2018 paper: Multi-Scale Truchet Patterns](http://archive.bridgesmathart.org/2018/bridges2018-39.html) </br>

Written for Software Development with C#/921CSPTST1K13/2020S

# Help
Standard functionality via CLI.
```
Syntax: truchet.exe [-h] [-d] [-r] [-p] [-b]
                    [--Palette id] [-l count] [-s seed]
                    [-rc count] [-cc count] [-ts size]

Options:
   -h              Displays this help screen.
   -d              Generates additional debug images. (default: off)
   -r              Sets generating method to random. (default: off)
   -p              Sets generating method to perlin noise.(default: on)
   -b              Turns on border cropping. (default: off)
   --Palette id    Specifies a palette. (default: Monochrome)
   -l count        Specifies the number of subdivision levels. (default: 3)
   -s seed         Specifies a seed. (default: random seed)
   -rc count       Specifies the amount of rows. (default: 10)
   -cc count       Specifies the amount of columns. (default: 10)
   -ts size        Specifies the tile size. (default: 300)

The following palettes are available:
   0: Monochrome
   1: Sapphire
   2: Imperial
   3: Deep
   4: Apricot
   5: Xiketic
   6: Canary
   7: Meadow
```
## Sample Images

![sample1](https://user-images.githubusercontent.com/32034333/81904386-8160fd80-95c3-11ea-9840-5bce8c946883.png)
![sample2](https://user-images.githubusercontent.com/32034333/81904407-8920a200-95c3-11ea-94e9-94c8e7fbe2ff.png)
![sample3](https://user-images.githubusercontent.com/32034333/81904423-8cb42900-95c3-11ea-9c69-c644c8e17ad8.png)


## Features
- [x] basic functionality
- [x] perlin noise
- [x] color schemes
- [x] command line arguments
