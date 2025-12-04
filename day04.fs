0 Value max-x
0 Value max-y

Create chart 20000 allot

0 Value infile

: read-input ( ccc len --- )
    r/o open-file throw to infile
    chart
    0 to max-y
    ( pointer )
    begin
        dup 256 infile read-line throw
        ( pointer len end? )
    while
            dup to max-x
            max-y 1+ to max-y
            +
    repeat
    2drop
;

: print-input ( --- )
    cr chart max-y 0 ?do
        dup max-x type cr
        max-x +
    loop
    drop
;

: in-range ( x y -- f )
    dup 0 >= swap max-y < and
    swap dup 0 >= swap max-x < and
    and
;

: addr ( x y -- pointer )
    assert( 2dup in-range )
    max-x * + chart +
;

: @@ ( x y -- c )
    addr c@
;

: f01 ( f x y -- x-or-y )
    1 0 rot select
;

: @? ( x y -- f )
    2dup in-range if
        @@ '@' = f01
    else
        2drop 0
    then
;

: input-tests
    s" day04.example" read-input
    assert( 0 0 @@ '.' = )
    assert( 1 0 @@ '.' = )
    assert( 2 0 @@ '@' = )
    assert( 3 0 @@ '@' = )
    assert( 4 0 @@ '.' = )
    assert( 5 0 @@ '@' = )
    assert( 6 0 @@ '@' = )
    assert( 7 0 @@ '@' = )
    assert( 8 0 @@ '@' = )
    assert( 9 0 @@ '.' = )

    assert( 8 9 @@ '@' = )
    assert( 9 9 @@ '.' = )

    assert( 8 9 @? 1 = )
    assert( 9 9 @? 0 = )
    assert( 10 9 @? 0 = )
;

: solve1
    0
    max-x 0 ?do
        max-y 0 ?do
            ." DBG " i . j . cr
            i j @? 0> if
                0
                i 1- j 1- @? +
                i 1- j @? +
                i 1- j 1+ @? +
                i j 1- @? +
                i j 1+ @? +
                i 1+ j 1- @? +
                i 1+ j @? +
                i 1+ j 1+ @? +
                dup . cr
                4 < f01 +
                dup . cr
            then
        loop
    loop
;

: example
    s" day04.example" read-input
    solve1
    cr cr ." example "
    .
;

: part1
    s" day04.input" read-input
    solve1
    cr cr ." part1 " .
;
\ answer 1351

: remove ( x y -- )
    addr '.' swap c!
;

: solve2 {: | removed :}
    0 to removed
    begin
        removed
        max-x 0 ?do
            max-y 0 ?do
                i j @? 0> if
                    0
                    i 1- j 1- @? +
                    i 1- j @? +
                    i 1- j 1+ @? +
                    i j 1- @? +
                    i j 1+ @? +
                    i 1+ j 1- @? +
                    i 1+ j @? +
                    i 1+ j 1+ @? +
                    4 < if
                        ." RM " i . j . cr
                        i j remove
                        removed 1+ to removed
                    then
                then
            loop
        loop
        ." LOOP " removed . cr
        removed <> while
    repeat
    removed
;

: example2
    s" day04.example" read-input
    solve2
    cr cr ." example2 " .
;

: part2
    s" day04.input" read-input
    solve2
    cr cr ." example2 " .
;
\ answer 8345

: tests
    input-tests
;