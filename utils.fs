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
    cr
    0 +do
        dup i th@ .
    loop
    cr
;

: b.
    ['] . 2 base-execute
;