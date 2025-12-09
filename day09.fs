s" utils.fs" included

Create data 2000 cells allot
0 Value n-points

: parse-input ( ptr len -- )
    slurp-file
    0 to n-points
    begin
        parse-number
        data n-points th!
        n-points 1+ to n-points
        skip-char
        dup 0>
    while
    repeat
    assert( n-points 2 mod 0= )
    n-points 2 / to n-points
    2drop
;

: get-coord ( i axis -- u )
    swap 2 * + data swap th@
;

: get-coords ( i -- u u )
    dup 0 get-coord
    swap 1 get-coord
;

: rect-area {: i1 i2 -- u :}
    i1 0 get-coord
    i2 0 get-coord
    - abs 1+
    i1 1 get-coord
    i2 1 get-coord
    - abs 1+
    *
;

: solve1
    0
    n-points 0 +do
        n-points i 1+ +do
            i j rect-area
            max
        loop
    loop
;

: example1
    s" day09.example" parse-input
    solve1
    cr cr ." example1 " . cr
;

: part1
    s" day09.input" parse-input
    solve1
    cr cr ." part1 " . cr
;
\ answer 4777409595

: line-hits-rect {: rx0 ry0 rx1 ry1 lx0 ly0 lx1 ly1 | lymin lymax -- f :}
    \ cr ."    LINE( x " lx0 . lx1 . ." y " ly0 . ly1 . ." )"
    assert( rx0 rx1 <= )
    assert( ry0 ry1 <= )
    lx0 lx1 = if
        ly0 ly1 min to lymin
        ly0 ly1 max to lymax
        \ y-aligned line
        rx0 lx0 < \ ." A" dup .
        lx0 rx1 < \ ." B" dup .
        and
        lymin ry1 < \ ." C" dup .
        ry0 lymax < \ ." D" dup .
        and
        and
    else
        assert( ly0 ly1 = )
        ry0 rx0 ry1 rx1 ly0 lx0 ly1 lx1 recurse
    then
;

: test-line-hits-rect
    \ ...|..............
    \ ...|..............
    \ ..#|###...........
    \ ..#|#------.......
    \ ..#|###...........
    \ ..#|###...........
    \ ...|..............

    \ y-aligned, sweep along x axis
    assert( 1 10 5 13   0 0 0 20  line-hits-rect invert )
    assert( 1 10 5 13   1 0 1 20  line-hits-rect invert )
    assert( 1 10 5 13   2 0 2 20  line-hits-rect )
    assert( 1 10 5 13   4 0 4 20  line-hits-rect )
    assert( 1 10 5 13   5 0 5 20  line-hits-rect invert )
    assert( 1 10 5 13   6 0 6 20  line-hits-rect invert )

    \ y-aligned, grow along y axis
    assert( 1 10 5 13   3 0 3 9  line-hits-rect invert )
    assert( 1 10 5 13   3 0 3 10  line-hits-rect invert )
    assert( 1 10 5 13   3 0 3 11  line-hits-rect )
    assert( 1 10 5 13   3 0 3 12  line-hits-rect )
    assert( 1 10 5 13   3 0 3 13  line-hits-rect )
    assert( 1 10 5 13   3 0 3 14  line-hits-rect )

    \ y-aligned, shrink along y axis
    assert( 1 10 5 13   3 0 3 20  line-hits-rect )
    assert( 1 10 5 13   3 0 3 14  line-hits-rect )
    assert( 1 10 5 13   3 0 3 13  line-hits-rect )
    assert( 1 10 5 13   3 0 3 12  line-hits-rect )
    assert( 1 10 5 13   3 0 3 11  line-hits-rect )
    assert( 1 10 5 13   3 0 3 10  line-hits-rect invert )
    assert( 1 10 5 13   3 0 3 9  line-hits-rect invert )

    \ y-aligned, contained
    assert( 1 10 5 13   3 10 3 12 line-hits-rect )
    assert( 1 10 5 13   3 10 3 13 line-hits-rect )
    assert( 1 10 5 13   3 11 3 12 line-hits-rect )

    \ x-aligned, simpler test due to symmetry
    assert( 1 10 5 13   0 11 2 11 line-hits-rect )
    assert( 1 10 5 13   6 11 10 11 line-hits-rect invert )
;

: rect-area2 {: i1 i2 | x0 x1 y0 y1 -- u :}
    \ compute bounds
    i1 0 get-coord
    i2 0 get-coord
    2dup max to x1
    min to x0

    i1 1 get-coord
    i2 1 get-coord
    2dup max to y1
    min to y0

    cr ." CONSIDER(" i1 . i2 . ." x " x0 . ." - " x1 . ." y " y0 . ." - " y1 . ." )"

    \ check if a line passes through _interior_ of the rectangle
    n-points 0 +do
        x0 y0 x1 y1
        i get-coords
        i 1+ n-points mod get-coords
        line-hits-rect if ." INVALID " 0 unloop exit then
    loop

    x1 x0 - 1+ y1 y0 - 1+ *
;

: solve2
    0
    n-points 0 +do
        n-points i 1+ +do
            i j rect-area2
            cr ." FOUND(" dup . ." )"
            max
        loop
    loop
;

: example2
    s" day09.example" parse-input
    solve2
    cr cr ." example2 " . cr
;

: part2
    s" day09.input" parse-input
    solve2
    cr cr ." part2 " . cr
;
\ answer 1473551379

: tests
    test-line-hits-rect
;