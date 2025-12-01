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
\ answer 1150

: turn2 {: old step | sum -- new zero-crossings :}
    old step + to sum
    sum 0 > if
        \ ." positive "
        sum 100 / ( full-rotations )
        sum 100 mod swap ( new full-rotations )
    else
        \ ." negative "
        sum abs 100 / ( full-rotations )
        old 0 > ( full-rotations started-above-zero )
        if 1 + then ( zero-crossings )
        sum 100 mod swap ( new full-rotations )
    then
;

: eq2 ( x1 y1 x2 y2 -- f )
    rot ( x1 x2 y2 y1 )
    = ( x1 x2 yf )
    -rot ( yf x1 x2 )
    = ( yf xf )
    and
;

: turn2-tests ( -- )
    ." no-crossings "
    assert( 0 -10 turn2 90 0 eq2 )
    assert( 0 10 turn2 10 0 eq2 )
    ." simple-crossing "
    assert( 10 -20 turn2 90 1 eq2 )
    assert( 90 25 turn2 15 1 eq2 )
    ." multiple-crossings "
    assert( 90 125 turn2 15 2 eq2 )
    assert( 90 225 turn2 15 3 eq2 )
    assert( 10 -125 turn2 85 2 eq2 )
    assert( 10 -225 turn2 85 3 eq2 )
    ." from-zero-positive "
    assert( 0 10 turn2 10 0 eq2 )
    assert( 0 110 turn2 10 1 eq2 )
    assert( 0 210 turn2 10 2 eq2 )
    ." from-zero-negative "
    assert( 0 -10 turn2 90 0 eq2 )
    assert( 0 -110 turn2 90 1 eq2 )
    assert( 0 -210 turn2 90 2 eq2 )
    ." to-zero-positive "
    assert( 90 10 turn2 0 1 eq2 )
    assert( 90 110 turn2 0 2 eq2 )
    ." to-zero-negative "
    assert( 10 -10 turn2 0 1 eq2 )
    assert( 10 -110 turn2 0 2 eq2 )
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
        while
        tick2
    repeat

    cr cr zeros .
;
\ answer 6738