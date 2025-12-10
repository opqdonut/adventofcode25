s" utils.fs" included

Create data 256 16 * cells allot
\ layout [n-lights] [target] [n-buttons] [b1] [b2] ...
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

    cr \ ." REST:" 2dup type cr
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
    0 {: index :}
    n-machines 0 +do
        ." N-LIGHTS " data index th@ .
        index 1+ to index
        ." TARGET " data index th@ b.
        index 1+ to index
        data index th@
        ." N-BUTTONS " dup .
        index 1+ to index
        0 +do
            ." B " data index th@ b.
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
    addr n-buttons th
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