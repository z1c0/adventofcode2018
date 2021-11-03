# Advent of Code 2018

## Day15

This was quite a head-scratcher and I don'T know how many hours I wasted on this one.
...

## Day18

Part 2 was one of those "your assembly code does not terminate in endless
time" problems, which I both enjoy and hate because of the amount of time
it takes me to "decompile" and fully understand those instructions.
This time I had the idea to create a helper method that rewrites
(`Rewrite()`) the low-level instructions to (slightly) more readable
code that I then manually decompile further. This saved me a bit of time
(and mistakes) I think.

For example:

* `Addi 1 1 1` beomes `b = b + 1;`
* `eqrr 5 2 5` becomes `f = f == c ? 1 : 0;`

(The variables `a` to `f` represent the registers from `0` to `5` here.)
