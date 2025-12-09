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

: rect-area {: i1 i2 -- u :}
    data i1 2 * th@
    data i2 2 * th@
    - abs 1+
    data i1 2 * 1+ th@
    data i2 2 * 1+ th@
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