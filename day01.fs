: >u ( ccc len --- n )
    \ s>number? produces double-length integer, I only want single for now
    s>number? drop drop ;

: turn ( n n -- n )
    + 100 mod ;

: parse ( ccc len -- n )
    over c@ 'L' = ( do we start with an L? )
    if -1 else 1 endif
    -rot ( bury the multiplier )
    \ .s
    1 - swap 1 + swap ( increment ccc, decrement len)
    >u
    * ( apply multiplier )
;

0 Value zeros

: tick ( n ccc len -- n )
    parse
    turn
    \ store zero count
    dup 0 = if zeros 1 + to zeros endif
    ." TICK"
    dup .
    .\" \n"
;

: example
    50
    s" L68" tick
    s" L30" tick
    s" R48" tick
    s" L5" tick
    s" R60" tick
    s" L55" tick
    s" L1" tick
    s" L99" tick
    s" R14" tick
    s" L82" tick
    .\" \nEND:\n"
    .
    .\" \nZEROS:\n"
    zeros .
;

0 Value infile
Create line-buffer 256 allot

: getline ( -- ccc len end? )
    line-buffer
    line-buffer 256 infile read-line throw
    ;

: part1
    s" day01.input" r/o open-file throw to infile
    50
    begin
        getline
        .s
        while
        tick
    repeat

    cr cr zeros .
;

: turn2 ( old step -- new zero-crossings )
    +
    dup ( unmodded unmodded )
    100 / abs ( unmodded zero-crossings )
    \ check if we landed on 0
    over 0 = if 1 + then
    \ TODO: if we started from zero, don't count first cross into negative side
    swap 100 mod swap ( modded zero-crossings )
;

: tick2 ( n ccc len -- n )
    parse ( old step )
    ." STEP"
    dup .
    turn2 ( new crossings )

    \ store zero crossings
    zeros + to zeros

    ." TO"
    dup .
    ." ZEROS"
    zeros .
    .\" \n"
;

: part2
    s" day01.input" r/o open-file throw to infile
    50
    begin
        getline
        .s
        while
        tick2
    repeat

    cr cr zeros .
;