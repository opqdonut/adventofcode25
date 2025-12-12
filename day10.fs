s" utils.fs" included

Create data 256 16 * cells allot
\ layout [n-lights] [target] [n-buttons] [b1] [b2] ... [joltages]
0 Value write-to
0 Value n-machines


0 Value infile
Create line-buffer 512 allot

: parse-line ( ptr len -- )
    0 {: n-lights | target button n-buttons :}
    skip-char \ '['
    begin
        over c@ ']' <>
    while
            n-lights 1+ to n-lights
            target 1 lshift to target
            over c@ '#' = if
                target 1+ to target
            then
            skip-char
    repeat
    ." N-LIGHTS " n-lights .
    n-lights data write-to th!
    write-to 1+ to write-to
    ." TARGET " target b.
    target data write-to th!
    write-to 1+ to write-to

    skip-char \ ']'
    skip-char \ ' '

    0 to n-buttons
    write-to 1+ to write-to
    begin
        assert( over c@ '(' = )
        skip-char \ '('
        0 to button
        begin
            parse-number
            n-lights swap - 1-
            1 swap lshift
            button + to button
            over c@ ')' <>
        while
                skip-char
        repeat
        ." B " button b.
        n-buttons 1+ to n-buttons
        button data write-to th!
        write-to 1+ to write-to


        skip-char \ ')'
        skip-char \ ' '
        over c@ '{' <>
    while
    repeat
    ." N-BUTTONS " n-buttons .

    n-buttons data write-to n-buttons - 1- th!

    assert( over c@ '{' = )
    skip-char
    ." JOLTAGES "
    n-lights 0 +do
        parse-number dup .
        data write-to th!
        write-to 1+ to write-to
        skip-char
    loop
    cr
;

: parse-input ( ptr len -- )
    0 to write-to
    r/o open-file throw to infile
    begin
        line-buffer line-buffer 512 infile read-line throw
        ( pointer len end? )
    while
            parse-line
            n-machines 1+ to n-machines
            2drop
    repeat
    2drop
;

: show
    0 {: index | n-lights :}
    n-machines 0 +do
        data index th@ to n-lights
        index 1+ to index
        ." N-LIGHTS " n-lights .
        ." TARGET " data index th@ b.
        index 1+ to index
        data index th@
        ." N-BUTTONS " dup .
        index 1+ to index
        0 +do
            ." B " data index th@ b.
            index 1+ to index
        loop
        ." JOLTAGES "
        n-lights 0 +do
            data index th@ .
            index 1+ to index
        loop
        cr
    loop
;

Create counts 32 cells allot

: press {: bitmap buttons n-buttons | -- output :}
    0
    n-buttons 0 do
        i bitmap bit-set? if
            buttons i th@
            xor
        then
    loop
;

: solve-machine {: addr | n-lights target n-buttons :}
    cr
    addr @ to n-lights
    addr cell + to addr
    addr @ to target
    addr cell + to addr
    addr @ to n-buttons
    addr cell + to addr
    ." N-LIGHTS " n-lights . ." TARGET " target b. ." N-BUTTONS " n-buttons .
    n-buttons 0 +do
        ." B " addr i th@ b.
    loop

    counts 32 cells 0 fill

    9999
    1 n-buttons 1+ lshift 0 +do
        \ cr ." TRY " i b.
        i addr n-buttons press
        \ ." GOT " dup b.
        target = if
            i popcount
            ." HIT! " i b. dup .
            min
        then
    loop
    addr n-buttons th n-lights th
    swap
    ." MIN " dup .
;

: solve1
    0
    data
    n-machines 0 +do
        solve-machine
        assert( dup 9999 < )
        under+
    loop
    drop
;

: example1
    s" day10.example" parse-input
    solve1
    cr cr ." example1 " . cr
;

: difficult
    s" day10.difficult" parse-input
    solve1
    cr cr ." difficult " . cr
;

: part1
    s" day10.input" parse-input
    solve1
    cr cr ." part1 " . cr
;
\ answer 571

0 Value n-lights
0 Value n-buttons
0 Value buttons
Create joltages 32 cells allot

: jolt {: button incr | -- :}
    n-lights 0 do
        n-lights i - 1- button bit-set? if
            incr joltages i th +!
        then
    loop
;

: reduce-over-button {: button xt init | -- :}
    init
    n-lights 0 do
        n-lights i - 1- button bit-set? if
            joltages i th@
            xt execute
        then
    loop
;

