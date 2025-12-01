: >u ( ccc len --- n )
    \ s>number? produces double-length integer, I only want single for now
    s>number? drop drop ;

: turn ( n n -- n )
    + 100 mod ;

: parse ( ccc len -- n )
    over c@ 'L' = ( do we start with an L? )
    if -1 else 1 endif
    -rot ( bury the multiplier )
    .s
    1 - swap 1 + swap ( increment ccc, decrement len)
    >u
    * ( apply multiplier )
;

: tick ( n ccc len -- n )
    parse
    turn
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
;