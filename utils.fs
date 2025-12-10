: skip-char ( ccc len -- ccc+1 len-1 )
    1- swap 1+ swap
;

: parse-number ( ccc len -- ccc len u )
    0 0 2swap ( ud ccc len )
    >number ( ud ccc len )
    2swap ( ccc len ud )
    drop
;

: print-arr ( arr n -- )
    0 +do
        dup i th@ .
    loop
    drop
;

: reduce-arr ( xt init arr n -- res )
    cells bounds u+do
        i @
        third execute
    cell +loop
    nip
;

: b.
    ['] . 2 base-execute
;

: bit-set? ( i bits -- f )
    swap rshift 1 and 0 <>
;

: popcount ( u -- u )
    0 swap
    begin
        dup 0<>
    while
            dup 1 and under+
            1 rshift
    repeat
    drop
;