: min-over-button ( button -- u )
    ['] min 9999 reduce-over-button
;

: test-jolt
    4 to n-lights
    0 joltages 0 th!
    0 joltages 1 th!
    0 joltages 2 th!
    0 joltages 3 th!
    %0001 1 jolt
    assert( joltages 3 th@ 1 = )
    assert( %0001 min-over-button 1 = )
    assert( %1110 min-over-button 0 = )
;

: sum-button-mins ( )
    0
    n-buttons 0 do
        buttons i th@
        min-over-button
        +
    loop
;

: mash-buttons {: presses-remaining start-i -- min-presses-needed :}
    \ cr 25 presses-remaining - 0 +do ." ." loop
    \ ." MASH " presses-remaining . cr
    \ ." STATE " joltages n-lights print-arr
    \ early exit: pressed too much
    ['] min 9999 joltages n-lights reduce-arr 0< if ( ." TOO MUCH " ) 9999 exit then
    \ early exit: we're done
    ['] max 0 joltages n-lights reduce-arr dup 0= if ." DONE! " drop 0 exit then
    \ early exit: too many things to press
    presses-remaining > if ( ." CAN'T REACH " ) 9999 exit then
    \ early exit: no presses left
    presses-remaining 0 = if ( ." NO PRESSES " ) 9999 exit then
    \ recurse
    9999
    n-buttons start-i +do
        buttons i th@ -1 jolt
        \ cr 15 presses-remaining - 0 +do ." ." loop ." TRY " buttons i th@ b.
        dup presses-remaining
        min 1- i recurse 1+
        min
        buttons i th@ 1 jolt \ undo state change
    loop
    \ ." RET " dup .
;

Create test-buttons %1 , %101 , %10 , %11 , %1010 , %1100 ,
: test-mash-buttons
    6 to n-buttons
    4 to n-lights
    test-buttons to buttons
    \ simple cases
    0 joltages 0 th!
    0 joltages 1 th!
    0 joltages 2 th!
    3 joltages 3 th!
    assert( 4 0 mash-buttons 3 = )
    3 joltages 0 th!
    0 joltages 1 th!
    0 joltages 2 th!
    3 joltages 3 th!
    assert( 4 0 mash-buttons 9999 = )
    2 joltages 0 th!
    2 joltages 1 th!
    2 joltages 2 th!
    2 joltages 3 th!
    assert( 5 0 mash-buttons 4 = )
    \ real case
    3 joltages 0 th!
    5 joltages 1 th!
    4 joltages 2 th!
    7 joltages 3 th!
    assert( 1 0 mash-buttons 9999 = )
    assert( 2 0 mash-buttons 9999 = )
    assert( 10 0 mash-buttons 10 ~~ = )
;

: jolt-to {: src dest button | -- :}
    n-lights 0 do
        src i th@
        n-lights i - 1- button bit-set? if
            1-
        then
        dest i th!
    loop
;

s" heap.fs" included

0 Value the-heap

: heap-top-last-button
    the-heap heap-top-payload @
;

: heap-top-joltages
    the-heap heap-top-payload cell +
;

: fail? ( arr -- f)
    ['] min 9999 rot n-lights reduce-arr 0<
;

: done? ( arr -- f )
    ['] max 0 rot n-lights reduce-arr 0=
;

: mash-bfs
    cr
    n-lights 1+ cells to heap-payload-len
    10000000 heap-allot to the-heap
    ." SIZ" the-heap heap-size .
    0 the-heap heap-insert
    0 over !
    cell +
    joltages swap n-lights cells move
    the-heap .heap-full
    ." THE-HEAP " the-heap h.

    10000000 0 +do
        \  ." ITER " the-heap heap-top . the-heap heap-size . cr
        heap-top-joltages done? if
            the-heap heap-top
            ." DONE! " dup .
            unloop exit
        then
        heap-top-joltages fail? if
        else
            n-buttons heap-top-last-button +do
                heap-top-joltages ( src )
                the-heap heap-top ( src presses )
                1+ the-heap heap-insert ( src new-payload )
                i over !
                cell +
                buttons i th@ jolt-to ( )
            loop
        then
        the-heap heap-pop
        \ the-heap .heap-full
    loop
    assert( false )

;

: solve2
    0
    data {: addr :}
    \ n-machines
    1
    0 +do
        addr @ to n-lights
        addr cell + to addr
        addr cell + to addr \ skip target
        addr @ to n-buttons
        addr cell + to addr
        addr to buttons
        addr n-buttons cells + to addr
        n-lights 0 +do
            addr @ joltages i th!
            addr cell + to addr
        loop

        ." MACHINE " joltages n-lights print-arr
        ." N-BUTTONS " n-buttons .
        (
        sum-button-mins
        ." SUM-BUTTON-MINS " dup .
        \ ['] + 0 joltages n-lights reduce-arr
        0 mash-buttons )

        mash-bfs

        \ assert( dup 9999 < )
        +
    loop
;

: example2
    s" day10.example" parse-input
    solve2
    cr cr ." example2 " . cr
;

: difficult2
    s" day10.difficult" parse-input
    solve2
    cr cr ." difficult2 " . cr
;


: part2
    s" day10.input" parse-input
    solve2
    cr cr ." part2 " . cr
;